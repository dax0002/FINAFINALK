using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace FinalProject.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;

        public MoviesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Movies
        public IActionResult Index(string SearchString)
        {
            var query = _context.Movies
                .Include(m => m.Genre)
                .Include(m => m.Reviews)
                .Include(m => m.Rating)
                .Include(m => m.Schedules)
                .AsQueryable();  // Convert to IQueryable for dynamic composition
            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(m =>
                    m.MovieTitle.Contains(SearchString) ||
                    m.Tagline.Contains(SearchString) ||
                    m.ReleaseYear.ToString().Contains(SearchString) ||
                    m.Genre.GenreTitle.Contains(SearchString) ||
                    m.Rating.MPAARating.Contains(SearchString) ||
                    m.Actors.Contains(SearchString));
            }
            List<Movie> SelectedMovies = query.ToList();
            // Populate the view bag with a count of all movies
            ViewBag.AllMovies = _context.Movies.Count();
            // Populate the view bag with a count of selected movies
            ViewBag.SelectedMovies = SelectedMovies.Count();
            return View(SelectedMovies.OrderByDescending(m => m.ReleaseYear));
        }


        // GET: Movies/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) //JobPostingID not specified
            {
                return View("Error", new String[] { "MovieID not specified - which movie do you want to view?" });
            }

            Movie movie = _context.Movies
                                .Include(m => m.Genre)
                                .Include(m => m.Rating)
                                .Include(m => m.Reviews)
                                .FirstOrDefault(m => m.MovieID == id);

            if (movie == null) //Job posting does not exist in database
            {
                return View("Error", new String[] { "Movie not found in database" });
            }
            //if code gets this far, all is well
            return View(movie);
        }

        // WE NEED CREATE FOR MANAGERS

        private string SelectedGenre
        {
            get => TempData[nameof(SelectedGenre)]?.ToString();
            set => TempData[nameof(SelectedGenre)] = value;
        }

        // GET: Movies/Create
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create()
        {
            // Populate the genres dropdown
            ViewBag.AllGenres = GetAllGenresSelectList();

            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, string newGenre)
        {
            if (ModelState.IsValid)
            {
                // If a new genre is provided, add it to the database
                if (!string.IsNullOrEmpty(newGenre))
                {
                    Genre genre = new Genre { GenreTitle = newGenre };
                    _context.Genres.Add(genre);
                    await _context.SaveChangesAsync();

                    // Set the GenreID property of the movie
                    movie.Genre.GenreID = genre.GenreID;
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If the model state is not valid, repopulate the genres dropdown
            ViewBag.AllGenres = GetAllGenresSelectList();

            return View(movie);
        }


        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieID == id);
        }

        public IActionResult DetailedSearch(String SearchString)
        {
            ViewBag.AllGenres = GetAllGenresSelectList();

            SearchViewModel svm = new SearchViewModel();

            return View(svm);
        }

        private SelectList GetAllGenresSelectList()
        {
            List<Genre> genreList = _context.Genres.ToList();

            Genre SelectNone = new Genre() { GenreID = 0, GenreTitle = "All Genres" };
            genreList.Add(SelectNone);

            SelectList genresSelectList = new SelectList(genreList.OrderBy(m => m.GenreTitle), "GenreID", "GenreTitle");

            return genresSelectList;
        }

        
        public IActionResult DisplaySearchResults(SearchViewModel svm)
        {
            var query = from c in _context.Movies
                        select c;

            if (svm.SearchTitle != null)
            {
                query = query.Where(c => c.MovieTitle.Contains(svm.SearchTitle));
            }

            if (svm.SearchTagline != null)
            {
                query = query.Where(c => c.Tagline.Contains(svm.SearchTagline));
            }

            if (svm.SelectedGenre != 0)
            {
                query = query.Where(c => c.Genre.GenreID == svm.SelectedGenre);
            }

            if (svm.SearchReleaseYear != 0)
            {
                query = query.Where(c => c.ReleaseYear == svm.SearchReleaseYear);
            }

            /*
            //REVIEWS???????? 
            if (svm.SearchReview != null)
            {
                if (svm.SearchType == SearchType.GreaterThan)
                {
                    query = query.Where(c => c.Reviews >= Decimal.Parse(svm.SearchReview));
                }

                if (svm.SearchType == SearchType.LessThan)
                {
                    query = query.Where(c => c.Reviews <= Decimal.Parse(svm.SearchReview));
                }
            }
            */

            if (svm.SearchRating != null)
            {
                query = query.Where(c => c.Rating.MPAARating == svm.SearchRating);
            }

            if (svm.SelectedActors != null)
            {
                query = query.Where(c => c.Actors.Contains(svm.SelectedActors));
            }

            List<Movie> SelectedMovies = query.Include(c => c.Genre)
                                              .Include(c => c.Reviews)
                                              .Include(c => c.Rating)
                                              .Include(c => c.Schedules)
                                              .ToList();

            /*
            IEnumerable<Movie> SelectedMovies = query.Include(c => c.Genre)
                                              .Include(c => c.Reviews)
                                              .Include(c => c.Rating)
                                              .Include(c => c.Schedules)
                                              .Include(c => c.Actors)
                                              .ToList()
                                              .Select(c => new Movie
                                              {
                                                  MovieID = c.MovieID,
                                                  MovieTitle = c.MovieTitle,
                                                  Description = c.Description,
                                                  Tagline = c.Tagline,
                                                  ReleaseYear = c.ReleaseYear,
                                                  Runtime = c.Runtime,
                                                  Actors = c.Actors,
                                                  Genre = c.Genre,
                                                  Rating = c.Rating,
                                          
                                              })
                                              .OrderByDescending(c => c.ReleaseYear);
                                                  //AvgRating = c.Reviews.Average(od => od.Rating); // Assuming Reviews is a collection of some type
                                                                                                  // Include other properties as needed; 
            */

            //List<JobPosting> SelectedJobPostings = query.ToList();

            // Populate the view bag with a count of all job postings
            ViewBag.AllMovies = _context.Movies.Count();
            //Populate the view bag with a count of selected job postings
            ViewBag.SelectedMovies = SelectedMovies.Count();

            return View("Index", SelectedMovies.OrderByDescending(c => c.ReleaseYear));
        }
        
    }
}

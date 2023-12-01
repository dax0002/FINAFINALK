using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProject.DAL;
using FinalProject.Models;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace FinalProject.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly AppDbContext _context;

        public SchedulesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Schedules
        public async Task<IActionResult> Index(string sortOrder, string searchTitle, string searchGenre, DateTime? searchDate)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["StartTimeSortParm"] = sortOrder == "StartTime" ? "start_time_desc" : "StartTime";

            var schedules = _context.Schedules
                .Include(s => s.Movie)
                    .ThenInclude(m => m.Genre)
                .Include(s => s.Movie.Rating)
                .Include(s => s.Movie.Reviews)
                .Include(s => s.Price)
                .Include(s => s.TransactionDetails)
                .AsQueryable();

            // Filtering based on search parameters
            if (!String.IsNullOrEmpty(searchTitle))
            {
                schedules = schedules.Where(s => s.Movie.MovieTitle.Contains(searchTitle));
            }

            if (!String.IsNullOrEmpty(searchGenre))
            {
                schedules = schedules.Where(s => s.Movie.Genre.GenreTitle.Contains(searchGenre));
            }

            if (searchDate.HasValue)
            {
                schedules = schedules.Where(s => s.StartTime.Date == searchDate.Value.Date);
            }

            // Sorting logic
            switch (sortOrder)
            {
                case "title_desc":
                    schedules = schedules.OrderByDescending(s => s.Movie.MovieTitle);
                    break;
                case "StartTime":
                    schedules = schedules.OrderBy(s => s.StartTime);
                    break;
                case "start_time_desc":
                    schedules = schedules.OrderByDescending(s => s.StartTime);
                    break;
                default:
                    schedules = schedules.OrderBy(s => s.Movie.MovieTitle);
                    break;
            }

            // Seat count

            return View(await schedules.ToListAsync());
        }





        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Movie)
                .Include(s => s.Price)
                .Include(s => s.TransactionDetails)  // Include TransactionDetails
                .FirstOrDefaultAsync(m => m.ScheduleID == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }


        // GET: Schedules/Create
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create()
        {
            // Populate dropdown for movies
            ViewBag.Movies = new SelectList(_context.Movies.Select(p => p.MovieTitle).Distinct());
            ViewBag.TicketTypes = new SelectList(_context.Prices.Select(p => p.TicketType).Distinct());
            return View();
        }

        // POST: Schedule/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                // Validate scheduling rules here
                // ...

                // Manually load associated entities based on available data
                schedule.Movie = _context.Movies.FirstOrDefault(m => m.MovieTitle == schedule.Movie.MovieTitle);
                schedule.Price = _context.Prices.FirstOrDefault(p => p.TicketType == schedule.Price.TicketType);

                // EDIT                 _context.Schedules.Add(schedule);
                _context.Add(schedule);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Repopulate dropdown for movies in case of validation error
            // title title
            ViewBag.Movies = new SelectList(_context.Movies, "MovieID", "Title");
            return View(schedule);
        }


        // GET: Schedules/Edit/5
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            ViewBag.Movies = new SelectList(_context.Movies.Select(p => p.MovieTitle).Distinct());
            ViewBag.TicketTypes = new SelectList(_context.Prices.Select(p => p.TicketType).Distinct());
            ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(Status)));

            return View(schedule);
        }

        // POST: Schedules/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Schedule schedule)
        {
            if (id != schedule.ScheduleID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Manually load associated entities based on available data
                    schedule.Movie = _context.Movies.FirstOrDefault(m => m.MovieTitle == schedule.Movie.MovieTitle);
                    schedule.Price = _context.Prices.FirstOrDefault(p => p.TicketType == schedule.Price.TicketType);

                    _context.Update(schedule);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.ScheduleID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index");
            }

            // Repopulate dropdown for movies in case of validation error
            ViewBag.Movies = new SelectList(_context.Movies, "Title", "Title");
            return View(schedule);
        }


        private bool ScheduleExists(int id)
        {
          return _context.Schedules.Any(e => e.ScheduleID == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProject.DAL;
using FinalProject.Models;

namespace FinalProject.Controllers
{
    public class TransactionDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: TransactionDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.TransactionDetails.ToListAsync());
        }

        // GET: TransactionDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TransactionDetails == null)
            {
                return NotFound();
            }

            var transactionDetail = await _context.TransactionDetails
                .FirstOrDefaultAsync(m => m.TransactionDetailID == id);
            if (transactionDetail == null)
            {
                return NotFound();
            }

            return View(transactionDetail);
        }

        // GET: TransactionDetails/Create
        [HttpGet]
        public IActionResult Create(int transactionID)
        {
            var transaction = _context.Transactions.Find(transactionID);

            // Fetch movies to populate the dropdown list
            var movies = _context.Schedules
                .Select(s => s.Movie)
                .Distinct()
                .ToList();

            ViewData["Movies"] = new SelectList(movies, "MovieID", "MovieTitle");

            // Set an empty schedule list initially
            var schedules = new List<Schedule>();
            ViewData["Schedules"] = new SelectList(schedules, "ScheduleID", "StartTime");

            // Create a new TransactionDetail with the associated transaction
            var transactionDetail = new TransactionDetail { Transaction = transaction };

            // Return the view with the new TransactionDetail
            return View(transactionDetail);
        }

        [HttpPost]
        public IActionResult Create(TransactionDetail model)
        {
            // Check if a movie is selected
            if (model.SelectedMovieID != 0)
            {
                // Fetch schedules based on the selected movie
                var schedules = _context.Schedules
                    .Where(s => s.Movie.MovieID == model.SelectedMovieID)
                    .ToList();

                ViewData["Schedules"] = new SelectList(schedules, "ScheduleID", "StartTime");

                // Set ViewData["Movies"] again to ensure it's available in the view
                var movies = _context.Schedules
                .Select(s => s.Movie)
                .Distinct()
                .ToList();

                ViewData["Movies"] = new SelectList(movies, "MovieID", "MovieTitle");

                // Continue with the view
                return RedirectToAction("DisplayStartTimesAndTheaters", new { transactionID = model.Transaction.TransactionID, movieID = model.SelectedMovieID });
            }

            // Handle the case when no movie is selected
            return View(model);
        }

        [HttpGet]
        public IActionResult DisplayStartTimesAndTheaters(int transactionID, int movieID)
        {
            // Retrieve start times and theater information based on the selected movie
            var schedules = _context.Schedules
                .Where(s => s.Movie.MovieID == movieID)
                .ToList();

            // Retrieve the selected movie information from the context or database
            var selectedMovie = _context.Movies.FirstOrDefault(m => m.MovieID == movieID);

            // Create a view model to hold the necessary data
            var viewModel = new DisplayStartTimesViewModel
            {
                TransactionID = transactionID,
                Schedules = schedules,
                SelectedMovie = selectedMovie
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult DisplayStartTimesAndTheaters(DisplayStartTimesViewModel viewModel)
        {
            // Assuming the selected schedule ID is passed in the viewModel.SelectedScheduleID
            int selectedScheduleID = viewModel.SelectedScheduleID;
            int transactionID = viewModel.TransactionID;

            // Redirect to the final controller action with the selected schedule ID and transaction ID
            return RedirectToAction("FinalAction", new { scheduleID = selectedScheduleID, transactionID = transactionID });
        }


        [HttpGet]
        public IActionResult FinalAction(int scheduleID, int transactionID)
        {
            // Retrieve the selected schedule
            var selectedSchedule = _context.Schedules
                .Include(s => s.Movie)
                .FirstOrDefault(s => s.ScheduleID == scheduleID);
            // Check if the schedule exists
            if (selectedSchedule == null)
            {
                // Handle the case when the schedule is not found
                return NotFound();
            }

            // You can now use selectedSchedule.Movie, selectedSchedule.StartTime, etc.
            var transactionDetail = new TransactionDetail
            {
                Schedule = selectedSchedule,
                // Other properties of TransactionDetail can be set as needed
            };

            // You might want to pass the necessary data to the view or perform other actions here

            return View(transactionDetail);
        }


        [HttpPost]
        public IActionResult FinalAction(TransactionDetail transactionDetail, int scheduleID, string seat, PaymentMethod paymentMethod)
        {
            // Retrieve the selected schedule
            Schedule dbschedule = _context.Schedules.Find(scheduleID);
            // Retrieve the transaction using the provided transaction ID

            // Set the properties of the transactionDetail
            transactionDetail.Schedule = dbschedule;
            transactionDetail.Seat = seat;
            transactionDetail.PaymentMethod = paymentMethod;

            // Create a new TransactionDetail entry

            // Add the new transactionDetail to the context and save changes
            _context.Add(transactionDetail);
            _context.SaveChanges();

            // Optionally, you might want to redirect to a confirmation page or another action
            return RedirectToAction("Index", "Transactions");
        }





        private bool TransactionDetailExists(int id)
        {
            return _context.TransactionDetails.Any(e => e.TransactionDetailID == id);
        }
    }
}
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
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Check if the user is an admin
            bool isAdmin = User.IsInRole("Admin");

            // Retrieve transactions based on the user's role
            var transactionsQuery = isAdmin
                ? _context.Transactions.Include(t => t.TransactionDetails).ThenInclude(td => td.Schedule).ThenInclude(s => s.Price)
                : _context.Transactions.Include(t => t.TransactionDetails).ThenInclude(td => td.Schedule).ThenInclude(s => s.Price)
                                      .Where(t => t.User.UserName == User.Identity.Name);

            var transactions = transactionsQuery.ToList();

            // Calculate the total transaction price for each transaction
            var totalTransactionPrices = transactions.Select(t => t.TransactionDetails.Sum(td => td.TicketPrice));

            // Now you can use the transactions model for the view
            return View("Index", transactions);
        }

        public IActionResult Reports()
        {
            // Retrieve transactions and calculate reports
            var transactions = _context.Transactions
                .Include(t => t.TransactionDetails)
                .ThenInclude(td => td.Schedule)
                .ThenInclude(s => s.Price)
                .ToList();

            var totalSeatsSold = transactions.SelectMany(t => t.TransactionDetails).Count();
            var totalRevenue = transactions.SelectMany(t => t.TransactionDetails).Sum(td => td.TicketPrice);

            // Pass the data to the view
            ViewBag.Transactions = transactions;
            ViewBag.TotalSeatsSold = totalSeatsSold;
            ViewBag.TotalRevenue = totalRevenue;

            // Return the view
            return View();
        }


        // GET: Transactions/Details/5
        public IActionResult Details(int id)
        {
            var transaction = _context.Transactions
    .Include(t => t.TransactionDetails)
        .ThenInclude(td => td.Schedule)
            .ThenInclude(s => s.Price)
    .Include(t => t.TransactionDetails)
        .ThenInclude(td => td.Schedule)
            .ThenInclude(s => s.Movie)
    .FirstOrDefault(t => t.TransactionID == id);

            if (transaction == null)
            {
                return NotFound(); // Handle the case where the transaction is not found
            }

            return View(transaction);
        }

        // GET: Transactions/Create

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionID,TransactionStatus")] Transaction transaction)
        {
            if (id != transaction.TransactionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTransaction = await _context.Transactions
                        .Include(t => t.TransactionDetails)
                            .ThenInclude(td => td.Schedule)
                        .FirstOrDefaultAsync(t => t.TransactionID == id);

                    if (existingTransaction == null)
                    {
                        return NotFound();
                    }

                    // Check if the status is not already "Cancelled"
                    if (existingTransaction.TransactionStatus != TransactionStatus.Cancelled)
                    {
                        // Only update the TransactionStatus
                        existingTransaction.TransactionStatus = transaction.TransactionStatus;

                        // If the new status is "Cancelled," update other transactions with the same Schedule
                        if (transaction.TransactionStatus == TransactionStatus.Cancelled)
                        {
                            foreach (var detail in existingTransaction.TransactionDetails)
                            {
                                if (detail.Schedule != null)
                                {
                                    var relatedTransactions = _context.Transactions
                                        .Where(t => t.TransactionDetails.Any(td => td.Schedule == detail.Schedule))
                                        .ToList();

                                    foreach (var relatedTransaction in relatedTransactions)
                                    {
                                        relatedTransaction.TransactionStatus = TransactionStatus.Cancelled;
                                    }
                                }
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // If status is already "Cancelled," do not allow further edits
                        TempData["ErrorMessage"] = "Cancelled transactions cannot be edited.";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaction transaction)
        {
            //Find the next order number from the utilities class
            transaction.TransactionNumber = Utilities.GenerateNextOrderNumber.GetNextOrderNumber(_context);

            //Set the date of this order
            transaction.TransactionDate = DateTime.Now;

            //Associate the order with the logged-in customer
            transaction.User = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            transaction.TransactionStatus = TransactionStatus.Purchased;

            //make sure all properties are valid

            //if code gets this far, add the order to the database
            _context.Add(transaction);
            await _context.SaveChangesAsync();

            //send the user on to the action that will allow them to 
            //create a order detail.  Be sure to pass along the OrderID
            //that you created when you added the order to the database above
            return RedirectToAction("Create", "TransactionDetails", new { transactionID = transaction.TransactionID });
        }




        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionID == id);
        }
    }
}
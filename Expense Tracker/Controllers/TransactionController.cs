using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;

namespace Expense_Tracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDBContext _context;

        public TransactionController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.Transactions.Include(t => t.Category);
            return View(await applicationDBContext.ToListAsync());
        }

        public IActionResult AddOrEdit(int id=0)
        {
            PopulateCategories();
            if(id==0)
            {
                return View(new Transaction());
            }
            else
            {
                return View(_context.Transactions.Find(id));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
				if (transaction.TransactionId == 0)
				{
					_context.Add(transaction);
				}
				else
				{
					_context.Update(transaction);
				}
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategories();
            return View(transaction);
        }


        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [NonAction]
        public void PopulateCategories()
        {
            var categoryCollection = _context.Categories.ToList();
            Category defaultcategory = new Category()
            {
                CategoryId = 0,
                Title = "Choose a Category"
            };
            categoryCollection.Insert(0,defaultcategory);
            ViewBag.categories = categoryCollection;
         }

    }
}

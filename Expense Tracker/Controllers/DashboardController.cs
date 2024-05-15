using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Expense_Tracker.Controllers
{
	public class DashboardController : Controller
	{
		private readonly ApplicationDBContext _context;

        public DashboardController(ApplicationDBContext context)
        {
			_context = context;
		}

        public async Task<IActionResult> Index()
		{
			// get last 7 days
			DateTime startdate = DateTime.Now.AddDays(-6);
			DateTime enddate = DateTime.Today;

			List<Transaction> SelectTransactions = await _context.Transactions
				.Include(x=> x.Category)
				.Where(y=> y.Date >= startdate && y.Date<=enddate)
				.ToListAsync();

			// total income
			int totalincome = SelectTransactions
				.Where(i => i.Category.Type == "Income")
				.Sum(j => j.Amount);
			ViewBag.TotalIncome = totalincome.ToString("C0");

			// total income
			int totalexpense = SelectTransactions
				.Where(i => i.Category.Type == "Expense")
				.Sum(j => j.Amount);
			ViewBag.TotalExpense = totalexpense.ToString("C0");

			int balance = totalincome - totalexpense;
			CultureInfo culture = CultureInfo.CreateSpecificCulture("en-In");
			culture.NumberFormat.CurrencyNegativePattern = 1;
			ViewBag.Balance = string.Format(culture,"{0:C0}",balance);


			// for doughnut chart - expense by category

			ViewBag.DoughnutChartData = SelectTransactions
				.Where(x => x.Category.Type == "Expense")
				.GroupBy(j => j.Category.CategoryId)
				.Select(k => new
				{
					categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
					amount = k.Sum(i=>i.Amount),
					formattedAmount = k.Sum(j=>j.Amount)
				})
				.OrderByDescending( j =>j.amount)
				.ToList();

			// recent transactions

			ViewBag.RecentTransactions = await _context.Transactions
				.Include(i => i.Category)
				.OrderByDescending(j => j.Date)
				.Take(5)
				.ToListAsync();

			return View();
		}
	}
}

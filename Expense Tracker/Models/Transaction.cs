using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
	public class Transaction
	{
		[Key]
        public int TransactionId { get; set; }

		[Range(1, int.MaxValue,ErrorMessage ="Please Choose Something")]
        public int CategoryId { get; set; }
		public Category? Category {  get; set; }

		[Required(ErrorMessage = "Amount is required")]
		[Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than 0")]
		public int Amount { get; set; }

		[Column(TypeName = "nvarchar(75)")]
		public string? Note { get; set; }

		[Required(ErrorMessage = "Date is required")]
		public DateTime Date { get; set; } = DateTime.Now;


		// for custom properties
		[NotMapped]
		public string? CategoryTitleWithIcon
		{
			get
			{
				return Category == null ? " " : Category.Icon + " " + Category.Title;
			}
		}

		[NotMapped]
		public string? FormattedAmount
		{
			get
			{
				return ((Category == null || Category.Type=="Expense" ) ? "- " :"+ ") + Amount.ToString("C0");
			}
		}

	}
}

namespace GerenciadorDeAlunos.Models;

public class MonthlyPayment
{
	public int Id { get; set; }
	public int StudentId { get; set; }
	public int Year { get; set; }
	public int Month { get; set; }

	public decimal TotalAmount { get; set; }
	public bool IsPaid { get; set; }
	public DateTime? PaymentDate { get; set; }
	public DateTime DueDate { get; set; }
	public DateTime CreatedDate { get; set; }

	// Propriedades calculadas dinamicamente
	public bool IsOverdue => !IsPaid && DateTime.UtcNow > DueDate;
	public int DaysOverdue => IsOverdue ? (DateTime.UtcNow - DueDate).Days : 0;

	public Student Student { get; set; } = null!;
	public ICollection<MonthlyPaymentDetail> Details { get; set; } = new List<MonthlyPaymentDetail>();
}

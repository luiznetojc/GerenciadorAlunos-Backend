namespace GerenciadorDeAlunos.Models.DTOs;

public class MonthlyPaymentRequestDto
{
	public int StudentId { get; set; }
	public int Year { get; set; }
	public int Month { get; set; }
	public decimal TotalAmount { get; set; }
	public bool IsPaid { get; set; }
	public DateTime? PaymentDate { get; set; }
	public DateTime DueDate { get; set; }
}

public class MonthlyPaymentResponseDto
{
	public int Id { get; set; }
	public int StudentId { get; set; }
	public string StudentName { get; set; } = string.Empty;
	public int Year { get; set; }
	public int Month { get; set; }
	public string MonthName { get; set; } = string.Empty;
	public decimal TotalAmount { get; set; }
	public bool IsPaid { get; set; }
	public DateTime? PaymentDate { get; set; }
	public DateTime DueDate { get; set; }
	public DateTime CreatedDate { get; set; }

	// Propriedades calculadas automaticamente
	public bool IsOverdue => !IsPaid && DateTime.UtcNow > DueDate;
	public int DaysOverdue => IsOverdue ? (DateTime.UtcNow - DueDate).Days : 0;
	public string StatusDescription => IsPaid ? "Pago" : (IsOverdue ? $"{DaysOverdue} dias em atraso" : "Em dia");

	public List<MonthlyPaymentDetailResponseDto> Details { get; set; } = new();
}

public class MonthlyPaymentSummaryDto
{
	public int StudentId { get; set; }
	public string StudentName { get; set; } = string.Empty;
	public List<MonthlyPaymentResponseDto> Payments { get; set; } = new();

	// Propriedades calculadas automaticamente
	public decimal TotalDebt => Payments.Where(p => !p.IsPaid).Sum(p => p.TotalAmount);
	public int OverdueCount => Payments.Count(p => p.IsOverdue);
	public bool HasOverduePayments => OverdueCount > 0;
}

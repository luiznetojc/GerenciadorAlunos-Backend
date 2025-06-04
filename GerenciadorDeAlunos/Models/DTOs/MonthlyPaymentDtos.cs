namespace GerenciadorDeAlunos.Models.DTOs;

public class MonthlyPaymentRequestDto
{
	public int StudentId { get; set; }
	public int Year { get; set; }
	public int Month { get; set; }
	public decimal TotalAmount { get; set; }
	public bool IsPaid { get; set; }
	public DateTime? PaymentDate { get; set; }
}

public class MonthlyPaymentResponseDto
{
	public int Id { get; set; }
	public int StudentId { get; set; }
	public int Year { get; set; }
	public int Month { get; set; }
	public decimal TotalAmount { get; set; }
	public bool IsPaid { get; set; }
	public DateTime? PaymentDate { get; set; }
}

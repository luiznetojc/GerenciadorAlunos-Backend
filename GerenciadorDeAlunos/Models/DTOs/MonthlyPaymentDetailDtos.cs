namespace GerenciadorDeAlunos.Models.DTOs;

public class MonthlyPaymentDetailRequestDto
{
	public int MonthlyPaymentId { get; set; }
	public int EnrollmentId { get; set; }
	public decimal OriginalAmount { get; set; }
	public decimal DiscountAmount { get; set; }
}

public class MonthlyPaymentDetailResponseDto
{
	public int Id { get; set; }
	public int MonthlyPaymentId { get; set; }
	public int EnrollmentId { get; set; }
	public string DisciplineName { get; set; } = string.Empty;
	public decimal OriginalAmount { get; set; }
	public decimal DiscountAmount { get; set; }
	public decimal FinalAmount => OriginalAmount - DiscountAmount;
}

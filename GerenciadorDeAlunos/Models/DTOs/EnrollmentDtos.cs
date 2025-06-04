namespace GerenciadorDeAlunos.Models.DTOs;

public class EnrollmentRequestDto
{
	public int StudentId { get; set; }
	public int DisciplineId { get; set; }
	public decimal? Discount { get; set; }
}

public class EnrollmentResponseDto
{
	public int Id { get; set; }
	public int StudentId { get; set; }
	public int DisciplineId { get; set; }
	public decimal? Discount { get; set; }
}

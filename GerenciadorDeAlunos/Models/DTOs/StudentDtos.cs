namespace GerenciadorDeAlunos.Models.DTOs;

public class StudentRequestDto
{
	public int RegistrationNumber { get; set; }
	public string FullName { get; set; } = null!;
	public string EnrollmentDate { get; set; } = null!;
}

public class StudentResponseDto
{
	public int Id { get; set; }
	public int RegistrationNumber { get; set; }
	public string FullName { get; set; } = null!;
	public DateTime EnrollmentDate { get; set; }
}

namespace GerenciadorDeAlunos.Models.DTOs;

public class DisciplineRequestDto
{
	public string Name { get; set; } = null!;
	public decimal BasePrice { get; set; }
}

public class DisciplineResponseDto
{
	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public decimal BasePrice { get; set; }
}

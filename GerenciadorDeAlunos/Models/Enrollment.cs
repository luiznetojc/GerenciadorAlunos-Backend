namespace GerenciadorDeAlunos.Models;

public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int DisciplineId { get; set; }

    public decimal? Discount { get; set; }

    public Student Student { get; set; } = null!;
    public Discipline Discipline { get; set; } = null!;
}

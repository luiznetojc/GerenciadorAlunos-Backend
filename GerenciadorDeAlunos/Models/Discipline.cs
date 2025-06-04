namespace GerenciadorDeAlunos.Models;

public class Discipline
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal BasePrice { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

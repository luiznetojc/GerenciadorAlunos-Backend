namespace GerenciadorDeAlunos.Models;

public class Student
{
    public int Id { get; set; }
    public int RegistrationNumber { get; set; }
    public string FullName { get; set; } = null!;
    public DateTime EnrollmentDate { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<MonthlyPayment> MonthlyPayments { get; set; } = new List<MonthlyPayment>();
}

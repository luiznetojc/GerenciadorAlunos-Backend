namespace GerenciadorDeAlunos.Models;

public class MonthlyPaymentDetail
{
    public int Id { get; set; }
    public int MonthlyPaymentId { get; set; }
    public int EnrollmentId { get; set; }

    public decimal OriginalAmount { get; set; }
    public decimal DiscountAmount { get; set; }

    public MonthlyPayment MonthlyPayment { get; set; } = null!;
    public Enrollment Enrollment { get; set; } = null!;
}

namespace GerenciadorDeAlunos.Models;
public class MonthlyPayment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }

    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaymentDate { get; set; }

    public Student Student { get; set; } = null!;
    public ICollection<MonthlyPaymentDetail> Details { get; set; } = new List<MonthlyPaymentDetail>();
}

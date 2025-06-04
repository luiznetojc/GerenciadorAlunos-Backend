using GerenciadorDeAlunos.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorDeAlunos;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Discipline> Disciplines => Set<Discipline>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<MonthlyPayment> MonthlyPayments => Set<MonthlyPayment>();
    public DbSet<MonthlyPaymentDetail> MonthlyPaymentDetails => Set<MonthlyPaymentDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Discipline)
            .WithMany(d => d.Enrollments)
            .HasForeignKey(e => e.DisciplineId);

        modelBuilder.Entity<MonthlyPayment>()
            .HasOne(mp => mp.Student)
            .WithMany(s => s.MonthlyPayments)
            .HasForeignKey(mp => mp.StudentId);

        modelBuilder.Entity<MonthlyPaymentDetail>()
            .HasOne(d => d.MonthlyPayment)
            .WithMany(mp => mp.Details)
            .HasForeignKey(d => d.MonthlyPaymentId);

        modelBuilder.Entity<MonthlyPaymentDetail>()
            .HasOne(d => d.Enrollment)
            .WithMany()
            .HasForeignKey(d => d.EnrollmentId);
    }
}

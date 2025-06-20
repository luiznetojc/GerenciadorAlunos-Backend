using GerenciadorDeAlunos.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorDeAlunos.Repositories;

public class MonthlyPaymentRepository : IMonthlyPaymentRepository
{
	private readonly AppDbContext _context;
	public MonthlyPaymentRepository(AppDbContext context) => _context = context;

	public async Task<IEnumerable<MonthlyPayment>> GetAllAsync() =>
		await _context.MonthlyPayments
			.Include(mp => mp.Student)
			.Include(mp => mp.Details)
				.ThenInclude(d => d.Enrollment)
					.ThenInclude(e => e.Discipline)
			.ToListAsync();

	public async Task<MonthlyPayment?> GetByIdAsync(int id) =>
		await _context.MonthlyPayments
			.Include(mp => mp.Student)
			.Include(mp => mp.Details)
				.ThenInclude(d => d.Enrollment)
					.ThenInclude(e => e.Discipline)
			.FirstOrDefaultAsync(mp => mp.Id == id);

	public async Task<IEnumerable<MonthlyPayment>> GetByStudentIdAsync(int studentId) =>
		await _context.MonthlyPayments
			.Include(mp => mp.Student)
			.Include(mp => mp.Details)
				.ThenInclude(d => d.Enrollment)
					.ThenInclude(e => e.Discipline)
			.Where(mp => mp.StudentId == studentId)
			.OrderByDescending(mp => mp.Year)
			.ThenByDescending(mp => mp.Month)
			.ToListAsync();

	public async Task<MonthlyPayment?> GetByStudentAndMonthAsync(int studentId, int year, int month) =>
		await _context.MonthlyPayments
			.Include(mp => mp.Student)
			.Include(mp => mp.Details)
				.ThenInclude(d => d.Enrollment)
					.ThenInclude(e => e.Discipline)
			.FirstOrDefaultAsync(mp => mp.StudentId == studentId && mp.Year == year && mp.Month == month);

	public async Task<IEnumerable<MonthlyPayment>> GetUnpaidPaymentsAsync() =>
		await _context.MonthlyPayments
			.Include(mp => mp.Student)
			.Include(mp => mp.Details)
				.ThenInclude(d => d.Enrollment)
					.ThenInclude(e => e.Discipline)
			.Where(mp => !mp.IsPaid)
			.OrderBy(mp => mp.DueDate)
			.ToListAsync();

	public async Task<MonthlyPayment> AddAsync(MonthlyPayment payment)
	{
		_context.MonthlyPayments.Add(payment);
		await _context.SaveChangesAsync();
		return payment;
	}

	public async Task UpdateAsync(MonthlyPayment payment)
	{
		_context.Entry(payment).State = EntityState.Modified;
		await _context.SaveChangesAsync();
	}

	public async Task DeleteAsync(int id)
	{
		var entity = await _context.MonthlyPayments.FindAsync(id);
		if (entity != null)
		{
			_context.MonthlyPayments.Remove(entity);
			await _context.SaveChangesAsync();
		}
	}
}

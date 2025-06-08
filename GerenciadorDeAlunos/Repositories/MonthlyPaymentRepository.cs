using GerenciadorDeAlunos.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorDeAlunos.Repositories;

public class MonthlyPaymentRepository : IMonthlyPaymentRepository
{
	private readonly AppDbContext _context;
	public MonthlyPaymentRepository(AppDbContext context) => _context = context;

	public async Task<IEnumerable<MonthlyPayment>> GetAllAsync() => await _context.MonthlyPayments.ToListAsync();
	public async Task<MonthlyPayment?> GetByIdAsync(int id) => await _context.MonthlyPayments.FindAsync(id);
	public async Task<IEnumerable<MonthlyPayment>> GetByStudentIdAsync(int studentId) =>
		await _context.MonthlyPayments.Where(mp => mp.StudentId == studentId).ToListAsync();
	public async Task<MonthlyPayment?> GetByStudentAndMonthAsync(int studentId, int year, int month) =>
		await _context.MonthlyPayments.FirstOrDefaultAsync(mp => mp.StudentId == studentId && mp.Year == year && mp.Month == month);
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

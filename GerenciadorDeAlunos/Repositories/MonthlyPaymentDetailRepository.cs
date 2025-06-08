using GerenciadorDeAlunos.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorDeAlunos.Repositories;

public class MonthlyPaymentDetailRepository : IMonthlyPaymentDetailRepository
{
	private readonly AppDbContext _context;
	public MonthlyPaymentDetailRepository(AppDbContext context) => _context = context;

	public async Task<IEnumerable<MonthlyPaymentDetail>> GetAllAsync() => await _context.MonthlyPaymentDetails.ToListAsync();
	public async Task<MonthlyPaymentDetail?> GetByIdAsync(int id) => await _context.MonthlyPaymentDetails.FindAsync(id);
	public async Task<IEnumerable<MonthlyPaymentDetail>> GetByMonthlyPaymentIdAsync(int monthlyPaymentId) =>
		await _context.MonthlyPaymentDetails.Where(mpd => mpd.MonthlyPaymentId == monthlyPaymentId).ToListAsync();
	public async Task<MonthlyPaymentDetail> AddAsync(MonthlyPaymentDetail detail)
	{
		_context.MonthlyPaymentDetails.Add(detail);
		await _context.SaveChangesAsync();
		return detail;
	}
	public async Task UpdateAsync(MonthlyPaymentDetail detail)
	{
		_context.Entry(detail).State = EntityState.Modified;
		await _context.SaveChangesAsync();
	}
	public async Task DeleteAsync(int id)
	{
		var entity = await _context.MonthlyPaymentDetails.FindAsync(id);
		if (entity != null)
		{
			_context.MonthlyPaymentDetails.Remove(entity);
			await _context.SaveChangesAsync();
		}
	}
}

using GerenciadorDeAlunos.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorDeAlunos.Repositories;

public class DisciplineRepository : IDisciplineRepository
{
	private readonly AppDbContext _context;
	public DisciplineRepository(AppDbContext context) => _context = context;

	public async Task<IEnumerable<Discipline>> GetAllAsync() => await _context.Disciplines.ToListAsync();
	public async Task<Discipline?> GetByIdAsync(int id) => await _context.Disciplines.FindAsync(id);
	public async Task<Discipline> AddAsync(Discipline discipline)
	{
		_context.Disciplines.Add(discipline);
		await _context.SaveChangesAsync();
		return discipline;
	}
	public async Task UpdateAsync(Discipline discipline)
	{
		_context.Entry(discipline).State = EntityState.Modified;
		await _context.SaveChangesAsync();
	}
	public async Task DeleteAsync(int id)
	{
		var entity = await _context.Disciplines.FindAsync(id);
		if (entity != null)
		{
			_context.Disciplines.Remove(entity);
			await _context.SaveChangesAsync();
		}
	}
}

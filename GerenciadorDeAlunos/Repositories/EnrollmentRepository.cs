using GerenciadorDeAlunos.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorDeAlunos.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
	private readonly AppDbContext _context;
	public EnrollmentRepository(AppDbContext context) => _context = context;

	public async Task<IEnumerable<Enrollment>> GetAllAsync() => await _context.Enrollments.ToListAsync();
	public async Task<Enrollment?> GetByIdAsync(int id) => await _context.Enrollments.FindAsync(id);
	public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId) =>
		await _context.Enrollments.Where(e => e.StudentId == studentId).ToListAsync();
	public async Task<Enrollment> AddAsync(Enrollment enrollment)
	{
		_context.Enrollments.Add(enrollment);
		await _context.SaveChangesAsync();
		return enrollment;
	}
	public async Task UpdateAsync(Enrollment enrollment)
	{
		_context.Entry(enrollment).State = EntityState.Modified;
		await _context.SaveChangesAsync();
	}
	public async Task DeleteAsync(int id)
	{
		var entity = await _context.Enrollments.FindAsync(id);
		if (entity != null)
		{
			_context.Enrollments.Remove(entity);
			await _context.SaveChangesAsync();
		}
	}
}

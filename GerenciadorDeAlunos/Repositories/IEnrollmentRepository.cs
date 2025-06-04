using GerenciadorDeAlunos.Models;

namespace GerenciadorDeAlunos.Repositories;

public interface IEnrollmentRepository
{
	Task<IEnumerable<Enrollment>> GetAllAsync();
	Task<Enrollment?> GetByIdAsync(int id);
	Task<Enrollment> AddAsync(Enrollment enrollment);
	Task UpdateAsync(Enrollment enrollment);
	Task DeleteAsync(int id);
}

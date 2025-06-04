using GerenciadorDeAlunos.Models;

namespace GerenciadorDeAlunos.Repositories;

public interface IDisciplineRepository
{
	Task<IEnumerable<Discipline>> GetAllAsync();
	Task<Discipline?> GetByIdAsync(int id);
	Task<Discipline> AddAsync(Discipline discipline);
	Task UpdateAsync(Discipline discipline);
	Task DeleteAsync(int id);
}

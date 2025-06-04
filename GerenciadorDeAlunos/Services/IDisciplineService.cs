using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;

namespace GerenciadorDeAlunos.Services;

public interface IDisciplineService
{
	Task<IEnumerable<DisciplineResponseDto>> GetAllAsync();
	Task<DisciplineResponseDto?> GetByIdAsync(int id);
	Task<DisciplineResponseDto> AddAsync(DisciplineRequestDto discipline);
	Task<bool> UpdateAsync(int id, DisciplineRequestDto discipline);
	Task<bool> DeleteAsync(int id);
}

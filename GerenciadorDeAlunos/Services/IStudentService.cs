using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;

namespace GerenciadorDeAlunos.Services;

public interface IStudentService
{
	Task<IEnumerable<StudentResponseDto>> GetAllAsync();
	Task<StudentResponseDto?> GetByIdAsync(int id);
	Task<StudentResponseDto> AddAsync(StudentRequestDto student);
	Task<bool> UpdateAsync(int id, StudentRequestDto student);
	Task<bool> DeleteAsync(int id);
}

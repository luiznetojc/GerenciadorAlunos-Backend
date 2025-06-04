using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;

namespace GerenciadorDeAlunos.Services;

public interface IEnrollmentService
{
	Task<IEnumerable<EnrollmentResponseDto>> GetAllAsync();
	Task<EnrollmentResponseDto?> GetByIdAsync(int id);
	Task<EnrollmentResponseDto> AddAsync(EnrollmentRequestDto enrollment);
	Task<bool> UpdateAsync(int id, EnrollmentRequestDto enrollment);
	Task<bool> DeleteAsync(int id);
}

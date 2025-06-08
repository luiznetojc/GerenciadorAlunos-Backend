using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;

namespace GerenciadorDeAlunos.Services;

public interface IMonthlyPaymentService
{
	Task<IEnumerable<MonthlyPaymentResponseDto>> GetAllAsync();
	Task<MonthlyPaymentResponseDto?> GetByIdAsync(int id);
	Task<IEnumerable<MonthlyPaymentResponseDto>> GetByStudentIdAsync(int studentId);
	Task<MonthlyPaymentResponseDto?> GetByStudentAndMonthAsync(int studentId, int year, int month);
	Task<MonthlyPaymentResponseDto> AddAsync(MonthlyPaymentRequestDto payment);
	Task<bool> UpdateAsync(int id, MonthlyPaymentRequestDto payment);
	Task<bool> DeleteAsync(int id);
}

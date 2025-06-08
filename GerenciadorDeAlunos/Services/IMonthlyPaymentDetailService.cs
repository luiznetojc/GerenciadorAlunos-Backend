using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;

namespace GerenciadorDeAlunos.Services;

public interface IMonthlyPaymentDetailService
{
	Task<IEnumerable<MonthlyPaymentDetailResponseDto>> GetAllAsync();
	Task<MonthlyPaymentDetailResponseDto?> GetByIdAsync(int id);
	Task<IEnumerable<MonthlyPaymentDetailResponseDto>> GetByMonthlyPaymentIdAsync(int monthlyPaymentId);
	Task<MonthlyPaymentDetailResponseDto> AddAsync(MonthlyPaymentDetailRequestDto detail);
	Task<bool> UpdateAsync(int id, MonthlyPaymentDetailRequestDto detail);
	Task<bool> DeleteAsync(int id);
}

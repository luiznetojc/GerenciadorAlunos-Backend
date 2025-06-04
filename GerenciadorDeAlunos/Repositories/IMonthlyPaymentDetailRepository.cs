using GerenciadorDeAlunos.Models;

namespace GerenciadorDeAlunos.Repositories;

public interface IMonthlyPaymentDetailRepository
{
	Task<IEnumerable<MonthlyPaymentDetail>> GetAllAsync();
	Task<MonthlyPaymentDetail?> GetByIdAsync(int id);
	Task<MonthlyPaymentDetail> AddAsync(MonthlyPaymentDetail detail);
	Task UpdateAsync(MonthlyPaymentDetail detail);
	Task DeleteAsync(int id);
}

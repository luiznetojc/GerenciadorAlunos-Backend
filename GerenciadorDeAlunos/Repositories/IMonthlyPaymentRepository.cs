using GerenciadorDeAlunos.Models;

namespace GerenciadorDeAlunos.Repositories;

public interface IMonthlyPaymentRepository
{
	Task<IEnumerable<MonthlyPayment>> GetAllAsync();
	Task<MonthlyPayment?> GetByIdAsync(int id);
	Task<IEnumerable<MonthlyPayment>> GetByStudentIdAsync(int studentId);
	Task<MonthlyPayment?> GetByStudentAndMonthAsync(int studentId, int year, int month);
	Task<IEnumerable<MonthlyPayment>> GetUnpaidPaymentsAsync();
	Task<MonthlyPayment> AddAsync(MonthlyPayment payment);
	Task UpdateAsync(MonthlyPayment payment);
	Task DeleteAsync(int id);
}

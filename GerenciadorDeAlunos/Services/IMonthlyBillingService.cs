using GerenciadorDeAlunos.Models.DTOs;

namespace GerenciadorDeAlunos.Services;

public interface IMonthlyBillingService
{
	/// <summary>
	/// Gera débitos mensais para todos os alunos ativos com base em suas matrículas
	/// </summary>
	Task<int> GenerateMonthlyBillingsAsync(int year, int month);

	/// <summary>
	/// Gera débito mensal para um aluno específico
	/// </summary>
	Task<MonthlyPaymentResponseDto?> GenerateMonthlyBillingForStudentAsync(int studentId, int year, int month);

	/// <summary>
	/// Obtém resumo de débitos por aluno
	/// </summary>
	Task<IEnumerable<MonthlyPaymentSummaryDto>> GetDebtSummaryAsync();

	/// <summary>
	/// Obtém débitos em atraso (calculado dinamicamente)
	/// </summary>
	Task<IEnumerable<MonthlyPaymentResponseDto>> GetOverduePaymentsAsync();
}

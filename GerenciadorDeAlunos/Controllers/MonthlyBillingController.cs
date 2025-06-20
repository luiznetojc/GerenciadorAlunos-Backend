using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorDeAlunos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonthlyBillingController : ControllerBase
{
	private readonly IMonthlyBillingService _billingService;

	public MonthlyBillingController(IMonthlyBillingService billingService)
	{
		_billingService = billingService;
	}

	/// <summary>
	/// Gera débitos mensais para todos os alunos
	/// </summary>
	[HttpPost("generate/{year}/{month}")]
	public async Task<ActionResult<int>> GenerateMonthlyBillings(int year, int month)
	{
		try
		{
			var count = await _billingService.GenerateMonthlyBillingsAsync(year, month);
			return Ok(new { GeneratedCount = count, Message = $"Gerados {count} débitos para {month:D2}/{year}" });
		}
		catch (Exception ex)
		{
			return BadRequest(new { Error = ex.Message });
		}
	}

	/// <summary>
	/// Gera débito mensal para um aluno específico
	/// </summary>
	[HttpPost("generate/student/{studentId}/{year}/{month}")]
	public async Task<ActionResult<MonthlyPaymentResponseDto>> GenerateMonthlyBillingForStudent(int studentId, int year, int month)
	{
		try
		{
			var result = await _billingService.GenerateMonthlyBillingForStudentAsync(studentId, year, month);
			if (result == null)
				return BadRequest(new { Message = "Débito já existe ou aluno não possui matrículas ativas" });

			return Ok(result);
		}
		catch (Exception ex)
		{
			return BadRequest(new { Error = ex.Message });
		}
	}

	/// <summary>
	/// Obtém resumo de débitos por aluno
	/// </summary>
	[HttpGet("debt-summary")]
	public async Task<ActionResult<IEnumerable<MonthlyPaymentSummaryDto>>> GetDebtSummary()
	{
		try
		{
			var summary = await _billingService.GetDebtSummaryAsync();
			return Ok(summary);
		}
		catch (Exception ex)
		{
			return BadRequest(new { Error = ex.Message });
		}
	}

	/// <summary>
	/// Obtém débitos em atraso
	/// </summary>
	[HttpGet("overdue")]
	public async Task<ActionResult<IEnumerable<MonthlyPaymentResponseDto>>> GetOverduePayments()
	{
		try
		{
			var overduePayments = await _billingService.GetOverduePaymentsAsync();
			return Ok(overduePayments);
		}
		catch (Exception ex)
		{
			return BadRequest(new { Error = ex.Message });
		}
	}

	/// <summary>
	/// Gera débitos para o mês atual automaticamente
	/// </summary>
	[HttpPost("generate-current-month")]
	public async Task<ActionResult<int>> GenerateCurrentMonthBillings()
	{
		try
		{
			var now = DateTime.UtcNow;
			var count = await _billingService.GenerateMonthlyBillingsAsync(now.Year, now.Month);
			return Ok(new { GeneratedCount = count, Message = $"Gerados {count} débitos para {now:MM/yyyy}" });
		}
		catch (Exception ex)
		{
			return BadRequest(new { Error = ex.Message });
		}
	}
}

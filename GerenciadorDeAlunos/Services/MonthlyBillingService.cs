using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Repositories;
using System.Globalization;

namespace GerenciadorDeAlunos.Services;

public class MonthlyBillingService : IMonthlyBillingService
{
	private readonly IMonthlyPaymentRepository _paymentRepository;
	private readonly IMonthlyPaymentDetailRepository _detailRepository;
	private readonly IEnrollmentRepository _enrollmentRepository;
	private readonly IStudentRepository _studentRepository;

	public MonthlyBillingService(
		IMonthlyPaymentRepository paymentRepository,
		IMonthlyPaymentDetailRepository detailRepository,
		IEnrollmentRepository enrollmentRepository,
		IStudentRepository studentRepository)
	{
		_paymentRepository = paymentRepository;
		_detailRepository = detailRepository;
		_enrollmentRepository = enrollmentRepository;
		_studentRepository = studentRepository;
	}

	public async Task<int> GenerateMonthlyBillingsAsync(int year, int month)
	{
		var students = await _studentRepository.GetAllAsync();
		var generatedCount = 0;

		foreach (var student in students)
		{
			var result = await GenerateMonthlyBillingForStudentAsync(student.Id, year, month);
			if (result != null)
				generatedCount++;
		}

		return generatedCount;
	}

	public async Task<MonthlyPaymentResponseDto?> GenerateMonthlyBillingForStudentAsync(int studentId, int year, int month)
	{
		// Verificar se já existe um pagamento para este mês
		var existingPayment = await _paymentRepository.GetByStudentAndMonthAsync(studentId, year, month);
		if (existingPayment != null)
			return null;

		// Buscar matrículas ativas do aluno
		var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId);
		if (!enrollments.Any())
			return null;

		// Calcular data de vencimento (dia 10 do mês) - sempre em UTC
		var dueDate = new DateTime(year, month, 10, 0, 0, 0, DateTimeKind.Utc);

		// Criar o pagamento mensal
		var monthlyPayment = new MonthlyPayment
		{
			StudentId = studentId,
			Year = year,
			Month = month,
			TotalAmount = 0,
			IsPaid = false,
			DueDate = dueDate,
			CreatedDate = DateTime.UtcNow
		};

		var createdPayment = await _paymentRepository.AddAsync(monthlyPayment);

		// Criar os detalhes do pagamento para cada disciplina
		decimal totalAmount = 0;
		foreach (var enrollment in enrollments)
		{
			var discountAmount = enrollment.Discount ?? 0;
			var finalAmount = enrollment.Discipline.BasePrice - discountAmount;

			var detail = new MonthlyPaymentDetail
			{
				MonthlyPaymentId = createdPayment.Id,
				EnrollmentId = enrollment.Id,
				OriginalAmount = enrollment.Discipline.BasePrice,
				DiscountAmount = discountAmount
			};

			await _detailRepository.AddAsync(detail);
			totalAmount += finalAmount;
		}

		// Atualizar o valor total
		createdPayment.TotalAmount = totalAmount;
		await _paymentRepository.UpdateAsync(createdPayment);

		// Retornar o DTO de resposta
		return await ConvertToResponseDto(createdPayment);
	}

	public async Task<IEnumerable<MonthlyPaymentSummaryDto>> GetDebtSummaryAsync()
	{
		var students = await _studentRepository.GetAllAsync();
		var summaries = new List<MonthlyPaymentSummaryDto>();

		foreach (var student in students)
		{
			var payments = await _paymentRepository.GetByStudentIdAsync(student.Id);
			var unpaidPayments = payments.Where(p => !p.IsPaid).ToList();

			if (unpaidPayments.Any())
			{
				var paymentDtos = new List<MonthlyPaymentResponseDto>();
				foreach (var payment in unpaidPayments)
				{
					var responseDto = await ConvertToResponseDto(payment);
					paymentDtos.Add(responseDto);
				}

				var summary = new MonthlyPaymentSummaryDto
				{
					StudentId = student.Id,
					StudentName = student.FullName,
					Payments = paymentDtos
				};

				summaries.Add(summary);
			}
		}

		return summaries;
	}

	public async Task<IEnumerable<MonthlyPaymentResponseDto>> GetOverduePaymentsAsync()
	{
		var unpaidPayments = await _paymentRepository.GetUnpaidPaymentsAsync();
		var overduePayments = unpaidPayments.Where(p => p.IsOverdue).ToList();

		var responseDtos = new List<MonthlyPaymentResponseDto>();
		foreach (var payment in overduePayments)
		{
			var responseDto = await ConvertToResponseDto(payment);
			responseDtos.Add(responseDto);
		}

		return responseDtos.OrderByDescending(p => p.DaysOverdue);
	}

	private async Task<MonthlyPaymentResponseDto> ConvertToResponseDto(MonthlyPayment payment)
	{
		var student = await _studentRepository.GetByIdAsync(payment.StudentId);
		var details = await _detailRepository.GetByMonthlyPaymentIdAsync(payment.Id);

		var detailDtos = new List<MonthlyPaymentDetailResponseDto>();
		foreach (var detail in details)
		{
			var enrollment = await _enrollmentRepository.GetByIdAsync(detail.EnrollmentId);
			detailDtos.Add(new MonthlyPaymentDetailResponseDto
			{
				Id = detail.Id,
				MonthlyPaymentId = detail.MonthlyPaymentId,
				EnrollmentId = detail.EnrollmentId,
				DisciplineName = enrollment?.Discipline?.Name ?? "N/A",
				OriginalAmount = detail.OriginalAmount,
				DiscountAmount = detail.DiscountAmount
			});
		}

		return new MonthlyPaymentResponseDto
		{
			Id = payment.Id,
			StudentId = payment.StudentId,
			StudentName = student?.FullName ?? "N/A",
			Year = payment.Year,
			Month = payment.Month,
			MonthName = GetMonthName(payment.Month),
			TotalAmount = payment.TotalAmount,
			IsPaid = payment.IsPaid,
			PaymentDate = payment.PaymentDate,
			DueDate = payment.DueDate,
			CreatedDate = payment.CreatedDate,
			Details = detailDtos
		};
	}

	private static string GetMonthName(int month)
	{
		var culture = new CultureInfo("pt-BR");
		return culture.DateTimeFormat.GetMonthName(month);
	}
}

using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Repositories;
using System.Globalization;

namespace GerenciadorDeAlunos.Services;

public class MonthlyPaymentService : IMonthlyPaymentService
{
	private readonly IMonthlyPaymentRepository _repository;
	private readonly IMonthlyPaymentDetailRepository _detailRepository;
	private readonly IEnrollmentRepository _enrollmentRepository;

	public MonthlyPaymentService(
		IMonthlyPaymentRepository repository,
		IMonthlyPaymentDetailRepository detailRepository,
		IEnrollmentRepository enrollmentRepository)
	{
		_repository = repository;
		_detailRepository = detailRepository;
		_enrollmentRepository = enrollmentRepository;
	}

	public async Task<IEnumerable<MonthlyPaymentResponseDto>> GetAllAsync()
	{
		var items = await _repository.GetAllAsync();
		var responseDtos = new List<MonthlyPaymentResponseDto>();

		foreach (var mp in items)
		{
			var responseDto = await ConvertToResponseDto(mp);
			responseDtos.Add(responseDto);
		}

		return responseDtos;
	}

	public async Task<MonthlyPaymentResponseDto?> GetByIdAsync(int id)
	{
		var mp = await _repository.GetByIdAsync(id);
		if (mp == null) return null;
		return await ConvertToResponseDto(mp);
	}

	public async Task<IEnumerable<MonthlyPaymentResponseDto>> GetByStudentIdAsync(int studentId)
	{
		var items = await _repository.GetByStudentIdAsync(studentId);
		var responseDtos = new List<MonthlyPaymentResponseDto>();

		foreach (var mp in items)
		{
			var responseDto = await ConvertToResponseDto(mp);
			responseDtos.Add(responseDto);
		}

		return responseDtos;
	}

	public async Task<MonthlyPaymentResponseDto?> GetByStudentAndMonthAsync(int studentId, int year, int month)
	{
		var mp = await _repository.GetByStudentAndMonthAsync(studentId, year, month);
		if (mp == null) return null;
		return await ConvertToResponseDto(mp);
	}

	public async Task<MonthlyPaymentResponseDto> AddAsync(MonthlyPaymentRequestDto payment)
	{
		var entity = new MonthlyPayment
		{
			StudentId = payment.StudentId,
			Year = payment.Year,
			Month = payment.Month,
			TotalAmount = payment.TotalAmount,
			IsPaid = payment.IsPaid,
			PaymentDate = payment.PaymentDate,
			DueDate = payment.DueDate,
			CreatedDate = DateTime.UtcNow
		};
		var created = await _repository.AddAsync(entity);
		return await ConvertToResponseDto(created);
	}

	public async Task<bool> UpdateAsync(int id, MonthlyPaymentRequestDto payment)
	{
		var entity = await _repository.GetByIdAsync(id);
		if (entity == null) return false;

		entity.StudentId = payment.StudentId;
		entity.Year = payment.Year;
		entity.Month = payment.Month;
		entity.TotalAmount = payment.TotalAmount;
		entity.IsPaid = payment.IsPaid;
		entity.PaymentDate = payment.PaymentDate;
		entity.DueDate = payment.DueDate;
		// IsOverdue e DaysOverdue são calculados automaticamente

		await _repository.UpdateAsync(entity);
		return true;
	}

	public async Task<bool> DeleteAsync(int id)
	{
		var existing = await _repository.GetByIdAsync(id);
		if (existing == null) return false;
		await _repository.DeleteAsync(id);
		return true;
	}

	private async Task<MonthlyPaymentResponseDto> ConvertToResponseDto(MonthlyPayment payment)
	{
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
			StudentName = payment.Student?.FullName ?? "N/A",
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

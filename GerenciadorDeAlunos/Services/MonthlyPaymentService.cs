using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Repositories;

namespace GerenciadorDeAlunos.Services;

public class MonthlyPaymentService : IMonthlyPaymentService
{
	private readonly IMonthlyPaymentRepository _repository;
	public MonthlyPaymentService(IMonthlyPaymentRepository repository) => _repository = repository;

	public async Task<IEnumerable<MonthlyPaymentResponseDto>> GetAllAsync()
	{
		var items = await _repository.GetAllAsync();
		return items.Select(mp => new MonthlyPaymentResponseDto
		{
			Id = mp.Id,
			StudentId = mp.StudentId,
			Year = mp.Year,
			Month = mp.Month,
			TotalAmount = mp.TotalAmount,
			IsPaid = mp.IsPaid,
			PaymentDate = mp.PaymentDate
		});
	}

	public async Task<MonthlyPaymentResponseDto?> GetByIdAsync(int id)
	{
		var mp = await _repository.GetByIdAsync(id);
		if (mp == null) return null;
		return new MonthlyPaymentResponseDto
		{
			Id = mp.Id,
			StudentId = mp.StudentId,
			Year = mp.Year,
			Month = mp.Month,
			TotalAmount = mp.TotalAmount,
			IsPaid = mp.IsPaid,
			PaymentDate = mp.PaymentDate
		};
	}

	public async Task<IEnumerable<MonthlyPaymentResponseDto>> GetByStudentIdAsync(int studentId)
	{
		var items = await _repository.GetByStudentIdAsync(studentId);
		return items.Select(mp => new MonthlyPaymentResponseDto
		{
			Id = mp.Id,
			StudentId = mp.StudentId,
			Year = mp.Year,
			Month = mp.Month,
			TotalAmount = mp.TotalAmount,
			IsPaid = mp.IsPaid,
			PaymentDate = mp.PaymentDate
		});
	}

	public async Task<MonthlyPaymentResponseDto?> GetByStudentAndMonthAsync(int studentId, int year, int month)
	{
		var mp = await _repository.GetByStudentAndMonthAsync(studentId, year, month);
		if (mp == null) return null;
		return new MonthlyPaymentResponseDto
		{
			Id = mp.Id,
			StudentId = mp.StudentId,
			Year = mp.Year,
			Month = mp.Month,
			TotalAmount = mp.TotalAmount,
			IsPaid = mp.IsPaid,
			PaymentDate = mp.PaymentDate
		};
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
			PaymentDate = payment.PaymentDate
		};
		var created = await _repository.AddAsync(entity);
		return new MonthlyPaymentResponseDto
		{
			Id = created.Id,
			StudentId = created.StudentId,
			Year = created.Year,
			Month = created.Month,
			TotalAmount = created.TotalAmount,
			IsPaid = created.IsPaid,
			PaymentDate = created.PaymentDate
		};
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
}

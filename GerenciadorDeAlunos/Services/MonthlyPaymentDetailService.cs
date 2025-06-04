using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Repositories;

namespace GerenciadorDeAlunos.Services;

public class MonthlyPaymentDetailService : IMonthlyPaymentDetailService
{
	private readonly IMonthlyPaymentDetailRepository _repository;
	public MonthlyPaymentDetailService(IMonthlyPaymentDetailRepository repository) => _repository = repository;

	public async Task<IEnumerable<MonthlyPaymentDetailResponseDto>> GetAllAsync()
	{
		var items = await _repository.GetAllAsync();
		return items.Select(d => new MonthlyPaymentDetailResponseDto
		{
			Id = d.Id,
			MonthlyPaymentId = d.MonthlyPaymentId,
			EnrollmentId = d.EnrollmentId,
			OriginalAmount = d.OriginalAmount,
			DiscountAmount = d.DiscountAmount
		});
	}

	public async Task<MonthlyPaymentDetailResponseDto?> GetByIdAsync(int id)
	{
		var d = await _repository.GetByIdAsync(id);
		if (d == null) return null;
		return new MonthlyPaymentDetailResponseDto
		{
			Id = d.Id,
			MonthlyPaymentId = d.MonthlyPaymentId,
			EnrollmentId = d.EnrollmentId,
			OriginalAmount = d.OriginalAmount,
			DiscountAmount = d.DiscountAmount
		};
	}

	public async Task<MonthlyPaymentDetailResponseDto> AddAsync(MonthlyPaymentDetailRequestDto detail)
	{
		var entity = new MonthlyPaymentDetail
		{
			MonthlyPaymentId = detail.MonthlyPaymentId,
			EnrollmentId = detail.EnrollmentId,
			OriginalAmount = detail.OriginalAmount,
			DiscountAmount = detail.DiscountAmount
		};
		var created = await _repository.AddAsync(entity);
		return new MonthlyPaymentDetailResponseDto
		{
			Id = created.Id,
			MonthlyPaymentId = created.MonthlyPaymentId,
			EnrollmentId = created.EnrollmentId,
			OriginalAmount = created.OriginalAmount,
			DiscountAmount = created.DiscountAmount
		};
	}

	public async Task<bool> UpdateAsync(int id, MonthlyPaymentDetailRequestDto detail)
	{
		var entity = await _repository.GetByIdAsync(id);
		if (entity == null) return false;
		entity.MonthlyPaymentId = detail.MonthlyPaymentId;
		entity.EnrollmentId = detail.EnrollmentId;
		entity.OriginalAmount = detail.OriginalAmount;
		entity.DiscountAmount = detail.DiscountAmount;
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

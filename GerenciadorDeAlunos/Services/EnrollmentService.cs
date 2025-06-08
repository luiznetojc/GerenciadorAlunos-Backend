using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Repositories;

namespace GerenciadorDeAlunos.Services;

public class EnrollmentService : IEnrollmentService
{
	private readonly IEnrollmentRepository _repository;
	public EnrollmentService(IEnrollmentRepository repository) => _repository = repository;

	public async Task<IEnumerable<EnrollmentResponseDto>> GetAllAsync()
	{
		var items = await _repository.GetAllAsync();
		return items.Select(e => new EnrollmentResponseDto
		{
			Id = e.Id,
			StudentId = e.StudentId,
			DisciplineId = e.DisciplineId,
			Discount = e.Discount
		});
	}

	public async Task<EnrollmentResponseDto?> GetByIdAsync(int id)
	{
		var e = await _repository.GetByIdAsync(id);
		if (e == null) return null;
		return new EnrollmentResponseDto
		{
			Id = e.Id,
			StudentId = e.StudentId,
			DisciplineId = e.DisciplineId,
			Discount = e.Discount
		};
	}

	public async Task<IEnumerable<EnrollmentResponseDto>> GetByStudentIdAsync(int studentId)
	{
		var items = await _repository.GetByStudentIdAsync(studentId);
		return items.Select(e => new EnrollmentResponseDto
		{
			Id = e.Id,
			StudentId = e.StudentId,
			DisciplineId = e.DisciplineId,
			Discount = e.Discount
		});
	}

	public async Task<EnrollmentResponseDto> AddAsync(EnrollmentRequestDto enrollment)
	{
		var entity = new Enrollment
		{
			StudentId = enrollment.StudentId,
			DisciplineId = enrollment.DisciplineId,
			Discount = enrollment.Discount
		};
		var created = await _repository.AddAsync(entity);
		return new EnrollmentResponseDto
		{
			Id = created.Id,
			StudentId = created.StudentId,
			DisciplineId = created.DisciplineId,
			Discount = created.Discount
		};
	}

	public async Task<bool> UpdateAsync(int id, EnrollmentRequestDto enrollment)
	{
		var entity = await _repository.GetByIdAsync(id);
		if (entity == null) return false;
		entity.StudentId = enrollment.StudentId;
		entity.DisciplineId = enrollment.DisciplineId;
		entity.Discount = enrollment.Discount;
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

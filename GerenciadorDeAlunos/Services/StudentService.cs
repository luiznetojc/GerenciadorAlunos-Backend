using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Repositories;

namespace GerenciadorDeAlunos.Services;

public class StudentService : IStudentService
{
	private readonly IStudentRepository _repository;
	public StudentService(IStudentRepository repository)
	{
		_repository = repository;
	}

	public async Task<IEnumerable<StudentResponseDto>> GetAllAsync()
	{
		var students = await _repository.GetAllAsync();
		return students.Select(s => new StudentResponseDto
		{
			Id = s.Id,
			RegistrationNumber = s.RegistrationNumber,
			FullName = s.FullName,
			EnrollmentDate = s.EnrollmentDate
		});
	}

	public async Task<StudentResponseDto?> GetByIdAsync(int id)
	{
		var s = await _repository.GetByIdAsync(id);
		if (s == null) return null;
		return new StudentResponseDto
		{
			Id = s.Id,
			RegistrationNumber = s.RegistrationNumber,
			FullName = s.FullName,
			EnrollmentDate = s.EnrollmentDate
		};
	}

	public async Task<StudentResponseDto> AddAsync(StudentRequestDto student)
	{
		var entity = new Student
		{
			RegistrationNumber = student.RegistrationNumber,
			FullName = student.FullName,
			EnrollmentDate = student.EnrollmentDate
		};
		var created = await _repository.AddAsync(entity);
		return new StudentResponseDto
		{
			Id = created.Id,
			RegistrationNumber = created.RegistrationNumber,
			FullName = created.FullName,
			EnrollmentDate = created.EnrollmentDate
		};
	}

	public async Task<bool> UpdateAsync(int id, StudentRequestDto student)
	{
		var entity = await _repository.GetByIdAsync(id);
		if (entity == null) return false;
		entity.RegistrationNumber = student.RegistrationNumber;
		entity.FullName = student.FullName;
		entity.EnrollmentDate = student.EnrollmentDate;
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

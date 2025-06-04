using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Repositories;

namespace GerenciadorDeAlunos.Services;

public class DisciplineService : IDisciplineService
{
	private readonly IDisciplineRepository _repository;
	public DisciplineService(IDisciplineRepository repository) => _repository = repository;

	public async Task<IEnumerable<DisciplineResponseDto>> GetAllAsync()
	{
		var items = await _repository.GetAllAsync();
		return items.Select(d => new DisciplineResponseDto
		{
			Id = d.Id,
			Name = d.Name,
			BasePrice = d.BasePrice
		});
	}

	public async Task<DisciplineResponseDto?> GetByIdAsync(int id)
	{
		var d = await _repository.GetByIdAsync(id);
		if (d == null) return null;
		return new DisciplineResponseDto
		{
			Id = d.Id,
			Name = d.Name,
			BasePrice = d.BasePrice
		};
	}

	public async Task<DisciplineResponseDto> AddAsync(DisciplineRequestDto discipline)
	{
		var entity = new Discipline
		{
			Name = discipline.Name,
			BasePrice = discipline.BasePrice
		};
		var created = await _repository.AddAsync(entity);
		return new DisciplineResponseDto
		{
			Id = created.Id,
			Name = created.Name,
			BasePrice = created.BasePrice
		};
	}

	public async Task<bool> UpdateAsync(int id, DisciplineRequestDto discipline)
	{
		var entity = await _repository.GetByIdAsync(id);
		if (entity == null) return false;
		entity.Name = discipline.Name;
		entity.BasePrice = discipline.BasePrice;
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

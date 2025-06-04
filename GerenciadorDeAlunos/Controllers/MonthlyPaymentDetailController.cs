using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorDeAlunos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonthlyPaymentDetailController : ControllerBase
{
	private readonly IMonthlyPaymentDetailService _service;
	public MonthlyPaymentDetailController(IMonthlyPaymentDetailService service) => _service = service;

	[HttpGet]
	public async Task<ActionResult<IEnumerable<MonthlyPaymentDetailResponseDto>>> GetAll()
	{
		var items = await _service.GetAllAsync();
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<MonthlyPaymentDetailResponseDto>> GetById(int id)
	{
		var item = await _service.GetByIdAsync(id);
		if (item == null) return NotFound();
		return Ok(item);
	}

	[HttpPost]
	public async Task<ActionResult<MonthlyPaymentDetailResponseDto>> Create(MonthlyPaymentDetailRequestDto detail)
	{
		var created = await _service.AddAsync(detail);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, MonthlyPaymentDetailRequestDto detail)
	{
		var updated = await _service.UpdateAsync(id, detail);
		if (!updated) return NotFound();
		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		var deleted = await _service.DeleteAsync(id);
		if (!deleted) return NotFound();
		return NoContent();
	}
}

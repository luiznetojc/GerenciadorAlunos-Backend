using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorDeAlunos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonthlyPaymentController : ControllerBase
{
	private readonly IMonthlyPaymentService _service;
	public MonthlyPaymentController(IMonthlyPaymentService service) => _service = service;

	[HttpGet]
	public async Task<ActionResult<IEnumerable<MonthlyPaymentResponseDto>>> GetAll()
	{
		var items = await _service.GetAllAsync();
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<MonthlyPaymentResponseDto>> GetById(int id)
	{
		var item = await _service.GetByIdAsync(id);
		if (item == null) return NotFound();
		return Ok(item);
	}

	[HttpGet("student/{studentId}")]
	public async Task<ActionResult<IEnumerable<MonthlyPaymentResponseDto>>> GetByStudentId(int studentId)
	{
		var items = await _service.GetByStudentIdAsync(studentId);
		return Ok(items);
	}

	[HttpGet("student/{studentId}/month/{year}/{month}")]
	public async Task<ActionResult<MonthlyPaymentResponseDto>> GetByStudentAndMonth(int studentId, int year, int month)
	{
		var item = await _service.GetByStudentAndMonthAsync(studentId, year, month);
		if (item == null) return NotFound();
		return Ok(item);
	}

	[HttpPost]
	public async Task<ActionResult<MonthlyPaymentResponseDto>> Create(MonthlyPaymentRequestDto payment)
	{
		var created = await _service.AddAsync(payment);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, MonthlyPaymentRequestDto payment)
	{
		var updated = await _service.UpdateAsync(id, payment);
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

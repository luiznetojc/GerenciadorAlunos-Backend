using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorDeAlunos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
	private readonly IEnrollmentService _service;
	public EnrollmentController(IEnrollmentService service) => _service = service;

	[HttpGet]
	public async Task<ActionResult<IEnumerable<EnrollmentResponseDto>>> GetAll()
	{
		var items = await _service.GetAllAsync();
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<EnrollmentResponseDto>> GetById(int id)
	{
		var item = await _service.GetByIdAsync(id);
		if (item == null) return NotFound();
		return Ok(item);
	}

	[HttpGet("student/{studentId}")]
	public async Task<ActionResult<IEnumerable<EnrollmentResponseDto>>> GetByStudentId(int studentId)
	{
		var items = await _service.GetByStudentIdAsync(studentId);
		return Ok(items);
	}

	[HttpPost]
	public async Task<ActionResult<EnrollmentResponseDto>> Create(EnrollmentRequestDto enrollment)
	{
		var created = await _service.AddAsync(enrollment);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, EnrollmentRequestDto enrollment)
	{
		var updated = await _service.UpdateAsync(id, enrollment);
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

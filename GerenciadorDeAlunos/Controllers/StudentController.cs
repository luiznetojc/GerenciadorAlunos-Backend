using GerenciadorDeAlunos.Models;
using GerenciadorDeAlunos.Models.DTOs;
using GerenciadorDeAlunos.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorDeAlunos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
	private readonly IStudentService _service;

	public StudentController(IStudentService service)
	{
		_service = service;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<StudentResponseDto>>> GetAll()
	{
		var students = await _service.GetAllAsync();
		return Ok(students);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<StudentResponseDto>> GetById(int id)
	{
		var student = await _service.GetByIdAsync(id);
		if (student == null) return NotFound();
		return Ok(student);
	}
	
	[HttpPost]
	public async Task<ActionResult<StudentResponseDto>> Create(StudentRequestDto student)
	{
		var created = await _service.AddAsync(student);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, StudentRequestDto student)
	{
		var updated = await _service.UpdateAsync(id, student);
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

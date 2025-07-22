using Microsoft.AspNetCore.Mvc;
using PersonDirectory.Application.DTOs;
using PersonDirectory.Application.Interfaces;
using PersonDirectory.Domain.Entities;

namespace PersonDirectory.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly IPersonService _personService;

    public PersonsController(IPersonService personService)
    {
        _personService = personService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody] PersonCreateDto dto)
    {
        var exists = await _personService.ExistsByPersonalNumberAsync(dto.PersonalNumber);
        if (exists)
            return BadRequest($"Personal number '{dto.PersonalNumber}' already exists.");

        var person = new Person
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Gender = dto.Gender,
            PersonalNumber = dto.PersonalNumber,
            DateOfBirth = dto.DateOfBirth
        };

        var id = await _personService.AddPersonAsync(person);
        return CreatedAtAction(nameof(GetPerson), new { id }, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePerson(int id, [FromBody] PersonUpdateDto dto)
    {
        var existing = await _personService.GetPersonByIdAsync(id);
        if (existing == null) return NotFound();

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Gender = dto.Gender;
        existing.PersonalNumber = dto.PersonalNumber;
        existing.DateOfBirth = dto.DateOfBirth;

        await _personService.UpdatePersonAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson(int id)
    {
        var existing = await _personService.GetPersonByIdAsync(id);
        if (existing == null) return NotFound();

        await _personService.DeletePersonAsync(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PersonDetailDto>> GetPerson(int id)
    {
        var person = await _personService.GetPersonByIdAsync(id);
        if (person == null) return NotFound();

        var dto = new PersonDetailDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Gender = person.Gender,
            PersonalNumber = person.PersonalNumber,
            DateOfBirth = person.DateOfBirth
        };
        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonDetailDto>>> SearchPersons([FromQuery] PersonSearchRequestDto search)
    {
        var results = await _personService.SearchPersonsAsync(search.Name, search.Surname, search.PersonalNumber, search.Page, 100);
        return Ok(results.Select(p => new PersonDetailDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Gender = p.Gender,
            PersonalNumber = p.PersonalNumber,
            DateOfBirth = p.DateOfBirth
        }));
    }

    [HttpPost("related")]
    public async Task<IActionResult> AddRelatedPerson([FromBody] RelatedPerson relation)
    {
        await _personService.AddRelatedPersonAsync(relation);
        return Ok();
    }

    [HttpDelete("related/{relationId}")]
    public async Task<IActionResult> DeleteRelatedPerson(int relationId)
    {
        await _personService.DeleteRelatedPersonAsync(relationId);
        return NoContent();
    }
}

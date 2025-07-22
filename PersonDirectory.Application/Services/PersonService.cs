using PersonDirectory.Application.Interfaces;
using PersonDirectory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.Application.Services;
public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;

    public PersonService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public Task<int> AddPersonAsync(Person person) => _personRepository.AddPersonAsync(person);

    public Task UpdatePersonAsync(Person person) => _personRepository.UpdatePersonAsync(person);

    public Task DeletePersonAsync(int personId) => _personRepository.DeletePersonAsync(personId);

    public Task<Person?> GetPersonByIdAsync(int personId) => _personRepository.GetPersonByIdAsync(personId);

    public Task<IEnumerable<Person>> SearchPersonsAsync(string? name, string? lastName, string? personalNumber, int page, int pageSize)
        => _personRepository.SearchPersonsAsync(name, lastName, personalNumber, page, pageSize);

    public Task AddRelatedPersonAsync(RelatedPerson relation) => _personRepository.AddRelatedPersonAsync(relation);

    public Task DeleteRelatedPersonAsync(int relationId) => _personRepository.DeleteRelatedPersonAsync(relationId);

    public Task<bool> ExistsByPersonalNumberAsync(string personalNumber) => _personRepository.ExistsByPersonalNumberAsync(personalNumber);
}

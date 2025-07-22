using PersonDirectory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.Application.Interfaces
{
    public interface IPersonService
    {
        Task<int> AddPersonAsync(Person person);
        Task UpdatePersonAsync(Person person);
        Task DeletePersonAsync(int personId);
        Task<Person?> GetPersonByIdAsync(int personId);
        Task<IEnumerable<Person>> SearchPersonsAsync(string? name, string? lastName, string? personalNumber, int page, int pageSize);
        Task AddRelatedPersonAsync(RelatedPerson relation);
        Task DeleteRelatedPersonAsync(int relationId);
        Task<bool> ExistsByPersonalNumberAsync(string  personalNumber);
    }

}

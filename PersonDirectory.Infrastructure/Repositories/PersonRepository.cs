using Dapper;
using PersonDirectory.Application.DTOs;
using PersonDirectory.Application.Interfaces;
using PersonDirectory.Domain.Entities;
using PersonDirectory.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.Infrastructure.Repositories;
public class PersonRepository : IPersonRepository
{
    private readonly AppDbContext _context;

    public PersonRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddPersonAsync(Person person)
    {
        using var connection = _context.CreateConnection();
        var sql = @"
            INSERT INTO Persons (FirstName, LastName, Gender, PersonalNumber, DateOfBirth)
            VALUES (@FirstName, @LastName, @Gender, @PersonalNumber, @DateOfBirth);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";
        return await connection.QuerySingleAsync<int>(sql, person);
    }

    public async Task UpdatePersonAsync(Person person)
    {
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(@"
            UPDATE Persons
            SET FirstName = @FirstName,
                LastName = @LastName,
                Gender = @Gender,
                PersonalNumber = @PersonalNumber,
                DateOfBirth = @DateOfBirth
            WHERE Id = @Id
        ", person);
    }

    public async Task DeletePersonAsync(int personId)
    {
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync("DELETE FROM Persons WHERE Id = @Id", new { Id = personId });
    }

    public async Task<Person?> GetPersonByIdAsync(int personId)
    {
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Person>("SELECT * FROM Persons WHERE Id = @Id", new { Id = personId });
    }

    public async Task<IEnumerable<Person>> SearchPersonsAsync(string? name, string? lastName, string? personalNumber, int page, int pageSize)
    {
        using var connection = _context.CreateConnection();
        var sql = @"
            SELECT * FROM Persons
            WHERE (@Name IS NULL OR FirstName LIKE '%' + @Name + '%')
              AND (@LastName IS NULL OR LastName LIKE '%' + @LastName + '%')
              AND (@PersonalNumber IS NULL OR PersonalNumber LIKE '%' + @PersonalNumber + '%')
            ORDER BY Id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ";

        return await connection.QueryAsync<Person>(sql, new
        {
            Name = name,
            LastName = lastName,
            PersonalNumber = personalNumber,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize
        });
    }

    public async Task AddRelatedPersonAsync(RelatedPerson relation)
    {
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(@"
            INSERT INTO RelatedPersons (PersonId, RelatedPersonId, RelationType)
            VALUES (@PersonId, @RelatedPersonId, @RelationType)
        ", relation);
    }

    public async Task DeleteRelatedPersonAsync(int relationId)
    {
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync("DELETE FROM RelatedPersons WHERE Id = @Id", new { Id = relationId });
    }

    public async Task<bool> ExistsByPersonalNumberAsync(string personalNumber)
    {
        using var connection = _context.CreateConnection();
        var exists = await connection.QuerySingleOrDefaultAsync<int>(
            "SELECT 1 FROM Persons WHERE PersonalNumber = @PersonalNumber",
            new { PersonalNumber = personalNumber });
        return exists == 1;
    }
}
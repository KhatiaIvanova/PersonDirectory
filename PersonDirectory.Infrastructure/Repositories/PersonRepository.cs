using Dapper;
using PersonDirectory.Application.Interfaces;
using PersonDirectory.Domain.Entities;
using PersonDirectory.Domain.Enums;
using PersonDirectory.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
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
        var sql = @"
            INSERT INTO Persons (FirstName, LastName, Gender, PersonalNumber, DateOfBirth)
            VALUES (@FirstName, @LastName, @Gender, @PersonalNumber, @DateOfBirth);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        using var connection = await _context.CreateConnectionAsync();

        // პარამეტრებში პირდაპირ enum-ს ასობით ჩავასვათ, DB-ში უნდა იყოს nvarchar/varchar ტიპი
        return await connection.QuerySingleAsync<int>(sql, new
        {
            person.FirstName,
            person.LastName,
            Gender = person.Gender.ToString(),
            person.PersonalNumber,
            person.DateOfBirth
        });
    }

    public async Task UpdatePersonAsync(Person person)
    {
        var sql = @"
            UPDATE Persons SET
                FirstName = @FirstName,
                LastName = @LastName,
                Gender = @Gender,
                PersonalNumber = @PersonalNumber,
                DateOfBirth = @DateOfBirth
            WHERE Id = @Id;
        ";

        using var connection = await _context.CreateConnectionAsync();

        await connection.ExecuteAsync(sql, new
        {
            person.FirstName,
            person.LastName,
            Gender = person.Gender.ToString(),
            person.PersonalNumber,
            person.DateOfBirth,
            person.Id
        });
    }

    public async Task DeletePersonAsync(int personId)
    {
        const string sql = "DELETE FROM Persons WHERE Id = @Id";

        using var connection = await _context.CreateConnectionAsync();

        await connection.ExecuteAsync(sql, new { Id = personId });
    }

    public async Task<Person?> GetPersonByIdAsync(int personId)
    {
        const string sql = "SELECT * FROM Persons WHERE Id = @Id";

        using var connection = await _context.CreateConnectionAsync();

        var person = await connection.QuerySingleOrDefaultAsync<Person>(sql, new { Id = personId });

        if (person == null)
            return null;

        // თუ გინდა Gender string-იდან enum-ში გადაყვანა (მაგ: Dapper-ით პირდაპირ არ გადადის)
        person.Gender = Enum.Parse<Gender>(person.Gender.ToString());

        return person;
    }

    public async Task<IEnumerable<Person>> SearchPersonsAsync(string? name, string? lastName, string? personalNumber, int page, int pageSize)
    {
        var sql = @"
            SELECT * FROM Persons
            WHERE (@Name IS NULL OR FirstName LIKE '%' + @Name + '%')
              AND (@LastName IS NULL OR LastName LIKE '%' + @LastName + '%')
              AND (@PersonalNumber IS NULL OR PersonalNumber LIKE '%' + @PersonalNumber + '%')
            ORDER BY Id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ";

        using var connection = await _context.CreateConnectionAsync();

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
        const string sql = @"
            INSERT INTO RelatedPersons (PersonId, RelatedPersonId, RelationType)
            VALUES (@PersonId, @RelatedPersonId, @RelationType);
        ";

        using var connection = await _context.CreateConnectionAsync();

        await connection.ExecuteAsync(sql, new
        {
            relation.PersonId,
            relation.RelatedToPersonId,
            RelationType = relation.RelationType.ToString()
        });
    }

    public async Task DeleteRelatedPersonAsync(int relationId)
    {
        const string sql = "DELETE FROM RelatedPersons WHERE Id = @Id";

        using var connection = await _context.CreateConnectionAsync();

        await connection.ExecuteAsync(sql, new { Id = relationId });
    }

    public async Task<bool> ExistsByPersonalNumberAsync(string personalNumber)
    {
        const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Persons WHERE PersonalNumber = @PersonalNumber) THEN 1 ELSE 0 END";

        using var connection = await _context.CreateConnectionAsync();

        var exists = await connection.QuerySingleAsync<int>(sql, new { PersonalNumber = personalNumber });

        return exists == 1;
    }
}

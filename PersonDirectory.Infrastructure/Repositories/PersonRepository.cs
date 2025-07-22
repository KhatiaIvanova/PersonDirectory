using Dapper;
using PersonDirectory.Application.Interfaces;
using PersonDirectory.Domain.Entities;
using PersonDirectory.Domain.Enums;
using PersonDirectory.Infrastructure.Data;

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
        string GenderEnumValue = person.Gender.ToString();
        using var connection = _context.CreateConnection();
        var sql = @"
            INSERT INTO Persons (FirstName, LastName, Gender, PersonalNumber, DateOfBirth)
            VALUES (@FirstName, @LastName, @GenderEnumValue, @PersonalNumber, @DateOfBirth);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";
        return await connection.QuerySingleAsync<int>(sql, new { person, GenderEnumValue });
    }

    public async Task UpdatePersonAsync(Person person)
    {
        string GenderEnumValue = person.Gender.ToString();
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(@"
            UPDATE Persons
            SET FirstName = @FirstName,
                LastName = @LastName,
                Gender = @GenderEnumValue,
                PersonalNumber = @PersonalNumber,
                DateOfBirth = @DateOfBirth
            WHERE Id = @Id
        ", new { person, GenderEnumValue });
    }

    public async Task DeletePersonAsync(int personId)
    {
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync("DELETE FROM Persons WHERE Id = @Id", new { Id = personId });
    }

    public async Task<Person?> GetPersonByIdAsync(int personId)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<Person>("SELECT * FROM Persons WHERE Id = @Id", new { Id = personId });
        if (result == null)
            return null;

        return new Person
        {
            Id = result.Id,
            FirstName = result.FirstName,
            LastName = result.LastName,
            Gender = Enum.Parse<Gender>(result.Gender.ToString()),
            PersonalNumber = result.PersonalNumber,
            DateOfBirth = result.DateOfBirth
        };
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
        string relationEnum = relation.RelationType.ToString();
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(@"
            INSERT INTO RelatedPersons (PersonId, RelatedPersonId, RelationType)
            VALUES (@PersonId, @RelatedPersonId, @relationEnum)
        ", new { relation, relationEnum });
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
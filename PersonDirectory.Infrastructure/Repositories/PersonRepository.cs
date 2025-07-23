using Dapper;
using PersonDirectory.Application.Interfaces;
using PersonDirectory.Domain.Entities;
using PersonDirectory.Domain.Enums;
using PersonDirectory.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        using var transaction = connection.BeginTransaction();

        try
        {
            var personId = await connection.QuerySingleAsync<int>(sql, new
            {
                person.FirstName,
                person.LastName,
                Gender = person.Gender.ToString(),
                person.PersonalNumber,
                person.DateOfBirth
            }, transaction);

            if (person.PhoneNumbers?.Any() == true)
            {
                const string phoneSql = @"
                    INSERT INTO PhoneNumbers (PersonId, Number, PhoneType)
                    VALUES (@PersonId, @Number, @Type);
                ";

                foreach (var phone in person.PhoneNumbers)
                {
                    await connection.ExecuteAsync(phoneSql, new
                    {
                        PersonId = personId,
                        phone.Number,
                        Type = phone.Type.ToString()
                    }, transaction);
                }
            }

            transaction.Commit();
            return personId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
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
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql, new
            {
                person.FirstName,
                person.LastName,
                Gender = person.Gender.ToString(),
                person.PersonalNumber,
                person.DateOfBirth,
                person.Id
            }, transaction);

            const string deletePhones = "DELETE FROM PhoneNumbers WHERE PersonId = @PersonId";
            await connection.ExecuteAsync(deletePhones, new { PersonId = person.Id }, transaction);

            if (person.PhoneNumbers?.Any() == true)
            {
                const string insertPhones = @"
                    INSERT INTO PhoneNumbers (PersonId, Number, PhoneType)
                    VALUES (@PersonId, @Number, @Type);
                ";

                foreach (var phone in person.PhoneNumbers)
                {
                    await connection.ExecuteAsync(insertPhones, new
                    {
                        PersonId = person.Id,
                        phone.Number,
                        Type = phone.Type.ToString()
                    }, transaction);
                }
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task DeletePersonAsync(int personId)
    {
        const string deletePhones = "DELETE FROM PhoneNumbers WHERE PersonId = @Id";
        const string deletePerson = "DELETE FROM Persons WHERE Id = @Id";

        using var connection = await _context.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(deletePhones, new { Id = personId }, transaction);
            await connection.ExecuteAsync(deletePerson, new { Id = personId }, transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<Person?> GetPersonByIdAsync(int personId)
    {
        const string personSql = "SELECT * FROM Persons WHERE Id = @Id";
        const string phonesSql = "SELECT * FROM PhoneNumbers WHERE PersonId = @PersonId";

        using var connection = await _context.CreateConnectionAsync();

        var person = await connection.QuerySingleOrDefaultAsync<Person>(personSql, new { Id = personId });
        if (person == null) return null;

        person.Gender = Enum.Parse<Gender>(person.Gender.ToString());

        var phones = await connection.QueryAsync<PhoneNumber>(phonesSql, new { PersonId = personId });
        person.PhoneNumbers = phones.ToList();

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

        var people = await connection.QueryAsync<Person>(sql, new
        {
            Name = name,
            LastName = lastName,
            PersonalNumber = personalNumber,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize
        });

        var list = people.ToList();
        foreach (var person in list)
        {
            const string phonesSql = "SELECT * FROM PhoneNumbers WHERE PersonId = @PersonId";
            var phones = await connection.QueryAsync<PhoneNumber>(phonesSql, new { PersonId = person.Id });
            person.PhoneNumbers = phones.ToList();
        }

        return list;
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
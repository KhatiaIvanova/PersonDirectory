using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.Infrastructure.Data;
public class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        using var connection = context.CreateConnection();
        var people = await connection.QueryAsync<int>("SELECT TOP 1 1 FROM Persons");
        if (!people.Any())
        {
            await connection.ExecuteAsync(@"
                INSERT INTO Persons (FirstName, LastName, Gender, PersonalNumber, DateOfBirth) VALUES
                ('ნინო', 'კახიშვილი', 'ქალი', '01001001010', '1990-01-01'),
                ('გიორგი', 'მამულაშვილი', 'კაცი', '02002002020', '1985-05-05'),
                ('ანა', 'ჩიტაძე', 'ქალი', '03003003030', '1992-12-12')
            ");
        }
    }
}


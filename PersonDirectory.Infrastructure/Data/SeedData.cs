using Dapper;

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
                (N'ნინო', N'კახიშვილი', N'ქალი', '01001001010', '1990-01-01'),
                (N'გიორგი', N'მამულაშვილი', N'კაცი', '02002002020', '1985-05-05'),
                (N'ანა', N'ჩიტაძე', N'ქალი', '03003003030', '1992-12-12')
            ");
        }
    }
}


using Dapper;
using System.Linq;
using System.Threading.Tasks;

namespace PersonDirectory.Infrastructure.Data;
public class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        using var connection = await context.CreateConnectionAsync();

        var exists = await connection.QuerySingleOrDefaultAsync<int?>("SELECT TOP 1 1 FROM Persons");

        if (!exists.HasValue)
        {
            using var transaction = connection.BeginTransaction();

            try
            {
                var insertPersonsSql = @"
                    INSERT INTO Persons (FirstName, LastName, Gender, PersonalNumber, DateOfBirth)
                    OUTPUT INSERTED.Id
                    VALUES
                    (N'ნინო', N'კახიშვილი', N'ქალი', '01001001010', '1990-01-01'),
                    (N'გიორგი', N'მამულაშვილი', N'კაცი', '02002002020', '1985-05-05'),
                    (N'ანა', N'ჩიტაძე', N'ქალი', '03003003030', '1992-12-12');
                ";

                var personIds = (await connection.QueryAsync<int>(insertPersonsSql, transaction: transaction)).ToList();

                var insertPhonesSql = @"
                    INSERT INTO PhoneNumbers (PersonId, Number, Type) VALUES (@PersonId, @Number, @Type);
                ";

                var phones = new[]
                {
                    new { PersonId = personIds[0], Number = "+995555123456", Type = "მობილური" },
                    new { PersonId = personIds[0], Number = "+995555654321", Type = "მობილური" },
                    new { PersonId = personIds[1], Number = "+995599987654", Type = "მობილური" },
                    new { PersonId = personIds[2], Number = "+995577112233", Type = "მობილური" }
                };

                foreach (var phone in phones)
                {
                    await connection.ExecuteAsync(insertPhonesSql, phone, transaction);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

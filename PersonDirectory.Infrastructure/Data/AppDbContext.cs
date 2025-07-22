using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using System;
using System.Data;
using System.Threading.Tasks;

namespace PersonDirectory.Infrastructure.Data
{
    public class AppDbContext : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly AsyncRetryPolicy _retryPolicy;

        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            _retryPolicy = Policy
                .Handle<SqlException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} due to: {exception.Message}. Waiting {timespan}.");
                    });
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);

            await _retryPolicy.ExecuteAsync(async () =>
            {
                await connection.OpenAsync();
            });

            return connection;
        }

        public void Dispose()
        {
            
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreLibrary.Tests.IntegrationTests.Mocks
{
    public class MockDbContextFactory<T>
        : IDbContextFactory<T> where T : DbContext
    {
        private readonly DbContextOptions<T> _options;

        public MockDbContextFactory(string databaseName = "InMemoryTestDb",
            QueryTrackingBehavior queryBehavior = QueryTrackingBehavior.NoTracking,
            bool logToConsole = false)
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(databaseName)
                .UseQueryTrackingBehavior(queryBehavior);

            if (logToConsole)
            {
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            }

            _options = optionsBuilder.Options;
        }

        public T CreateDbContext()
        {
            var context = (T?)Activator.CreateInstance(typeof(T), _options);

            return context ?? throw new NullReferenceException();
        }
    }
}

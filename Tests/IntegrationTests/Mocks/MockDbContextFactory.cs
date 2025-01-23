using Microsoft.EntityFrameworkCore;

namespace CoreLibrary.Tests.IntegrationTests.Mocks
{
    public class MockDbContextFactory<T>
        : IDbContextFactory<T> where T : DbContext
    {
        private readonly DbContextOptions<T> _options;

        public MockDbContextFactory(QueryTrackingBehavior queryBehavior = QueryTrackingBehavior.NoTracking,
            bool sensitiveDataLogging = false)
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>()
                .UseSqlite("DataSource=:memory:")
                .UseQueryTrackingBehavior(queryBehavior);

            if (sensitiveDataLogging)
            {
                optionsBuilder.EnableSensitiveDataLogging();
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

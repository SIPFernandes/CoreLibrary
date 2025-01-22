using Microsoft.EntityFrameworkCore;

namespace CoreLibrary.Tests.IntegrationTests.Mocks
{
    public class MockDbContextFactory<T>(string databaseName = "InMemoryTestDb",
        QueryTrackingBehavior queryBehavior = QueryTrackingBehavior.NoTracking) 
        : IDbContextFactory<T> where T : DbContext
    {
        private readonly DbContextOptions<T> _options = new DbContextOptionsBuilder<T>()
            .UseInMemoryDatabase(databaseName)
            .UseQueryTrackingBehavior(queryBehavior)
            
            .Options;

        public T CreateDbContext()
        {
            var context = (T?)Activator.CreateInstance(typeof(T), _options);

            return context ?? throw new NullReferenceException();
        }
    }
}

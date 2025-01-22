using CoreLibrary.Tests.IntegrationTests.Mocks;
using Microsoft.EntityFrameworkCore;

namespace CoreLibrary.Tests.IntegrationTests.Fixtures
{
    public abstract class RepositoryFixture<T> : IAsyncDisposable
        where T : DbContext
    {
        public readonly MockDbContextFactory<T> _dbContextFact;
        public readonly T _context;

        public RepositoryFixture(string databaseName = "InMemoryTestDb")
        {
            _dbContextFact = new MockDbContextFactory<T>(databaseName);

            _context = _dbContextFact.CreateDbContext();
        }


        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}

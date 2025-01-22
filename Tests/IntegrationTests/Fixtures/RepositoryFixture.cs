﻿using CoreLibrary.Tests.IntegrationTests.Mocks;
using Microsoft.EntityFrameworkCore;

namespace CoreLibrary.Tests.IntegrationTests.Fixtures
{
    public abstract class RepositoryFixture<T>
        where T : DbContext
    {
        public readonly MockDbContextFactory<T> _dbContextFact;
        public readonly T _context;

        public RepositoryFixture()
        {
            _dbContextFact = new MockDbContextFactory<T>("InMemoryTestDbRepository");

            _context = _dbContextFact.CreateDbContext();
        }

        public void Dispose()
        {
            _context.SaveChanges();
            _context.Dispose();
        }
    }
}

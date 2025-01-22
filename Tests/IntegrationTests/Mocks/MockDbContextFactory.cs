﻿using Microsoft.EntityFrameworkCore;

namespace CoreLibrary.Tests.IntegrationTests.Mocks
{
    public class MockDbContextFactory<T> : IDbContextFactory<T>
        where T : DbContext
    {
        private readonly DbContextOptions<T> _options;

        public MockDbContextFactory(string databaseName = "InMemoryTestDb")
        {
            _options = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(databaseName)
                .Options;
        }

        public T CreateDbContext()
        {
            var context = (T?)Activator.CreateInstance(typeof(T), _options);

            return context ?? throw new NullReferenceException();
        }
    }
}

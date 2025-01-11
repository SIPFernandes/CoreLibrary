using AutoMapper;
using CoreLibrary.Core.DocumentEntities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CoreLibrary.Infrastructure.NoSQL
{
    public class MongoDBService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<DocumentBaseEntity> _schemaCollection;

        public MongoDBService(IMapper mapper,
            IOptions<MongoDBSettings> mongoDBSettings)
        {
            _mapper = mapper;

            var mongoClient = new MongoClient(
                mongoDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                mongoDBSettings.Value.DatabaseName);

            _schemaCollection = mongoDatabase.GetCollection<DocumentBaseEntity>(
                mongoDBSettings.Value.CollectionName);
        }

        public async Task CreateAsync<T>(T doc) where T : DocumentBaseEntity =>
            await GetCollection<T>().InsertOneAsync(doc);

        public async Task UpdateAsync<T>(string id, T doc) where T : DocumentBaseEntity =>
            await GetCollection<T>().ReplaceOneAsync(x => x.Id == id, doc);

        public async Task RemoveAsync<T>(string id) where T : DocumentBaseEntity =>
            await GetCollection<T>().DeleteOneAsync(x => x.Id == id);

        private IMongoCollection<T> GetCollection<T>() where T : DocumentBaseEntity
        {
            IMongoCollection<T> result = (IMongoCollection<T>)_schemaCollection;

            //if (typeof(T) == typeof(ValidationSchema))
            //{
            //    result = (IMongoCollection<T>)_validationSchemaCollection;
            //}
            //else
            //{
            //    result = (IMongoCollection<T>)_validationSchemaCollection;
            //}

            return result;
        }
    }
}

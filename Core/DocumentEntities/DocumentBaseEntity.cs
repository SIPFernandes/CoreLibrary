using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreLibrary.Core.DocumentEntities
{
    public class DocumentBaseEntity(string name, string data, string creatorId)
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string? Id { get; set; }
        public string Name { get; set; } = name;
        public string Data { get; set; } = data;
        public bool IsDeleted { get; set; } = false;
        public virtual string CreatorId { get; protected set; } = creatorId;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}

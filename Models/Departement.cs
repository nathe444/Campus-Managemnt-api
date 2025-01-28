using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Department
{
    [BsonId]
    public ObjectId Id { get; set; } 
    public required string Name { get; set; }
    public required string Description { get; set; }
}

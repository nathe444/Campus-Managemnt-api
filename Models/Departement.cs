using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Department
{
    [BsonId]
    public ObjectId Id { get; set; } 
    public string Name { get; set; }
    public string Description { get; set; }
}

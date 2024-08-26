using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TaskManagementSystem.Models
{ 
    public class ManagementTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        //  public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int Priority { get; set; }
        public string? ListId { get; set; }
        public string? GroupId { get; set; }
        public List<string>? AssignedUsers { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ManagementList
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? GroupId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ManagementGroup
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }







}

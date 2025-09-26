using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PatientNotesService.Domain
{
    public class Note
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // Ce champ mappe _id de MongoDB
        public int PatId { get; set; }
        public string Content { get; set; }
    }

}

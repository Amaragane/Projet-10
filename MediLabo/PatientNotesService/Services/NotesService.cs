using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Services;
using DomainNote = PatientNotesService.Domain.Note;


public class Note
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string PatientId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}

public class NotesService
{
    private readonly IMongoCollection<DomainNote> _notesCollection;

    public NotesService(IOptions<MongoSettings> options)
    {
        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
        _notesCollection = mongoDatabase.GetCollection<DomainNote>("notes");
    }

    public List<DomainNote> GetNotesForPatient(int patientId) =>
        _notesCollection.Find(n => n.PatId == patientId).ToList();

    public DomainNote Create(DomainNote note)
    {
        _notesCollection.InsertOne(note);
        return note;
    }

    public void Update(int id, DomainNote updatedNote) =>
        _notesCollection.ReplaceOne(n => n.PatId == id, updatedNote);
    public async Task<List<DomainNote>> GetAllAsync() =>
    await _notesCollection.Find(_ => true).ToListAsync();

    public async Task InsertManyAsync(IEnumerable<DomainNote> notes) =>
        await _notesCollection.InsertManyAsync(notes);
    public void Delete(DomainNote note) =>
        _notesCollection.DeleteOne(n => n.Id == note.Id);
    public bool Exists(int noteId) =>
        _notesCollection.Find(n => n.PatId == noteId).Any();
    public DomainNote GetNote(int noteId) =>
        _notesCollection.Find(n => n.PatId == noteId).FirstOrDefault();

}

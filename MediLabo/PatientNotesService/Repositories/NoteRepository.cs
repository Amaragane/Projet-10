using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PatientNotesService.Domain;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NoteRepository : INoteRepository
{
    private readonly IMongoCollection<Note> _notesCollection;

    public NoteRepository(IOptions<MongoSettings> options)
    {
        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
        _notesCollection = mongoDatabase.GetCollection<Note>("notes");
    }

    public async Task<List<Note>> GetNotesForPatientAsync(int patientId) =>
        await _notesCollection.Find(n => n.PatId == patientId).ToListAsync();

    public async Task<Note> CreateAsync(Note note)
    {
        await _notesCollection.InsertOneAsync(note);
        return note;
    }

    public async Task UpdateAsync(string id, Note updatedNote) =>
        await _notesCollection.ReplaceOneAsync(n => n.Id == id, updatedNote);

    public async Task DeleteAsync(string id) =>
        await _notesCollection.DeleteOneAsync(n => n.Id == id);

    public async Task<bool> ExistsAsync(string id) =>
        await _notesCollection.Find(n => n.Id == id).AnyAsync();

    public async Task<Note> GetNoteByIdAsync(string id) =>
        await _notesCollection.Find(n => n.Id == id).FirstOrDefaultAsync();

    public async Task<List<Note>> GetAllAsync() =>
        await _notesCollection.Find(_ => true).ToListAsync();

    public async Task InsertManyAsync(IEnumerable<Note> notes) =>
        await _notesCollection.InsertManyAsync(notes);
}

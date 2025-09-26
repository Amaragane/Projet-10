using System.Collections.Generic;
using System.Threading.Tasks;
using PatientNotesService.Domain;

public class NotesService
{
    private readonly INoteRepository _noteRepository;

    public NotesService(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public Task<List<Note>> GetNotesForPatientAsync(int patientId) =>
        _noteRepository.GetNotesForPatientAsync(patientId);

    public Task<Note> CreateAsync(Note note) =>
        _noteRepository.CreateAsync(note);

    public Task UpdateAsync(string id, Note updatedNote) =>
        _noteRepository.UpdateAsync(id, updatedNote);

    public Task DeleteAsync(string id) =>
        _noteRepository.DeleteAsync(id);

    public Task<bool> ExistsAsync(string id) =>
        _noteRepository.ExistsAsync(id);

    public Task<Note> GetNoteByIdAsync(string id) =>
        _noteRepository.GetNoteByIdAsync(id);

    public Task<List<Note>> GetAllAsync() =>
        _noteRepository.GetAllAsync();

    public Task InsertManyAsync(IEnumerable<Note> notes) =>
        _noteRepository.InsertManyAsync(notes);
}

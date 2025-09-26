using System.Collections.Generic;
using System.Threading.Tasks;
using PatientNotesService.Domain;

public interface INoteRepository
{
    Task<List<Note>> GetNotesForPatientAsync(int patientId);
    Task<Note> CreateAsync(Note note);
    Task UpdateAsync(string id, Note updatedNote);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<Note> GetNoteByIdAsync(string id);
    Task<List<Note>> GetAllAsync();
    Task InsertManyAsync(IEnumerable<Note> notes);
}

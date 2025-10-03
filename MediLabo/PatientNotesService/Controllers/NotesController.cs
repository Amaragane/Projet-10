using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DomainNote = PatientNotesService.Domain.Note;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly NotesService _notesService;

    public NotesController(NotesService notesService)
    {
        _notesService = notesService;
    }

    // GET api/notes/{patientId}
    [HttpGet("{patientId}")]
    public async Task<IActionResult> GetNotes([FromRoute] int patientId)
    {
        var notes = await _notesService.GetNotesForPatientAsync(patientId);
        return Ok(notes);
    }


    // POST api/notes/{patientId}
    [HttpPost("{patientId}")]
    public async Task<IActionResult> CreateNote([FromRoute] int patientId, [FromBody] string content)
    {
        var newNote = new DomainNote {PatId=patientId, Content= content };
        var createdNote = await _notesService.CreateAsync(newNote);
        return CreatedAtAction(nameof(GetNotes), new { patientId = createdNote.PatId }, createdNote);
    }

    // DELETE api/notes/{noteId}
    [HttpDelete("{noteId}")]
    public async Task<IActionResult> DeleteNote( string noteId)
    {
        var noteToDelete = await _notesService.GetNoteByIdAsync(noteId);
        if (noteToDelete == null)
        {
            return NotFound();
        }
        await _notesService.DeleteAsync(noteId);
        return Ok();
    }
}

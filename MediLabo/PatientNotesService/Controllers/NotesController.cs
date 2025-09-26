using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
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

    [HttpGet]
    public IActionResult GetNotes([FromQuery] int patientId)
    {
        var notes = _notesService.GetNotesForPatient(patientId);
        return Ok(notes);
    }

    [HttpPost]
    public IActionResult CreateNote([FromBody] DomainNote note)
    {
        var createdNote = _notesService.Create(note);
        return CreatedAtAction(nameof(GetNotes), new { patientId = createdNote.PatId }, createdNote);
    }
    [HttpDelete]
    public IActionResult DeleteNote([FromQuery] int noteId)
    {
        var noteToDelete = _notesService.GetNote(noteId);
        if (noteToDelete == null)
        {
            return NotFound();
        }
        _notesService.Delete(noteToDelete);
        return Ok();
    }
}

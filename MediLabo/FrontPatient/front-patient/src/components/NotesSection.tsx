import React from 'react';

interface Note {
  _id: string;
  patId: number;
  content: string;
}

interface Props {
  notes: Note[];
  newNote: string;
  setNewNote: React.Dispatch<React.SetStateAction<string>>;
  onAddNote: () => void;
  error: string;
}

export default function NotesSection({ notes, newNote, setNewNote, onAddNote, error }: Props) {
  return (
    <>
      <h3>Notes du patient</h3>
      <ul>
        {notes.map(note => <li key={note._id}>{note.content}</li>)}
      </ul>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <textarea
        rows={3}
        value={newNote}
        onChange={e => setNewNote(e.target.value)}
        placeholder="Ajouter une note..."
      />
      <button onClick={onAddNote} disabled={!newNote.trim()}>Ajouter une note</button>
    </>
  );
}

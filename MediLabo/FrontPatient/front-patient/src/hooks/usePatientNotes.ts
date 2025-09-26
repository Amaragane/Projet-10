import { useState, useEffect } from 'react';

export default function usePatientNotes(patientId: string) {
  const [notes, setNotes] = useState<any[]>([]);
  const [newNote, setNewNote] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    const token = localStorage.getItem('jwtToken');
    if (!token || !patientId) return;
    fetch(`https://localhost:5002/api/notes/${patientId}`, {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(res => res.json())
      .then(setNotes)
      .catch(() => setNotes([]));
  }, [patientId]);

  function addNote() {
    const token = localStorage.getItem('jwtToken');
    if (!token || !patientId || !newNote.trim()) {
      setError('Note invalide ou authentification requise');
      return;
    }
    fetch(`https://localhost:5002/api/notes/${patientId}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify({ content: newNote })
    })
      .then(res => res.json())
      .then(addedNote => {
        setNotes(prev => [...prev, addedNote]);
        setNewNote('');
        setError('');
      })
      .catch(() => setError('Erreur lors de l\'ajout de la note'));
  }

  return { notes, newNote, setNewNote, error, addNote };
}

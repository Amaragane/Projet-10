import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import PatientInfo from '../components/PatientInfo';
import NotesSection from '../components/NotesSection';
import usePatientData from '../hooks/usePatientData';
import usePatientNotes from '../hooks/usePatientNotes';

export default function PatientDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const { patient, formData, setFormData, error: patientError, handleChange, handleSubmit, handleDelete } = usePatientData(id!, navigate);
  const { notes, newNote, setNewNote, error: notesError, addNote } = usePatientNotes(id!);

  if (!patient) return <div>Chargement...</div>;

  return (
    <div style={{ display: 'flex', gap: '2rem' }}>
      <div style={{ flex: 2 }}>
        <PatientInfo
          patient={formData}
          onChange={handleChange}
          onSubmit={handleSubmit}
          onDelete={handleDelete}
          error={patientError}
          navigateBack={() => navigate('/patients')}
        />
      </div>
      <div style={{ flex: 1, borderLeft: '1px solid #ccc', paddingLeft: '1rem' }}>
        <NotesSection
          notes={notes}
          newNote={newNote}
          setNewNote={setNewNote}
          onAddNote={addNote}
          error={notesError}
        />
      </div>
    </div>
  );
}

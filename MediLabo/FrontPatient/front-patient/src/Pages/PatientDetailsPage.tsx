import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { PatientForm } from '../components/PatientForm';
interface Note {
  _id: number;
  patId : number;
  content: string;
}
interface Patient {
  id: number;
  firstName: string;
  lastName: string;
  dateOfBirth: Date;
  gender: string;
  address: string | null; // Corrige la propriété ici
  phoneNumber: string | null;
}

export default function PatientDetailsPage() {
    const [successMessage, setSuccessMessage] = React.useState('');
  const { id } = useParams<{ id: string }>();
  const [patient, setPatient] = useState<Patient | null>(null);
  const [notes, setNotes] = useState<Note[]>([]);
  const [newNote, setNewNote] = useState('');
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    gender: '',
    address: '',
    phoneNumber: ''
  });
  const [error, setError] = useState('');
  const navigate = useNavigate();
  useEffect(() => {
    const token = localStorage.getItem('jwtToken');
    if (!token || !id) return;
    fetch(`https://localhost:5002/api/patients/${id}/notes`, {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(res => res.json())
      .then(setNotes)
      .catch(() => setNotes([]));
  }, [id]);
  useEffect(() => {
    const token = localStorage.getItem('jwtToken');
    if (!token || !id) {
      navigate('/');
      return;
    }
    fetch(`https://localhost:5002/api/patients/${id}`, {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(res => res.json())
      .then((data: Patient) => {
        setPatient(data);
        setFormData({
          firstName: data.firstName,
          lastName: data.lastName,
          dateOfBirth: new Date(data.dateOfBirth).toISOString().split('T')[0],
          gender: data.gender,
          address: data.address || '',
          phoneNumber: data.phoneNumber || ''
        });
      })
      .catch(() => setError('Erreur lors du chargement du patient'));
  }, [id, navigate]);

  function handleChange(field: string, value: string) {
    setFormData(prev => ({ ...prev, [field]: value }));
  }

  function handleSubmit() {
    // Ici, tu fais ton fetch PUT avec formData
    const token = localStorage.getItem('jwtToken');
    if (!token || !id) {
      setError('Authentification requise');
      return;
    }
    fetch(`https://localhost:5002/api/patients/${id}`, {
      method: "PUT",
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({
        ...formData,
        dateOfBirth: new Date(formData.dateOfBirth).toISOString(),
        id
      })
    }).then(res => {
      if (res.ok) {
        setSuccessMessage('Enregistrement effectué avec succès');
        setTimeout(() => setSuccessMessage(''), 3000);
        // window.location.reload();
        // navigate(`/patients/${id}`);
      } else {
        res.text().then(text => setError(`Erreur modification: ${text}`));
      }
    });
  }
    function handleDelete() {
    const token = localStorage.getItem('jwtToken');
    if (!token || !id) {
      setError('Authentification requise');
      return;
    }
    fetch(`https://localhost:5002/api/patients/${id}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${token}`
      }
    }).then(res => {
                  if (res.status === 401) {
    // Token expiré ou invalide
    localStorage.removeItem('jwtToken');
    window.location.href = '/';
    throw new Error('Unauthorized');
  }
      if (res.ok) {
        navigate('/patients');
      } else {
        res.text().then(text => setError(`Erreur suppression : ${text}`));
      }
    }).catch(() => setError('Erreur réseau'));
  }
  if (!patient) return <div>Chargement...</div>;

  return (
    <div>
      <h2>Détails de : {patient.firstName} {patient.lastName}</h2>
      <PatientForm
        patient={formData}
        onChange={handleChange}
        onSubmit={handleSubmit}
      />
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <button onClick={() => navigate('/patients')}>Retour à la liste</button>
      <button onClick={handleDelete} style={{ color: 'red', marginLeft: '10px' }}>Supprimer</button>
      {successMessage && <p style={{ color: 'green' }}>{successMessage}</p>}
    </div>
  );
}

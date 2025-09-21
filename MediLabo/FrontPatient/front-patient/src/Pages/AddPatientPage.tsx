import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { PatientForm } from '../components/PatientForm';

export default function AddPatientPage() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    gender: '',
    address: '',
    phoneNumber: ''
  });
  const [error, setError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');

  function handleChange(field: string, value: string) {
    setFormData(prev => ({ ...prev, [field]: value }));
  }

  function handleSubmit() {
    const token = localStorage.getItem('jwtToken');
    if (!token) {
      setError('Authentification requise');
      return;
    }
    fetch('https://localhost:5002/api/patients', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify({
        ...formData,
        dateOfBirth: new Date(formData.dateOfBirth).toISOString()
      }),
    })
      .then(res => {
          if (res.status === 401) {
    // Token expiré ou invalide
    localStorage.removeItem('jwtToken');
    window.location.href = '/';
    throw new Error('Unauthorized');
  }
        if (res.ok) {
          setSuccessMessage('Patient créé avec succès');
          setTimeout(() => {
            setSuccessMessage('');
            navigate('/patients');
          }, 2000);
        } else {
          res.text().then(text => setError(`Erreur création : ${text}`));
        }
      })
      .catch(() => setError('Erreur réseau'));
  }

  return (
    <div>
      <h2>Ajouter un patient</h2>
      <PatientForm patient={formData} onChange={handleChange} onSubmit={handleSubmit} />
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {successMessage && <p style={{ color: 'green' }}>{successMessage}</p>}
      <button onClick={() => navigate('/patients')}>Retour à la liste</button>
    </div>
  );
}

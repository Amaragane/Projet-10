import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

interface Patient {
  id: number;
  firstName: string;
  lastName: string;
}

export default function PatientsPage() {
  const [patients, setPatients] = useState<Patient[]>([]);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem('jwtToken');
    if (!token) {
      navigate('/');
      return;
    }
    fetch('https://localhost:5002/api/patients', {
      headers: { Authorization: `Bearer ${token}` }
    })
        .then(res => {
    if (res.status === 401 || res.status === 403) {
      // Token expiré ou invalide
      localStorage.removeItem('jwtToken');
      window.location.href = '/';
      throw new Error('Unauthorized');
    }
    return res.json();
  })
      .then(setPatients)
      .catch(() => setError('Erreur lors du chargement des patients'));
  }, [navigate]);

  return (
    <div>
      <h2>Liste des patients</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <ul>
        {patients.map(p => (
          <li key={p.id} onClick={() => navigate(`/patients/${p.id}`)} style={{ cursor: 'pointer' }}>
            {p.firstName} {p.lastName}
          </li>
        ))}
      </ul>
      <div>
          <button onClick={() => navigate('/patients/add')}>Ajouter un Patient</button>
      </div>
    </div>
  );
}

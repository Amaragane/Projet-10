import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

export default function usePatientData(id: string, navigate: any) {
  const [patient, setPatient] = useState<any>(null);
  const [formData, setFormData] = useState<any>({});
  const [error, setError] = useState('');
  
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
      .then(data => {
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
    setFormData((prev: any) => ({ ...prev, [field]: value }));
  }

  function handleSubmit() {
    const token = localStorage.getItem('jwtToken');
    if (!token || !id) {
      setError('Authentification requise');
      return;
    }
    fetch(`https://localhost:5002/api/patients/${id}`, {
      method: "PUT",
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify({
        ...formData,
        dateOfBirth: new Date(formData.dateOfBirth).toISOString(),
        id
      })
    }).then(res => {
      if (res.ok) {
        setError('');
        // Optionally show success message here
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
      headers: { Authorization: `Bearer ${token}` }
    }).then(res => {
      if (res.ok) {
        navigate('/patients');
      } else {
        res.text().then(text => setError(`Erreur suppression : ${text}`));
      }
    });
  }

  return { patient, formData, setFormData, error, handleChange, handleSubmit, handleDelete };
}

import React from 'react';

export interface PatientFormProps {
  patient: {
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    gender: string;
    address?: string;
    phoneNumber?: string;
  };
  onChange: (field: string, value: string) => void;
  onSubmit: () => void;
}

export function PatientForm({ patient, onChange, onSubmit }: PatientFormProps) {
  return (
<form onSubmit={e => { e.preventDefault(); onSubmit(); }}>
  <div>
    <label htmlFor="firstName">Prénom :</label>
    <input
      id="firstName"
      name="firstName"
      value={patient.firstName}
      onChange={e => onChange('firstName', e.target.value)}
    />
  </div>
  <div>
    <label htmlFor="lastName">Nom :</label>
    <input
      id="lastName"
      name="lastName"
      value={patient.lastName}
      onChange={e => onChange('lastName', e.target.value)}
    />
  </div>
  <div>
    <label htmlFor="dateOfBirth">Date de naissance :</label>
    <input
      id="dateOfBirth"
      name="dateOfBirth"
      type="date"
      value={patient.dateOfBirth}
      onChange={e => onChange('dateOfBirth', e.target.value)}
    />
  </div>
<div>
  <label htmlFor="gender">Sexe :</label>
  <select
    id="gender"
    name="gender"
    value={patient.gender}
    onChange={e => onChange('gender', e.target.value)}
    required
  >
    <option value="">Sélectionnez</option>
    <option value="M">Masculin</option>
    <option value="F">Féminin</option>
  </select>
</div>

  <div>
    <label htmlFor="address">Adresse :</label>
    <input
      id="address"
      name="address"
      value={patient.address}
      onChange={e => onChange('address', e.target.value)}
    />
  </div>
  <div>
    <label htmlFor="phoneNumber">Téléphone :</label>
    <input
      id="phoneNumber"
      name="phoneNumber"
      value={patient.phoneNumber}
      onChange={e => onChange('phoneNumber', e.target.value)}
    />
  </div>
  <button type="submit">Enregistrer</button>
</form>

  );
}

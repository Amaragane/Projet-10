import React from 'react';
import { PatientForm, PatientFormProps } from './PatientForm'; // adapter le chemin d'import selon ta structure

interface Props extends Omit<PatientFormProps, 'onSubmit'> {
  onSubmit: () => void;
  onDelete: () => void;
  error: string;
  navigateBack: () => void;
}

export default function PatientInfo({ patient, onChange, onSubmit, onDelete, error, navigateBack }: Props) {
  return (
    <>
      <h2>Détails de : {patient.firstName} {patient.lastName}</h2>
      
      {/* Utilisation du PatientForm en transmission des props */}
      <PatientForm patient={patient} onChange={onChange} onSubmit={onSubmit} />
      
      {error && <p style={{color: 'red'}}>{error}</p>}
      
      <button onClick={navigateBack}>Retour à la liste</button>
      <button onClick={onDelete} style={{color: 'red', marginLeft: '10px'}}>Supprimer</button>
    </>
  );
}

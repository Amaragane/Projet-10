import React from 'react';

interface Patient {
  dateOfBirth: string;
  gender: string;
}

interface Note {
  content: string;
}

interface Props {
  patient: Patient;
  notes: Note[];
}

const RiskLevelComponent: React.FC<Props> = ({ patient, notes }) => {
  // Fonction d'analyse du risque
  function calculateRiskLevel(patient: Patient, notes: Note[]) {
    const triggers = [
      "hémoglobine a1c",
      "microalbumine",
      "taille",
      "poids",
      "fumeur",
      "fumer",
      "fumeuse",
      "anormal",
      "cholestérol",
      "vertiges",
      "vertige",
      "rechute",
      "réaction",
      "anticorps"
    ];

    // Calcul âge
    const age = calculateAge(patient.dateOfBirth);


    // Trouver déclencheurs uniques dans les notes
    const foundTriggers = new Set<string>();
    notes.forEach(note => {
      const content = note.content.toLowerCase();
      triggers.forEach(trigger => {
        if (content.includes(trigger)) {
            console.log(trigger);
          foundTriggers.add(trigger);
        }
      });
    });

    const countTriggers = foundTriggers.size;
    console.log(countTriggers);
    const isMale = patient.gender.toLowerCase() === 'm';

    if (countTriggers === 0) return "None";
    if (countTriggers >= 2 && countTriggers <= 5 && age > 30) return "Borderline";

    if (age < 30) {
      if (isMale) {
        if (countTriggers >= 5) return "Early onset";
        if (countTriggers >= 3) return "In Danger";
      } else {
        if (countTriggers >= 7) return "Early onset";
        if (countTriggers >= 4) return "In Danger";
      }
    } else {
      if (countTriggers >= 8) return "Early onset";
      if (countTriggers === 6 || countTriggers === 7) return "In Danger";
    }

    return "None";
  }

  const riskLevel = calculateRiskLevel(patient, notes);

  return (
    <div>
      <h3>Niveau de risque : {riskLevel}</h3>
    </div>
  );
};
function calculateAge(birthDateString: string): number {
  const today = new Date();
  const birthDate = new Date(birthDateString);

  let age = today.getFullYear() - birthDate.getFullYear();

  const monthDifference = today.getMonth() - birthDate.getMonth();
  const dayDifference = today.getDate() - birthDate.getDate();

  if (monthDifference < 0 || (monthDifference === 0 && dayDifference < 0)) {
    age--;
  }

  return age;
}


export default RiskLevelComponent;

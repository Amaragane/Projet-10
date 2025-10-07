import React, { useEffect, useState } from 'react';

interface Props {
  patientId: string;
}

const RiskLevelComponent: React.FC<Props> = ({ patientId }) => {
  const [riskLevel, setRiskLevel] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const token = localStorage.getItem('jwtToken');
    if (!token || !patientId) return;

    fetch(`https://localhost:5002/api/risklevel/${patientId}`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      }
    })
      .then(res => {
        if (!res.ok) {
          throw new Error('Erreur lors de la récupération du niveau de risque');
        }
        return res.json();
      })
      .then(data => {
        // Adapte le champ selon la réponse du backend : ici 'level' ou le type renvoyé
        setRiskLevel(data.level || data);
      })
      .catch(err => {
        setError(err.message || 'Erreur inconnue');
        setRiskLevel(null);
      });
  }, [patientId]);

  if (error) return <div style={{color: 'red'}}>Erreur : {error}</div>;
  if (!riskLevel) return <div>Calcul du risque en cours...</div>;

  return (
    <div>
      <h3>Niveau de risque : {riskLevel}</h3>
    </div>
  );
};

export default RiskLevelComponent;

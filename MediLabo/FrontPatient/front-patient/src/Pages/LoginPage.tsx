import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleLogin = async () => {
    setError('');
    try {
      const res = await fetch('https://localhost:5002/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
      });
      if (res.status === 401) {
    // Token expiré ou invalide
    localStorage.removeItem('jwtToken');
    window.location.href = '/';
    throw new Error('Unauthorized');
  }
      if (res.ok) {
        const data = await res.json();
        localStorage.setItem('jwtToken', data.token);
        navigate('/patients');
      } else {
        setError('Erreur authentification');
      }
    } catch {
      setError('Erreur réseau');
    }
  };

  return (
    <div>
      <h2>Connexion</h2>
      <input type="email" value={email} onChange={e => setEmail(e.target.value)} placeholder="Email" />
      <input type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Mot de passe" />
      <button onClick={handleLogin}>Se connecter</button>
      {error && <p style={{ color: 'red' }}>{error}</p>}
    </div>
  );
}

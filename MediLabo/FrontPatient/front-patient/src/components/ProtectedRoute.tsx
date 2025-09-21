// components/ProtectedRoute.tsx
import { Navigate } from 'react-router-dom';
import {ReactElement}  from 'react';

export function ProtectedRoute({ children }: { children: ReactElement }) {
  const token = localStorage.getItem("jwtToken");
  if (!token) {
    return <Navigate to="/" replace />;
  }
  return children;
}

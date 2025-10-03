import { BrowserRouter, Routes, Route } from 'react-router-dom';
import LoginPage from './Pages/LoginPage';
import PatientsPage from './Pages/PatientsPage';
import PatientDetailsPage from './Pages/PatientDetailsPage';
import { ProtectedRoute } from './components/ProtectedRoute';
import AddPatientPage from './Pages/AddPatientPage';

function App() {
  return (
    <BrowserRouter>

<Routes>
  <Route path="/" element={<LoginPage />} />
  <Route
    path="/patients"
    element={
      <ProtectedRoute>
        <PatientsPage />
      </ProtectedRoute>
    }
  />
  <Route
    path="/patients/:id"
    element={
      <ProtectedRoute>
        <PatientDetailsPage />
      </ProtectedRoute>
    }
  />
    <Route
    path="/patients/add"
    element={
      <ProtectedRoute>
        <AddPatientPage />
      </ProtectedRoute>
    }
  />

</Routes>
    </BrowserRouter>
  );
}

export default App;

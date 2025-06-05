// src/App.jsx
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import Navbar from './components/Layout/Navbar';
import PrivateRoute from './components/Layout/PrivateRoute'; // Import PrivateRoute

// Pages
import LoginPage from './pages/Auth/LoginPage'; // This import will now correctly find the renamed file
import RegisterPage from './pages/Auth/RegisterPage'; // Corrected path and name
import Dashboard from './pages/Dashboard';
import StudentsPage from './pages/StudentsPage';
import CoursesPage from './pages/CoursesPage';
import GradesPage from './pages/GradesPage';
import AttendancePage from './pages/AttendancePage';
import UsersPage from './pages/UsersPage'; // New: Admin user management
import TeachersPage from './pages/TeachersPage'; // New: Teacher management
import ParentsPage from "./pages/ParentsPage";
import NotFound from './pages/NotFound';
import Root from './pages/Root'; // This is likely your homepage

// Global Styles
// import './styles/index.css'; // Main global styles

function App() {
  return (
      <AuthProvider>
        <Navbar />
        <div className="main-content-area"> {/* Add a div for main content */}
          <Routes>
            {/* Public Routes */}
            <Route path="/" element={<Root />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />

            {/* Authenticated Routes with Role-Based Authorization */}
            <Route path="/dashboard" element={
              <PrivateRoute> {/* All authenticated users can access dashboard */}
                <Dashboard />
              </PrivateRoute>
            } />

            {/* Admin-only routes */}
            <Route path="/users" element={
              <PrivateRoute allowedRoles={['Admin']}>
                <UsersPage />
              </PrivateRoute>
            } />
            <Route path="/teachers" element={
              <PrivateRoute allowedRoles={['Admin']}>
                <TeachersPage />
              </PrivateRoute>
            } />
            <Route path="/parents" element={
              <PrivateRoute allowedRoles={['Admin']}>
                <ParentsPage />
              </PrivateRoute>
            } />

            {/* Admin and Teacher can manage students */}
            <Route path="/students" element={
              <PrivateRoute allowedRoles={['Admin', 'Teacher', 'Student', 'Parent']}>
                <StudentsPage />
              </PrivateRoute>
            } />

            {/* Admin and Teacher can manage courses, others can view */}
            <Route path="/courses" element={
              <PrivateRoute allowedRoles={['Admin', 'Teacher', 'Student', 'Parent']}>
                <CoursesPage />
              </PrivateRoute>
            } />

            {/* Admin and Teacher can manage grades, others can view */}
            <Route path="/grades" element={
              <PrivateRoute allowedRoles={['Admin', 'Teacher', 'Student', 'Parent']}>
                <GradesPage />
              </PrivateRoute>
            } />

            {/* Admin and Teacher can manage attendance, others can view */}
            <Route path="/attendance" element={
              <PrivateRoute allowedRoles={['Admin', 'Teacher', 'Student', 'Parent']}>
                <AttendancePage />
              </PrivateRoute>
            } />

            {/* Fallback for unmatched routes */}
            <Route path="*" element={<NotFound />} />
          </Routes>
        </div>
      </AuthProvider>
  );
}

export default App;
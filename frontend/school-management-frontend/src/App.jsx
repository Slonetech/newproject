import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, ProtectedRoute } from './context/AuthContext';

// Layout Components
import Navbar from './components/Layout/Navbar';

// Pages - Organized by function/role
import LoginPage from './pages/Auth/LoginPage';
import RegisterPage from './pages/Auth/RegisterPage';

// Common Pages (accessible to specific roles or public)
import Root from './pages/Common/Root';
import Dashboard from './pages/Common/Dashboard'; // General dashboard
import NotFound from './pages/Common/NotFound';
import UnauthorizedPage from './pages/Common/UnauthorizedPage';

// Admin Management Pages
import AdminDashboard from './pages/Admin/AdminDashboard'; // Admin landing page
import UserManagementPage from './pages/Admin/UserManagementPage';
import TeacherManagementPage from './pages/Admin/TeacherManagementPage';
import ParentManagementPage from './pages/Admin/ParentManagementPage';

// General Data Pages (content adapts based on user's role)
import StudentsPage from './pages/StudentsPage';
import CoursesPage from './pages/CoursesPage';
import GradesPage from './pages/GradesPage';
import AttendancePage from './pages/AttendancePage';

// Role-Specific Dashboards
import TeacherDashboard from './pages/Common/TeacherDashboard';
import StudentDashboard from './pages/Common/StudentDashboard';
import ParentDashboard from './pages/Common/ParentDashboard';

function App() {
  return (
    <Router>
      <AuthProvider>
        <Navbar />
        {/* Main content area with consistent padding and min height */}
        <div className="container mx-auto p-4 min-h-screen">
          <Routes>
            {/* Public Routes */}
            <Route path="/" element={<Root />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/unauthorized" element={<UnauthorizedPage />} />
            <Route path="/not-found" element={<NotFound />} />

            {/* General Authenticated Dashboard - Accessible by anyone logged in */}
            <Route path="/dashboard" element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            } />

            {/* Admin-Specific Routes & Dashboards */}
            <Route path="/admin" element={
              <ProtectedRoute allowedRoles={['Admin']}>
                <AdminDashboard /> {/* Admin landing page */}
              </ProtectedRoute>
            } />
            <Route path="/admin/users" element={
              <ProtectedRoute allowedRoles={['Admin']}>
                <UserManagementPage /> {/* Manage all users */}
              </ProtectedRoute>
            } />
            <Route path="/admin/teachers" element={
              <ProtectedRoute allowedRoles={['Admin']}>
                <TeacherManagementPage /> {/* Manage teacher profiles */}
              </ProtectedRoute>
            } />
            <Route path="/admin/parents" element={
              <ProtectedRoute allowedRoles={['Admin']}>
                <ParentManagementPage /> {/* Manage parent profiles */}
              </ProtectedRoute>
            } />

            {/* Role-Specific Dashboards (primary landing for role) */}
            <Route path="/teacher" element={
              <ProtectedRoute allowedRoles={['Teacher']}>
                <TeacherDashboard />
              </ProtectedRoute>
            } />
            <Route path="/student" element={
              <ProtectedRoute allowedRoles={['Student']}>
                <StudentDashboard />
              </ProtectedRoute>
            } />
            <Route path="/parent" element={
              <ProtectedRoute allowedRoles={['Parent']}>
                <ParentDashboard />
              </ProtectedRoute>
            } />

            {/* General Data Pages - Content adapts based on user's roles (e.g., Admin sees all, Student sees self) */}
            {/* These routes allow multi-role access but the component itself should filter/display data */}
            <Route path="/students" element={
              <ProtectedRoute allowedRoles={['Admin', 'Teacher', 'Student', 'Parent']}>
                <StudentsPage />
              </ProtectedRoute>
            } />
            <Route path="/courses" element={
              <ProtectedRoute allowedRoles={['Admin', 'Teacher', 'Student', 'Parent']}>
                <CoursesPage />
              </ProtectedRoute>
            } />
            <Route path="/grades" element={
              <ProtectedRoute allowedRoles={['Admin', 'Teacher', 'Student', 'Parent']}>
                <GradesPage />
              </ProtectedRoute>
            } />
            <Route path="/attendance" element={
              <ProtectedRoute allowedRoles={['Admin', 'Teacher', 'Student', 'Parent']}>
                <AttendancePage />
              </ProtectedRoute>
            } />

            {/* Catch-all for any other unmatched routes - redirects to /not-found */}
            <Route path="*" element={<Navigate to="/not-found" replace />} />
          </Routes>
        </div>
      </AuthProvider>
    </Router>
  );
}

export default App;

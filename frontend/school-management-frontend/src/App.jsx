import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, ProtectedRoute } from './context/AuthContext'; // Import ProtectedRoute

// Layout Components
import Navbar from './components/Layout/Navbar';

// Auth Pages
import LoginPage from './pages/Auth/LoginPage';
import RegisterPage from './pages/Auth/RegisterPage';

// Common Pages
import NotFound from './pages/Common/NotFound';
import UnauthorizedPage from './pages/Common/UnauthorizedPage';
import HomePage from './pages/HomePage'; // Ensure you have a HomePage component for general logged-in users

// Admin Management Pages
import AdminDashboard from './pages/Admin/AdminDashboard';
import UserManagementPage from './pages/Admin/UserManagementPage';
import TeacherManagementPage from './pages/Admin/TeacherManagementPage';
import ParentManagementPage from './pages/Admin/ParentManagementPage';

// Role-Specific Dashboards (ensure these components exist)
import TeacherDashboard from './pages/Teacher/TeacherDashboard';
import StudentDashboard from './pages/Student/StudentDashboard';
import ParentDashboard from './pages/Parent/ParentDashboard';

// You might need these for general content or specific role access
// const CoursesPage = () => <div>Courses Content</div>;
// const GradesPage = () => <div>Grades Content</div>;
// const AttendancePage = () => <div>Attendance Content</div>;


function App() {
  return (
    <Router future={{ v7_relativeSplatPath: true, v7_startTransition: true }}>
      <AuthProvider>
        <Navbar />
        <div className="container mx-auto p-4 min-h-screen">
          <Routes>
            {/* Public Routes */}
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/unauthorized" element={<UnauthorizedPage />} />
            <Route path="/not-found" element={<NotFound />} />

            {/* Root Path - Handled by ProtectedRoute for role-based landing */}
            <Route path="/" element={
              <ProtectedRoute>
                {/* HomePage will render for authenticated users who are not explicitly redirected
                    to their specific dashboards by ProtectedRoute (e.g., if a new role is added). */}
                <HomePage />
              </ProtectedRoute>
            } />

            {/* Admin Routes - Protected for Admin role only */}
            <Route path="/admin" element={
              <ProtectedRoute allowedRoles={['Admin']}>
                <AdminDashboard />
              </ProtectedRoute>
            } />
            <Route path="/admin/users" element={
              <ProtectedRoute allowedRoles={['Admin']}>
                <UserManagementPage />
              </ProtectedRoute>
            } />
            <Route path="/admin/teachers" element={
              <ProtectedRoute allowedRoles={['Admin']}>
                <TeacherManagementPage />
              </ProtectedRoute>
            } />
            <Route path="/admin/parents" element={
              <ProtectedRoute allowedRoles={['Admin']}>
                <ParentManagementPage />
              </ProtectedRoute>
            } />

            {/* Role-Specific Dashboard Routes - Protected for their respective roles */}
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

            {/* Other Common Authenticated Routes (adjust allowedRoles as needed) */}
            {/* Example: A general Courses page accessible by students and teachers */}
            {/* You'll need to create src/pages/Courses/CoursesPage.jsx */}
            <Route path="/courses" element={
                <ProtectedRoute allowedRoles={['Student', 'Teacher', 'Admin']}>
                    {/* Placeholder, replace with your actual CoursesPage component */}
                    <div>Courses Page Content</div>
                </ProtectedRoute>
            } />
            {/* Example: A general Grades page accessible by students, teachers, and parents */}
            {/* You'll need to create src/pages/Grades/GradesPage.jsx */}
            <Route path="/grades" element={
                <ProtectedRoute allowedRoles={['Student', 'Teacher', 'Parent', 'Admin']}>
                    {/* Placeholder, replace with your actual GradesPage component */}
                    <div>Grades Page Content</div>
                </ProtectedRoute>
            } />
            {/* Example: A general Attendance page accessible by students, teachers, and parents */}
            {/* You'll need to create src/pages/Attendance/AttendancePage.jsx */}
            <Route path="/attendance" element={
                <ProtectedRoute allowedRoles={['Student', 'Teacher', 'Parent', 'Admin']}>
                    {/* Placeholder, replace with your actual AttendancePage component */}
                    <div>Attendance Page Content</div>
                </ProtectedRoute>
            } />

            {/* Catch-all for any unmatched routes - redirects to /not-found */}
            <Route path="*" element={<Navigate to="/not-found" replace />} />
          </Routes>
        </div>
      </AuthProvider>
    </Router>
  );
}

export default App;
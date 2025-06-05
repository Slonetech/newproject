// src/pages/Dashboard.jsx
import React from 'react';
import { useAuth } from '../context/AuthContext';
import '../styles/pages/_dashboard.css'; // Specific dashboard styles

function Dashboard() {
  const { user } = useAuth(); // Get user details from context

  return (
    <div className="dashboard-container">
      <h1 className="dashboard-title">Welcome to Your Dashboard, {user?.username}!</h1>
      <p className="dashboard-subtitle">Your roles: {user?.roles.join(', ')}</p>

      <div className="dashboard-section">
        <h3>Quick Access</h3>
        {user?.roles.includes('Admin') && (
          <p>As an Admin, you can manage all users, students, teachers, courses, grades, and attendance.</p>
        )}
        {user?.roles.includes('Teacher') && (
          <p>As a Teacher, you can view your enrolled students, manage grades, and mark attendance.</p>
        )}
        {user?.roles.includes('Student') && (
          <p>As a Student, you can view your profile, courses, grades, and attendance records.</p>
        )}
        {user?.roles.includes('Parent') && (
          <p>As a Parent, you can view your child's profile, grades, and attendance records.</p>
        )}
        {/* Add more specific links/cards here based on role */}
      </div>

      {/* Add sections for recent activities, quick stats, etc. */}
    </div>
  );
}

export default Dashboard;
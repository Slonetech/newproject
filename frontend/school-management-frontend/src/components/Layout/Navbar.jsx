// src/components/Layout/Navbar.jsx
import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import '../../styles/components/_navbar.css'; // Component specific styles

export default function Navbar() {
  const { user, logout, hasRole } = useAuth();

  return (
    <nav className="navbar">
      <div className="navbar-brand">
        <Link to="/" className="navbar-logo">
          School Management
        </Link>
      </div>
      <div className="navbar-links">
        <Link to="/dashboard" className="nav-item">
          Dashboard
        </Link>
        {user ? ( // Logged in
          <>
            {hasRole('Admin') && (
              <>
                <Link to="/students" className="nav-item">Students</Link>
                <Link to="/teachers" className="nav-item">Teachers</Link>
                <Link to="/parents" className="nav-item">Parents</Link>
                <Link to="/users" className="nav-item">Users</Link> {/* For Admin to manage all users */}
              </>
            )}
            {hasRole('Teacher') && (
              <>
                <Link to="/students" className="nav-item">My Students</Link> {/* Can filter on frontend */}
                <Link to="/grades" className="nav-item">Grades</Link>
                <Link to="/attendance" className="nav-item">Attendance</Link>
              </>
            )}
            {hasRole('Student') && (
              <>
                <Link to="/courses" className="nav-item">My Courses</Link>
                <Link to="/grades" className="nav-item">My Grades</Link>
                <Link to="/attendance" className="nav-item">My Attendance</Link>
              </>
            )}
            {hasRole('Parent') && (
              <>
                <Link to="/children-data" className="nav-item">Child's Info</Link> {/* Dedicated page for child data */}
              </>
            )}
            <span className="nav-username">Hello, {user.username} ({user.roles.join(', ')})</span>
            <button onClick={logout} className="nav-logout-btn">Logout</button>
          </>
        ) : ( // Not logged in
          <>
            <Link to="/login" className="nav-item">Login</Link>
            <Link to="/register" className="nav-item">Register</Link>
          </>
        )}
      </div>
    </nav>
  );
}
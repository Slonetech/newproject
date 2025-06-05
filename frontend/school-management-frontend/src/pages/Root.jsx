// src/pages/Root.jsx
import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext'; // To check if user is logged in

function Root() {
  const { user } = useAuth();
  return (
    <div className="text-center" style={{ marginTop: '50px' }}>
      <h1>Welcome to the School Management System</h1>
      <p>Your one-stop solution for managing school activities.</p>
      {!user ? (
        <div style={{ marginTop: '30px' }}>
          <Link to="/login" className="btn-primary" style={{ marginRight: '10px' }}>Login</Link>
          <Link to="/register" className="btn-primary">Register</Link>
        </div>
      ) : (
        <div style={{ marginTop: '30px' }}>
          <Link to="/dashboard" className="btn-primary">Go to Dashboard</Link>
        </div>
      )}
    </div>
  );
}

export default Root;
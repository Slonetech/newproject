// src/components/Layout/PrivateRoute.jsx
import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import './PrivateRoute.css'; // For basic styling

const PrivateRoute = ({ children, allowedRoles }) => {
  const { user, loading, hasRole } = useAuth();

  if (loading) {
    return (
      <div className="private-route-loading">
        <p>Loading authentication...</p>
        {/* Optional: Add a spinner here */}
      </div>
    );
  }

  if (!user) {
    // Not logged in, redirect to login page
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && allowedRoles.length > 0) {
    const userHasRequiredRole = allowedRoles.some(role => hasRole(role));
    if (!userHasRequiredRole) {
      // Logged in but doesn't have required role
      return (
        <div className="private-route-access-denied">
          <h2>Access Denied</h2>
          <p>You do not have the necessary permissions to view this page.</p>
          <Navigate to="/dashboard" replace /> {/* Redirect to dashboard or home */}
        </div>
      );
    }
  }

  // User is authenticated and has required roles (if any)
  return children;
};

export default PrivateRoute;
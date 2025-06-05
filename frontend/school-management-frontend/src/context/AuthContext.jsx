// src/context/AuthContext.jsx
import React, { createContext, useState, useEffect, useContext, useMemo, useCallback } from 'react';
import { jwtDecode } from 'jwt-decode'; // npm install jwt-decode
import { login as apiLogin } from '../services/authService'; // Rename to avoid conflict with local login

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null); // { username: string, roles: string[] }
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Load user from localStorage on initial render
  useEffect(() => {
    try {
      const storedToken = localStorage.getItem('authToken');
      const storedUsername = localStorage.getItem('username');
      const storedRoles = localStorage.getItem('userRoles');

      if (storedToken && storedUsername && storedRoles) {
        const decodedToken = jwtDecode(storedToken);
        // Check if token is expired (exp is in seconds, Date.now() is ms)
        if (decodedToken.exp * 1000 > Date.now()) {
          setUser({
            username: storedUsername,
            roles: JSON.parse(storedRoles),
          });
        } else {
          // Token expired, clear storage
          console.warn('Authentication token expired. Logging out.');
          logout(); // Use the logout function
        }
      }
    } catch (err) {
      console.error('Error loading auth state from localStorage:', err);
      logout(); // Clear potentially corrupted storage
    } finally {
      setLoading(false);
    }
  }, []); // Only run once on mount

  const login = useCallback(async (username, password) => {
    setLoading(true);
    setError(null);
    try {
      const data = await apiLogin({ username, password }); // Use the renamed API login
      
      // Store token and user details in localStorage
      localStorage.setItem('authToken', data.token);
      localStorage.setItem('username', data.username);
      localStorage.setItem('userRoles', JSON.stringify(data.roles));

      // Update state
      setUser({
        username: data.username,
        roles: data.roles,
      });
      return true; // Indicate success
    } catch (err) {
      setError(err.message || 'Login failed.');
      console.error('Login error:', err);
      throw err; // Re-throw for component to handle
    } finally {
      setLoading(false);
    }
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('username');
    localStorage.removeItem('userRoles');
    setUser(null);
    setError(null);
    // You might want to force a redirect here if not using a PrivateRoute
    // window.location.href = '/login'; // Example: Force redirect
  }, []);

  const hasRole = useCallback((requiredRole) => {
    if (!user || !user.roles) return false;
    return user.roles.includes(requiredRole);
  }, [user]); // Recompute if user changes

  const value = useMemo(() => ({
    user,
    loading,
    error,
    login,
    logout,
    hasRole,
  }), [user, loading, error, login, logout, hasRole]);

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

// Custom hook for easier consumption
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
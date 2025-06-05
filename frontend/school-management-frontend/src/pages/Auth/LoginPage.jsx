// src/pages/Auth/LoginPage.jsx (This is the file after renaming)
import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom'; // Add Link import
import { useAuth } from '../../context/AuthContext';
import '../../styles/pages/_auth.css';

function LoginPage() { // Component name should match the file/export
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  // Error and loading state are now managed by AuthContext, so remove local ones
  const { login, loading, error } = useAuth(); // Get login, loading, and error from context
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await login(username, password); // Call login from context
      navigate('/dashboard'); // Redirect on success
    } catch (err) {
      // The error will be set in AuthContext and displayed by the component via `error` prop
      console.error('Login form submission error:', err);
    }
  };

  return (
    <div className="auth-container">
      <h2 className="auth-title">Login to School Management</h2>
      <form onSubmit={handleSubmit} className="auth-form">
        <div className="form-group">
          <label htmlFor="username">Username:</label>
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            className="form-input"
          />
        </div>

        <div className="form-group">
          <label htmlFor="password">Password:</label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            className="form-input"
          />
        </div>

        {error && <p className="error-message">{error}</p>}

        <button type="submit" disabled={loading} className="auth-button">
          {loading ? 'Logging in...' : 'Login'}
        </button>
      </form>
      <p className="auth-link-text">
        Don't have an account? <Link to="/register">Register here</Link>
      </p>
    </div>
  );
}

export default LoginPage;
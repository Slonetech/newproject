// src/pages/Auth/Register.jsx
import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { register as apiRegister } from '../../services/authService'; // Rename to avoid conflict
import '../../styles/pages/_auth.css'; // Component specific styles

function RegisterPage() { // Renamed to RegisterPage
  const [form, setForm] = useState({
    username: '',
    email: '',
    password: '',
    firstName: '', // Added as per your DTO
    lastName: '',  // Added as per your DTO
    role: 'Student', // Default to 'Student' for public registration
  });
  const [error, setError] = useState(null);
  const [successMessage, setSuccessMessage] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  // In a real system, roles for registration might be restricted to 'Student'
  // Or handled by an Admin-specific user creation flow.
  // For public registration, we'll hardcode 'Student' as the default/only option.
  const availableRolesForRegistration = ['Student']; // Only 'Student' for general users

  const handleChange = (e) => {
    const { id, value } = e.target; // Use id for consistency
    setForm((prev) => ({ ...prev, [id]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSuccessMessage(null);

    // Frontend validation: Ensure role is selected, though it's defaulted now
    if (!form.role) {
      setError('Please select a role.');
      return;
    }

    setIsLoading(true);
    try {
      const data = await apiRegister(form); // Pass the whole form object
      setSuccessMessage(data.message || 'Registration successful! You can now log in.');
      setForm({ username: '', email: '', password: '', firstName: '', lastName: '', role: 'Student' }); // Clear form
      navigate('/login'); // Redirect to login after successful registration
    } catch (err) {
      console.error('Registration error:', err);
      setError(err.message || 'An unexpected error occurred during registration.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <h2 className="auth-title">Register for School Management</h2>
      <form onSubmit={handleSubmit} className="auth-form">
        <div className="form-group">
          <label htmlFor="firstName">First Name:</label>
          <input
            type="text"
            id="firstName"
            value={form.firstName}
            onChange={handleChange}
            required
            className="form-input"
          />
        </div>
        <div className="form-group">
          <label htmlFor="lastName">Last Name:</label>
          <input
            type="text"
            id="lastName"
            value={form.lastName}
            onChange={handleChange}
            required
            className="form-input"
          />
        </div>
        <div className="form-group">
          <label htmlFor="username">Username:</label>
          <input
            type="text"
            id="username"
            value={form.username}
            onChange={handleChange}
            required
            className="form-input"
          />
        </div>

        <div className="form-group">
          <label htmlFor="email">Email:</label>
          <input
            type="email"
            id="email"
            value={form.email}
            onChange={handleChange}
            required
            className="form-input"
          />
        </div>

        <div className="form-group">
          <label htmlFor="password">Password:</label>
          <input
            type="password"
            id="password"
            value={form.password}
            onChange={handleChange}
            required
            className="form-input"
          />
        </div>

        <div className="form-group">
          <label htmlFor="role">Role:</label>
          <select
            id="role"
            value={form.role}
            onChange={handleChange}
            required
            className="form-input"
            // If you only allow 'Student' publicly, you can disable the select or hide it.
            // disabled={true} // uncomment this if you want to hardcode 'Student' and not allow selection
          >
            {availableRolesForRegistration.map((r) => (
              <option key={r} value={r}>
                {r}
              </option>
            ))}
          </select>
        </div>

        {error && <p className="error-message">{error}</p>}
        {successMessage && <p className="success-message">{successMessage}</p>}

        <button type="submit" disabled={isLoading} className="auth-button">
          {isLoading ? 'Registering...' : 'Register'}
        </button>
      </form>
      <p className="auth-link-text">
        Already have an account? <Link to="/login">Login here</Link>
      </p>
    </div>
  );
}

export default RegisterPage;
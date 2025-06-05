// src/components/Auth/Register.jsx
import React, { useState } from 'react';
import { register } from '../../services/authService';

function Register() {
  const [form, setForm] = useState({
    username: '',
    email: '',
    password: '',
    role: '',
  });
  const [error, setError] = useState(null);
  const [successMessage, setSuccessMessage] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const availableRoles = ['Student', 'Teacher', 'Admin', 'Parent'];

  const handleChange = (e) => {
    const { id, value } = e.target;
    setForm((prev) => ({ ...prev, [id]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSuccessMessage(null);

    if (!form.role) {
      setError('Please select a role.');
      return;
    }

    setIsLoading(true);
    try {
      const data = await register(
        form.username,
        form.email,
        form.password,
        form.role
      );
      setSuccessMessage(data.message || 'Registration successful!');
      setForm({ username: '', email: '', password: '', role: '' });
    } catch (err) {
      setError(err.message || 'An error occurred.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: '400px', margin: '0 auto' }}>
      <h2>Register</h2>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '10px' }}>
          <label htmlFor="username">Username:</label>
          <input
            id="username"
            value={form.username}
            onChange={handleChange}
            required
          />
        </div>

        <div style={{ marginBottom: '10px' }}>
          <label htmlFor="email">Email:</label>
          <input
            id="email"
            type="email"
            value={form.email}
            onChange={handleChange}
            required
          />
        </div>

        <div style={{ marginBottom: '10px' }}>
          <label htmlFor="password">Password:</label>
          <input
            id="password"
            type="password"
            value={form.password}
            onChange={handleChange}
            required
          />
        </div>

        <div style={{ marginBottom: '10px' }}>
          <label htmlFor="role">Role:</label>
          <select
            id="role"
            value={form.role}
            onChange={handleChange}
            required
          >
            <option value="">Select a role</option>
            {availableRoles.map((r) => (
              <option key={r} value={r}>
                {r}
              </option>
            ))}
          </select>
        </div>

        {error && <p style={{ color: 'red' }}>{error}</p>}
        {successMessage && <p style={{ color: 'green' }}>{successMessage}</p>}

        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Registering...' : 'Register'}
        </button>
      </form>
    </div>
  );
}

export default Register;

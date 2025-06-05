// src/services/authService.js
import apiClient from './api'; // Import the configured axios instance

export const login = async ({ username, password }) => { // Destructure directly
  try {
    const response = await apiClient.post('/Auth/login', { username, password });
    // The backend's AuthController returns token, expiration, username, roles
    return response.data; 
  } catch (error) {
    const errorMessage = error.response?.data?.message || 'Login failed. Please check your credentials.';
    throw new Error(errorMessage);
  }
};

export const register = async ({ username, email, password, firstName, lastName, role }) => { // Destructure and add firstName, lastName
  try {
    const response = await apiClient.post('/Auth/register', { username, email, password, firstName, lastName, role });
    return response.data;
  } catch (error) {
    let errorMessage = 'Registration failed. Please try again.';
    if (error.response?.data?.message) {
      errorMessage = error.response.data.message;
    } else if (error.response?.data?.errors) {
      // Aggregate Identity errors
      const errors = Object.values(error.response.data.errors).flat();
      errorMessage = errors.map(err => err.description || err).join('; '); // Join all error descriptions
    } else if (Array.isArray(error.response?.data) && error.response.data[0]?.description) {
      // Fallback for array of errors
      errorMessage = error.response.data.map(err => err.description).join('; ');
    }
    throw new Error(errorMessage);
  }
};
// src/services/api.js
import axios from 'axios';

// IMPORTANT: Match your backend URL
const API_BASE_URL = 'http://localhost:5248/api'; 

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor: Attach JWT token to outgoing requests
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken'); // Consistent key
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response Interceptor: Handle global errors like 401 Unauthorized
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      console.warn('Unauthorized request. Token might be expired or invalid.');
      // Optional: Redirect to login or show a session expired message
      // Note: Full logout logic should ideally be handled by AuthContext or a global error handler
      // to avoid circular dependencies if AuthContext uses this `apiClient`.
      // For now, a simple local storage clear is fine if AuthContext also handles it.
      localStorage.removeItem('authToken');
      localStorage.removeItem('userRoles'); // Clear roles too
      localStorage.removeItem('username'); // Clear username
      // This will trigger the AuthContext useEffect to update state
      // If not using AuthContext, you'd navigate here: window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default apiClient;
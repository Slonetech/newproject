import axios from 'axios';
import { getToken, logout } from '../services/authService'; // Corrected import for getToken and logout

// IMPORTANT: Update baseURL to match your backend port (e.g., http://localhost:5000 or https://localhost:5001)
const axiosInstance = axios.create({
    baseURL: 'http://localhost:5248', // Change this if your backend runs on a different port
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request interceptor to add the JWT token to every outgoing request
axiosInstance.interceptors.request.use(
    (config) => {
        const token = getToken(); // Get token from localStorage using authService helper
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Response interceptor to handle token expiration/unauthorized responses
axiosInstance.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;
        // If the error is 401 Unauthorized and it's not the login request itself,
        // and we haven't already retried this request
        if (error.response && error.response.status === 401 && originalRequest.url !== '/Auth/login' && !originalRequest._retry) {
            originalRequest._retry = true; // Mark as retried
            console.warn("Unauthorized response. Token might be expired or invalid. Forcing logout.");
            logout(); // Force logout using authService helper
            // Redirect to login page
            window.location.href = '/login'; // This forces a full page reload and redirect
            return Promise.reject(error);
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;

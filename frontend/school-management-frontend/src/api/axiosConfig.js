import axios from 'axios';
import { getToken, getUser, logout } from '../services/authService'; // Corrected import for getToken, getUser, and logout
import { handleApiError } from '../utils/errorHandler';

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
        const token = getToken();
        // === API REQUEST DEBUG ===
        console.log('=== API REQUEST DEBUG ===');
        console.log('Request URL:', config.url);
        console.log('Token being sent:', token);
        console.log('User object:', getUser());
        console.log('========================');
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
    (error) => {
        console.log('=== API ERROR DEBUG ===');
        console.log('Error status:', error.response?.status);
        console.log('Error data:', error.response?.data);
        console.log('Request config:', error.config);
        console.log('Current token:', getToken());
        console.log('Current user:', getUser());
        console.log('=======================');
        if (error.response?.status === 401) {
            // Show toast before logout
            handleApiError(error, 'Session expired. Please login again.');
            logout();
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;

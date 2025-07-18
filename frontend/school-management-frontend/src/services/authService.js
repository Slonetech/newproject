import axiosInstance from '../api/axiosConfig';

const API_AUTH_PATH = '/api/Auth/';

// Function for user login
export const login = async (email, password) => {
    try {
        const response = await axiosInstance.post(API_AUTH_PATH + 'login', {
            email,
            password,
        });

        if (response.data.token) {
            localStorage.setItem('authToken', response.data.token);
            localStorage.setItem('user', JSON.stringify(response.data.user));
            // === LOGIN DEBUG ===
            console.log('=== LOGIN DEBUG ===');
            console.log('Login response:', response.data);
            console.log('Token being stored:', response.data.token);
            console.log('User being stored:', response.data.user);
            console.log('User roles:', response.data.user?.roles);
            console.log('==================');
            return response.data;
        } else {
            throw new Error('Login successful but no token received.');
        }
    } catch (error) {
        console.error('AuthService login error:', error);
        if (error.response) {
            throw new Error(error.response.data.message || `Server error: ${error.response.status}`);
        } else if (error.request) {
            throw new Error('No response from server. Check network connection.');
        } else {
            throw new Error(error.message || 'An unexpected error occurred during login.');
        }
    }
};

// Function for user registration
export const register = async ({ email, password, firstName, lastName }) => {
    try {
        const response = await axiosInstance.post(API_AUTH_PATH + 'register', {
            email,
            password,
            firstName,
            lastName
        });
        return response.data;
    } catch (error) {
        console.error('AuthService register error:', error);
        if (error.response) {
            let errorMessage = error.response.data.message || `Registration error: ${error.response.status}`;
            if (error.response.data.errors && Array.isArray(error.response.data.errors)) {
                errorMessage = error.response.data.errors.map(err => err.description || err).join('; ');
            } else if (typeof error.response.data === 'string') {
                errorMessage = error.response.data;
            }
            throw new Error(errorMessage);
        } else if (error.request) {
            throw new Error('No response from server during registration. Check network connection.');
        } else {
            throw new Error(error.message || 'An unexpected error occurred during registration.');
        }
    }
};

// Function to logout user by clearing local storage
export const logout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
};

// Helper functions to retrieve authentication data from local storage
export const getToken = () => localStorage.getItem('authToken');
export const getUser = () => {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
};

// Token validation utility
export const isTokenValid = () => {
    const token = getToken();
    if (!token) return false;
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const currentTime = Date.now() / 1000;
        console.log('Token payload:', payload);
        console.log('Token expires at:', new Date(payload.exp * 1000));
        console.log('Current time:', new Date(currentTime * 1000));
        return payload.exp > currentTime;
    } catch (error) {
        console.error('Token validation error:', error);
        return false;
    }
};

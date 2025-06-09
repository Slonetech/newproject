import axiosInstance from '../api/axiosConfig';

const API_AUTH_PATH = '/Auth/';

// Function for user login
export const login = async (username, password) => { // Expects individual parameters, not destructured object
    try {
        const response = await axiosInstance.post(API_AUTH_PATH + 'login', {
            username,
            password,
        });

        // Backend's AuthController returns token, expiration, username, email, roles
        if (response.data.token) {
            localStorage.setItem('authToken', response.data.token);
            localStorage.setItem('userUsername', response.data.username);
            localStorage.setItem('userEmail', response.data.email);
            localStorage.setItem('userRoles', JSON.stringify(response.data.roles));
            localStorage.setItem('tokenExpiration', response.data.expiration);

            return response.data;
        } else {
            throw new Error('Login successful but no token received.');
        }
    } catch (error) {
        console.error('AuthService login error:', error);
        if (error.response) {
            throw new Error(error.response.data.Message || `Server error: ${error.response.status}`);
        } else if (error.request) {
            throw new Error('No response from server. Check network connection.');
        } else {
            throw new Error(error.message || 'An unexpected error occurred during login.');
        }
    }
};

// Function for user registration
export const register = async ({ username, email, password, firstName, lastName }) => { // Destructure parameters
    try {
        // The backend's AuthController.Register method automatically assigns 'Student' role
        // For public registration, 'role' parameter is not needed if backend handles default.
        // If your backend expects 'role' in register, you'd add it here.
        const response = await axiosInstance.post(API_AUTH_PATH + 'register', {
            username,
            email,
            password,
            firstName,
            lastName
        });
        return response.data;
    } catch (error) {
        console.error('AuthService register error:', error);
        if (error.response) {
            let errorMessage = error.response.data.Message || `Registration error: ${error.response.status}`;
            if (error.response.data.Errors && Array.isArray(error.response.data.Errors)) {
                // If Identity returns a list of errors
                errorMessage = error.response.data.Errors.map(err => err.description || err).join('; ');
            } else if (typeof error.response.data === 'string') {
                // If the response data is just a string error message
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
    localStorage.removeItem('userUsername');
    localStorage.removeItem('userEmail');
    localStorage.removeItem('userRoles');
    localStorage.removeItem('tokenExpiration');
};

// Helper functions to retrieve authentication data from local storage
export const getToken = () => localStorage.getItem('authToken');
export const getUsername = () => localStorage.getItem('userUsername');
export const getUserEmail = () => localStorage.getItem('userEmail');
export const getUserRoles = () => {
    const roles = localStorage.getItem('userRoles');
    return roles ? JSON.parse(roles) : []; // Parse JSON string back to array
};
export const getTokenExpiration = () => localStorage.getItem('tokenExpiration');

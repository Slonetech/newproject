import axiosInstance from '../api/axiosConfig'; // Use the configured axios instance

const API_ADMIN_PATH = '/Users/'; // Corresponds to SchoolApi/Controllers/UsersController.cs

const adminService = {
    // Get all users
    getAllUsers: async () => {
        try {
            const response = await axiosInstance.get(API_ADMIN_PATH);
            return response.data;
        } catch (error) {
            console.error('Error fetching all users:', error.response?.data || error.message);
            throw error;
        }
    },

    // Get user by ID
    getUserById: async (userId) => {
        try {
            const response = await axiosInstance.get(`${API_ADMIN_PATH}${userId}`);
            return response.data;
        } catch (error) {
            console.error(`Error fetching user ${userId}:`, error.response?.data || error.message);
            throw error;
        }
    },

    // Create a new user
    createUser: async (userData) => { // userData should match UserCreationDto: username, email, password, firstName, lastName, initialRole
        try {
            const response = await axiosInstance.post(API_ADMIN_PATH, userData);
            return response.data;
        } catch (error) {
            console.error('Error creating user:', error.response?.data || error.message);
            throw error;
        }
    },

    // Update an existing user
    updateUser: async (userId, userData) => { // userData should match UserUpdateDto: username, email, firstName, lastName
        try {
            const response = await axiosInstance.put(`${API_ADMIN_PATH}${userId}`, userData);
            return response.data; // Usually returns 204 No Content for PUT
        } catch (error) {
            console.error(`Error updating user ${userId}:`, error.response?.data || error.message);
            throw error;
        }
    },

    // Delete a user
    deleteUser: async (userId) => {
        try {
            const response = await axiosInstance.delete(`${API_ADMIN_PATH}${userId}`);
            return response.data; // Usually returns 204 No Content for DELETE
        } catch (error) {
            console.error(`Error deleting user ${userId}:`, error.response?.data || error.message);
            throw error;
        }
    },

    // Assign a role to a user
    assignRole: async (userId, roleName) => { // roleName is just a string
        try {
            const response = await axiosInstance.post(`${API_ADMIN_PATH}${userId}/assign-role`, { roleName });
            return response.data;
        } catch (error) {
            console.error(`Error assigning role '${roleName}' to user ${userId}:`, error.response?.data || error.message);
            throw error;
        }
    },

    // Remove a role from a user
    removeRole: async (userId, roleName) => { // roleName is just a string
        try {
            const response = await axiosInstance.post(`${API_ADMIN_PATH}${userId}/remove-role`, { roleName });
            return response.data;
        } catch (error) {
            console.error(`Error removing role '${roleName}' from user ${userId}:`, error.response?.data || error.message);
            throw error;
        }
    },

    // Get available roles (assuming a backend endpoint for this or hardcoding for now)
    getAvailableRoles: async () => {
        // In a real app, you might have an API endpoint: /api/Roles
        // For now, hardcode them as we seeded them in the backend.
        return ["Admin", "Teacher", "Student", "Parent"];
    },

    // Link a user to a student profile (example, others can be added)
    linkStudentProfile: async (userId) => {
        try {
            const response = await axiosInstance.post(`${API_ADMIN_PATH}${userId}/link-student-profile`);
            return response.data;
        } catch (error) {
            console.error(`Error linking student profile for user ${userId}:`, error.response?.data || error.message);
            throw error;
        }
    }
};

export default adminService;

import axiosInstance from '../api/axiosConfig';

const API_PARENT_PATH = '/api/Parents';

const parentService = {
    // Get all parents
    getAllParents: async () => {
        const response = await axiosInstance.get(API_PARENT_PATH);
        return response.data;
    },
    // Add a new parent
    addParent: async (parentData) => {
        const response = await axiosInstance.post(API_PARENT_PATH, parentData);
        return response.data;
    },
    // Update a parent
    updateParent: async (id, parentData) => {
        const response = await axiosInstance.put(`${API_PARENT_PATH}/${id}`, parentData);
        return response.data;
    },
    // Delete a parent
    deleteParent: async (id) => {
        const response = await axiosInstance.delete(`${API_PARENT_PATH}/${id}`);
        return response.data;
    }
};

export default parentService; 
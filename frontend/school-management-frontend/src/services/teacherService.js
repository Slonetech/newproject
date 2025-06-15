import axiosInstance from '../api/axiosConfig';

const API_TEACHER_PATH = '/api/Teachers';

const teacherService = {
    // Get all teachers
    getAllTeachers: async () => {
        try {
            const response = await axiosInstance.get(API_TEACHER_PATH);
            return response.data;
        } catch (error) {
            console.error('Error fetching teachers:', error.response?.data || error.message);
            throw error;
        }
    },

    // Get a single teacher by ID
    getTeacher: async (id) => {
        try {
            const response = await axiosInstance.get(`${API_TEACHER_PATH}/${id}`);
            return response.data;
        } catch (error) {
            console.error(`Error fetching teacher ${id}:`, error.response?.data || error.message);
            throw error;
        }
    },

    // Create a new teacher
    createTeacher: async (teacherData) => {
        try {
            const response = await axiosInstance.post(API_TEACHER_PATH, teacherData);
            return response.data;
        } catch (error) {
            console.error('Error creating teacher:', error.response?.data || error.message);
            throw error;
        }
    },

    // Update a teacher
    updateTeacher: async (id, teacherData) => {
        try {
            const response = await axiosInstance.put(`${API_TEACHER_PATH}/${id}`, teacherData);
            return response.data;
        } catch (error) {
            console.error(`Error updating teacher ${id}:`, error.response?.data || error.message);
            throw error;
        }
    },

    // Delete a teacher
    deleteTeacher: async (id) => {
        try {
            const response = await axiosInstance.delete(`${API_TEACHER_PATH}/${id}`);
            return response.data;
        } catch (error) {
            console.error(`Error deleting teacher ${id}:`, error.response?.data || error.message);
            throw error;
        }
    },

    // Get teacher's courses
    getTeacherCourses: async (teacherId) => {
        try {
            const response = await axiosInstance.get(`${API_TEACHER_PATH}/${teacherId}/courses`);
            return response.data;
        } catch (error) {
            console.error(`Error fetching courses for teacher ${teacherId}:`, error.response?.data || error.message);
            throw error;
        }
    },

    // Assign a course to a teacher
    assignCourse: async (teacherId, courseId) => {
        try {
            const response = await axiosInstance.post(`${API_TEACHER_PATH}/${teacherId}/courses`, { id: courseId });
            return response.data;
        } catch (error) {
            console.error(`Error assigning course ${courseId} to teacher ${teacherId}:`, error.response?.data || error.message);
            throw error;
        }
    },

    // Remove a course from a teacher
    removeCourse: async (teacherId, courseId) => {
        try {
            const response = await axiosInstance.delete(`${API_TEACHER_PATH}/${teacherId}/courses/${courseId}`);
            return response.data;
        } catch (error) {
            console.error(`Error removing course ${courseId} from teacher ${teacherId}:`, error.response?.data || error.message);
            throw error;
        }
    }
};

export default teacherService; 
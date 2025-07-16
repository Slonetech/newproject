import api from '../api/axiosConfig';
import { handleApiError, handleApiSuccess } from '../utils/errorHandler';

export const studentService = {
  getAllStudents: async () => {
    try {
      const response = await api.get('/students');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch students');
      throw error;
    }
  },
  getStudentById: async (id) => {
    try {
      const response = await api.get(`/students/${id}`);
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch student');
      throw error;
    }
  },
  createStudent: async (studentData) => {
    try {
      const response = await api.post('/students', studentData);
      handleApiSuccess('Student created successfully!');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to create student');
      throw error;
    }
  },
  updateStudent: async (id, studentData) => {
    try {
      const response = await api.put(`/students/${id}`, studentData);
      handleApiSuccess('Student updated successfully!');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to update student');
      throw error;
    }
  },
  deleteStudent: async (id) => {
    try {
      const response = await api.delete(`/students/${id}`);
      handleApiSuccess('Student deleted successfully!');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to delete student');
      throw error;
    }
  },
  getStudentCourses: async (studentId) => {
    try {
      const response = await api.get(`/students/${studentId}/courses`);
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch student courses');
      throw error;
    }
  },
  getStudentGrades: async (studentId) => {
    try {
      const response = await api.get(`/students/${studentId}/grades`);
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch student grades');
      throw error;
    }
  }
}; 
import api from '../api/axiosConfig';
import { handleApiError, handleApiSuccess } from '../utils/errorHandler';

export const gradeService = {
  getAllGrades: async () => {
    try {
      const response = await api.get('/grades');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch grades');
      throw error;
    }
  },
  getGradeById: async (id) => {
    try {
      const response = await api.get(`/grades/${id}`);
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch grade');
      throw error;
    }
  },
  createGrade: async (gradeData) => {
    try {
      const response = await api.post('/grades', gradeData);
      handleApiSuccess('Grade created successfully!');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to create grade');
      throw error;
    }
  },
  updateGrade: async (id, gradeData) => {
    try {
      const response = await api.put(`/grades/${id}`, gradeData);
      handleApiSuccess('Grade updated successfully!');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to update grade');
      throw error;
    }
  },
  deleteGrade: async (id) => {
    try {
      const response = await api.delete(`/grades/${id}`);
      handleApiSuccess('Grade deleted successfully!');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to delete grade');
      throw error;
    }
  },
  getGradesByStudent: async (studentId) => {
    try {
      const response = await api.get(`/grades/student/${studentId}`);
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch grades by student');
      throw error;
    }
  },
  getGradesByAssignment: async (assignmentId) => {
    try {
      const response = await api.get(`/grades/assignment/${assignmentId}`);
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch grades by assignment');
      throw error;
    }
  }
}; 
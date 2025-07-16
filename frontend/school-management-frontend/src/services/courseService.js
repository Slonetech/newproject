import api from '../api/axiosConfig';
import { handleApiError, handleApiSuccess } from '../utils/errorHandler';

export const courseService = {
  getAllCourses: async () => {
    try {
      const response = await api.get('/courses');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch courses');
      throw error;
    }
  },
  getCourseById: async (id) => {
    try {
      const response = await api.get(`/courses/${id}`);
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch course');
      throw error;
    }
  },
  createCourse: async (courseData) => {
    try {
      const response = await api.post('/courses', courseData);
      handleApiSuccess('Course created successfully!');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to create course');
      throw error;
    }
  },
  updateCourse: async (id, courseData) => {
    try {
      const response = await api.put(`/courses/${id}`, courseData);
      handleApiSuccess('Course updated successfully!');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to update course');
      throw error;
    }
  },
  deleteCourse: async (id) => {
    try {
      const response = await api.delete(`/courses/${id}`);
      handleApiSuccess('Course deleted successfully!');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to delete course');
      throw error;
    }
  },
  getCoursesByTeacher: async (teacherId) => {
    try {
      const response = await api.get(`/courses/teacher/${teacherId}`);
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch teacher courses');
      throw error;
    }
  },
  getCourseStudents: async (courseId) => {
    try {
      const response = await api.get(`/courses/${courseId}/students`);
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to fetch course students');
      throw error;
    }
  },
  enrollStudent: async (courseId, studentId) => {
    try {
      const response = await api.post(`/courses/${courseId}/students/${studentId}`);
      handleApiSuccess('Student enrolled successfully!');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to enroll student');
      throw error;
    }
  },
  removeStudent: async (courseId, studentId) => {
    try {
      const response = await api.delete(`/courses/${courseId}/students/${studentId}`);
      handleApiSuccess('Student removed from course!');
      return response.data;
    } catch (error) {
      handleApiError(error, 'Failed to remove student from course');
      throw error;
    }
  }
}; 
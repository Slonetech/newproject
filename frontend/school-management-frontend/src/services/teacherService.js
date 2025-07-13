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
    },

    // Get courses assigned to the logged-in teacher (mock data for development)
    getCoursesForTeacher: async (teacherId) => {
        // Simulate API delay
        await new Promise((res) => setTimeout(res, 300));
        return [
            {
                id: 'course1',
                name: 'Mathematics',
                code: 'MATH101',
                department: 'Mathematics',
                studentCount: 3,
            },
            {
                id: 'course2',
                name: 'Physics',
                code: 'PHYS201',
                department: 'Science',
                studentCount: 2,
            },
        ];
    },
    // Get students and their grades for a course (mock data)
    getCourseStudentsWithGrades: async (courseId) => {
        await new Promise((res) => setTimeout(res, 300));
        const students = [
            { id: 'stu1', firstName: 'Alice', lastName: 'Johnson' },
            { id: 'stu2', firstName: 'Bob', lastName: 'Smith' },
            { id: 'stu3', firstName: 'Charlie', lastName: 'Lee' },
        ];
        const grades = {
            stu1: 'A',
            stu2: 'B+',
            stu3: 'C',
        };
        return { students, grades };
    },
    // Update a student's grade for a course (mock, always succeeds)
    updateStudentGrade: async (courseId, studentId, grade) => {
        await new Promise((res) => setTimeout(res, 200));
        return { success: true };
    },
    // Get students for a course (mock data)
    getCourseStudents: async (courseId) => {
        await new Promise((res) => setTimeout(res, 300));
        return {
            students: [
                { id: 'stu1', firstName: 'Alice', lastName: 'Johnson' },
                { id: 'stu2', firstName: 'Bob', lastName: 'Smith' },
                { id: 'stu3', firstName: 'Charlie', lastName: 'Lee' },
            ],
        };
    },
    // Record attendance for a course and date (mock, always succeeds)
    recordAttendance: async (courseId, date, attendance) => {
        await new Promise((res) => setTimeout(res, 200));
        return { success: true };
    },
};

export default teacherService; 
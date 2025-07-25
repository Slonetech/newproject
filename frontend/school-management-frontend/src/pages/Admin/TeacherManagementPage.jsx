import React, { useState, useEffect } from 'react';
import teacherService from '../../services/teacherService';
import { useAuth } from '../../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';

// Helper for date input formatting
function toDateInputValue(date) {
  if (!date || date === "0000-12-31" || date === "0001-01-01" || date === "0001-01-01T00:00:00" || date === "Invalid Date") return "";
  const d = new Date(date);
  if (isNaN(d.getTime())) return "";
  return d.toISOString().slice(0, 10);
}

const TeacherManagementPage = () => {
    const { user } = useAuth();
    const navigate = useNavigate();
    const [teachers, setTeachers] = useState([]);
    const [selectedTeacher, setSelectedTeacher] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isCourseModalOpen, setIsCourseModalOpen] = useState(false);
    const [availableCourses, setAvailableCourses] = useState([]);
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        phoneNumber: '',
        specialization: '',
        department: '',
        dateOfBirth: '',
        hireDate: '',
        address: '',
    });

    useEffect(() => {
        loadTeachers();
    }, []);

    const loadTeachers = async () => {
        try {
            const data = await teacherService.getAllTeachers();
            setTeachers(data);
        } catch (error) {
            toast.error('Failed to load teachers');
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value ?? ""
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            // Prepare payload with ISO date strings or null
            const payload = {
                ...formData,
                dateOfBirth: formData.dateOfBirth ? new Date(formData.dateOfBirth).toISOString() : null,
                hireDate: formData.hireDate ? new Date(formData.hireDate).toISOString() : null,
            };
            if (selectedTeacher) {
                await teacherService.updateTeacher(selectedTeacher.id, payload);
                toast.success('Teacher updated successfully');
            } else {
                await teacherService.createTeacher(payload);
                toast.success('Teacher created successfully');
            }
            setIsModalOpen(false);
            loadTeachers();
        } catch (error) {
            toast.error(error.response?.data?.message || 'Operation failed');
        }
    };

    const handleDelete = async (id) => {
        if (window.confirm('Are you sure you want to delete this teacher?')) {
            try {
                await teacherService.deleteTeacher(id);
                toast.success('Teacher deleted successfully');
                loadTeachers();
            } catch (error) {
                toast.error('Failed to delete teacher');
            }
        }
    };

    const handleEdit = (teacher) => {
        setSelectedTeacher(teacher);
        setFormData({
            firstName: teacher.firstName || "",
            lastName: teacher.lastName || "",
            email: teacher.email || "",
            password: '',
            phoneNumber: teacher.phoneNumber || "",
            specialization: teacher.specialization || "",
            department: teacher.department || "",
            dateOfBirth: toDateInputValue(teacher.dateOfBirth),
            hireDate: toDateInputValue(teacher.hireDate),
            address: teacher.address || "",
        });
        setIsModalOpen(true);
    };

    const handleAssignCourse = async (teacherId, courseId) => {
        try {
            await teacherService.assignCourse(teacherId, courseId);
            toast.success('Course assigned successfully');
            loadTeachers();
        } catch (error) {
            toast.error('Failed to assign course');
        }
    };

    const handleRemoveCourse = async (teacherId, courseId) => {
        try {
            await teacherService.removeCourse(teacherId, courseId);
            toast.success('Course removed successfully');
            loadTeachers();
        } catch (error) {
            toast.error('Failed to remove course');
        }
    };

    return (
        <div className="container mx-auto px-4 py-8">
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-2xl font-bold">Teacher Management</h1>
                <button
                    onClick={() => {
                        setSelectedTeacher(null);
                        setFormData({
                            firstName: '',
                            lastName: '',
                            email: '',
                            password: '',
                            phoneNumber: '',
                            specialization: '',
                            department: '',
                            dateOfBirth: '',
                            hireDate: '',
                            address: '',
                        });
                        setIsModalOpen(true);
                    }}
                    className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
                >
                    Add New Teacher
                </button>
            </div>

            {/* Teachers List */}
            <div className="bg-white shadow-md rounded-lg overflow-hidden">
                <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                        <tr>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Specialization</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Courses</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                        {teachers.map((teacher) => (
                            <tr key={teacher.id}>
                                <td className="px-6 py-4 whitespace-nowrap">
                                    {teacher.firstName} {teacher.lastName}
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap">{teacher.email}</td>
                                <td className="px-6 py-4 whitespace-nowrap">{teacher.specialization}</td>
                                <td className="px-6 py-4">
                                    <div className="flex flex-wrap gap-2">
                                        {teacher.courses?.map((course) => (
                                            <span
                                                key={course.id}
                                                className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800"
                                            >
                                                {course.name}
                                                <button
                                                    onClick={() => handleRemoveCourse(teacher.id, course.id)}
                                                    className="ml-1 text-blue-600 hover:text-blue-800"
                                                >
                                                    ×
                                                </button>
                                            </span>
                                        ))}
                                        <button
                                            onClick={() => {
                                                setSelectedTeacher(teacher);
                                                setIsCourseModalOpen(true);
                                            }}
                                            className="text-blue-600 hover:text-blue-800"
                                        >
                                            + Add Course
                                        </button>
                                    </div>
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                    <button
                                        onClick={() => handleEdit(teacher)}
                                        className="text-indigo-600 hover:text-indigo-900 mr-4"
                                    >
                                        Edit
                                    </button>
                                    <button
                                        onClick={() => handleDelete(teacher.id)}
                                        className="text-red-600 hover:text-red-900"
                                    >
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {/* Teacher Form Modal */}
            {isModalOpen && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full">
                    <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
                        <div className="mt-3">
                            <h3 className="text-lg font-medium leading-6 text-gray-900 mb-4">
                                {selectedTeacher ? 'Edit Teacher' : 'Add New Teacher'}
                            </h3>
                            <form onSubmit={handleSubmit} className="space-y-4">
                                <input name="firstName" value={formData.firstName || ""} onChange={handleInputChange} placeholder="First Name" required className="input" />
                                <input name="lastName" value={formData.lastName || ""} onChange={handleInputChange} placeholder="Last Name" required className="input" />
                                <input name="email" value={formData.email || ""} onChange={handleInputChange} placeholder="Email" type="email" required className="input" />
                                <input name="password" value={formData.password || ""} onChange={handleInputChange} placeholder="Password" type="password" required className="input" />
                                <input name="phoneNumber" value={formData.phoneNumber || ""} onChange={handleInputChange} placeholder="Phone Number" className="input" />
                                <input name="specialization" value={formData.specialization || ""} onChange={handleInputChange} placeholder="Specialization" className="input" />
                                <input name="department" value={formData.department || ""} onChange={handleInputChange} placeholder="Department" className="input" />
                                <input name="dateOfBirth" value={formData.dateOfBirth || ""} onChange={handleInputChange} placeholder="Date of Birth" type="date" required className="input" />
                                <input name="hireDate" value={formData.hireDate || ""} onChange={handleInputChange} placeholder="Hire Date" type="date" required className="input" />
                                <input name="address" value={formData.address || ""} onChange={handleInputChange} placeholder="Address" className="input" />
                                <button type="submit" className="btn btn-primary">{selectedTeacher ? 'Update' : 'Create'}</button>
                            </form>
                        </div>
                    </div>
                </div>
            )}

            {/* Course Assignment Modal */}
            {isCourseModalOpen && selectedTeacher && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full">
                    <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
                        <div className="mt-3">
                            <h3 className="text-lg font-medium leading-6 text-gray-900 mb-4">
                                Assign Course to {selectedTeacher.firstName} {selectedTeacher.lastName}
                            </h3>
                            <div className="space-y-4">
                                {availableCourses.map((course) => (
                                    <div key={course.id} className="flex items-center justify-between">
                                        <span>{course.name}</span>
                                        <button
                                            onClick={() => handleAssignCourse(selectedTeacher.id, course.id)}
                                            className="text-blue-600 hover:text-blue-800"
                                        >
                                            Assign
                                        </button>
                                    </div>
                                ))}
                                <button
                                    onClick={() => setIsCourseModalOpen(false)}
                                    className="mt-4 btn btn-secondary"
                                >
                                    Close
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default TeacherManagementPage;

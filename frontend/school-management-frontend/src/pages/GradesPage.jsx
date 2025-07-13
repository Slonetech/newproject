import React, { useEffect, useState } from 'react';
import { useProfileData } from '../hooks/useProfileData';
import teacherService from '../services/teacherService';
import { toast } from 'react-toastify';

function GradesPage() {
    const { data: teacher, loading, error } = useProfileData('/Teachers/me');
    const [selectedCourseId, setSelectedCourseId] = useState('');
    const [students, setStudents] = useState([]);
    const [grades, setGrades] = useState({});
    const [saving, setSaving] = useState(false);
    const [fetching, setFetching] = useState(false);

    useEffect(() => {
        if (selectedCourseId) {
            fetchStudentsAndGrades(selectedCourseId);
        } else {
            setStudents([]);
            setGrades({});
        }
    }, [selectedCourseId]);

    const fetchStudentsAndGrades = async (courseId) => {
        setFetching(true);
        try {
            const res = await teacherService.getCourseStudentsWithGrades(courseId);
            setStudents(res.students);
            setGrades(res.grades);
        } catch (err) {
            toast.error('Failed to fetch students/grades');
        }
        setFetching(false);
    };

    const handleGradeChange = (studentId, value) => {
        setGrades((prev) => ({ ...prev, [studentId]: value }));
    };

    const handleSave = async (studentId) => {
        setSaving(true);
        try {
            await teacherService.updateStudentGrade(selectedCourseId, studentId, grades[studentId]);
            toast.success('Grade updated');
        } catch (err) {
            toast.error('Failed to update grade');
        }
        setSaving(false);
    };

    if (loading) return <div className="text-center py-8">Loading...</div>;
    if (error) return <div className="text-center text-red-500 py-8">{error}</div>;
    if (!teacher) return <div className="text-center py-8 text-gray-500">No teacher data found.</div>;

    return (
        <div className="container mx-auto px-4 py-8">
            <h1 className="text-2xl font-bold mb-6">Manage Grades</h1>
            <div className="mb-6">
                <label className="block mb-2 font-semibold">Select Course:</label>
                <select
                    className="input w-full max-w-xs"
                    value={selectedCourseId}
                    onChange={e => setSelectedCourseId(e.target.value)}
                >
                    <option value="">-- Select a course --</option>
                    {teacher.courses && teacher.courses.map(course => (
                        <option key={course.id} value={course.id}>{course.name} ({course.code})</option>
                    ))}
                </select>
            </div>
            {fetching ? (
                <div>Loading students...</div>
            ) : selectedCourseId && students.length > 0 ? (
                <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-gray-200">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Student</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Grade</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                            </tr>
                        </thead>
                        <tbody className="bg-white divide-y divide-gray-200">
                            {students.map(student => (
                                <tr key={student.id}>
                                    <td className="px-6 py-4 whitespace-nowrap">{student.firstName} {student.lastName}</td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <input
                                            type="text"
                                            className="input w-24"
                                            value={grades[student.id] ?? ''}
                                            onChange={e => handleGradeChange(student.id, e.target.value)}
                                        />
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <button
                                            className="btn btn-primary btn-sm"
                                            onClick={() => handleSave(student.id)}
                                            disabled={saving}
                                        >
                                            Save
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            ) : selectedCourseId ? (
                <div className="text-gray-500">No students found for this course.</div>
            ) : null}
        </div>
    );
}

export default GradesPage;

import React, { useEffect, useState } from 'react';
import { useProfileData } from '../hooks/useProfileData';
import teacherService from '../services/teacherService';
import { toast } from 'react-toastify';

function AttendancePage() {
    const { data: teacher, loading, error } = useProfileData('/Teachers/me');
    const [selectedCourseId, setSelectedCourseId] = useState('');
    const [date, setDate] = useState(() => new Date().toISOString().slice(0, 10));
    const [students, setStudents] = useState([]);
    const [attendance, setAttendance] = useState({});
    const [saving, setSaving] = useState(false);
    const [fetching, setFetching] = useState(false);

    useEffect(() => {
        if (selectedCourseId) {
            fetchStudents(selectedCourseId);
        } else {
            setStudents([]);
            setAttendance({});
        }
    }, [selectedCourseId]);

    const fetchStudents = async (courseId) => {
        setFetching(true);
        try {
            const res = await teacherService.getCourseStudents(courseId);
            setStudents(res.students);
            setAttendance({});
        } catch (err) {
            toast.error('Failed to fetch students');
        }
        setFetching(false);
    };

    const handleToggle = (studentId) => {
        setAttendance((prev) => ({ ...prev, [studentId]: !prev[studentId] }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setSaving(true);
        try {
            await teacherService.recordAttendance(selectedCourseId, date, attendance);
            toast.success('Attendance recorded');
        } catch (err) {
            toast.error('Failed to record attendance');
        }
        setSaving(false);
    };

    if (loading) return <div className="text-center py-8">Loading...</div>;
    if (error) return <div className="text-center text-red-500 py-8">{error}</div>;
    if (!teacher) return <div className="text-center py-8 text-gray-500">No teacher data found.</div>;

    return (
        <div className="container mx-auto px-4 py-8">
            <h1 className="text-2xl font-bold mb-6">Mark Attendance</h1>
            <form onSubmit={handleSubmit} className="mb-6">
                <div className="mb-4">
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
                <div className="mb-4">
                    <label className="block mb-2 font-semibold">Date:</label>
                    <input
                        type="date"
                        className="input w-full max-w-xs"
                        value={date}
                        onChange={e => setDate(e.target.value)}
                    />
                </div>
                {fetching ? (
                    <div>Loading students...</div>
                ) : selectedCourseId && students.length > 0 ? (
                    <div className="overflow-x-auto mb-4">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Student</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Present</th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {students.map(student => (
                                    <tr key={student.id}>
                                        <td className="px-6 py-4 whitespace-nowrap">{student.firstName} {student.lastName}</td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <input
                                                type="checkbox"
                                                checked={!!attendance[student.id]}
                                                onChange={() => handleToggle(student.id)}
                                            />
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                ) : selectedCourseId ? (
                    <div className="text-gray-500">No students found for this course.</div>
                ) : null}
                <button
                    type="submit"
                    className="btn btn-primary mt-4"
                    disabled={saving || !selectedCourseId || students.length === 0}
                >
                    {saving ? 'Saving...' : 'Save Attendance'}
                </button>
            </form>
        </div>
    );
}

export default AttendancePage;

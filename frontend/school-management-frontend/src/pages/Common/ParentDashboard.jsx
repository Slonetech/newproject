import React, { useEffect, useState } from 'react';
import axiosInstance from '../../api/axiosConfig';
import { useAuth } from '../../context/AuthContext';

function ParentDashboard() {
    const { auth } = useAuth();
    const [children, setChildren] = useState([]);
    const [grades, setGrades] = useState([]);
    const [attendance, setAttendance] = useState([]);
    const [courses, setCourses] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const fetchData = async () => {
        setLoading(true);
        setError('');
        try {
            const [childrenRes, gradesRes, attendanceRes, coursesRes] = await Promise.all([
                axiosInstance.get(`/api/parents/${auth.user.id}/children`),
                axiosInstance.get(`/api/parents/${auth.user.id}/children-grades`),
                axiosInstance.get(`/api/parents/${auth.user.id}/children-attendance`),
                axiosInstance.get(`/api/parents/${auth.user.id}/children-courses`)
            ]);
            setChildren(childrenRes.data);
            setGrades(gradesRes.data);
            setAttendance(attendanceRes.data);
            setCourses(coursesRes.data);
        } catch (err) {
            setError('Failed to load dashboard data.');
        }
        setLoading(false);
    };

    useEffect(() => {
        if (auth.user?.id) fetchData();
        // eslint-disable-next-line
    }, [auth.user?.id]);

    if (loading) return <div className="text-center py-20 text-sky-700">Loading dashboard...</div>;
    if (error) return <div className="text-center py-20 text-red-600">{error}</div>;

    return (
        <div className="p-6 bg-white rounded-lg shadow-md mt-4 text-gray-800">
            <div className="flex justify-between items-center mb-4">
                <h1 className="text-3xl font-bold">Welcome, {auth.user?.firstName}!</h1>
                <button onClick={fetchData} className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600">Refresh</button>
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Children Profiles</h2>
                {children.length > 0 ? (
                    <ul className="space-y-2">
                        {children.map(child => (
                            <li key={child.id} className="border p-2 rounded">{child.firstName} {child.lastName} (ID: {child.id})</li>
                        ))}
                    </ul>
                ) : <div>No children linked.</div>}
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Children's Grades</h2>
                {grades.length > 0 ? (
                    <ul className="space-y-2">
                        {grades.slice(0, 5).map((g, idx) => (
                            <li key={idx} className="border p-2 rounded">{g.childName}: {g.courseName} - {g.value} ({g.letterGrade})</li>
                        ))}
                    </ul>
                ) : <div>No grades available.</div>}
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Children's Attendance</h2>
                {attendance.length > 0 ? (
                    <ul className="space-y-2">
                        {attendance.map((a, idx) => (
                            <li key={idx} className="border p-2 rounded">{a.childName}: {a.presentDays}/{a.totalDays} present ({a.attendancePercentage}%)</li>
                        ))}
                    </ul>
                ) : <div>No attendance data.</div>}
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Children's Courses</h2>
                {courses.length > 0 ? (
                    <ul className="space-y-2">
                        {courses.map((c, idx) => (
                            <li key={idx} className="border p-2 rounded">{c.childName}: {c.courseName} ({c.credits} credits)</li>
                        ))}
                    </ul>
                ) : <div>No course data.</div>}
            </div>
        </div>
    );
}

export default ParentDashboard;

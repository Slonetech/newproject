import React, { useEffect, useState } from 'react';
import axiosInstance from '../../api/axiosConfig';
import { useAuth } from '../../context/AuthContext';

function StudentDashboard() {
    const { auth } = useAuth();
    const [courses, setCourses] = useState([]);
    const [grades, setGrades] = useState([]);
    const [attendance, setAttendance] = useState(null);
    const [profile, setProfile] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const fetchData = async () => {
        setLoading(true);
        setError('');
        try {
            const [coursesRes, gradesRes, attendanceRes, profileRes] = await Promise.all([
                axiosInstance.get(`/api/students/${auth.user.id}/courses`),
                axiosInstance.get(`/api/students/${auth.user.id}/grades`),
                axiosInstance.get(`/api/students/${auth.user.id}/attendance`),
                axiosInstance.get(`/api/students/${auth.user.id}/profile`)
            ]);
            setCourses(coursesRes.data);
            setGrades(gradesRes.data);
            setAttendance(attendanceRes.data);
            setProfile(profileRes.data);
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
                <h1 className="text-3xl font-bold">Welcome, {profile?.firstName || auth.user?.firstName}!</h1>
                <button onClick={fetchData} className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600">Refresh</button>
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Profile</h2>
                {profile ? (
                    <ul className="list-disc ml-6 text-gray-700">
                        <li>Email: {profile.email}</li>
                        <li>Full Name: {profile.firstName} {profile.lastName}</li>
                        <li>Date of Birth: {profile.dateOfBirth}</li>
                        <li>Student ID: {profile.id}</li>
                    </ul>
                ) : <div>No profile data.</div>}
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Enrolled Courses</h2>
                {courses.length > 0 ? (
                    <ul className="space-y-2">
                        {courses.map(c => (
                            <li key={c.id} className="border p-2 rounded">{c.name} ({c.credits} credits)</li>
                        ))}
                    </ul>
                ) : <div>No courses enrolled.</div>}
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Recent Grades</h2>
                {grades.length > 0 ? (
                    <ul className="space-y-2">
                        {grades.slice(0, 5).map(g => (
                            <li key={g.id} className="border p-2 rounded">{g.courseName}: {g.value} ({g.letterGrade})</li>
                        ))}
                    </ul>
                ) : <div>No grades available.</div>}
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Attendance Summary</h2>
                {attendance ? (
                    <ul className="list-disc ml-6 text-gray-700">
                        <li>Total Days: {attendance.totalDays}</li>
                        <li>Present: {attendance.presentDays}</li>
                        <li>Absent: {attendance.absentDays}</li>
                        <li>Attendance %: {attendance.attendancePercentage}%</li>
                    </ul>
                ) : <div>No attendance data.</div>}
            </div>
        </div>
    );
}

export default StudentDashboard;

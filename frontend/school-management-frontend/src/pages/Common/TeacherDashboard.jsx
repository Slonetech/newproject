import React, { useEffect, useState } from 'react';
import axiosInstance from '../../api/axiosConfig';
import { useAuth } from '../../context/AuthContext';

function TeacherDashboard() {
    const { auth } = useAuth();
    const [courses, setCourses] = useState([]);
    const [students, setStudents] = useState([]);
    const [grades, setGrades] = useState([]);
    const [attendance, setAttendance] = useState(null);
    const [schedule, setSchedule] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const fetchData = async () => {
        setLoading(true);
        setError('');
        try {
            const [coursesRes, studentsRes, gradesRes, attendanceRes, scheduleRes] = await Promise.all([
                axiosInstance.get(`/api/teachers/${auth.user.id}/courses`),
                axiosInstance.get(`/api/teachers/${auth.user.id}/students`),
                axiosInstance.get(`/api/teachers/${auth.user.id}/grades`),
                axiosInstance.get(`/api/teachers/${auth.user.id}/attendance-summary`),
                axiosInstance.get(`/api/teachers/${auth.user.id}/schedule`)
            ]);
            setCourses(coursesRes.data);
            setStudents(studentsRes.data);
            setGrades(gradesRes.data);
            setAttendance(attendanceRes.data);
            setSchedule(scheduleRes.data);
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
                <h2 className="text-xl font-semibold mb-2">Assigned Courses</h2>
                {courses.length > 0 ? (
                    <ul className="space-y-2">
                        {courses.map(c => (
                            <li key={c.id} className="border p-2 rounded">{c.name} ({c.credits} credits)</li>
                        ))}
                    </ul>
                ) : <div>No assigned courses.</div>}
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Total Students</h2>
                <div>{students.length}</div>
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Recent Grades Entered</h2>
                {grades.length > 0 ? (
                    <ul className="space-y-2">
                        {grades.slice(0, 5).map(g => (
                            <li key={g.id} className="border p-2 rounded">{g.studentName} - {g.courseName}: {g.value} ({g.letterGrade})</li>
                        ))}
                    </ul>
                ) : <div>No grades entered recently.</div>}
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Attendance Statistics</h2>
                {attendance ? (
                    <ul className="list-disc ml-6 text-gray-700">
                        <li>Total Classes: {attendance.totalClasses}</li>
                        <li>Attendance Marked: {attendance.markedCount}</li>
                        <li>Average Attendance %: {attendance.averageAttendancePercent}%</li>
                    </ul>
                ) : <div>No attendance data.</div>}
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-2">Teaching Schedule</h2>
                {schedule.length > 0 ? (
                    <ul className="space-y-2">
                        {schedule.map((s, idx) => (
                            <li key={idx} className="border p-2 rounded">{s.day} - {s.time} - {s.courseName}</li>
                        ))}
                    </ul>
                ) : <div>No schedule data.</div>}
            </div>
        </div>
    );
}

export default TeacherDashboard;

import React, { useEffect, useState } from 'react';
import axiosInstance from '../../api/axiosConfig';
import { Link } from 'react-router-dom';

const AdminDashboard = () => {
    const [users, setUsers] = useState([]);
    const [teachers, setTeachers] = useState([]);
    const [students, setStudents] = useState([]);
    const [parents, setParents] = useState([]);
    const [courses, setCourses] = useState([]);
    const [grades, setGrades] = useState([]);
    const [attendances, setAttendances] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const fetchData = async () => {
        setLoading(true);
        setError('');
        try {
            const [usersRes, teachersRes, studentsRes, parentsRes, coursesRes, gradesRes, attendancesRes] = await Promise.all([
                axiosInstance.get('/api/Users'),
                axiosInstance.get('/api/Teachers'),
                axiosInstance.get('/api/Students'),
                axiosInstance.get('/api/Parents'),
                axiosInstance.get('/api/Courses'),
                axiosInstance.get('/api/Grades'),
                axiosInstance.get('/api/Attendances'),
            ]);
            setUsers(usersRes.data);
            setTeachers(teachersRes.data);
            setStudents(studentsRes.data);
            setParents(parentsRes.data);
            setCourses(coursesRes.data);
            setGrades(gradesRes.data);
            setAttendances(attendancesRes.data);
        } catch (err) {
            setError('Failed to load admin dashboard data.');
        }
        setLoading(false);
    };

    useEffect(() => {
        fetchData();
    }, []);

    // System statistics
    const totalUsers = users.length;
    const totalStudents = students.length;
    const totalTeachers = teachers.length;
    const totalParents = parents.length;
    const totalCourses = courses.length;
    const totalGrades = grades.length;
    const totalAttendance = attendances.length;

    // Recent activity (last 5 grades, attendance, users)
    const recentGrades = [...grades].sort((a, b) => new Date(b.date) - new Date(a.date)).slice(0, 5);
    const recentAttendance = [...attendances].sort((a, b) => new Date(b.date) - new Date(a.date)).slice(0, 5);
    const recentUsers = [...users].sort((a, b) => new Date(b.createdAt || 0) - new Date(a.createdAt || 0)).slice(0, 5);

    if (loading) return <div className="text-center py-20 text-sky-700">Loading admin dashboard...</div>;
    if (error) return <div className="text-center py-20 text-red-600">{error}</div>;

    return (
        <div className="min-h-screen bg-gray-50 flex flex-col items-center py-10 px-4 sm:px-6 lg:px-8 font-sans">
            <div className="max-w-6xl w-full bg-white rounded-xl shadow-lg p-8 sm:p-10 transition-all duration-300 ease-in-out">
                <div className="flex justify-between items-center mb-8">
                    <h2 className="text-4xl font-extrabold text-gray-900">Admin Control Panel</h2>
                    <button onClick={fetchData} className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600">Refresh</button>
                </div>
                {/* System Overview Cards */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-10">
                    <div className="bg-blue-100 rounded-lg p-6 flex flex-col items-center shadow">
                        <div className="text-3xl font-bold text-blue-700">{totalUsers}</div>
                        <div className="text-lg text-blue-900 font-semibold">Total Users</div>
                    </div>
                    <div className="bg-green-100 rounded-lg p-6 flex flex-col items-center shadow">
                        <div className="text-3xl font-bold text-green-700">{totalStudents}</div>
                        <div className="text-lg text-green-900 font-semibold">Students</div>
                    </div>
                    <div className="bg-indigo-100 rounded-lg p-6 flex flex-col items-center shadow">
                        <div className="text-3xl font-bold text-indigo-700">{totalTeachers}</div>
                        <div className="text-lg text-indigo-900 font-semibold">Teachers</div>
                    </div>
                    <div className="bg-purple-100 rounded-lg p-6 flex flex-col items-center shadow">
                        <div className="text-3xl font-bold text-purple-700">{totalParents}</div>
                        <div className="text-lg text-purple-900 font-semibold">Parents</div>
                    </div>
                    <div className="bg-yellow-100 rounded-lg p-6 flex flex-col items-center shadow">
                        <div className="text-3xl font-bold text-yellow-700">{totalCourses}</div>
                        <div className="text-lg text-yellow-900 font-semibold">Courses</div>
                    </div>
                    <div className="bg-pink-100 rounded-lg p-6 flex flex-col items-center shadow">
                        <div className="text-3xl font-bold text-pink-700">{totalGrades}</div>
                        <div className="text-lg text-pink-900 font-semibold">Grades Recorded</div>
                    </div>
                </div>
                {/* User Management Widget */}
                <div className="mb-10">
                    <h3 className="text-2xl font-bold mb-4">User Management</h3>
                    <div className="flex flex-wrap gap-4 mb-4">
                        <Link to="/admin/users" className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600">Manage Users</Link>
                        <Link to="/admin/teachers" className="bg-indigo-500 text-white px-4 py-2 rounded hover:bg-indigo-600">Manage Teachers</Link>
                        <Link to="/admin/parents" className="bg-purple-500 text-white px-4 py-2 rounded hover:bg-purple-600">Manage Parents</Link>
                        <Link to="/admin/courses" className="bg-yellow-500 text-white px-4 py-2 rounded hover:bg-yellow-600">Manage Courses</Link>
                    </div>
                    <div className="text-gray-700">Recent Registrations:</div>
                    <ul className="list-disc ml-6 mt-2">
                        {recentUsers.map(u => (
                            <li key={u.id}>{u.firstName} {u.lastName} ({u.email}) - Roles: {u.roles?.join(', ')}</li>
                        ))}
                    </ul>
                </div>
                {/* Course Management Widget */}
                <div className="mb-10">
                    <h3 className="text-2xl font-bold mb-4">Course Management</h3>
                    <div className="text-gray-700 mb-2">Active Courses: {totalCourses}</div>
                    <div className="text-gray-700 mb-2">Popular Courses: {courses.slice(0, 3).map(c => c.name).join(', ')}</div>
                    <div className="text-gray-700 mb-2">Recent Course Activities:</div>
                    <ul className="list-disc ml-6 mt-2">
                        {grades.slice(0, 5).map(g => (
                            <li key={g.id}>Grade: {g.value} for Student {g.studentId} in Course {g.courseId}</li>
                        ))}
                    </ul>
                </div>
                {/* Activity Feed */}
                <div className="mb-10">
                    <h3 className="text-2xl font-bold mb-4">Recent Activity Feed</h3>
                    <ul className="list-disc ml-6">
                        {recentGrades.map(g => (
                            <li key={g.id}>Grade {g.value} entered for Student {g.studentId} in Course {g.courseId} ({g.date && new Date(g.date).toLocaleString()})</li>
                        ))}
                        {recentAttendance.map(a => (
                            <li key={a.id}>Attendance for Student {a.studentId} in Course {a.courseId}: {a.isPresent ? 'Present' : 'Absent'} ({a.date && new Date(a.date).toLocaleString()})</li>
                        ))}
                    </ul>
                </div>
            </div>
        </div>
    );
};

export default AdminDashboard;

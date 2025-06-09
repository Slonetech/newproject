import React from 'react';
import { useAuth } from '../../context/AuthContext';
import { Link } from 'react-router-dom';

function Dashboard() {
    const { auth } = useAuth(); // Get auth object from context (which contains user and roles)

    return (
        <div className="p-6 bg-white rounded-lg shadow-md mt-4 text-gray-800">
            <h1 className="text-3xl font-bold mb-4">Welcome to Your Dashboard, {auth.user?.username}!</h1>
            <p className="text-lg text-gray-600 mb-6">Your roles: <span className="font-semibold">{auth.roles.join(', ')}</span></p>

            <div className="bg-gray-50 border border-gray-200 rounded-lg p-6">
                <h3 className="text-xl font-bold mb-3 text-gray-700">Quick Access & Information</h3>
                {auth.roles.includes('Admin') && (
                    <p className="mb-2">As an <span className="font-semibold text-blue-700">Admin</span>, you have full control over users, students, teachers, courses, grades, and attendance. <Link to="/admin" className="text-blue-500 hover:underline">Go to Admin Panel</Link></p>
                )}
                {auth.roles.includes('Teacher') && (
                    <p className="mb-2">As a <span className="font-semibold text-green-700">Teacher</span>, you can manage your enrolled students, record grades, and mark attendance. <Link to="/teacher" className="text-green-500 hover:underline">Go to Teacher Dashboard</Link></p>
                )}
                {auth.roles.includes('Student') && (
                    <p className="mb-2">As a <span className="font-semibold text-purple-700">Student</span>, you can view your profile, enrolled courses, grades, and attendance records. <Link to="/student" className="text-purple-500 hover:underline">Go to Student Dashboard</Link></p>
                )}
                {auth.roles.includes('Parent') && (
                    <p className="mb-2">As a <span className="font-semibold text-orange-700">Parent</span>, you can monitor your child's academic progress, including their courses, grades, and attendance. <Link to="/parent" className="text-orange-500 hover:underline">Go to Parent Dashboard</Link></p>
                )}
            </div>

            {/* Placeholder for additional dashboard sections */}
            <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="bg-blue-50 p-4 rounded-lg shadow-sm">
                    <h4 className="font-semibold text-blue-700">Announcements</h4>
                    <p className="text-gray-700 text-sm">No new announcements.</p>
                </div>
                <div className="bg-green-50 p-4 rounded-lg shadow-sm">
                    <h4 className="font-semibold text-green-700">Upcoming Events</h4>
                    <p className="text-gray-700 text-sm">No upcoming events.</p>
                </div>
            </div>
        </div>
    );
}

export default Dashboard;

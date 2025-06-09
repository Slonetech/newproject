import React from 'react';
import { Link } from 'react-router-dom';

const AdminDashboard = () => {
    return (
        <div className="p-6 bg-white rounded-lg shadow-md mt-4">
            <h2 className="text-2xl font-semibold mb-4 text-gray-800">Admin Dashboard</h2>
            <p className="text-gray-600 mb-6">Welcome, Administrator! Here are your management options:</p>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {/* User Management Card */}
                <div className="bg-blue-50 border border-blue-200 rounded-lg p-6 flex flex-col items-center shadow-sm hover:shadow-md transition-shadow duration-200">
                    <h3 className="text-xl font-bold mb-3 text-blue-700">User Management</h3>
                    <p className="text-center text-gray-700 mb-4">Create, view, update, and delete user accounts. Assign and remove roles.</p>
                    <Link
                        to="/admin/users"
                        className="bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 transition duration-200 text-center"
                    >
                        Manage Users
                    </Link>
                </div>

                {/* Teacher Management Card (Admin's view) */}
                <div className="bg-indigo-50 border border-indigo-200 rounded-lg p-6 flex flex-col items-center shadow-sm hover:shadow-md transition-shadow duration-200">
                    <h3 className="text-xl font-bold mb-3 text-indigo-700">Teacher Profiles</h3>
                    <p className="text-center text-gray-700 mb-4">Manage detailed teacher profiles, departments, and linked user accounts.</p>
                    <Link
                        to="/admin/teachers"
                        className="bg-indigo-600 text-white py-2 px-4 rounded-md hover:bg-indigo-700 transition duration-200 text-center"
                    >
                        Manage Teachers
                    </Link>
                </div>

                {/* Parent Management Card (Admin's view) */}
                <div className="bg-purple-50 border border-purple-200 rounded-lg p-6 flex flex-col items-center shadow-sm hover:shadow-md transition-shadow duration-200">
                    <h3 className="text-xl font-bold mb-3 text-purple-700">Parent Profiles</h3>
                    <p className="text-center text-gray-700 mb-4">Manage detailed parent profiles and associate them with student children.</p>
                    <Link
                        to="/admin/parents"
                        className="bg-purple-600 text-white py-2 px-4 rounded-md hover:bg-purple-700 transition duration-200 text-center"
                    >
                        Manage Parents
                    </Link>
                </div>

                {/* General Data Management Links (Admin can access general student/course/grade/attendance pages) */}
                <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-6 flex flex-col items-center shadow-sm hover:shadow-md transition-shadow duration-200">
                    <h3 className="text-xl font-bold mb-3 text-yellow-700">General Data</h3>
                    <p className="text-center text-gray-700 mb-4">Access and manage all student, course, grade, and attendance records.</p>
                    <div className="flex flex-wrap justify-center gap-2">
                        <Link to="/students" className="bg-yellow-600 text-white py-1.5 px-3 rounded-md hover:bg-yellow-700 text-center text-sm">All Students</Link>
                        <Link to="/courses" className="bg-yellow-600 text-white py-1.5 px-3 rounded-md hover:bg-yellow-700 text-center text-sm">All Courses</Link>
                        <Link to="/grades" className="bg-yellow-600 text-white py-1.5 px-3 rounded-md hover:bg-yellow-700 text-center text-sm">All Grades</Link>
                        <Link to="/attendance" className="bg-yellow-600 text-white py-1.5 px-3 rounded-md hover:bg-yellow-700 text-center text-sm">All Attendance</Link>
                    </div>
                </div>

                <div className="bg-teal-50 border border-teal-200 rounded-lg p-6 flex flex-col items-center shadow-sm hover:shadow-md transition-shadow duration-200">
                    <h3 className="text-xl font-bold mb-3 text-teal-700">Reporting & Analytics</h3>
                    <p className="text-center text-gray-700 mb-4">Generate various reports on students, grades, and attendance.</p>
                    <Link
                        to="/admin/reports" // Placeholder for a future reports page
                        className="bg-teal-600 text-white py-2 px-4 rounded-md hover:bg-teal-700 transition duration-200 text-center"
                    >
                        View Reports
                    </Link>
                </div>
            </div>
        </div>
    );
};

export default AdminDashboard;

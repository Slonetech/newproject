import React from 'react';
import { Link } from 'react-router-dom';

const AdminDashboard = () => {
    return (
        <div className="min-h-screen bg-gray-50 flex flex-col items-center py-10 px-4 sm:px-6 lg:px-8 font-sans">
            <div className="max-w-4xl w-full bg-white rounded-xl shadow-lg p-8 sm:p-10 transition-all duration-300 ease-in-out">
                <h2 className="text-4xl font-extrabold text-gray-900 mb-6 text-center">Admin Control Panel</h2>
                <p className="text-lg text-gray-700 mb-10 text-center leading-relaxed">
                    Welcome, Administrator! This central hub provides quick access to manage users, school data, and generate reports.
                </p>

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                    {/* User Management Card */}
                    <div className="bg-gradient-to-br from-blue-500 to-blue-600 text-white rounded-xl p-6 sm:p-8 flex flex-col items-center justify-between shadow-lg hover:shadow-xl transform hover:-translate-y-1 transition-all duration-300 ease-in-out cursor-pointer">
                        <div className="mb-4 text-5xl">
                            <i className="fas fa-users"></i>
                        </div>
                        <h3 className="text-2xl font-bold mb-3 text-center">User Management</h3>
                        <p className="text-center text-blue-100 mb-6 flex-grow">Create, view, update, and delete user accounts. Assign and remove roles with precision.</p>
                        <Link
                            to="/admin/users"
                            className="w-full bg-white text-blue-700 font-semibold py-3 px-6 rounded-lg hover:bg-blue-100 transition duration-200 text-center shadow-md hover:shadow-lg"
                        >
                            Manage Users
                        </Link>
                    </div>

                    {/* Teacher Profiles Card */}
                    <div className="bg-gradient-to-br from-indigo-500 to-indigo-600 text-white rounded-xl p-6 sm:p-8 flex flex-col items-center justify-between shadow-lg hover:shadow-xl transform hover:-translate-y-1 transition-all duration-300 ease-in-out cursor-pointer">
                        <div className="mb-4 text-5xl">
                            <i className="fas fa-chalkboard-teacher"></i>
                        </div>
                        <h3 className="text-2xl font-bold mb-3 text-center">Teacher Profiles</h3>
                        <p className="text-center text-indigo-100 mb-6 flex-grow">Maintain detailed teacher profiles, including departments and linked user accounts.</p>
                        <Link
                            to="/admin/teachers"
                            className="w-full bg-white text-indigo-700 font-semibold py-3 px-6 rounded-lg hover:bg-indigo-100 transition duration-200 text-center shadow-md hover:shadow-lg"
                        >
                            Manage Teachers
                        </Link>
                    </div>

                    {/* Parent Profiles Card */}
                    <div className="bg-gradient-to-br from-purple-500 to-purple-600 text-white rounded-xl p-6 sm:p-8 flex flex-col items-center justify-between shadow-lg hover:shadow-xl transform hover:-translate-y-1 transition-all duration-300 ease-in-out cursor-pointer">
                        <div className="mb-4 text-5xl">
                            <i className="fas fa-user-friends"></i>
                        </div>
                        <h3 className="text-2xl font-bold mb-3 text-center">Parent Profiles</h3>
                        <p className="text-center text-purple-100 mb-6 flex-grow">Manage parent accounts and their association with student children for easy monitoring.</p>
                        <Link
                            to="/admin/parents"
                            className="w-full bg-white text-purple-700 font-semibold py-3 px-6 rounded-lg hover:bg-purple-100 transition duration-200 text-center shadow-md hover:shadow-lg"
                        >
                            Manage Parents
                        </Link>
                    </div>

                    {/* System Settings Card */}
                    <div className="bg-gradient-to-br from-gray-700 to-gray-800 text-white rounded-xl p-6 sm:p-8 flex flex-col items-center justify-between shadow-lg hover:shadow-xl transform hover:-translate-y-1 transition-all duration-300 ease-in-out cursor-pointer">
                        <div className="mb-4 text-5xl">
                            <i className="fas fa-cog"></i>
                        </div>
                        <h3 className="text-2xl font-bold mb-3 text-center">System Settings</h3>
                        <p className="text-center text-gray-300 mb-6 flex-grow">Configure global system settings, academic years, and other administrative parameters.</p>
                        <Link
                            to="/admin/settings"
                            className="w-full bg-white text-gray-700 font-semibold py-3 px-6 rounded-lg hover:bg-gray-100 transition duration-200 text-center shadow-md hover:shadow-lg"
                        >
                            Configure Settings
                        </Link>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AdminDashboard;

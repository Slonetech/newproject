import React from 'react';
import { useAuth } from '../../context/AuthContext';

const TeacherDashboard = () => {
    const { user } = useAuth();

    return (
        <div className="container mx-auto px-4 py-8">
            <h1 className="text-2xl font-bold mb-6">Teacher Dashboard</h1>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {/* Welcome Card */}
                <div className="bg-white rounded-lg shadow-md p-6">
                    <h2 className="text-xl font-semibold mb-4">Welcome, {user?.firstName}!</h2>
                    <p className="text-gray-600">Manage your courses, students, and teaching schedule.</p>
                </div>

                {/* Quick Stats */}
                <div className="bg-white rounded-lg shadow-md p-6">
                    <h2 className="text-xl font-semibold mb-4">Quick Stats</h2>
                    <div className="space-y-2">
                        <p className="text-gray-600">Active Courses: <span className="font-semibold">0</span></p>
                        <p className="text-gray-600">Total Students: <span className="font-semibold">0</span></p>
                        <p className="text-gray-600">Upcoming Classes: <span className="font-semibold">0</span></p>
                    </div>
                </div>

                {/* Recent Activity */}
                <div className="bg-white rounded-lg shadow-md p-6">
                    <h2 className="text-xl font-semibold mb-4">Recent Activity</h2>
                    <p className="text-gray-600">No recent activity to display.</p>
                </div>
            </div>

            {/* Upcoming Classes */}
            <div className="mt-8">
                <h2 className="text-xl font-semibold mb-4">Upcoming Classes</h2>
                <div className="bg-white rounded-lg shadow-md p-6">
                    <p className="text-gray-600">No upcoming classes scheduled.</p>
                </div>
            </div>

            {/* Course Management */}
            <div className="mt-8">
                <h2 className="text-xl font-semibold mb-4">Your Courses</h2>
                <div className="bg-white rounded-lg shadow-md p-6">
                    <p className="text-gray-600">No courses assigned yet.</p>
                </div>
            </div>
        </div>
    );
};

export default TeacherDashboard; 
import React from 'react';
import { useAuth } from '../../context/AuthContext';

const ParentDashboard = () => {
    const { user } = useAuth();

    return (
        <div className="container mx-auto px-4 py-8">
            <h1 className="text-2xl font-bold mb-6">Parent Dashboard</h1>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {/* Welcome Card */}
                <div className="bg-white rounded-lg shadow-md p-6">
                    <h2 className="text-xl font-semibold mb-4">Welcome, {user?.firstName}!</h2>
                    <p className="text-gray-600">Monitor your children's academic progress and activities.</p>
                </div>

                {/* Quick Stats */}
                <div className="bg-white rounded-lg shadow-md p-6">
                    <h2 className="text-xl font-semibold mb-4">Quick Stats</h2>
                    <div className="space-y-2">
                        <p className="text-gray-600">Children: <span className="font-semibold">0</span></p>
                        <p className="text-gray-600">Average Grade: <span className="font-semibold">N/A</span></p>
                        <p className="text-gray-600">Attendance Rate: <span className="font-semibold">N/A</span></p>
                    </div>
                </div>

                {/* Recent Activity */}
                <div className="bg-white rounded-lg shadow-md p-6">
                    <h2 className="text-xl font-semibold mb-4">Recent Activity</h2>
                    <p className="text-gray-600">No recent activity to display.</p>
                </div>
            </div>

            {/* Children Overview */}
            <div className="mt-8">
                <h2 className="text-xl font-semibold mb-4">Your Children</h2>
                <div className="bg-white rounded-lg shadow-md p-6">
                    <p className="text-gray-600">No children registered yet.</p>
                </div>
            </div>

            {/* Academic Progress */}
            <div className="mt-8">
                <h2 className="text-xl font-semibold mb-4">Academic Progress</h2>
                <div className="bg-white rounded-lg shadow-md p-6">
                    <p className="text-gray-600">No academic records available.</p>
                </div>
            </div>
        </div>
    );
};

export default ParentDashboard; 
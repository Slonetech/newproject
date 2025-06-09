import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

function ParentDashboard() {
    const { auth } = useAuth(); // Get auth object from context

    return (
        <div className="p-6 bg-white rounded-lg shadow-md mt-4 text-gray-800">
            <h1 className="text-3xl font-bold mb-4">Welcome, Parent {auth.user?.firstName}!</h1>
            <p className="text-lg text-gray-600 mb-6">This is your personalized dashboard.</p>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {/* Link to view child's grades */}
                <div className="bg-purple-50 border border-purple-200 rounded-lg p-6 flex flex-col items-center shadow-sm hover:shadow-md transition-shadow duration-200">
                    <h3 className="text-xl font-bold mb-3 text-purple-700">Child's Grades</h3>
                    <p className="text-center text-gray-700 mb-4">Monitor your child's academic performance and grades.</p>
                    <Link
                        to="/grades" // This page will need to filter by child's ID (via ParentProfile API)
                        className="bg-purple-600 text-white py-2 px-4 rounded-md hover:bg-purple-700 transition duration-200 text-center"
                    >
                        View Grades
                    </Link>
                </div>

                {/* Link to view child's attendance */}
                <div className="bg-orange-50 border border-orange-200 rounded-lg p-6 flex flex-col items-center shadow-sm hover:shadow-md transition-shadow duration-200">
                    <h3 className="text-xl font-bold mb-3 text-orange-700">Child's Attendance</h3>
                    <p className="text-center text-gray-700 mb-4">Review daily attendance records for your child.</p>
                    <Link
                        to="/attendance" // This page will need to filter by child's ID (via ParentProfile API)
                        className="bg-orange-600 text-white py-2 px-4 rounded-md hover:bg-orange-700 transition duration-200 text-center"
                    >
                        View Attendance
                    </Link>
                </div>

                {/* Link to view child's courses */}
                <div className="bg-green-50 border border-green-200 rounded-lg p-6 flex flex-col items-center shadow-sm hover:shadow-md transition-shadow duration-200">
                    <h3 className="text-xl font-bold mb-3 text-green-700">Child's Courses</h3>
                    <p className="text-center text-gray-700 mb-4">See the list of courses your child is currently enrolled in.</p>
                    <Link
                        to="/courses" // This page will need to filter by child's ID (via ParentProfile API)
                        className="bg-green-600 text-white py-2 px-4 rounded-md hover:bg-green-700 transition duration-200 text-center"
                    >
                        View Courses
                    </Link>
                </div>
            </div>
        </div>
    );
}

export default ParentDashboard;

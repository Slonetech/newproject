import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

function TeacherDashboard() {
    const { auth } = useAuth(); // Get auth object from context

    return (
        <div className="p-6 bg-white rounded-lg shadow-md mt-4 text-gray-800">
            <h1 className="text-3xl font-bold mb-4">Welcome, Teacher {auth.user?.firstName}!</h1>
            <p className="text-lg text-gray-600 mb-6">This is your personalized dashboard.</p>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {/* Link to Manage Students (only those assigned to this teacher) */}
                <div className="bg-green-50 border border-green-200 rounded-lg p-6 flex flex-col items-center shadow-sm hover:shadow-md transition-shadow duration-200">
                    <h3 className="text-xl font-bold mb-3 text-green-700">Manage Students</h3>
                    <p className="text-center text-gray-700 mb-4">View and manage your assigned students.</p>
                    <Link
                        to="/students" // This page will need to filter by teacher's assigned students
                        className="bg-green-600 text-white py-2 px-4 rounded-md hover:bg-green-700 transition duration-200 text-center"
                    >
                        View Students
                    </Link>
                </div>

                {/* Link to Manage Grades */}
                <div className="bg-blue-50 border border-blue-200 rounded-lg p-6 flex flex-col items-center shadow-sm hover:shadow-md transition-shadow duration-200">
                    <h3 className="text-xl font-bold mb-3 text-blue-700">Manage Grades</h3>
                    <p className="text-center text-gray-700 mb-4">Record and update grades for your courses.</p>
                    <Link
                        to="/grades" // This page will need to filter by teacher's assigned courses/students
                        className="bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 transition duration-200 text-center"
                    >
                        Manage Grades
                    </Link>
                </div>

                {/* Link to Mark Attendance */}
                <div className="bg-purple-50 border border-purple-200 rounded-lg p-6 flex flex-col items-center shadow-sm hover:shadow-md transition-shadow duration-200">
                    <h3 className="text-xl font-bold mb-3 text-purple-700">Mark Attendance</h3>
                    <p className="text-center text-gray-700 mb-4">Record student attendance for your classes.</p>
                    <Link
                        to="/attendance" // This page will need to filter by teacher's assigned courses/students
                        className="bg-purple-600 text-white py-2 px-4 rounded-md hover:bg-purple-700 transition duration-200 text-center"
                    >
                        Mark Attendance
                    </Link>
                </div>
            </div>
        </div>
    );
}

export default TeacherDashboard;

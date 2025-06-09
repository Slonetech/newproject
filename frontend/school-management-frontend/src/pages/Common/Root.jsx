import React from 'react';
import { Link } from 'react-router-dom';

function Root() {
    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100 text-gray-800 font-sans">
            <h1 className="text-5xl font-bold text-blue-700 mb-4">Welcome to School Management System</h1>
            <p className="text-xl text-gray-700 mb-8 text-center max-w-2xl">
                A comprehensive platform to manage students, teachers, parents, courses, grades, and attendance.
            </p>
            <div className="flex space-x-4">
                <Link
                    to="/login"
                    className="bg-blue-600 text-white py-3 px-6 rounded-md text-lg hover:bg-blue-700 transition duration-200 shadow-lg"
                >
                    Login
                </Link>
                <Link
                    to="/register"
                    className="bg-green-600 text-white py-3 px-6 rounded-md text-lg hover:bg-green-700 transition duration-200 shadow-lg"
                >
                    Register
                </Link>
            </div>
        </div>
    );
}

export default Root;

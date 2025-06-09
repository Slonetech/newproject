import React from 'react';
import { Link } from 'react-router-dom';

function NotFound() {
    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100 text-gray-800 font-sans">
            <h1 className="text-6xl font-bold text-red-600 mb-4">404</h1>
            <h2 className="text-2xl font-semibold mb-3">Page Not Found</h2>
            <p className="text-lg text-gray-700 mb-6">The page you are looking for does not exist.</p>
            <Link
                to="/"
                className="bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 transition duration-200"
            >
                Go to Home
            </Link>
        </div>
    );
}

export default NotFound;

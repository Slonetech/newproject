// src/pages/Common/NotFound.jsx
import React from 'react';
import { Link } from 'react-router-dom';

const NotFound = () => {
    return (
        <div className="min-h-screen flex flex-col items-center justify-center bg-gray-50 text-gray-800 p-4">
            <h1 className="text-5xl font-bold mb-4">404</h1>
            <h2 className="text-2xl font-semibold mb-6">Page Not Found</h2>
            <p className="text-lg text-center mb-8 max-w-prose">
                The page you are looking for might have been removed, had its name changed, or is temporarily unavailable.
            </p>
            <Link to="/" className="bg-sky-600 text-white px-6 py-3 rounded-md shadow-md hover:bg-sky-700 transition duration-200">
                Go to Home
            </Link>
        </div>
    );
};

export default NotFound;
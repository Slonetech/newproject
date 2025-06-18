// src/pages/Common/UnauthorizedPage.jsx
import React from 'react';
import { Link } from 'react-router-dom';

const UnauthorizedPage = () => {
    return (
        <div className="min-h-screen flex flex-col items-center justify-center bg-red-50 text-red-800 p-4">
            <h1 className="text-5xl font-bold mb-4">403</h1>
            <h2 className="text-2xl font-semibold mb-6">Access Denied!</h2>
            <p className="text-lg text-center mb-8 max-w-prose">
                You don't have the necessary permissions to view this page.
                Please contact your administrator if you believe this is an error.
            </p>
            <Link to="/" className="bg-red-600 text-white px-6 py-3 rounded-md shadow-md hover:bg-red-700 transition duration-200">
                Go to Home
            </Link>
        </div>
    );
};

export default UnauthorizedPage;
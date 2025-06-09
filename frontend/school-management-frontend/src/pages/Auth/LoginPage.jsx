import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

function LoginPage() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const { login, auth, loading, error } = useAuth(); // Get auth, login, loading, error from context
    const navigate = useNavigate();

    // Effect to redirect if already logged in (e.g., user navigates back to /login when authenticated)
    useEffect(() => {
        // Ensure auth.user exists before attempting to redirect, as auth.roles might be empty initially
        if (auth.token && auth.user && auth.roles && auth.roles.length > 0) {
            redirectBasedOnRole(auth.roles);
        }
    }, [auth.token, auth.user, auth.roles, navigate]); // Dependencies for useEffect

    // Helper function to redirect based on user roles
    const redirectBasedOnRole = (roles) => {
        if (!roles || roles.length === 0) {
            navigate('/dashboard'); // Default if no specific roles
            return;
        }
        if (roles.includes("Admin")) {
            navigate('/admin/users'); // Redirect Admin to user management
        } else if (roles.includes("Teacher")) {
            navigate('/teacher'); // Teacher dashboard
        } else if (roles.includes("Student")) {
            navigate('/student'); // Student dashboard
        } else if (roles.includes("Parent")) {
            navigate('/parent'); // Parent dashboard
        } else {
            navigate('/dashboard'); // Fallback to a general dashboard
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const success = await login(username, password); // Call login from context
            if (success) {
                // The auth state should be updated by now, so use auth.roles for redirection
                redirectBasedOnRole(auth.roles);
            }
        } catch (err) {
            // Error is already handled and set in AuthContext and displayed by the component via `error` prop
            console.error('Login form submission error:', err);
        }
    };

    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-100 font-sans">
            <div className="bg-white p-8 rounded-lg shadow-md w-full max-w-md">
                <h2 className="text-3xl font-bold mb-6 text-center text-gray-800">Login to School Management</h2>
                <form onSubmit={handleSubmit} className="space-y-4">
                    <div className="form-group">
                        <label htmlFor="username" className="block text-gray-700 text-sm font-medium mb-2">Username:</label>
                        <input
                            type="text"
                            id="username"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                            className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="password" className="block text-gray-700 text-sm font-medium mb-2">Password:</label>
                        <input
                            type="password"
                            id="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>

                    {error && <p className="text-red-600 text-sm text-center">{error}</p>}

                    <button type="submit" disabled={loading} className="w-full bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition duration-200">
                        {loading ? 'Logging in...' : 'Login'}
                    </button>
                </form>
                <p className="mt-6 text-center text-gray-600">
                    Don't have an account? <Link to="/register" className="text-blue-600 hover:underline">Register here</Link>
                </p>
            </div>
        </div>
    );
}

export default LoginPage;

import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

function LoginPage() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const { login, auth, loading, error } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        if (auth.token && auth.user && auth.roles && auth.roles.length > 0) {
            redirectBasedOnRole(auth.roles);
        }
    }, [auth.token, auth.user, auth.roles, navigate]);

    const redirectBasedOnRole = (roles) => {
        if (!roles || roles.length === 0) {
            navigate('/dashboard');
            return;
        }
        if (roles.includes("Admin")) {
            navigate('/admin/users');
        } else if (roles.includes("Teacher")) {
            navigate('/teacher');
        } else if (roles.includes("Student")) {
            navigate('/student');
        } else if (roles.includes("Parent")) {
            navigate('/parent');
        } else {
            navigate('/dashboard');
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const success = await login(username, password);
            if (success) {
                redirectBasedOnRole(auth.roles);
            }
        } catch (err) {
            console.error('Login form submission error:', err);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-sky-50 to-sky-100 p-4">
            <div className="bg-white p-8 rounded-lg shadow-lg w-full max-w-md">
                <div className="text-center mb-8">
                    <h2 className="text-3xl font-bold text-sky-700 mb-2">Welcome Back</h2>
                    <p className="text-sky-600">Sign in to your school management account</p>
                </div>

                <form onSubmit={handleSubmit} className="space-y-6">
                    <div className="space-y-2">
                        <label htmlFor="username" className="flex items-center text-sky-700 font-medium">
                            <i className="fas fa-user text-sky-400 mr-2"></i>
                            Username
                        </label>
                        <input
                            type="text"
                            id="username"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                            className="w-full px-4 py-2 border border-sky-200 rounded-md focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent"
                            placeholder="Enter your username"
                        />
                    </div>

                    <div className="space-y-2">
                        <label htmlFor="password" className="flex items-center text-sky-700 font-medium">
                            <i className="fas fa-lock text-sky-400 mr-2"></i>
                            Password
                        </label>
                        <input
                            type="password"
                            id="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            className="w-full px-4 py-2 border border-sky-200 rounded-md focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent"
                            placeholder="Enter your password"
                        />
                    </div>

                    {error && (
                        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md animate-slide-in">
                            <i className="fas fa-exclamation-circle mr-2"></i>
                            {error}
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={loading}
                        className="w-full bg-sky-600 text-white py-2 px-4 rounded-md hover:bg-sky-700 transition duration-200 flex items-center justify-center"
                    >
                        {loading ? (
                            <>
                                <i className="fas fa-spinner fa-spin mr-2"></i>
                                Signing in...
                            </>
                        ) : (
                            <>
                                <i className="fas fa-sign-in-alt mr-2"></i>
                                Sign In
                            </>
                        )}
                    </button>
                </form>

                <div className="mt-8 text-center">
                    <p className="text-sky-600">
                        Don't have an account?{' '}
                        <Link to="/register" className="text-sky-700 hover:text-sky-800 font-medium">
                            Register here
                        </Link>
                    </p>
                </div>

                <div className="mt-6 text-center">
                    <Link to="/forgot-password" className="text-sky-600 hover:text-sky-700 text-sm">
                        Forgot your password?
                    </Link>
                </div>
            </div>
        </div>
    );
}

export default LoginPage;

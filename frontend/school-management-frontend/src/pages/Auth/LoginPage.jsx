import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    // Ensure you destructure 'login' directly and get the 'auth' state, 'loading', and 'error'
    const { login, auth, loading, error: authError } = useAuth(); // Renamed 'error' to 'authError' to avoid conflict
    const navigate = useNavigate();

    // The redirect logic has been moved to AuthContext's login function for better control.
    // This useEffect is now primarily for initial load/refresh if already authenticated.
    useEffect(() => {
        if (auth.isAuthenticated && auth.user && auth.user.roles) {
            // The AuthContext's ProtectedRoute or login function will handle the redirect.
            // No need for explicit redirect logic here unless it's a very specific edge case.
            // For example, if a user directly navigates to /login while already logged in.
            // If they are logged in and hit /login, AuthContext.ProtectedRoute on '/' will
            // redirect them appropriately. If they specifically land on /login
            // while already logged in, you might want to redirect them away.
            // Example: navigate('/admin' or appropriate dashboard based on their current roles)
            // However, AuthContext's login already handles this, and ProtectedRoute for '/' also does.
            // So, for simplicity, we can rely on those.
        }
    }, [auth.isAuthenticated, auth.user, navigate]);


    const handleSubmit = async (e) => {
        e.preventDefault();
        // The error state is now managed by AuthContext, so we don't need a local one here
        // unless you want to add specific form validation errors *before* calling login.

        try {
            // Call the login function from AuthContext, passing email and password directly
            // AuthContext's login function will now handle the state updates (loading, error)
            // and the navigation after successful login.
            await login(email, password);
            // If login is successful, AuthContext will navigate, so no code below this line is needed for success.
        } catch (err) {
            // Error is already set in AuthContext and can be displayed via authError
            // console.error('Login form submission error:', err); // Already logged in AuthContext
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
                        <label htmlFor="email" className="flex items-center text-sky-700 font-medium">
                            <i className="fas fa-envelope text-sky-400 mr-2"></i>
                            Email
                        </label>
                        <input
                            type="email"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                            className="w-full px-4 py-2 border border-sky-200 rounded-md focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent"
                            placeholder="Enter your email"
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

                    {authError && ( // Use authError from AuthContext
                        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md animate-slide-in">
                            <i className="fas fa-exclamation-circle mr-2"></i>
                            {authError}
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={loading} // Use loading from AuthContext
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
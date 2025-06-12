import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { register as apiRegister } from '../../services/authService';

function RegisterPage() {
    const [form, setForm] = useState({
        username: '',
        email: '',
        password: '',
        firstName: '',
        lastName: '',
    });
    const [error, setError] = useState(null);
    const [successMessage, setSuccessMessage] = useState(null);
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { id, value } = e.target;
        setForm((prev) => ({ ...prev, [id]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);
        setSuccessMessage(null);

        setIsLoading(true);
        try {
            await apiRegister(form);
            setSuccessMessage('Registration successful! You can now log in.');
            setForm({ username: '', email: '', password: '', firstName: '', lastName: '' });
            setTimeout(() => {
                navigate('/login');
            }, 1500);
        } catch (err) {
            console.error('Registration error:', err);
            setError(err.message || 'An unexpected error occurred during registration.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-sky-50 to-sky-100 p-4">
            <div className="bg-white p-8 rounded-lg shadow-lg w-full max-w-md">
                <div className="text-center mb-8">
                    <h2 className="text-3xl font-bold text-sky-700 mb-2">Create Account</h2>
                    <p className="text-sky-600">Join our school management system</p>
                </div>

                <form onSubmit={handleSubmit} className="space-y-6">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div className="space-y-2">
                            <label htmlFor="firstName" className="flex items-center text-sky-700 font-medium">
                                <i className="fas fa-user text-sky-400 mr-2"></i>
                                First Name
                            </label>
                            <input
                                type="text"
                                id="firstName"
                                value={form.firstName}
                                onChange={handleChange}
                                required
                                className="w-full px-4 py-2 border border-sky-200 rounded-md focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent"
                                placeholder="Enter your first name"
                            />
                        </div>

                        <div className="space-y-2">
                            <label htmlFor="lastName" className="flex items-center text-sky-700 font-medium">
                                <i className="fas fa-user text-sky-400 mr-2"></i>
                                Last Name
                            </label>
                            <input
                                type="text"
                                id="lastName"
                                value={form.lastName}
                                onChange={handleChange}
                                required
                                className="w-full px-4 py-2 border border-sky-200 rounded-md focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent"
                                placeholder="Enter your last name"
                            />
                        </div>
                    </div>

                    <div className="space-y-2">
                        <label htmlFor="username" className="flex items-center text-sky-700 font-medium">
                            <i className="fas fa-at text-sky-400 mr-2"></i>
                            Username
                        </label>
                        <input
                            type="text"
                            id="username"
                            value={form.username}
                            onChange={handleChange}
                            required
                            className="w-full px-4 py-2 border border-sky-200 rounded-md focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent"
                            placeholder="Choose a username"
                        />
                    </div>

                    <div className="space-y-2">
                        <label htmlFor="email" className="flex items-center text-sky-700 font-medium">
                            <i className="fas fa-envelope text-sky-400 mr-2"></i>
                            Email
                        </label>
                        <input
                            type="email"
                            id="email"
                            value={form.email}
                            onChange={handleChange}
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
                            value={form.password}
                            onChange={handleChange}
                            required
                            className="w-full px-4 py-2 border border-sky-200 rounded-md focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent"
                            placeholder="Choose a password"
                        />
                    </div>

                    {error && (
                        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md animate-slide-in">
                            <i className="fas fa-exclamation-circle mr-2"></i>
                            {error}
                        </div>
                    )}

                    {successMessage && (
                        <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-md animate-slide-in">
                            <i className="fas fa-check-circle mr-2"></i>
                            {successMessage}
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={isLoading}
                        className="w-full bg-sky-600 text-white py-2 px-4 rounded-md hover:bg-sky-700 transition duration-200 flex items-center justify-center"
                    >
                        {isLoading ? (
                            <>
                                <i className="fas fa-spinner fa-spin mr-2"></i>
                                Creating Account...
                            </>
                        ) : (
                            <>
                                <i className="fas fa-user-plus mr-2"></i>
                                Create Account
                            </>
                        )}
                    </button>
                </form>

                <div className="mt-8 text-center">
                    <p className="text-sky-600">
                        Already have an account?{' '}
                        <Link to="/login" className="text-sky-700 hover:text-sky-800 font-medium">
                            Sign in here
                        </Link>
                    </p>
                </div>
            </div>
        </div>
    );
}

export default RegisterPage;

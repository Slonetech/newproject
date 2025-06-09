import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { register as apiRegister } from '../../services/authService'; // Rename to avoid conflict with local function

function RegisterPage() {
    const [form, setForm] = useState({
        username: '',
        email: '',
        password: '',
        firstName: '',
        lastName: '',
        // role is typically defaulted to 'Student' by the backend for public registration
        // if your backend requires it here, ensure it's handled. Otherwise, remove.
        // For now, assuming backend assigns 'Student' by default if not provided by public.
        // If your backend Register endpoint expects 'role' for public, pass it: role: 'Student'
    });
    const [error, setError] = useState(null);
    const [successMessage, setSuccessMessage] = useState(null);
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { id, value } = e.target; // Use id for consistency
        setForm((prev) => ({ ...prev, [id]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);
        setSuccessMessage(null);

        setIsLoading(true);
        try {
            // Ensure the parameters match what authService.register expects:
            // It expects an object: { username, email, password, firstName, lastName }
            await apiRegister(form); // Pass the form object directly

            setSuccessMessage('Registration successful! You can now log in.');
            setForm({ username: '', email: '', password: '', firstName: '', lastName: '' }); // Clear form
            setTimeout(() => {
                navigate('/login'); // Redirect to login after successful registration and a short delay
            }, 1500); // Give user time to read success message
        } catch (err) {
            console.error('Registration error:', err);
            setError(err.message || 'An unexpected error occurred during registration.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-100 font-sans">
            <div className="bg-white p-8 rounded-lg shadow-md w-full max-w-md">
                <h2 className="text-3xl font-bold mb-6 text-center text-gray-800">Register for School Management</h2>
                <form onSubmit={handleSubmit} className="space-y-4">
                    <div>
                        <label htmlFor="firstName" className="block text-gray-700 text-sm font-medium mb-2">First Name:</label>
                        <input
                            type="text"
                            id="firstName"
                            name="firstName"
                            value={form.firstName}
                            onChange={handleChange}
                            required
                            className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                    <div>
                        <label htmlFor="lastName" className="block text-gray-700 text-sm font-medium mb-2">Last Name:</label>
                        <input
                            type="text"
                            id="lastName"
                            name="lastName"
                            value={form.lastName}
                            onChange={handleChange}
                            required
                            className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                    <div>
                        <label htmlFor="username" className="block text-gray-700 text-sm font-medium mb-2">Username:</label>
                        <input
                            type="text"
                            id="username"
                            name="username"
                            value={form.username}
                            onChange={handleChange}
                            required
                            className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>

                    <div>
                        <label htmlFor="email" className="block text-gray-700 text-sm font-medium mb-2">Email:</label>
                        <input
                            type="email"
                            id="email"
                            name="email"
                            value={form.email}
                            onChange={handleChange}
                            required
                            className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>

                    <div>
                        <label htmlFor="password" className="block text-gray-700 text-sm font-medium mb-2">Password:</label>
                        <input
                            type="password"
                            id="password"
                            name="password"
                            value={form.password}
                            onChange={handleChange}
                            required
                            className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>

                    {/* Role selection removed for public registration; backend defaults to Student */}
                    {error && <p className="text-red-600 text-sm text-center">{error}</p>}
                    {successMessage && <p className="text-green-600 text-sm text-center">{successMessage}</p>}

                    <button type="submit" disabled={isLoading} className="w-full bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition duration-200">
                        {isLoading ? 'Registering...' : 'Register'}
                    </button>
                </form>
                <p className="mt-6 text-center text-gray-600">
                    Already have an account? <Link to="/login" className="text-blue-600 hover:underline">Login here</Link>
                </p>
            </div>
        </div>
    );
}

export default RegisterPage;

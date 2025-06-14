import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

export default function Navbar() {
    const { auth, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
    };

    const userRoles = auth.user?.roles || [];

    return (
        <nav className="bg-sky-500 p-4 text-white shadow-md">
            <div className="container mx-auto flex justify-between items-center">
                <div className="navbar-brand">
                    <Link to="/" className="text-xl font-bold text-white hover:text-sky-100 transition duration-200">
                        School Management
                    </Link>
                </div>
                <div className="flex space-x-4 items-center">
                    <Link to="/dashboard" className="hover:text-sky-100 transition duration-200">
                        Dashboard
                    </Link>

                    {auth.user ? (
                        <>
                            {userRoles.includes('Admin') && (
                                <>
                                    <Link to="/admin" className="hover:text-sky-100 transition duration-200">Admin Panel</Link>
                                    <Link to="/admin/users" className="hover:text-sky-100 transition duration-200">Users</Link>
                                </>
                            )}

                            <Link to="/students" className="hover:text-sky-100 transition duration-200">Students</Link>
                            <Link to="/courses" className="hover:text-sky-100 transition duration-200">Courses</Link>
                            <Link to="/grades" className="hover:text-sky-100 transition duration-200">Grades</Link>
                            <Link to="/attendance" className="hover:text-sky-100 transition duration-200">Attendance</Link>

                            <span className="text-sky-100 ml-4 hidden md:block">
                                Hello, {auth.user.username} ({userRoles.join(', ')})
                            </span>
                            <button
                                onClick={handleLogout}
                                className="bg-sky-600 text-white py-1 px-3 rounded-md hover:bg-sky-700 transition duration-200"
                            >
                                Logout
                            </button>
                        </>
                    ) : (
                        <>
                            <Link to="/login" className="bg-sky-600 text-white py-1 px-3 rounded-md hover:bg-sky-700 transition duration-200">Login</Link>
                            <Link to="/register" className="bg-sky-600 text-white py-1 px-3 rounded-md hover:bg-sky-700 transition duration-200">Register</Link>
                        </>
                    )}
                </div>
            </div>
        </nav>
    );
}

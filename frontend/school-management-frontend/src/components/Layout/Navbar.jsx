import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

export default function Navbar() {
    const { auth, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        // The logout function in AuthContext already navigates to /login
    };

    const userRoles = auth.user?.roles || []; // Ensure userRoles is an array

    return (
        <nav className="bg-sky-500 p-4 text-white shadow-md">
            <div className="container mx-auto flex justify-between items-center">
                <div className="navbar-brand">
                    {/* The main brand link can remain '/' to go to the initial landing,
                        which ProtectedRoute will then handle based on role. */}
                    <Link to="/" className="text-xl font-bold text-white hover:text-sky-100 transition duration-200">
                        School Management
                    </Link>
                </div>
                <div className="flex space-x-4 items-center">
                    {/* Removed the 'Dashboard' link entirely as requested */}
                    {/* {auth.user && !userRoles.includes('Admin') && (
                        <Link to="/dashboard" className="hover:text-sky-100 transition duration-200">
                            Dashboard
                        </Link>
                    )} */}

                    {auth.user ? (
                        <>
                            {/* Admin Links - only visible to Admins */}
                            {userRoles.includes('Admin') && (
                                <>
                                    <Link to="/admin" className="hover:text-sky-100 transition duration-200">Admin Panel</Link>
                                    <Link to="/admin/users" className="hover:text-sky-100 transition duration-200">Users</Link>
                                    <Link to="/admin/teachers" className="hover:text-sky-100 transition duration-200">Teachers</Link>
                                    <Link to="/admin/parents" className="hover:text-sky-100 transition duration-200">Parents</Link>
                                </>
                            )}

                            {/* General Links (or specific per role if needed) */}
                            {userRoles.includes('Student') && (
                                <>
                                    <Link to="/student" className="hover:text-sky-100 transition duration-200">My Dashboard</Link>
                                    {/* <Link to="/students" className="hover:text-sky-100 transition duration-200">Students List</Link> // If students can see a list of other students */}
                                    <Link to="/courses" className="hover:text-sky-100 transition duration-200">Courses</Link>
                                    <Link to="/grades" className="hover:text-sky-100 transition duration-200">Grades</Link>
                                    <Link to="/attendance" className="hover:text-sky-100 transition duration-200">Attendance</Link>
                                </>
                            )}

                            {userRoles.includes('Teacher') && (
                                <>
                                    <Link to="/teacher" className="hover:text-sky-100 transition duration-200">My Dashboard</Link>
                                    <Link to="/courses" className="hover:text-sky-100 transition duration-200">Courses</Link>
                                    <Link to="/grades" className="hover:text-sky-100 transition duration-200">Manage Grades</Link>
                                    <Link to="/attendance" className="hover:text-sky-100 transition duration-200">Manage Attendance</Link>
                                </>
                            )}

                            {userRoles.includes('Parent') && (
                                <>
                                    <Link to="/parent" className="hover:text-sky-100 transition duration-200">My Dashboard</Link>
                                    <Link to="/grades" className="hover:text-sky-100 transition duration-200">Children's Grades</Link>
                                    <Link to="/attendance" className="hover:text-sky-100 transition duration-200">Children's Attendance</Link>
                                </>
                            )}

                            {/* User Info and Logout */}
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
                            {/* Public Links when not authenticated */}
                            <Link to="/login" className="bg-sky-600 text-white py-1 px-3 rounded-md hover:bg-sky-700 transition duration-200">Login</Link>
                            <Link to="/register" className="bg-sky-600 text-white py-1 px-3 rounded-md hover:bg-sky-700 transition duration-200">Register</Link>
                        </>
                    )}
                </div>
            </div>
        </nav>
    );
}
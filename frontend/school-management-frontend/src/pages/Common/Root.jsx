import React from 'react';
import { Link } from 'react-router-dom';

function Root() {
    return (
        <div className="min-h-screen bg-gradient-to-b from-gray-50 to-gray-100">
            {/* Navigation */}
            <nav className="bg-white shadow-sm">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="flex justify-between h-16 items-center">
                        <div className="flex-shrink-0">
                            <h1 className="text-2xl font-bold text-primary">School Management</h1>
                        </div>
                        <div className="hidden md:flex space-x-4">
                            <Link to="/login" className="nav-link">Login</Link>
                            <Link to="/register" className="btn btn-primary">Register</Link>
                        </div>
                    </div>
                </div>
            </nav>

            {/* Hero Section */}
            <div className="relative overflow-hidden">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20">
                    <div className="text-center">
                        <h1 className="text-4xl sm:text-5xl md:text-6xl font-extrabold text-gray-900 mb-6">
                            Welcome to <span className="text-primary">School Management</span>
                        </h1>
                        <p className="text-xl text-gray-600 max-w-3xl mx-auto mb-10">
                            A comprehensive platform designed to streamline administration, enhance learning, 
                            and connect the entire school community.
                        </p>
                        <div className="flex flex-col sm:flex-row justify-center gap-4">
                            <Link to="/login" className="btn btn-primary">
                                Get Started
                            </Link>
                            <Link to="/register" className="btn btn-secondary">
                                Learn More
                            </Link>
                        </div>
                    </div>
                </div>
            </div>

            {/* Features Section */}
            <div className="bg-white py-20">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="text-center mb-16">
                        <h2 className="text-3xl font-bold text-gray-900 mb-4">Key Features</h2>
                        <p className="text-xl text-gray-600">Everything you need to manage your school effectively</p>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                        {/* Feature 1 */}
                        <div className="card">
                            <div className="text-primary text-4xl mb-4">
                                <i className="fas fa-users"></i>
                            </div>
                            <h3 className="card-title">User Management</h3>
                            <p className="text-gray-600">
                                Efficiently manage students, teachers, and staff with our comprehensive user management system.
                            </p>
                        </div>
                        {/* Feature 2 */}
                        <div className="card">
                            <div className="text-primary text-4xl mb-4">
                                <i className="fas fa-book"></i>
                            </div>
                            <h3 className="card-title">Course Management</h3>
                            <p className="text-gray-600">
                                Organize and track courses, assignments, and academic progress seamlessly.
                            </p>
                        </div>
                        {/* Feature 3 */}
                        <div className="card">
                            <div className="text-primary text-4xl mb-4">
                                <i className="fas fa-chart-line"></i>
                            </div>
                            <h3 className="card-title">Performance Analytics</h3>
                            <p className="text-gray-600">
                                Monitor student performance and generate detailed reports with our analytics tools.
                            </p>
                        </div>
                    </div>
                </div>
            </div>

            {/* Footer */}
            <footer className="bg-gray-900 text-white py-12">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
                        <div>
                            <h3 className="text-lg font-semibold mb-4">School Management</h3>
                            <p className="text-gray-400">
                                Empowering education through technology
                            </p>
                        </div>
                        <div>
                            <h4 className="text-lg font-semibold mb-4">Quick Links</h4>
                            <ul className="space-y-2">
                                <li><Link to="/login" className="text-gray-400 hover:text-white">Login</Link></li>
                                <li><Link to="/register" className="text-gray-400 hover:text-white">Register</Link></li>
                            </ul>
                        </div>
                        <div>
                            <h4 className="text-lg font-semibold mb-4">Contact</h4>
                            <ul className="space-y-2 text-gray-400">
                                <li>Email: support@school.com</li>
                                <li>Phone: (+254) 123-4567</li>
                            </ul>
                        </div>
                        <div>
                            <h4 className="text-lg font-semibold mb-4">Follow Us</h4>
                            <div className="flex space-x-4">
                                <a href="#" className="text-gray-400 hover:text-white">
                                    <i className="fab fa-facebook"></i>
                                </a>
                                <a href="#" className="text-gray-400 hover:text-white">
                                    <i className="fab fa-twitter"></i>
                                </a>
                                <a href="#" className="text-gray-400 hover:text-white">
                                    <i className="fab fa-linkedin"></i>
                                </a>
                            </div>
                        </div>
                    </div>
                    <div className="border-t border-gray-800 mt-8 pt-8 text-center text-gray-400">
                        <p>&copy; {new Date().getFullYear()} School Management System. All rights reserved.</p>
                    </div>
                </div>
            </footer>
        </div>
    );
}

export default Root;

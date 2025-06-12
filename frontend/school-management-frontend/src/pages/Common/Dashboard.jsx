import React from 'react';
import { useAuth } from '../../context/AuthContext';
import { Link } from 'react-router-dom';

function Dashboard() {
    const { auth } = useAuth();

    const getRoleColor = (role) => {
        switch (role) {
            case 'Admin': return 'text-blue-700';
            case 'Teacher': return 'text-green-700';
            case 'Student': return 'text-purple-700';
            case 'Parent': return 'text-orange-700';
            default: return 'text-gray-700';
        }
    };

    const getRoleLinkColor = (role) => {
        switch (role) {
            case 'Admin': return 'text-blue-600 hover:text-blue-800'; // Changed to use the btn-primary color family
            case 'Teacher': return 'text-green-600 hover:text-green-800'; // Changed to use the btn-success color family
            case 'Student': return 'text-purple-600 hover:text-purple-800';
            case 'Parent': return 'text-orange-600 hover:text-orange-800';
            default: return 'text-gray-600 hover:text-gray-800';
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 flex flex-col items-center py-10 px-4 sm:px-6 lg:px-8 font-sans">
            <div className="form-container w-full max-w-4xl p-8 sm:p-10"> {/* Using form-container */}
                <h1 className="form-title text-4xl">
                    Welcome to Your Dashboard, <span className="text-blue-600">{auth.user?.username}!</span>
                </h1>
                <p className="text-lg text-gray-700 mb-8 text-center">
                    Your roles: <span className="font-semibold text-gray-800">{auth.roles.join(', ')}</span>
                </p>

                <div className="bg-gray-100 border border-gray-200 rounded-lg p-6 mb-8 shadow-sm">
                    <h3 className="text-xl font-bold mb-4 text-gray-800">Quick Access & Role Information</h3>
                    {auth.roles.map(role => (
                        <p key={role} className={`mb-2 text-gray-700 leading-relaxed`}>
                            As a <span className={`font-semibold ${getRoleColor(role)}`}>{role}</span>, you can:
                            {role === 'Admin' && (
                                <> manage all users, students, teachers, courses, grades, and attendance. <Link to="/admin" className={`hover:underline transition duration-200 ${getRoleLinkColor(role)}`}>Go to Admin Panel</Link></>
                            )}
                            {role === 'Teacher' && (
                                <> view your enrolled students, manage grades, and mark attendance. <Link to="/teacher" className={`hover:underline transition duration-200 ${getRoleLinkColor(role)}`}>Go to Teacher Dashboard</Link></>
                            )}
                            {role === 'Student' && (
                                <> view your profile, courses, grades, and attendance records. <Link to="/student" className={`hover:underline transition duration-200 ${getRoleLinkColor(role)}`}>Go to Student Dashboard</Link></>
                            )}
                            {role === 'Parent' && (
                                <> monitor your child's academic progress, including their courses, grades, and attendance. <Link to="/parent" className={`hover:underline transition duration-200 ${getRoleLinkColor(role)}`}>Go to Parent Dashboard</Link></>
                            )}
                        </p>
                    ))}
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div className="bg-blue-100 p-6 rounded-lg shadow-md border border-blue-300">
                        <h4 className="font-semibold text-blue-800 text-lg mb-2"><i className="fas fa-bullhorn mr-2"></i>Announcements</h4>
                        <p className="text-gray-700 text-sm">No new announcements at this time.</p>
                    </div>
                    <div className="bg-green-100 p-6 rounded-lg shadow-md border border-green-300">
                        <h4 className="font-semibold text-green-800 text-lg mb-2"><i className="fas fa-calendar-alt mr-2"></i>Upcoming Events</h4>
                        <p className="text-gray-700 text-sm">Check your calendar for upcoming school events.</p>
                    </div>
                    <div className="bg-purple-100 p-6 rounded-lg shadow-md border border-purple-300">
                        <h4 className="font-semibold text-purple-800 text-lg mb-2"><i className="fas fa-info-circle mr-2"></i>Help & Support</h4>
                        <p className="text-gray-700 text-sm">Need assistance? Visit our help section or contact support.</p>
                    </div>
                    <div className="bg-yellow-100 p-6 rounded-lg shadow-md border border-yellow-300">
                        <h4 className="font-semibold text-yellow-800 text-lg mb-2"><i className="fas fa-bell mr-2"></i>Notifications</h4>
                        <p className="text-gray-700 text-sm">You have no unread notifications.</p>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Dashboard;

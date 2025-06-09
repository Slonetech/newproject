import React from 'react';
import { useAuth } from '../context/AuthContext';

function CoursesPage() {
    const { auth } = useAuth(); // Get auth context to check roles

    const renderContent = () => {
        if (auth.roles.includes('Admin')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">All Courses (Admin View)</h2>
                    <p className="text-gray-700">Display table of all courses with full CRUD operations here.</p>
                    {/* TODO: Implement Admin-specific course management UI */}
                </>
            );
        } else if (auth.roles.includes('Teacher')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">My Assigned Courses (Teacher View)</h2>
                    <p className="text-gray-700">Display courses assigned to this teacher, with options to manage student enrollment.</p>
                    {/* TODO: Implement Teacher-specific course management UI */}
                </>
            );
        } else if (auth.roles.includes('Student')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">My Enrolled Courses (Student View)</h2>
                    <p className="text-gray-700">Display courses the logged-in student is enrolled in.</p>
                    {/* TODO: Implement Student's enrolled courses UI */}
                </>
            );
        } else if (auth.roles.includes('Parent')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">Child's Courses (Parent View)</h2>
                    <p className="text-gray-700">Display courses for your children.</p>
                    {/* TODO: Implement Parent's child courses view UI */}
                </>
            );
        } else {
            return <p className="text-red-600">Access denied or role not recognized.</p>;
        }
    };

    return (
        <div className="p-6 bg-white rounded-lg shadow-md mt-4">
            <h1 className="text-2xl font-bold mb-4 text-gray-800">Courses Information</h1>
            {renderContent()}
        </div>
    );
}

export default CoursesPage;

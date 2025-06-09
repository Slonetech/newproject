import React from 'react';
import { useAuth } from '../context/AuthContext'; // Correct path

function StudentsPage() {
    const { auth } = useAuth(); // Get auth context to check roles

    const renderContent = () => {
        if (auth.roles.includes('Admin')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">All Students (Admin View)</h2>
                    <p className="text-gray-700">Display table of all students with full CRUD operations here.</p>
                    {/* TODO: Implement Admin-specific student management UI */}
                </>
            );
        } else if (auth.roles.includes('Teacher')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">My Assigned Students (Teacher View)</h2>
                    <p className="text-gray-700">Display list of students assigned to this teacher, with options to view grades/attendance.</p>
                    {/* TODO: Implement Teacher-specific student view UI */}
                </>
            );
        } else if (auth.roles.includes('Student')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">My Profile (Student View)</h2>
                    <p className="text-gray-700">Display the logged-in student's personal profile and details.</p>
                    {/* TODO: Implement Student's own profile UI */}
                </>
            );
        } else if (auth.roles.includes('Parent')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">My Children's Data (Parent View)</h2>
                    <p className="text-gray-700">Display list of children linked to this parent, with links to their grades/attendance/courses.</p>
                    {/* TODO: Implement Parent's children data UI */}
                </>
            );
        } else {
            return <p className="text-red-600">Access denied or role not recognized.</p>;
        }
    };

    return (
        <div className="p-6 bg-white rounded-lg shadow-md mt-4">
            <h1 className="text-2xl font-bold mb-4 text-gray-800">Students Information</h1>
            {renderContent()}
        </div>
    );
}

export default StudentsPage;

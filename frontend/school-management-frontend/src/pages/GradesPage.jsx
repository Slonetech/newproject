import React from 'react';
import { useAuth } from '../context/AuthContext';

function GradesPage() {
    const { auth } = useAuth(); // Get auth context to check roles

    const renderContent = () => {
        if (auth.roles.includes('Admin')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">All Grades (Admin View)</h2>
                    <p className="text-gray-700">Display table of all grades with full management capabilities.</p>
                    {/* TODO: Implement Admin-specific grade management UI */}
                </>
            );
        } else if (auth.roles.includes('Teacher')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">Manage Grades (Teacher View)</h2>
                    <p className="text-gray-700">Record and update grades for your students and courses.</p>
                    {/* TODO: Implement Teacher-specific grade management UI */}
                </>
            );
        } else if (auth.roles.includes('Student')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">My Grades (Student View)</h2>
                    <p className="text-gray-700">Display the logged-in student's grades for their courses.</p>
                    {/* TODO: Implement Student's own grades view UI */}
                </>
            );
        } else if (auth.roles.includes('Parent')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">Child's Grades (Parent View)</h2>
                    <p className="text-gray-700">Display grades for your children.</p>
                    {/* TODO: Implement Parent's child grades view UI */}
                </>
            );
        } else {
            return <p className="text-red-600">Access denied or role not recognized.</p>;
        }
    };

    return (
        <div className="p-6 bg-white rounded-lg shadow-md mt-4">
            <h1 className="text-2xl font-bold mb-4 text-gray-800">Grades Information</h1>
            {renderContent()}
        </div>
    );
}

export default GradesPage;

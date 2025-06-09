import React from 'react';
import { useAuth } from '../context/AuthContext';

function AttendancePage() {
    const { auth } = useAuth(); // Get auth context to check roles

    const renderContent = () => {
        if (auth.roles.includes('Admin')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">Attendance Records (Admin View)</h2>
                    <p className="text-gray-700">Display all attendance records with full management capabilities.</p>
                    {/* TODO: Implement Admin-specific attendance management UI */}
                </>
            );
        } else if (auth.roles.includes('Teacher')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">Mark Attendance (Teacher View)</h2>
                    <p className="text-gray-700">Mark attendance for your assigned courses and students.</p>
                    {/* TODO: Implement Teacher-specific attendance marking UI */}
                </>
            );
        } else if (auth.roles.includes('Student')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">My Attendance Records (Student View)</h2>
                    <p className="text-gray-700">Display the logged-in student's attendance history.</p>
                    {/* TODO: Implement Student's own attendance view UI */}
                </>
            );
        } else if (auth.roles.includes('Parent')) {
            return (
                <>
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">Child's Attendance (Parent View)</h2>
                    <p className="text-gray-700">Display attendance records for your children.</p>
                    {/* TODO: Implement Parent's child attendance view UI */}
                </>
            );
        } else {
            return <p className="text-red-600">Access denied or role not recognized.</p>;
        }
    };

    return (
        <div className="p-6 bg-white rounded-lg shadow-md mt-4">
            <h1 className="text-2xl font-bold mb-4 text-gray-800">Attendance Information</h1>
            {renderContent()}
        </div>
    );
}

export default AttendancePage;

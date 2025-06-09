import React from 'react';

function ParentManagementPage() {
    return (
        <div className="p-6 bg-white rounded-lg shadow-md mt-4">
            <h1 className="text-2xl font-bold mb-4 text-gray-800">Parent Management (Admin Only)</h1>
            <p className="text-gray-700">
                This page will allow admins to create, update, and delete parent records and assign children to parents.
                It links to the `ApplicationUser` accounts managed on the <span className="font-semibold">User Management Page</span>.
            </p>
            {/* TODO: Implement UI for creating, listing, editing, deleting parent profiles and linking children */}
        </div>
    );
}

export default ParentManagementPage;

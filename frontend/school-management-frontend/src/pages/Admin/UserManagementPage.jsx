import React, { useEffect, useState, useCallback } from 'react';
import adminService from '../../services/adminService';
import { useAuth } from '../../context/AuthContext';
import { useNavigate } from 'react-router-dom';

const UserManagementPage = () => {
    const { auth } = useAuth();
    const navigate = useNavigate();
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [message, setMessage] = useState('');
    const [showCreateForm, setShowCreateForm] = useState(false);
    const [selectedUser, setSelectedUser] = useState(null); // For editing

    // Form states for creating/editing user
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '', // Only for creation
        firstName: '',
        lastName: '',
        initialRole: 'Student', // Default for creation
    });
    const [availableRoles, setAvailableRoles] = useState([]); // All roles for selection

    // Initialize form data when selectedUser changes
    useEffect(() => {
        if (selectedUser) {
            setFormData({
                username: selectedUser.username ?? '',
                email: selectedUser.email ?? '',
                firstName: selectedUser.firstName ?? '',
                lastName: selectedUser.lastName ?? '',
                password: '', // Password is not edited via this form for security
                initialRole: selectedUser.roles?.[0] ?? 'Student',
            });
        } else {
            setFormData({
                username: '',
                email: '',
                password: '',
                firstName: '',
                lastName: '',
                initialRole: 'Student',
            });
        }
    }, [selectedUser]);

    const fetchUsers = useCallback(async () => {
        setLoading(true);
        setError('');
        setMessage('');
        try {
            const data = await adminService.getAllUsers();
            setUsers(data);
        } catch (err) {
            setError(err.response?.data?.Message || err.message || 'Failed to fetch users.');
            // If it's a 403 Forbidden, redirect to unauthorized
            if (err.response?.status === 403) {
                navigate('/unauthorized');
            }
        } finally {
            setLoading(false);
        }
    }, [navigate]);

    const fetchRoles = useCallback(async () => {
        try {
            const roles = await adminService.getAvailableRoles();
            setAvailableRoles(roles);
        } catch (err) {
            console.error('Failed to fetch available roles:', err);
            setError('Failed to fetch available roles.');
        }
    }, []);

    useEffect(() => {
        fetchUsers();
        fetchRoles();
    }, [fetchUsers, fetchRoles]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value ?? '' })); // Ensure empty string if value is undefined
    };

    const handleCreateUser = async (e) => {
        e.preventDefault();
        setError('');
        setMessage('');
        try {
            console.log('Creating user with data:', formData); // Debug log
            await adminService.createUser(formData); // Pass all form data for UserCreationDto
            setMessage('User created successfully!');
            setShowCreateForm(false);
            setFormData({ // Reset form
                username: '', email: '', password: '', firstName: '', lastName: '', initialRole: 'Student',
            });
            fetchUsers(); // Refresh list
        } catch (err) {
            console.error('Error creating user:', err.response?.data); // Debug log
            setError(err.response?.data?.Message || err.message || 'Failed to create user.');
        }
    };

    const handleDeleteUser = async (userId, username) => {
        // Use a custom modal instead of window.confirm in real apps
        if (window.confirm(`Are you sure you want to delete user: ${username}? This action cannot be undone.`)) {
            // Prevent deleting self
            if (auth.user.id === userId) {
                setError("You cannot delete your own admin account through this interface.");
                return;
            }
            setLoading(true);
            setError('');
            setMessage('');
            try {
                await adminService.deleteUser(userId);
                setMessage(`User ${username} deleted successfully!`);
                fetchUsers(); // Refresh list
            } catch (err) {
                setError(err.response?.data?.Message || err.message || 'Failed to delete user.');
            } finally {
                setLoading(false);
            }
        }
    };

    const handleEditUser = (user) => {
        setSelectedUser(user);
        // Populate form with current user data for editing (excluding password)
        setFormData({
            username: user.username,
            email: user.email,
            firstName: user.firstName,
            lastName: user.lastName,
            password: '', // Password is not edited via this form for security
            initialRole: user.roles[0] || 'Student', // Pre-select first role if any (not used in update)
        });
        setShowCreateForm(true); // Reuse the same form for editing
    };

    const handleUpdateUser = async (e) => {
        e.preventDefault();
        setError('');
        setMessage('');
        if (!selectedUser) return; // Should not happen

        try {
            // Only send fields that can be updated by UserUpdateDto
            const updateData = {
                username: formData.username,
                email: formData.email,
                firstName: formData.firstName,
                lastName: formData.lastName,
            };
            await adminService.updateUser(selectedUser.id, updateData);
            setMessage('User updated successfully!');
            setSelectedUser(null);
            setShowCreateForm(false);
            setFormData({ // Reset form
                username: '', email: '', password: '', firstName: '', lastName: '', initialRole: 'Student'
            });
            fetchUsers(); // Refresh list
        } catch (err) {
            setError(err.response?.data?.Message || err.message || 'Failed to update user.');
        }
    };

    const handleAssignRole = async (userId, currentRoles) => {
        const roleToAssign = prompt("Enter role to assign (Admin, Teacher, Student, Parent):");
        if (!roleToAssign) return;

        if (!availableRoles.includes(roleToAssign)) {
            alert(`Invalid role: ${roleToAssign}. Please choose from: ${availableRoles.join(', ')}`);
            return;
        }
        if (currentRoles.includes(roleToAssign)) {
            alert(`User already has the role: ${roleToAssign}`);
            return;
        }

        setError('');
        setMessage('');
        try {
            await adminService.assignRole(userId, roleToAssign);
            setMessage(`Role '${roleToAssign}' assigned successfully!`);
            fetchUsers(); // Refresh list
        } catch (err) {
            setError(err.response?.data?.Message || err.message || 'Failed to assign role.');
        }
    };

    const handleRemoveRole = async (userId, currentRoles) => {
        const roleToRemove = prompt("Enter role to remove (Admin, Teacher, Student, Parent):");
        if (!roleToRemove) return;

        if (!currentRoles.includes(roleToRemove)) {
            alert(`User does not have the role: ${roleToRemove}`);
            return;
        }
        // Prevent removing the last Admin role from oneself
        if (userId === auth.user.id && roleToRemove === "Admin" && currentRoles.filter(r => r === "Admin").length === 1) {
            alert("You cannot remove the last 'Admin' role from your own account.");
            return;
        }

        setError('');
        setMessage('');
        try {
            await adminService.removeRole(userId, roleToRemove);
            setMessage(`Role '${roleToRemove}' removed successfully!`);
            fetchUsers(); // Refresh list
        } catch (err) {
            setError(err.response?.data?.Message || err.message || 'Failed to remove role.');
        }
    };

    const handleLinkProfile = async (userId, userRoles) => {
        // More robust logic would involve selecting profile type and/or existing profile ID
        // For now, we'll try to link based on the most common profile types.
        setError('');
        setMessage('');

        // Determine which profile type to link based on existing roles
        let linkedProfileType = null;
        if (userRoles.includes("Student") && !users.find(u => u.id === userId)?.studentId) {
            linkedProfileType = "Student";
        } else if (userRoles.includes("Teacher") && !users.find(u => u.id === userId)?.teacherId) {
            linkedProfileType = "Teacher";
        } else if (userRoles.includes("Parent") && !users.find(u => u.id === userId)?.parentId) {
            linkedProfileType = "Parent";
        }

        if (!linkedProfileType) {
            alert("User already has a linked profile for their current role, or no specific linking path is available.");
            return;
        }

        try {
            if (linkedProfileType === "Student") {
                await adminService.linkStudentProfile(userId);
                setMessage(`Student profile linked for user ${userId}.`);
            }
            // Add else if for Teacher/Parent linking when backend endpoints are ready
            fetchUsers(); // Refresh list to show linked profile ID
        } catch (err) {
            setError(err.response?.data?.Message || err.message || `Failed to link ${linkedProfileType} profile.`);
        }
    }


    if (loading) {
        return <div className="text-center p-4 text-gray-700">Loading users...</div>;
    }

    return (
        <div className="p-6 bg-white rounded-lg shadow-md mt-4">
            <h2 className="text-2xl font-semibold mb-4 text-gray-800">User Management</h2>

            {error && <p className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mb-4" role="alert">{error}</p>}
            {message && <p className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded relative mb-4" role="alert">{message}</p>}

            <button
                onClick={() => {
                    setShowCreateForm(!showCreateForm);
                    setSelectedUser(null); // Clear selection when toggling create form
                    setFormData({ // Reset form data
                        username: '', email: '', password: '', firstName: '', lastName: '', initialRole: 'Student'
                    });
                }}
                className="bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 transition duration-200 mb-6"
            >
                {showCreateForm ? 'Hide Form' : 'Create New User'}
            </button>

            {showCreateForm && (
                <div className="border border-gray-300 p-4 rounded-lg mb-6">
                    <h3 className="text-xl font-bold mb-4">{selectedUser ? 'Edit User' : 'Create New User'}</h3>
                    <form onSubmit={selectedUser ? handleUpdateUser : handleCreateUser} className="space-y-4">
                        <div>
                            <label className="block text-gray-700 text-sm font-medium mb-2">First Name:</label>
                            <input
                                type="text"
                                name="firstName"
                                value={formData.firstName}
                                onChange={handleChange}
                                maxLength={50}
                                className="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-gray-700 text-sm font-medium mb-2">Last Name:</label>
                            <input
                                type="text"
                                name="lastName"
                                value={formData.lastName}
                                onChange={handleChange}
                                maxLength={50}
                                className="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-gray-700 text-sm font-medium mb-2">Username:</label>
                            <input
                                type="text"
                                name="username"
                                value={formData.username}
                                onChange={handleChange}
                                maxLength={50}
                                className="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-gray-700 text-sm font-medium mb-2">Email:</label>
                            <input
                                type="email"
                                name="email"
                                value={formData.email}
                                onChange={handleChange}
                                maxLength={100}
                                className="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                required
                            />
                        </div>
                        {!selectedUser && ( // Only show password field for new user creation
                            <div>
                                <label className="block text-gray-700 text-sm font-medium mb-2">Password:</label>
                                <input
                                    type="password"
                                    name="password"
                                    value={formData.password}
                                    onChange={handleChange}
                                    minLength={6}
                                    maxLength={100}
                                    className="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    required={!selectedUser} // Required only for creation
                                />
                                <p className="text-sm text-gray-500 mt-1">Password must be at least 6 characters long</p>
                            </div>
                        )}
                        {!selectedUser && ( // Only show initial role for new user creation
                            <div>
                                <label className="block text-gray-700 text-sm font-medium mb-2">Initial Role:</label>
                                <select
                                    name="initialRole"
                                    value={formData.initialRole}
                                    onChange={handleChange}
                                    className="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                >
                                    {availableRoles.map(role => (
                                        <option key={role} value={role}>{role}</option>
                                    ))}
                                </select>
                            </div>
                        )}
                        <button
                            type="submit"
                            className="bg-green-600 text-white py-2 px-4 rounded-md hover:bg-green-700 transition duration-200"
                        >
                            {selectedUser ? 'Update User' : 'Create User'}
                        </button>
                        {selectedUser && (
                            <button
                                type="button"
                                onClick={() => { setSelectedUser(null); setShowCreateForm(false); setFormData({username: '', email: '', password: '', firstName: '', lastName: '', initialRole: 'Student'}) }}
                                className="ml-2 bg-gray-500 text-white py-2 px-4 rounded-md hover:bg-gray-600 transition duration-200"
                            >
                                Cancel Edit
                            </button>
                        )}
                    </form>
                </div>
            )}

            <h3 className="text-xl font-bold mb-3 text-gray-800">All Users</h3>
            {users.length === 0 ? (
                <p>No users found.</p>
            ) : (
                <div className="overflow-x-auto">
                    <table className="min-w-full bg-white border border-gray-200 rounded-lg shadow-sm">
                        <thead className="bg-gray-100">
                            <tr>
                                <th className="py-3 px-4 border-b text-left text-sm font-semibold text-gray-600">ID</th>
                                <th className="py-3 px-4 border-b text-left text-sm font-semibold text-gray-600">Username</th>
                                <th className="py-3 px-4 border-b text-left text-sm font-semibold text-gray-600">Email</th>
                                <th className="py-3 px-4 border-b text-left text-sm font-semibold text-gray-600">Name</th>
                                <th className="py-3 px-4 border-b text-left text-sm font-semibold text-gray-600">Roles</th>
                                <th className="py-3 px-4 border-b text-left text-sm font-semibold text-gray-600">Profiles</th>
                                <th className="py-3 px-4 border-b text-left text-sm font-semibold text-gray-600">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map((user) => (
                                <tr key={user.id} className="border-b last:border-b-0 hover:bg-gray-50">
                                    <td className="py-3 px-4 text-sm text-gray-800 break-all">{user.id}</td>
                                    <td className="py-3 px-4 text-sm text-gray-800">{user.username}</td>
                                    <td className="py-3 px-4 text-sm text-gray-800">{user.email}</td>
                                    <td className="py-3 px-4 text-sm text-gray-800">{user.firstName} {user.lastName}</td>
                                    <td className="py-3 px-4 text-sm text-gray-800">{user.roles.join(', ')}</td>
                                    <td className="py-3 px-4 text-sm text-gray-800">
                                        {user.studentId && <span className="mr-2 p-1 bg-blue-100 rounded-md text-blue-800 text-xs">Student:{user.studentId}</span>}
                                        {user.teacherId && <span className="mr-2 p-1 bg-green-100 rounded-md text-green-800 text-xs">Teacher:{user.teacherId}</span>}
                                        {user.parentId && <span className="mr-2 p-1 bg-purple-100 rounded-md text-purple-800 text-xs">Parent:{user.parentId}</span>}
                                        {!user.studentId && !user.teacherId && !user.parentId && (
                                            <span className="text-gray-500 text-xs">None</span>
                                        )}
                                    </td>
                                    <td className="py-3 px-4 text-sm">
                                        <div className="flex flex-wrap gap-2">
                                            <button
                                                onClick={() => handleEditUser(user)}
                                                className="bg-yellow-500 text-white py-1 px-3 rounded-md hover:bg-yellow-600 transition duration-200"
                                            >
                                                Edit
                                            </button>
                                            <button
                                                onClick={() => handleDeleteUser(user.id, user.username)}
                                                className="bg-red-600 text-white py-1 px-3 rounded-md hover:bg-red-700 transition duration-200"
                                            >
                                                Delete
                                            </button>
                                            <button
                                                onClick={() => handleAssignRole(user.id, user.roles)}
                                                className="bg-indigo-600 text-white py-1 px-3 rounded-md hover:bg-indigo-700 transition duration-200"
                                            >
                                                Assign Role
                                            </button>
                                            {user.roles.length > 1 && ( // Only show if user has more than one role
                                                <button
                                                    onClick={() => handleRemoveRole(user.id, user.roles)}
                                                    className="bg-gray-600 text-white py-1 px-3 rounded-md hover:bg-gray-700 transition duration-200"
                                                >
                                                    Remove Role
                                                </button>
                                            )}
                                            {/* Link Profile button based on roles/linked profiles */}
                                            {user.roles.includes("Student") && !user.studentId && (
                                                <button
                                                    onClick={() => handleLinkProfile(user.id, "Student")}
                                                    className="bg-teal-500 text-white py-1 px-3 rounded-md hover:bg-teal-600 transition duration-200"
                                                >
                                                    Link Student Profile
                                                </button>
                                            )}
                                            {/* Add similar buttons for Teacher/Parent if needed */}
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default UserManagementPage;

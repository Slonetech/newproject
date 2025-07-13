import React, { useState, useEffect } from 'react';
import parentService from '../../services/parentService';
import { toast } from 'react-toastify';

const initialForm = {
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    address: '',
};

function ParentManagementPage() {
    const [parents, setParents] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedParent, setSelectedParent] = useState(null);
    const [formData, setFormData] = useState(initialForm);

    useEffect(() => {
        loadParents();
    }, []);

    const loadParents = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await parentService.getAllParents();
            setParents(data);
        } catch (err) {
            setError('Failed to load parents');
        } finally {
            setLoading(false);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value ?? '' }));
    };

    const handleAdd = () => {
        setSelectedParent(null);
        setFormData(initialForm);
        setIsModalOpen(true);
    };

    const handleEdit = (parent) => {
        setSelectedParent(parent);
        setFormData({
            firstName: parent.firstName || '',
            lastName: parent.lastName || '',
            email: parent.email || '',
            phoneNumber: parent.phoneNumber || '',
            address: parent.address || '',
        });
        setIsModalOpen(true);
    };

    const handleDelete = async (id) => {
        if (window.confirm('Are you sure you want to delete this parent?')) {
            try {
                await parentService.deleteParent(id);
                toast.success('Parent deleted successfully');
                loadParents();
            } catch (err) {
                toast.error('Failed to delete parent');
            }
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (selectedParent) {
                await parentService.updateParent(selectedParent.id, formData);
                toast.success('Parent updated successfully');
            } else {
                await parentService.addParent(formData);
                toast.success('Parent added successfully');
            }
            setIsModalOpen(false);
            loadParents();
        } catch (err) {
            toast.error('Failed to save parent');
        }
    };

    return (
        <div className="container mx-auto px-4 py-8">
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-2xl font-bold">Parent Management</h1>
                <button
                    onClick={handleAdd}
                    className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
                >
                    Add New Parent
                </button>
            </div>

            {loading ? (
                <div className="text-center py-8">Loading...</div>
            ) : error ? (
                <div className="text-center text-red-500 py-8">{error}</div>
            ) : parents.length === 0 ? (
                <div className="text-center py-8 text-gray-500">No parents found.</div>
            ) : (
                <div className="bg-white shadow-md rounded-lg overflow-x-auto">
                    <table className="min-w-full divide-y divide-gray-200">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Full Name</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Phone</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Address</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Linked Students</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                            </tr>
                        </thead>
                        <tbody className="bg-white divide-y divide-gray-200">
                            {parents.map((parent) => (
                                <tr key={parent.id}>
                                    <td className="px-6 py-4 whitespace-nowrap">{parent.firstName} {parent.lastName}</td>
                                    <td className="px-6 py-4 whitespace-nowrap">{parent.email}</td>
                                    <td className="px-6 py-4 whitespace-nowrap">{parent.phoneNumber || '-'}</td>
                                    <td className="px-6 py-4 whitespace-nowrap">{parent.address || '-'}</td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        {parent.students && parent.students.length > 0 ? (
                                            <ul className="list-disc pl-4">
                                                {parent.students.map((student) => (
                                                    <li key={student.id}>{student.firstName} {student.lastName}</li>
                                                ))}
                                            </ul>
                                        ) : (
                                            <span className="text-gray-400">None</span>
                                        )}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                        <button
                                            onClick={() => handleEdit(parent)}
                                            className="text-indigo-600 hover:text-indigo-900 mr-4"
                                        >
                                            Edit
                                        </button>
                                        <button
                                            onClick={() => handleDelete(parent.id)}
                                            className="text-red-600 hover:text-red-900"
                                        >
                                            Delete
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}

            {/* Modal for Add/Edit Parent */}
            {isModalOpen && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
                    <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
                        <div className="mt-3">
                            <h3 className="text-lg font-medium leading-6 text-gray-900 mb-4">
                                {selectedParent ? 'Edit Parent' : 'Add New Parent'}
                            </h3>
                            <form onSubmit={handleSubmit} className="space-y-4">
                                <input name="firstName" value={formData.firstName} onChange={handleInputChange} placeholder="First Name" required className="input w-full" />
                                <input name="lastName" value={formData.lastName} onChange={handleInputChange} placeholder="Last Name" required className="input w-full" />
                                <input name="email" value={formData.email} onChange={handleInputChange} placeholder="Email" type="email" required className="input w-full" />
                                <input name="phoneNumber" value={formData.phoneNumber} onChange={handleInputChange} placeholder="Phone Number" className="input w-full" />
                                <input name="address" value={formData.address} onChange={handleInputChange} placeholder="Address" className="input w-full" />
                                <div className="flex justify-end gap-2">
                                    <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-secondary">Cancel</button>
                                    <button type="submit" className="btn btn-primary">{selectedParent ? 'Update' : 'Create'}</button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default ParentManagementPage;

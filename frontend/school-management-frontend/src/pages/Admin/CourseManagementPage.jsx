import React, { useEffect, useState } from 'react';
import adminService from '../../services/adminService';
import teacherService from '../../services/teacherService';

const CourseManagementPage = () => {
  const [courses, setCourses] = useState([]);
  const [form, setForm] = useState({ name: '', description: '', credits: 0 });
  const [editingId, setEditingId] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [teachers, setTeachers] = useState([]);
  const [students, setStudents] = useState([]);
  const [assigningCourseId, setAssigningCourseId] = useState(null);
  const [selectedTeacher, setSelectedTeacher] = useState('');
  const [selectedStudent, setSelectedStudent] = useState('');

  useEffect(() => {
    fetchCourses();
    fetchTeachers();
    fetchStudents();
  }, []);

  const fetchCourses = async () => {
    setLoading(true);
    try {
      const data = await adminService.getCourses();
      setCourses(data);
    } catch (err) {
      setError('Failed to fetch courses');
    }
    setLoading(false);
  };

  const fetchTeachers = async () => {
    try {
      const data = await teacherService.getAllTeachers();
      setTeachers(data);
    } catch (err) {
      setError('Failed to fetch teachers');
    }
  };

  const fetchStudents = async () => {
    try {
      const data = await adminService.getAllStudents();
      setStudents(data);
    } catch (err) {
      setError('Failed to fetch students');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      if (editingId) {
        await adminService.updateCourse(editingId, form);
      } else {
        await adminService.createCourse(form);
      }
      setForm({ name: '', description: '', credits: 0 });
      setEditingId(null);
      fetchCourses();
    } catch (err) {
      setError('Failed to save course');
    }
    setLoading(false);
  };

  const handleEdit = (course) => {
    setForm({ name: course.name, description: course.description, credits: course.credits });
    setEditingId(course.id);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Delete this course?')) {
      setLoading(true);
      try {
        await adminService.deleteCourse(id);
        fetchCourses();
      } catch (err) {
        setError('Failed to delete course');
      }
      setLoading(false);
    }
  };

  const handleAssignTeacher = async () => {
    if (!selectedTeacher || !assigningCourseId) return;
    setLoading(true);
    try {
      await adminService.assignTeacher(assigningCourseId, selectedTeacher);
      setAssigningCourseId(null);
      setSelectedTeacher('');
      fetchCourses();
    } catch (err) {
      setError('Failed to assign teacher');
    }
    setLoading(false);
  };

  const handleEnrollStudent = async () => {
    if (!selectedStudent || !assigningCourseId) return;
    setLoading(true);
    try {
      await adminService.enrollStudent(assigningCourseId, selectedStudent);
      setAssigningCourseId(null);
      setSelectedStudent('');
      fetchCourses();
    } catch (err) {
      setError('Failed to enroll student');
    }
    setLoading(false);
  };

  return (
    <div className="p-4">
      <h2 className="text-xl font-bold mb-4">Course Management</h2>
      {error && <div className="text-red-500 mb-2">{error}</div>}
      <form onSubmit={handleSubmit} className="mb-4 space-y-2">
        <input className="border p-1" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} placeholder="Name" required />
        <input className="border p-1" value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} placeholder="Description" required />
        <input className="border p-1" type="number" value={form.credits} onChange={e => setForm({ ...form, credits: +e.target.value })} placeholder="Credits" required />
        <button className="bg-blue-500 text-white px-2 py-1" type="submit" disabled={loading}>{editingId ? 'Update' : 'Create'}</button>
        {editingId && <button className="ml-2" type="button" onClick={() => { setForm({ name: '', description: '', credits: 0 }); setEditingId(null); }}>Cancel</button>}
      </form>
      {loading ? <div>Loading...</div> : (
        <ul className="space-y-2">
          {courses.map(c => (
            <li key={c.id} className="border p-2 flex flex-col md:flex-row md:justify-between md:items-center">
              <span>{c.name} ({c.credits} credits)</span>
              <span className="flex flex-col md:flex-row md:items-center gap-2 mt-2 md:mt-0">
                <button className="text-blue-500 mr-2" onClick={() => handleEdit(c)}>Edit</button>
                <button className="text-red-500 mr-2" onClick={() => handleDelete(c.id)}>Delete</button>
                <button className="text-green-600 mr-2" onClick={() => { setAssigningCourseId(c.id); setSelectedTeacher(''); setSelectedStudent(''); }}>Assign Teacher</button>
                <button className="text-purple-600" onClick={() => { setAssigningCourseId(c.id); setSelectedTeacher(''); setSelectedStudent(''); }}>Enroll Student</button>
              </span>
              {/* Assignment dropdowns */}
              {assigningCourseId === c.id && (
                <div className="mt-2 flex flex-col md:flex-row gap-2">
                  <select className="border p-1" value={selectedTeacher} onChange={e => setSelectedTeacher(e.target.value)}>
                    <option value="">Select Teacher</option>
                    {teachers.map(t => (
                      <option key={t.id} value={t.id}>{t.firstName} {t.lastName}</option>
                    ))}
                  </select>
                  <button className="bg-green-500 text-white px-2 py-1" onClick={handleAssignTeacher} disabled={!selectedTeacher}>Assign</button>
                  <select className="border p-1" value={selectedStudent} onChange={e => setSelectedStudent(e.target.value)}>
                    <option value="">Select Student</option>
                    {students.map(s => (
                      <option key={s.id} value={s.id}>{s.firstName} {s.lastName}</option>
                    ))}
                  </select>
                  <button className="bg-purple-500 text-white px-2 py-1" onClick={handleEnrollStudent} disabled={!selectedStudent}>Enroll</button>
                  <button className="ml-2 text-gray-500" onClick={() => setAssigningCourseId(null)}>Close</button>
                </div>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default CourseManagementPage; 
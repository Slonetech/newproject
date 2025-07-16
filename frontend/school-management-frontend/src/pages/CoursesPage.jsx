import React, { useState, useEffect } from 'react';
import { courseService } from '../services/courseService';
import { teacherService } from '../services/teacherService';
import LoadingSpinner from '../components/Common/LoadingSpinner';

const CoursesPage = () => {
  const [courses, setCourses] = useState([]);
  const [teachers, setTeachers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [editingCourse, setEditingCourse] = useState(null);
  const [newCourse, setNewCourse] = useState({
    name: '',
    description: '',
    teacherId: ''
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);
      const [coursesData, teachersData] = await Promise.all([
        courseService.getAllCourses(),
        teacherService.getAllTeachers()
      ]);
      setCourses(coursesData);
      setTeachers(teachersData);
    } catch (err) {
      setError('Failed to fetch data. Please try again.');
      console.error('Error fetching data:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateCourse = async (e) => {
    e.preventDefault();
    try {
      await courseService.createCourse(newCourse);
      setNewCourse({ name: '', description: '', teacherId: '' });
      setShowCreateForm(false);
      await fetchData(); // Refresh data
    } catch (err) {
      setError('Failed to create course. Please try again.');
      console.error('Error creating course:', err);
    }
  };

  const handleUpdateCourse = async (e) => {
    e.preventDefault();
    try {
      await courseService.updateCourse(editingCourse.id, newCourse);
      setEditingCourse(null);
      setNewCourse({ name: '', description: '', teacherId: '' });
      await fetchData(); // Refresh data
    } catch (err) {
      setError('Failed to update course. Please try again.');
      console.error('Error updating course:', err);
    }
  };

  const handleDeleteCourse = async (courseId) => {
    if (window.confirm('Are you sure you want to delete this course?')) {
      try {
        await courseService.deleteCourse(courseId);
        await fetchData(); // Refresh data
      } catch (err) {
        setError('Failed to delete course. Please try again.');
        console.error('Error deleting course:', err);
      }
    }
  };

  const startEditing = (course) => {
    setEditingCourse(course);
    setNewCourse({
      name: course.name,
      description: course.description,
      teacherId: course.teacherId || ''
    });
    setShowCreateForm(true);
  };

  const cancelEditing = () => {
    setEditingCourse(null);
    setNewCourse({ name: '', description: '', teacherId: '' });
    setShowCreateForm(false);
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">Courses</h1>
        <button
          onClick={() => setShowCreateForm(!showCreateForm)}
          className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600 transition-colors"
        >
          {showCreateForm ? 'Cancel' : 'Add Course'}
        </button>
      </div>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      {showCreateForm && (
        <div className="bg-white p-6 rounded-lg shadow-md mb-6">
          <h2 className="text-xl font-semibold mb-4">
            {editingCourse ? 'Edit Course' : 'Create New Course'}
          </h2>
          <form onSubmit={editingCourse ? handleUpdateCourse : handleCreateCourse}>
            <div className="mb-4">
              <label className="block text-gray-700 text-sm font-bold mb-2">
                Course Name *
              </label>
              <input
                type="text"
                value={newCourse.name}
                onChange={(e) => setNewCourse({...newCourse, name: e.target.value})}
                className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:border-blue-500"
                required
                placeholder="Enter course name"
              />
            </div>
            <div className="mb-4">
              <label className="block text-gray-700 text-sm font-bold mb-2">
                Description *
              </label>
              <textarea
                value={newCourse.description}
                onChange={(e) => setNewCourse({...newCourse, description: e.target.value})}
                className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:border-blue-500"
                rows="3"
                required
                placeholder="Enter course description"
              />
            </div>
            <div className="mb-4">
              <label className="block text-gray-700 text-sm font-bold mb-2">
                Teacher *
              </label>
              <select
                value={newCourse.teacherId}
                onChange={(e) => setNewCourse({...newCourse, teacherId: e.target.value})}
                className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:border-blue-500"
                required
              >
                <option value="">Select Teacher</option>
                {teachers.map(teacher => (
                  <option key={teacher.id} value={teacher.id}>
                    {teacher.firstName} {teacher.lastName}
                  </option>
                ))}
              </select>
            </div>
            <div className="flex gap-2">
              <button
                type="submit"
                className="bg-green-500 text-white px-4 py-2 rounded-lg hover:bg-green-600 transition-colors"
              >
                {editingCourse ? 'Update Course' : 'Create Course'}
              </button>
              <button
                type="button"
                onClick={cancelEditing}
                className="bg-gray-500 text-white px-4 py-2 rounded-lg hover:bg-gray-600 transition-colors"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {courses.length === 0 ? (
          <div className="col-span-full text-center py-8">
            <p className="text-gray-500">No courses found. Create your first course!</p>
          </div>
        ) : (
          courses.map(course => (
            <div key={course.id} className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
              <h3 className="text-xl font-semibold mb-2 text-gray-800">{course.name}</h3>
              <p className="text-gray-600 mb-4">{course.description}</p>
              <div className="mb-4">
                <p className="text-sm text-gray-500">
                  <span className="font-medium">Teacher:</span> {' '}
                  {course.teacher 
                    ? `${course.teacher.firstName} ${course.teacher.lastName}` 
                    : 'Not assigned'
                  }
                </p>
                <p className="text-sm text-gray-500">
                  <span className="font-medium">Students:</span> {course.students?.length || 0}
                </p>
              </div>
              <div className="flex justify-between gap-2">
                <button
                  onClick={() => startEditing(course)}
                  className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600 transition-colors"
                >
                  Edit
                </button>
                <button
                  onClick={() => handleDeleteCourse(course.id)}
                  className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600 transition-colors"
                >
                  Delete
                </button>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default CoursesPage;

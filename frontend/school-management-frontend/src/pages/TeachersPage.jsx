import React, { useState, useEffect } from 'react';
import { teacherService } from '../services/teacherService';
import { courseService } from '../services/courseService';
import LoadingSpinner from '../components/Common/LoadingSpinner';

const TeachersPage = () => {
  const [teachers, setTeachers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedTeacher, setSelectedTeacher] = useState(null);
  const [teacherCourses, setTeacherCourses] = useState([]);

  useEffect(() => {
    fetchTeachers();
  }, []);

  const fetchTeachers = async () => {
    try {
      setLoading(true);
      setError(null);
      const teachersData = await teacherService.getAllTeachers();
      setTeachers(teachersData);
    } catch (err) {
      setError('Failed to fetch teachers. Please try again.');
      console.error('Error fetching teachers:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchTeacherCourses = async (teacherId) => {
    try {
      const courses = await courseService.getCoursesByTeacher(teacherId);
      setTeacherCourses(courses);
    } catch (err) {
      console.error('Error fetching teacher courses:', err);
      setTeacherCourses([]);
    }
  };

  const handleTeacherClick = (teacher) => {
    setSelectedTeacher(teacher);
    fetchTeacherCourses(teacher.id);
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">Teachers</h1>
        <div className="text-sm text-gray-500">
          Total Teachers: {teachers.length}
        </div>
      </div>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Teachers List */}
        <div>
          <h2 className="text-xl font-semibold mb-4">All Teachers</h2>
          <div className="space-y-4">
            {teachers.length === 0 ? (
              <p className="text-gray-500">No teachers found.</p>
            ) : (
              teachers.map(teacher => (
                <div
                  key={teacher.id}
                  className={`bg-white rounded-lg shadow-md p-4 cursor-pointer hover:shadow-lg transition-shadow ${
                    selectedTeacher?.id === teacher.id ? 'ring-2 ring-blue-500' : ''
                  }`}
                  onClick={() => handleTeacherClick(teacher)}
                >
                  <h3 className="text-lg font-semibold text-gray-800">
                    {teacher.firstName} {teacher.lastName}
                  </h3>
                  <p className="text-gray-600">{teacher.email}</p>
                  <p className="text-sm text-gray-500">
                    Phone: {teacher.phoneNumber || 'Not provided'}
                  </p>
                  <p className="text-sm text-gray-500">
                    Subject: {teacher.subject || 'Not specified'}
                  </p>
                </div>
              ))
            )}
          </div>
        </div>

        {/* Teacher Details */}
        <div>
          {selectedTeacher ? (
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-xl font-semibold mb-4">
                {selectedTeacher.firstName} {selectedTeacher.lastName} - Details
              </h2>
              
              <div className="mb-6">
                <h3 className="text-lg font-medium mb-2">Contact Information</h3>
                <p className="text-gray-600">Email: {selectedTeacher.email}</p>
                <p className="text-gray-600">Phone: {selectedTeacher.phoneNumber || 'Not provided'}</p>
                <p className="text-gray-600">Subject: {selectedTeacher.subject || 'Not specified'}</p>
              </div>

              <div>
                <h3 className="text-lg font-medium mb-2">Assigned Courses</h3>
                {teacherCourses.length === 0 ? (
                  <p className="text-gray-500">No courses assigned.</p>
                ) : (
                  <div className="space-y-2">
                    {teacherCourses.map(course => (
                      <div key={course.id} className="bg-gray-50 rounded p-3">
                        <h4 className="font-medium text-gray-800">{course.name}</h4>
                        <p className="text-sm text-gray-600">{course.description}</p>
                        <p className="text-xs text-gray-500">
                          Students: {course.students?.length || 0}
                        </p>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          ) : (
            <div className="bg-gray-50 rounded-lg p-6 text-center">
              <p className="text-gray-500">Click on a teacher to view details and assigned courses.</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default TeachersPage;

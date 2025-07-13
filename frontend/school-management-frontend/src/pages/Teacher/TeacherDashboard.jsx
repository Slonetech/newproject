import React from 'react';
import { useProfileData } from '../../hooks/useProfileData';
import CourseCard from '../../components/Teacher/CourseCard';

const TeacherDashboard = () => {
  const { data: teacher, loading, error } = useProfileData('/Teachers/me');

  if (loading) return <div className="text-center py-8">Loading teacher info...</div>;
  if (error) return <div className="text-center text-red-500 py-8">{error}</div>;
  if (!teacher) return <div className="text-center py-8 text-gray-500">No teacher data found.</div>;

  // Calculate summary
  const courseCount = teacher.courses?.length || 0;
  // If students are available, count unique students
  const studentCount = teacher.students ? teacher.students.length : undefined;

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">Teacher Dashboard</h1>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div className="bg-white rounded-lg shadow-md p-6 flex flex-col items-center">
          <div className="text-3xl font-bold text-blue-600">{courseCount}</div>
          <div className="text-gray-700 mt-2">Assigned Courses</div>
        </div>
        <div className="bg-white rounded-lg shadow-md p-6 flex flex-col items-center">
          <div className="text-3xl font-bold text-green-600">{studentCount ?? '-'}</div>
          <div className="text-gray-700 mt-2">Students</div>
        </div>
        <div className="bg-white rounded-lg shadow-md p-6 flex flex-col items-center">
          <div className="text-3xl font-bold text-purple-600">{teacher.firstName[0]}{teacher.lastName[0]}</div>
          <div className="text-gray-700 mt-2">Profile</div>
        </div>
      </div>
      <div className="bg-white rounded-lg shadow-md p-6 mb-6">
        <h2 className="text-xl font-semibold mb-2">
          {teacher.firstName} {teacher.lastName}
        </h2>
        <p>Email: {teacher.email}</p>
      </div>
      <div className="bg-white rounded-lg shadow-md p-6 mb-6">
        <h2 className="text-xl font-semibold mb-4">Assigned Courses</h2>
        {teacher.courses && teacher.courses.length > 0 ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {teacher.courses.map((course) => (
              <CourseCard key={course.id} course={course} />
            ))}
          </div>
        ) : (
          <div className="text-gray-500">No assigned courses.</div>
        )}
      </div>
      {/* Optionally, if teacher.students is available */}
      {teacher.students && (
        <div className="bg-white rounded-lg shadow-md p-6 mb-6">
          <h2 className="text-xl font-semibold mb-4">Students</h2>
          <ul>
            {teacher.students.length > 0 ? (
              teacher.students.map((student) => (
                <li key={student.id}>
                  {student.firstName} {student.lastName} ({student.email})
                </li>
              ))
            ) : (
              <li>No students assigned.</li>
            )}
          </ul>
        </div>
      )}
    </div>
  );
};

export default TeacherDashboard; 
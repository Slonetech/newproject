import React from 'react';
import { useProfileData } from '../../hooks/useProfileData';

const StudentDashboard = () => {
  const { data: student, loading, error } = useProfileData('/Students/me');

  if (loading) return <p>Loading student info...</p>;
  if (error) return <p style={{ color: 'red' }}>{error}</p>;
  if (!student) return <p>No student data found.</p>;

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">Student Dashboard</h1>
      <div className="bg-white rounded-lg shadow-md p-6 mb-6">
        <h2 className="text-xl font-semibold mb-2">
          {student.firstName} {student.lastName}
        </h2>
        <p>Email: {student.email}</p>
        <p>Grade: {student.grade}</p>
      </div>
      <div className="bg-white rounded-lg shadow-md p-6 mb-6">
        <h2 className="text-xl font-semibold mb-4">Enrolled Courses</h2>
        <ul>
          {student.courses && student.courses.length > 0 ? (
            student.courses.map((course) => (
              <li key={course.id}>
                {course.name} ({course.code})
              </li>
            ))
          ) : (
            <li>No courses enrolled.</li>
          )}
        </ul>
      </div>
      {student.grades && (
        <div className="bg-white rounded-lg shadow-md p-6 mb-6">
          <h2 className="text-xl font-semibold mb-4">Grades</h2>
          <ul>
            {student.grades.length > 0 ? (
              student.grades.map((grade) => (
                <li key={grade.id}>
                  {grade.courseName}: {grade.value}
                </li>
              ))
            ) : (
              <li>No grades available.</li>
            )}
          </ul>
        </div>
      )}
    </div>
  );
};

export default StudentDashboard; 
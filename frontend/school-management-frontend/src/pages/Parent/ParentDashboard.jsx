import React from 'react';
import { useProfileData } from '../../hooks/useProfileData';

const ParentDashboard = () => {
  const { data: parent, loading, error } = useProfileData('/Parents/me');

  if (loading) return <p>Loading parent info...</p>;
  if (error) return <p style={{ color: 'red' }}>{error}</p>;
  if (!parent) return <p>No parent data found.</p>;

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">Parent Dashboard</h1>
      <div className="bg-white rounded-lg shadow-md p-6 mb-6">
        <h2 className="text-xl font-semibold mb-2">
          {parent.firstName} {parent.lastName}
        </h2>
        <p>Email: {parent.email}</p>
      </div>
      <div className="bg-white rounded-lg shadow-md p-6 mb-6">
        <h2 className="text-xl font-semibold mb-4">Children</h2>
        <ul>
          {parent.children && parent.children.length > 0 ? (
            parent.children.map((child) => (
              <li key={child.id} className="mb-4">
                <div className="font-semibold">{child.firstName} {child.lastName}</div>
                <div className="ml-4">
                  <div className="mb-2">
                    <strong>Courses:</strong>
                    <ul>
                      {child.courses && child.courses.length > 0 ? (
                        child.courses.map((course) => (
                          <li key={course.id}>{course.name} ({course.code})</li>
                        ))
                      ) : (
                        <li>No courses</li>
                      )}
                    </ul>
                  </div>
                  <div className="mb-2">
                    <strong>Grades:</strong>
                    <ul>
                      {child.grades && child.grades.length > 0 ? (
                        child.grades.map((grade) => (
                          <li key={grade.id}>{grade.courseName}: {grade.value}</li>
                        ))
                      ) : (
                        <li>No grades</li>
                      )}
                    </ul>
                  </div>
                  <div className="mb-2">
                    <strong>Attendance:</strong>
                    <ul>
                      {child.attendance && child.attendance.length > 0 ? (
                        child.attendance.map((att) => (
                          <li key={att.id}>{att.courseName}: {att.isPresent ? 'Present' : 'Absent'} ({new Date(att.date).toLocaleDateString()})</li>
                        ))
                      ) : (
                        <li>No attendance records</li>
                      )}
                    </ul>
                  </div>
                </div>
              </li>
            ))
          ) : (
            <li>No children found.</li>
          )}
        </ul>
      </div>
    </div>
  );
};

export default ParentDashboard; 
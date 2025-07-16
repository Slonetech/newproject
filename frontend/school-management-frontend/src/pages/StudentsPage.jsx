import React, { useState, useEffect } from 'react';
import { studentService } from '../services/studentService';
import { gradeService } from '../services/gradeService';
import LoadingSpinner from '../components/Common/LoadingSpinner';

const StudentsPage = () => {
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedStudent, setSelectedStudent] = useState(null);
  const [studentCourses, setStudentCourses] = useState([]);
  const [studentGrades, setStudentGrades] = useState([]);

  useEffect(() => {
    fetchStudents();
  }, []);

  const fetchStudents = async () => {
    try {
      setLoading(true);
      setError(null);
      const studentsData = await studentService.getAllStudents();
      setStudents(studentsData);
    } catch (err) {
      setError('Failed to fetch students. Please try again.');
      console.error('Error fetching students:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchStudentDetails = async (studentId) => {
    try {
      const [courses, grades] = await Promise.all([
        studentService.getStudentCourses(studentId),
        studentService.getStudentGrades(studentId)
      ]);
      setStudentCourses(courses);
      setStudentGrades(grades);
    } catch (err) {
      console.error('Error fetching student details:', err);
      setStudentCourses([]);
      setStudentGrades([]);
    }
  };

  const handleStudentClick = (student) => {
    setSelectedStudent(student);
    fetchStudentDetails(student.id);
  };

  const calculateGPA = (grades) => {
    if (!grades || grades.length === 0) return 'N/A';
    const total = grades.reduce((sum, grade) => sum + grade.points, 0);
    return (total / grades.length).toFixed(2);
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">Students</h1>
        <div className="text-sm text-gray-500">
          Total Students: {students.length}
        </div>
      </div>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Students List */}
        <div>
          <h2 className="text-xl font-semibold mb-4">All Students</h2>
          <div className="space-y-4">
            {students.length === 0 ? (
              <p className="text-gray-500">No students found.</p>
            ) : (
              students.map(student => (
                <div
                  key={student.id}
                  className={`bg-white rounded-lg shadow-md p-4 cursor-pointer hover:shadow-lg transition-shadow ${
                    selectedStudent?.id === student.id ? 'ring-2 ring-blue-500' : ''
                  }`}
                  onClick={() => handleStudentClick(student)}
                >
                  <h3 className="text-lg font-semibold text-gray-800">
                    {student.firstName} {student.lastName}
                  </h3>
                  <p className="text-gray-600">{student.email}</p>
                  <p className="text-sm text-gray-500">
                    Student ID: {student.studentId || student.id}
                  </p>
                  <p className="text-sm text-gray-500">
                    Grade Level: {student.gradeLevel || 'Not specified'}
                  </p>
                </div>
              ))
            )}
          </div>
        </div>

        {/* Student Details */}
        <div>
          {selectedStudent ? (
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-xl font-semibold mb-4">
                {selectedStudent.firstName} {selectedStudent.lastName} - Details
              </h2>
              
              <div className="mb-6">
                <h3 className="text-lg font-medium mb-2">Student Information</h3>
                <p className="text-gray-600">Email: {selectedStudent.email}</p>
                <p className="text-gray-600">Student ID: {selectedStudent.studentId || selectedStudent.id}</p>
                <p className="text-gray-600">Grade Level: {selectedStudent.gradeLevel || 'Not specified'}</p>
                <p className="text-gray-600">Current GPA: {calculateGPA(studentGrades)}</p>
              </div>

              <div className="mb-6">
                <h3 className="text-lg font-medium mb-2">Enrolled Courses</h3>
                {studentCourses.length === 0 ? (
                  <p className="text-gray-500">No courses enrolled.</p>
                ) : (
                  <div className="space-y-2">
                    {studentCourses.map(course => (
                      <div key={course.id} className="bg-gray-50 rounded p-3">
                        <h4 className="font-medium text-gray-800">{course.name}</h4>
                        <p className="text-sm text-gray-600">{course.description}</p>
                        <p className="text-xs text-gray-500">
                          Teacher: {course.teacher 
                            ? `${course.teacher.firstName} ${course.teacher.lastName}` 
                            : 'Not assigned'
                          }
                        </p>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              <div>
                <h3 className="text-lg font-medium mb-2">Recent Grades</h3>
                {studentGrades.length === 0 ? (
                  <p className="text-gray-500">No grades recorded.</p>
                ) : (
                  <div className="space-y-2">
                    {studentGrades.slice(0, 5).map(grade => (
                      <div key={grade.id} className="bg-gray-50 rounded p-3">
                        <div className="flex justify-between items-center">
                          <span className="font-medium text-gray-800">
                            {grade.assignment?.title || 'Assignment'}
                          </span>
                          <span className="text-blue-600 font-semibold">
                            {grade.points}/{grade.assignment?.maxPoints || 100}
                          </span>
                        </div>
                        <p className="text-sm text-gray-600">
                          Course: {grade.assignment?.course?.name || 'Unknown'}
                        </p>
                        {grade.feedback && (
                          <p className="text-xs text-gray-500 mt-1">
                            Feedback: {grade.feedback}
                          </p>
                        )}
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          ) : (
            <div className="bg-gray-50 rounded-lg p-6 text-center">
              <p className="text-gray-500">Click on a student to view details, courses, and grades.</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default StudentsPage;

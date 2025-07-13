import React, { useEffect, useState } from 'react';
import { useProfileData } from '../../hooks/useProfileData';
import teacherService from '../../services/teacherService';
import CourseCard from '../../components/Teacher/CourseCard';
import { useNavigate } from 'react-router-dom';

const TeacherCoursesPage = () => {
  const { data: teacher, loading, error } = useProfileData('/Teachers/me');
  const [courses, setCourses] = useState([]);
  const [fetching, setFetching] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    if (teacher && teacher.id) {
      fetchCourses();
    }
    // eslint-disable-next-line
  }, [teacher]);

  const fetchCourses = async () => {
    setFetching(true);
    try {
      // Replace with real API call if available
      const data = await teacherService.getCoursesForTeacher(teacher.id);
      setCourses(data);
    } catch (err) {
      setCourses([]);
    }
    setFetching(false);
  };

  const handleViewStudents = (course) => {
    navigate(`/courses/${course.id}/students`);
  };
  const handleGradeStudents = (course) => {
    navigate(`/grades?courseId=${course.id}`);
  };
  const handleMarkAttendance = (course) => {
    navigate(`/attendance?courseId=${course.id}`);
  };

  if (loading || fetching) return <div className="text-center py-8">Loading courses...</div>;
  if (error) return <div className="text-center text-red-500 py-8">{error}</div>;
  if (!teacher) return <div className="text-center py-8 text-gray-500">No teacher data found.</div>;

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">My Assigned Courses</h1>
      {courses.length === 0 ? (
        <div className="text-center py-8 text-gray-500">No assigned courses found.</div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {courses.map((course) => (
            <CourseCard
              key={course.id}
              course={course}
              onViewStudents={handleViewStudents}
              onUploadGrades={handleGradeStudents}
              onMarkAttendance={handleMarkAttendance}
            />
          ))}
        </div>
      )}
    </div>
  );
};

export default TeacherCoursesPage; 
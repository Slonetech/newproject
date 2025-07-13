import React, { useEffect, useState } from 'react';
import axiosInstance from '../../api/axiosConfig';

const StudentCoursesPage = () => {
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchCourses();
  }, []);

  const fetchCourses = async () => {
    setLoading(true);
    try {
      const res = await axiosInstance.get('/api/students/enrolled-courses');
      setCourses(res.data);
    } catch (err) {
      setError('Failed to fetch enrolled courses');
    }
    setLoading(false);
  };

  return (
    <div className="p-4">
      <h2 className="text-xl font-bold mb-4">My Enrolled Courses</h2>
      {error && <div className="text-red-500 mb-2">{error}</div>}
      {loading ? <div>Loading...</div> : (
        <ul className="space-y-2">
          {courses.map(c => (
            <li key={c.id} className="border p-2">{c.name} ({c.credits} credits)</li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default StudentCoursesPage; 
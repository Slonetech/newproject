import React from 'react';

const CourseCard = ({ course, onViewStudents, onUploadGrades }) => (
  <div className="bg-white rounded-lg shadow-md p-4 flex flex-col gap-2">
    <div className="font-semibold text-lg">{course.name} <span className="text-gray-500 text-sm">({course.code})</span></div>
    <div className="text-gray-600">Students: {course.studentCount ?? '-'}</div>
    <div className="flex gap-2 mt-2">
      {onViewStudents && (
        <button className="btn btn-sm btn-primary" onClick={() => onViewStudents(course)}>View Students</button>
      )}
      {onUploadGrades && (
        <button className="btn btn-sm btn-secondary" onClick={() => onUploadGrades(course)}>Upload Grades</button>
      )}
    </div>
  </div>
);

export default CourseCard; 
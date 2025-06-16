Full-Stack Assignment: School Management System API
Overview
Building a School Management System as a RESTful API using ASP.NET Core Web API and Entity Framework Core. This system must support various user roles — Admin, Teacher, Student, and Parent — and provide role-based access to features like managing students, teachers, courses, grades, and attendance.
This project will test your ability to:
Build a secure API with authentication and role-based authorization
Handle complex entity relationships
Implement real-world use cases for multiple stakeholders
Build scalable REST endpoints
Ensure data validation, integrity, and logging
System Modules & Functional Requirements
1. User Authentication & Role Management
Use JWT-based authentication with secure password hashing.
Register and log in users with username and password.
Support the following roles: Admin, Teacher, Student, Parent.
Apply role-based authorization using ASP.NET Identity.
Admin Capabilities:
Create, update, and delete any user (student, teacher, parent).
Assign user roles and relationships.
2. Student Management
Admin:
Create, edit, delete student records.
Assign students to courses.
Teacher:
View students enrolled in their courses.
Student:
View own profile, courses, and grades.
Parent:
View their child’s profile, grades, attendance.
3. Teacher Management
Admin:
Create, edit, and delete teacher records.
Assign teachers to courses.
Teacher:
View enrolled students.
Record attendance and grades.
View own teaching courses.
4. Course Management
Admin:
Add, edit, delete courses.
Assign teachers and students.
Teacher:
Manage their course enrollments.
Student:
View available and enrolled courses.
Parent:
View child's courses.
5. Grade Management
Teacher:
Assign, edit, delete student grades.
Student:
View their own grades.
Admin:
View/manage all grades.
Include date stamp when grades are awarded.
6. Attendance Management
Teacher:
Mark attendance (Present, Absent, Late).
View and edit records.
Student/Parent:
View attendance records.
Admin:
Audit all attendance data.
7. Parent-Child Relationship
Admin assigns student to parent.
Parent:
View only their child’s data (grades, attendance, courses).
Option to message school staff (bonus).
8. Reporting & Analytics
Generate reports (JSON or PDF) filtered by:
Student, Course, Time Range
Example reports:
A student’s progress report
Class-wide grade averages
Attendance summary
9. Search & Filtering
Implement dynamic search/filtering for:
Students (by name, course, grade)
Courses (by title, teacher, credits)
Grades and attendance (by course, student, date)
10. Data Validation & Integrity
Use model validation annotations:
Required fields
Valid email formats
Grade within valid range
Unique student ID/email
Prevent invalid/duplicate entries.
11. Security & Privacy
Store passwords securely using hashing.
Use HTTPS for all endpoints.
Restrict access based on role.
Track:
User logins
Data changes
Allow profile and password updates.
12. Communication & Notifications
Notify students/parents via email when:
Grades are posted
Attendance is updated
New courses are assigned
Simulate or integrate with services like SendGrid.
Bonus: Implement internal messaging between teachers and parents/students.
Technical Requirements
.NET 8/9 Web API
Entity Framework Core (Code-First)
SQL Server 
ASP.NET Core Identity + JWT Authentication
Swagger for API testing
Logging using Serilog or built-in logging
Use async/await for all DB operations
Optional: Unit Testing with xUnit
Bonus Features (Optional)
Real-time Notifications (SignalR)
Export Reports (CSV, PDF)
API Versioning
Global Exception Handler Middleware
Docker Support
CI/CD with GitHub Actions

using System;
using System.Collections.Generic;
using System.Linq;


        static void PrintMenu()
        {
            Console.WriteLine("=== University Management System ===");
            Console.WriteLine("1. Add student");
            Console.WriteLine("2. List students");
            Console.WriteLine("3. Add instructor");
            Console.WriteLine("4. List instructors");
            Console.WriteLine("5. Add course");
            Console.WriteLine("6. List courses");
            Console.WriteLine("7. Enroll student to course");
            Console.WriteLine("8. View student details & courses");
            Console.WriteLine("9. View course details & students");
            Console.WriteLine("10. Assign grade to student for course");
            Console.WriteLine("11. Show full lists (students/instructors/courses)");
            Console.WriteLine("0. Exit");
        }

        static void AddStudent()
        {
            Console.Write("Name: "); var name = Console.ReadLine();
            Console.Write("Age: "); if (!int.TryParse(Console.ReadLine(), out var age)) { Console.WriteLine("Invalid age"); return; }
            Console.Write("Contact: "); var contact = Console.ReadLine();
            var s = uni.CreateStudent(name, age, contact);
            Console.WriteLine($"Added: {s.GetInfo()}");
        }

        static void ListStudents()
        {
            Console.WriteLine("Students:");
            foreach (var s in uni.Students)
            {
                var gpa = s.GetGPA();
                var gpaText = gpa.HasValue ? gpa.Value.ToString("0.00") : "N/A";
                Console.WriteLine($"{s.Id}. {s.Name} — age {s.Age} — GPA: {gpaText}");
            }
        }

        static void AddInstructor()
        {
            Console.Write("Name: "); var name = Console.ReadLine();
            Console.Write("Age: "); if (!int.TryParse(Console.ReadLine(), out var age)) { Console.WriteLine("Invalid age"); return; }
            Console.Write("Contact: "); var contact = Console.ReadLine();
            var i = uni.CreateInstructor(name, age, contact);
            Console.WriteLine($"Added: {i.GetInfo()}");
        }

        static void ListInstructors()
        {
            Console.WriteLine("Instructors:");
            foreach (var i in uni.Instructors)
            {
                Console.WriteLine($"{i.Id}. {i.Name} — age {i.Age} — courses: {i.Courses.Count}");
            }
        }

        static void AddCourse()
        {
            Console.Write("Code (e.g. CS101): "); var code = Console.ReadLine();
            Console.Write("Title: "); var title = Console.ReadLine();
            Console.Write("Instructor ID (or empty): "); var instrInput = Console.ReadLine();
            Instructor instr = null;
            if (!string.IsNullOrWhiteSpace(instrInput) && int.TryParse(instrInput, out var iid)) instr = uni.GetInstructorById(iid);
            Console.Write("Capacity (0 = unlimited): "); if (!int.TryParse(Console.ReadLine(), out var cap)) cap = 0;

            var c = uni.CreateCourse(code, title, instr, cap);
            Console.WriteLine($"Added course: {c}");
        }

        static void ListCourses()
        {
            Console.WriteLine("Courses:");
            foreach (var c in uni.Courses)
            {
                Console.WriteLine(c);
            }
        }

        static void EnrollStudentToCourse()
        {
            Console.Write("Student ID: "); if (!int.TryParse(Console.ReadLine(), out var sid)) { Console.WriteLine("Invalid id"); return; }
            Console.Write("Course code: "); var code = Console.ReadLine();
            var result = uni.EnrollStudentToCourse(sid, code);
            Console.WriteLine(result ? "Enrolled successfully" : "Failed to enroll (maybe full / not found / already enrolled)");
        }

        static void ViewStudentCourses()
        {
            Console.Write("Student ID: "); if (!int.TryParse(Console.ReadLine(), out var sid)) { Console.WriteLine("Invalid id"); return; }
            var s = uni.GetStudentById(sid);
            if (s == null) { Console.WriteLine("Student not found"); return; }
            Console.WriteLine(s.GetInfo());
            Console.WriteLine("Courses:");
            foreach (var e in s.Enrollments)
            {
                var gradeText = e.Grade.HasValue ? e.Grade.Value.ToString("0.00") : "no grade";
                Console.WriteLine($" - {e.Course.Code}: {e.Course.Title} — Grade: {gradeText}");
            }
            var gpa = s.GetGPA();
            Console.WriteLine($"GPA: {(gpa.HasValue ? gpa.Value.ToString("0.00") : "N/A")}");
        }

        static void ViewCourseStudents()
        {
            Console.Write("Course code: "); var code = Console.ReadLine();
            var c = uni.GetCourseByCode(code);
            if (c == null) { Console.WriteLine("Course not found"); return; }
            Console.WriteLine(c);
            Console.WriteLine("Students:");
            foreach (var e in c.Enrollments)
            {
                var gradeText = e.Grade.HasValue ? e.Grade.Value.ToString("0.00") : "no grade";
                Console.WriteLine($" - {e.Student.Id}. {e.Student.Name} — Grade: {gradeText}");
            }
        }

        static void AssignGrade()
        {
            Console.Write("Student ID: "); if (!int.TryParse(Console.ReadLine(), out var sid)) { Console.WriteLine("Invalid id"); return; }
            Console.Write("Course code: "); var code = Console.ReadLine();
            Console.Write("Grade (numeric): "); if (!double.TryParse(Console.ReadLine(), out var grade)) { Console.WriteLine("Invalid grade"); return; }
            var ok = uni.AssignGrade(sid, code, grade);
            Console.WriteLine(ok ? "Grade assigned" : "Failed to assign grade (check enrollment and ids)");
        }

        static void ShowAll()
        {
            Console.WriteLine("=== All students ===");
            ListStudents();
            Console.WriteLine();
            Console.WriteLine("=== All instructors ===");
            ListInstructors();
            Console.WriteLine();
            Console.WriteLine("=== All courses ===");
            ListCourses();
        }

        static void SeedTestData()
        {
            var inst1 = uni.CreateInstructor("Dr. Alice", 45, "alice@uni.edu");
            var inst2 = uni.CreateInstructor("Prof. Bob", 52, "bob@uni.edu");

            var s1 = uni.CreateStudent("Ivan Petrov", 20, "ivan@example.com");
            var s2 = uni.CreateStudent("Olga Smirnova", 22, "olga@example.com");
            var s3 = uni.CreateStudent("Mark Johnson", 19, "mark@example.com");

            var c1 = uni.CreateCourse("CS101", "Intro to Programming", inst1, capacity: 2);
            var c2 = uni.CreateCourse("MATH10", "Calculus I", inst2, capacity: 0);

            uni.EnrollStudentToCourse(s1.Id, "CS101");
            uni.EnrollStudentToCourse(s2.Id, "CS101");
            var r = uni.EnrollStudentToCourse(s3.Id, "CS101");
            uni.EnrollStudentToCourse(s3.Id, "MATH10");

            uni.AssignGrade(s1.Id, "CS101", 4.5);
            uni.AssignGrade(s2.Id, "CS101", 3.8);
            uni.AssignGrade(s3.Id, "MATH10", 4.0);
        }
    }
}

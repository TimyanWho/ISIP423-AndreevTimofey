using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityApp
{
    abstract class Person
    {
        private static int _nextId = 1;
        private readonly int _id;
        private string _name;
        private int _age;
        private string _contact;

        protected Person(string name, int age, string contact)
        {
            _id = _nextId++;
            _name = name;
            _age = age;
            _contact = contact;
        }

        public int Id => _id;
        public string Name => _name;
        public int Age => _age;
        public string Contact => _contact;

        public void UpdateContact(string newContact)
        {
            if (!string.IsNullOrWhiteSpace(newContact))
                _contact = newContact;
        }

        static void PrintMenu()
        public void UpdateAge(int newAge)
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
            if (newAge > 0)
                _age = newAge;
        }

        public abstract string GetInfo();
    }

        static void AddStudent()
        {
            Console.Write("Name: "); var name = Console.ReadLine();
            Console.Write("Age: "); if (!int.TryParse(Console.ReadLine(), out var age)) { Console.WriteLine("Invalid age"); return; }
            Console.Write("Contact: "); var contact = Console.ReadLine();
            var s = uni.CreateStudent(name, age, contact);
            Console.WriteLine($"Added: {s.GetInfo()}");
    class Student : Person
    {
        private readonly List<Enrollment> _enrollments = new List<Enrollment>();

        public Student(string name, int age, string contact)
            : base(name, age, contact)
        {
        }

        static void ListStudents()
        public override string GetInfo()
        {
            Console.WriteLine("Students:");
            foreach (var s in uni.Students)
            {
                var gpa = s.GetGPA();
                var gpaText = gpa.HasValue ? gpa.Value.ToString("0.00") : "N/A";
                Console.WriteLine($"{s.Id}. {s.Name} — age {s.Age} — GPA: {gpaText}");
            }
        }
            return $"Student #{Id}: {Name}, Age {Age}, Contact: {Contact}";
        }


    }

    class Instructor : Person
    {
        private readonly List<Course> _courses = new List<Course>();

        public Instructor(string name, int age, string contact)
            : base(name, age, contact)
        {
        }

        public override string GetInfo()
        {
            return $"Instructor #{Id}: {Name}, Age {Age}, Contact: {Contact}";
        }

        internal void AddCourseTaught(Course course)
        {
            if (course != null && !_courses.Contains(course))
                _courses.Add(course);
        }

        internal void RemoveCourseTaught(Course course)
        {
            if (course != null)
                _courses.Remove(course);
        }

        public IReadOnlyList<Course> Courses => _courses.AsReadOnly();
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
    class Enrollment
    {
        public Student Student { get; }
        public Course Course { get; }
        public double? Grade { get; set; }

        public Enrollment(Student student, Course course)
        {
            Student = student ?? throw new ArgumentNullException(nameof(student));
            Course = course ?? throw new ArgumentNullException(nameof(course));
        }
    }

    class Course
    {
        private readonly List<Enrollment> _enrollments = new List<Enrollment>();
        private Instructor _instructor;
        private string _code;
        private string _title;
        private int _capacity;

        public Course(string code, string title, Instructor instructor = null, int capacity = 0)
        {
            _code = code;
            _title = title;
            _instructor = instructor;
            _capacity = capacity;
            instructor?.AddCourseTaught(this);
        }

        public string Code => _code;
        public string Title => _title;
        public Instructor Instructor
        {
            get => _instructor;
            set
            {
                _instructor?.RemoveCourseTaught(this);
                _instructor = value;
                _instructor?.AddCourseTaught(this);
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
        public int Capacity => _capacity;

        public IReadOnlyList<Enrollment> Enrollments => _enrollments.AsReadOnly();


    }

    class University
    {
        private readonly List<Student> _students = new List<Student>();
        private readonly List<Instructor> _instructors = new List<Instructor>();
        private readonly List<Course> _courses = new List<Course>();

            var c = uni.CreateCourse(code, title, instr, cap);
            Console.WriteLine($"Added course: {c}");
        }
        public Student CreateStudent(string name, int age, string contact)
        {
            var s = new Student(name, age, contact);
            _students.Add(s);
            return s;
        }


    }

        static void ListCourses()
        {
            Console.WriteLine("Courses:");
            foreach (var c in uni.Courses)
            {
                Console.WriteLine(c);
            }
        }
    class Program
    {
        static University uni = new University();

        static void Main(string[] args)
        {
            SeedTestData();
            while (true)
            {
                PrintMenu();
                Console.Write("Select option: ");
                var input = Console.ReadLine();
                Console.WriteLine();
                if (string.IsNullOrWhiteSpace(input)) continue;
                if (!int.TryParse(input, out var option)) continue;

        static void EnrollStudentToCourse()
        {
            Console.Write("Student ID: "); if (!int.TryParse(Console.ReadLine(), out var sid)) { Console.WriteLine("Invalid id"); return; }
            Console.Write("Course code: "); var code = Console.ReadLine();
            var result = uni.EnrollStudentToCourse(sid, code);
            Console.WriteLine(result ? "Enrolled successfully" : "Failed to enroll (maybe full / not found / already enrolled)");
        }
                switch (option)
                {
                    case 1: AddStudent(); break;
                    case 2: ListStudents(); break;
                    case 3: AddInstructor(); break;
                    case 4: ListInstructors(); break;
                    case 5: AddCourse(); break;
                    case 6: ListCourses(); break;
                    case 7: EnrollStudentToCourse(); break;
                    case 8: ViewStudentCourses(); break;
                    case 9: ViewCourseStudents(); break;
                    case 10: AssignGrade(); break;
                    case 11: ShowAll(); break;
                    case 0: Console.WriteLine("Bye"); return;
                    default: Console.WriteLine("Unknown option"); break;
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
                Console.WriteLine();
            }
        }

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

        static void ViewCourseStudents()
        static void ListStudents()
        {
            Console.Write("Course code: "); var code = Console.ReadLine();
            var c = uni.GetCourseByCode(code);
            if (c == null) { Console.WriteLine("Course not found"); return; }
            Console.WriteLine(c);
            Console.WriteLine("Students:");
            foreach (var e in c.Enrollments)
            Console.WriteLine("Students:");
            foreach (var s in uni.Students)
            {
                var gradeText = e.Grade.HasValue ? e.Grade.Value.ToString("0.00") : "no grade";
                Console.WriteLine($" - {e.Student.Id}. {e.Student.Name} — Grade: {gradeText}");
                var gpa = s.GetGPA();
                var gpaText = gpa.HasValue ? gpa.Value.ToString("0.00") : "N/A";
                Console.WriteLine($"{s.Id}. {s.Name} — age {s.Age} — GPA: {gpaText}");
            }
        }

        static void AssignGrade()
        static void AddInstructor()
        {
            Console.Write("Student ID: "); if (!int.TryParse(Console.ReadLine(), out var sid)) { Console.WriteLine("Invalid id"); return; }
            Console.Write("Course code: "); var code = Console.ReadLine();
            Console.Write("Grade (numeric): "); if (!double.TryParse(Console.ReadLine(), out var grade)) { Console.WriteLine("Invalid grade"); return; }
            var ok = uni.AssignGrade(sid, code, grade);
            Console.WriteLine(ok ? "Grade assigned" : "Failed to assign grade (check enrollment and ids)");
            Console.Write("Name: "); var name = Console.ReadLine();
            Console.Write("Age: "); if (!int.TryParse(Console.ReadLine(), out var age)) { Console.WriteLine("Invalid age"); return; }
            Console.Write("Contact: "); var contact = Console.ReadLine();
            var i = uni.CreateInstructor(name, age, contact);
            Console.WriteLine($"Added: {i.GetInfo()}");
        }

        static void ShowAll()
        static void ListInstructors()
        {
            Console.WriteLine("=== All students ===");
            ListStudents();
            Console.WriteLine();
            Console.WriteLine("=== All instructors ===");
            ListInstructors();
            Console.WriteLine();
            Console.WriteLine("=== All courses ===");
            ListCourses();
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

        static void SeedTestData()
        {
            var inst1 = uni.CreateInstructor("Dr. Alice", 45, "alice@uni.edu");
            var inst2 = uni.CreateInstructor("Prof. Bob", 52, "bob@uni.edu");
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

            var s1 = uni.CreateStudent("Ivan Petrov", 20, "ivan@example.com");
            var s2 = uni.CreateStudent("Olga Smirnova", 22, "olga@example.com");
            var s3 = uni.CreateStudent("Mark Johnson", 19, "mark@example.com");
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

            var c1 = uni.CreateCourse("CS101", "Intro to Programming", inst1, capacity: 2);
            var c2 = uni.CreateCourse("MATH10", "Calculus I", inst2, capacity: 0);
        static void AssignGrade()
        {
            Console.Write("Student ID: "); if (!int.TryParse(Console.ReadLine(), out var sid)) { Console.WriteLine("Invalid id"); return; }
            Console.Write("Course code: "); var code = Console.ReadLine();
            Console.Write("Grade (numeric): "); if (!double.TryParse(Console.ReadLine(), out var grade)) { Console.WriteLine("Invalid grade"); return; }
            var ok = uni.AssignGrade(sid, code, grade);
            Console.WriteLine(ok ? "Grade assigned" : "Failed to assign grade (check enrollment and ids)");
        }

            uni.EnrollStudentToCourse(s1.Id, "CS101");
            uni.EnrollStudentToCourse(s2.Id, "CS101");
            var r = uni.EnrollStudentToCourse(s3.Id, "CS101");
            uni.EnrollStudentToCourse(s3.Id, "MATH10");
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

            uni.AssignGrade(s1.Id, "CS101", 4.5);
            uni.AssignGrade(s2.Id, "CS101", 3.8);
            uni.AssignGrade(s3.Id, "MATH10", 4.0);
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

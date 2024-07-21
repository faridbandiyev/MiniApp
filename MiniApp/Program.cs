using MiniApp.Helpers.Enums;
using MiniApp.Helpers.Exceptions;
using MiniApp.Helpers;
using MiniApp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public class Program
{
    private static List<Classroom> classrooms = new List<Classroom>();

    public static void Main()
    {
        string classroomsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Jsons", "classroom.json");
        LoadData(classroomsPath);

        while (true)
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Create Classroom");
            Console.WriteLine("2. Create Student");
            Console.WriteLine("3. Display All Students");
            Console.WriteLine("4. Display Students in Specific Classroom");
            Console.WriteLine("5. Delete Student");
            Console.WriteLine("6. Display All Classrooms");
            Console.WriteLine("7. Update Student or Classroom");
            Console.WriteLine("0. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateClassroom();
                    break;
                case "2":
                    CreateStudent();
                    break;
                case "3":
                    DisplayAllStudents();
                    break;
                case "4":
                    DisplayClassroomStudents();
                    break;
                case "5":
                    DeleteStudent();
                    break;
                case "6":
                    DisplayAllClassrooms();
                    break;
                case "7":
                    UpdateStudentOrClassroom();
                    break;
                case "0":
                    SaveData(classroomsPath);
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private static void LoadData(string filePath)
    {
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                var jsonData = reader.ReadToEnd();
                classrooms = JsonConvert.DeserializeObject<List<Classroom>>(jsonData) ?? new List<Classroom>();
            }
        }
    }

    private static void SaveData(string path)
    {
        var json = JsonConvert.SerializeObject(classrooms, Formatting.Indented);
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine(json);
        }
    }

    private static void CreateClassroom()
    {
        while (true)
        {
            Console.Write("Enter classroom name (2 uppercase letters followed by 3 digits): ");
            string name = Console.ReadLine();

            if (!name.ValidClassroomName())
            {
                Console.WriteLine("Invalid classroom name format. It should be 2 uppercase letters followed by 3 digits.");
                continue;
            }

            bool classroomExists = false;

            foreach (var classroom in classrooms)
            {
                if (classroom != null && classroom.Name != null)
                {
                    if (classroom.Name.ToLower() == name.ToLower())
                    {
                        classroomExists = true;
                        break;
                    }
                }
            }

            if (classroomExists)
            {
                Console.WriteLine("Classroom with this name already exists. Please try again.");
                continue;
            }

            Console.WriteLine("Select classroom type:");
            Console.WriteLine("1. Backend");
            Console.WriteLine("2. FrontEnd");
            string typeChoice = Console.ReadLine();

            ClassroomType type;
            switch (typeChoice)
            {
                case "1":
                    type = ClassroomType.Backend;
                    break;
                case "2":
                    type = ClassroomType.Frontend;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select 1 for Backend or 2 for FrontEnd.");
                    continue;
            }

            try
            {
                classrooms.Add(new Classroom(name, type));
                SaveData(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Jsons", "classroom.json"));
                Console.WriteLine("Classroom created successfully.");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating classroom: {ex.Message}");
            }
        }
    }

    private static void CreateStudent()
    {
        if (classrooms.Count == 0)
        {
            Console.WriteLine("No classrooms available. Create a classroom first.");
            return;
        }

        while (true)
        {
            Console.Write("Enter student name: ");
            string name = Console.ReadLine();
            Console.Write("Enter student surname: ");
            string surname = Console.ReadLine();
            Console.WriteLine("Select a classroom by ID:");
            DisplayAllClassrooms();

            if (!int.TryParse(Console.ReadLine(), out int classroomId))
            {
                Console.WriteLine("Invalid classroom ID. Please try again.");
                continue;
            }

            var classroom = classrooms.Find(c => c.Id == classroomId);
            if (classroom == null)
            {
                Console.WriteLine("Classroom not found.");
                continue;
            }

            bool studentExists = false;
            foreach (var student in classroom.GetStudents())
            {
                if (student.Name.ToLower() == name.ToLower() &&
                    student.Surname.ToLower() == surname.ToLower())
                {
                    studentExists = true;
                    break;
                }
            }

            if (studentExists)
            {
                Console.WriteLine("Student with this name and surname already exists in the classroom. Please try again.");
                continue;
            }

            try
            {
                var student = new Student(name, surname);
                classroom.StudentAdd(student);
                SaveData(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Jsons", "classroom.json"));
                Console.WriteLine("Student added successfully.");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static void DisplayAllStudents()
    {
        foreach (var classroom in classrooms)
        {
            Console.WriteLine($"Classroom ID: {classroom.Id}, Name: {classroom.Name} (Type: {classroom.Type}):");
            foreach (var student in classroom.GetStudents())
            {
                Console.WriteLine($"  Student ID: {student.Id}, Name: {student.Name}, Surname: {student.Surname}");
            }
        }
    }

    private static void DisplayClassroomStudents()
    {
        DisplayAllClassrooms();
        Console.Write("Enter classroom ID: ");
        if (int.TryParse(Console.ReadLine(), out int classroomId))
        {
            var classroom = classrooms.Find(c => c.Id == classroomId);
            if (classroom != null)
            {
                Console.WriteLine($"Classroom {classroom.Name} (Type: {classroom.Type}):");
                foreach (var student in classroom.GetStudents())
                {
                    Console.WriteLine($"  {student.Name} {student.Surname}");
                }
            }
            else
            {
                Console.WriteLine("Classroom not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid classroom ID.");
        }
    }

    private static void DeleteStudent()
    {
        foreach (var classroom in classrooms)
        {
            foreach (var student in classroom.GetStudents())
            {
                Console.WriteLine($"Student ID: {student.Id}, Name: {student.Name} {student.Surname}, Classroom: {classroom.Name}");
            }
        }

        Console.Write("Enter student ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int studentId))
        {
            bool studentDeleted = false;
            foreach (var classroom in classrooms)
            {
                try
                {
                    classroom.Delete(studentId);
                    studentDeleted = true;
                    Console.WriteLine("Student deleted successfully.");
                    SaveData(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Jsons", "classroom.json"));
                    break;
                }
                catch (StudentNotFoundException)
                {
                    continue;
                }
            }
            if (!studentDeleted)
            {
                Console.WriteLine("Student not found in any classroom.");
            }
        }
        else
        {
            Console.WriteLine("Invalid student ID.");
        }
    }

    private static void DisplayAllClassrooms()
    {
        foreach (var classroom in classrooms)
        {
            Console.WriteLine($"Classroom ID: {classroom.Id}, Name: {classroom.Name}, Type: {classroom.Type}");
        }
    }

    private static void UpdateStudentOrClassroom()
    {
        string classroomsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Jsons", "classroom.json");
        LoadData(classroomsPath);

        Console.WriteLine("Choose what to update:");
        Console.WriteLine("1. Classroom");
        Console.WriteLine("2. Student");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                UpdateClassroom();
                break;
            case "2":
                UpdateStudent();
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }

        SaveData(classroomsPath);
    }

    private static void UpdateClassroom()
    {
        DisplayAllClassrooms();

        Console.Write("Enter the ID of the classroom to update: ");
        if (int.TryParse(Console.ReadLine(), out int classroomId))
        {
            var classroom = classrooms.Find(c => c.Id == classroomId);
            if (classroom != null)
            {
                Console.WriteLine($"Updating Classroom ID: {classroom.Id}, Name: {classroom.Name}, Type: {classroom.Type}");

                Console.Write("Enter new name (or press Enter to keep current name): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrEmpty(newName))
                {
                    if (!newName.ValidClassroomName())
                    {
                        Console.WriteLine("Invalid classroom name format.");
                        return;
                    }
                    classroom.Name = newName;
                }

                Console.WriteLine("Select new type (Backend/Frontend) (or press Enter to keep current type):");
                Console.WriteLine("1. Backend");
                Console.WriteLine("2. Frontend");
                string newTypeChoice = Console.ReadLine();

                if (!string.IsNullOrEmpty(newTypeChoice))
                {
                    switch (newTypeChoice)
                    {
                        case "1":
                            classroom.Type = ClassroomType.Backend;
                            break;
                        case "2":
                            classroom.Type = ClassroomType.Frontend;
                            break;
                        default:
                            Console.WriteLine("Invalid classroom type.");
                            return;
                    }
                }

                Console.WriteLine("Classroom updated successfully.");
            }
            else
            {
                Console.WriteLine("Classroom not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid classroom ID.");
        }
    }

    private static void UpdateStudent()
    {
        DisplayAllStudents();

        Console.Write("Enter the ID of the student to update: ");
        if (int.TryParse(Console.ReadLine(), out int studentId))
        {
            bool studentFound = false;

            foreach (var classroom in classrooms)
            {
                var student = classroom.GetStudents().FirstOrDefault(s => s.Id == studentId);
                if (student != null)
                {
                    Console.WriteLine($"Updating Student ID: {student.Id}, Name: {student.Name}, Surname: {student.Surname}");

                    Console.Write("Enter new name (or press Enter to keep current name): ");
                    string newName = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newName))
                    {
                        student.Name = newName;
                    }

                    Console.Write("Enter new surname (or press Enter to keep current surname): ");
                    string newSurname = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newSurname))
                    {
                        student.Surname = newSurname;
                    }

                    studentFound = true;
                    Console.WriteLine("Student updated successfully.");
                    break;
                }
            }

            if (!studentFound)
            {
                Console.WriteLine("Student not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid student ID.");
        }
    }
}

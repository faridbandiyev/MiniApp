using MiniApp.Helpers;
using MiniApp.Helpers.Enums;
using MiniApp.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniApp;

public class Classroom
{
    private static int _id = 1;
    public int Id { get; }
    public string Name { get; set; }
    public ClassroomType Type { get; set; }
    private int Capacity { get; set; }
    public List<Student> Students { get; private set; }

    public Classroom(string name, ClassroomType type)
    {
        if (!name.ValidClassroomName())
        {
            throw new Exception("Name must consist of 5 chracters; of which first 2 have to be capital, while the rest has to be only digits.");
        }
        Id = _id++;
        Name = name;
        Type = type;
        Students = new List<Student>();
        if (type == ClassroomType.Backend)
        {
            Capacity = 20;
        }
        else
            Capacity = 15;
    }

    public void StudentAdd(Student student)
    {
        if (Students.Count < Capacity)
        {
            Students.Add(student);
        }
        else
            Console.WriteLine("Classroom is full, new student can not be added.");
    }
    public Student FindId(int id)
    {
        var student = Students.FirstOrDefault(student => student.Id == id);
        if (student == null)
        {
            throw new StudentNotFoundException("Student could not be found");
        }
        return student;
    }

    public void Delete (int id)
    {
        var student = FindId(id);
        Students.Remove(student);
    }

    public List<Student> GetStudents()
    => Students;

}

using MiniApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniApp;

public class Student
{
    private static int _id = 1;
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    
    public Student(string name, string surname)
    {
        if (!name.StartsWithACapitalLetter() || !surname.StartsWithACapitalLetter() || !name.IsValid() || !surname.IsValid())
        {
            throw new Exception("Name and surname must start with a capital letter; be an only word; be over 3 letters bottom limit.");
        }
        Id = _id++;
        Name = name;
        Surname = surname;
    }
}

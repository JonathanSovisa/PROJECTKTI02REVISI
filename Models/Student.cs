using System;
using System.ComponentModel.DataAnnotations;

namespace SampleSecureWeb.Models;

public class Student
{

public int Id { get; set; } 
public String Nim { get; set; } = null!;
public String FullName { get; set; } = null!;
}

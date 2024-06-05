using BasicOpenTelemetry.Data;
using Microsoft.AspNetCore.Mvc;

namespace BasicOpenTelemetry.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController : ControllerBase
{
   private readonly StudentDbContext _context;
   private readonly ILogger<StudentController> _logger;

   public StudentController(StudentDbContext context, ILogger<StudentController> logger)
   {
       _context = context;
       _logger = logger;
   }

    [HttpGet]
    public IEnumerable<Student> Get()
    {
        _logger.LogInformation("Getting all students");
        return _context.Students.ToList();
    }
    
    [HttpGet("{id}")]
    public Student Get(int id)
    {
        _logger.LogInformation("Getting student by id");
        return _context.Students.Find(id);
    }
    
    [HttpPost]
    public Student Post(StudentCreateDto studentCreateDto)
    {
        _logger.LogInformation("Creating a new student");
        var student = new Student
        {
            Name = studentCreateDto.Name,
            Age = studentCreateDto.Age
        };
        _context.Students.Add(student);
        _context.SaveChanges();
        //Code to increment the following metric value by one each time a student is added
        DiagnosticsConfig.StudentCounter.Add(1,new KeyValuePair<string, object>("student.id",student.Id));
        _logger.LogInformation("Student created");
        return student;
    }
    
    [HttpPut]
    public Student Put(StudentUpdateDto studentUpdateDto)
    {
        _logger.LogInformation("Updating student");
        var student = _context.Students.Find(studentUpdateDto.Id);
        student.Name = studentUpdateDto.Name;
        student.Age = studentUpdateDto.Age;
        _context.SaveChanges();
        _logger.LogInformation("Student updated");
        return student;
    }
    
    [HttpDelete("{id}")]
    public Student Delete(int id)
    {
        _logger.LogInformation("Deleting student");
        var student = _context.Students.Find(id);
        _context.Students.Remove(student);
        _context.SaveChanges();
        _logger.LogInformation("Student deleted");
        return student;
    }
    
    
}
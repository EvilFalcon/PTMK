namespace EmployeeDirectory;

public interface IEmployeeRepository
{
    void CreateTable();
    void Add(Employee employee);
    List<Employee> GetAllEmployees();
    void PopulateDatabase(int count, int maleCountWithFLastName = 0);
    List<Employee> GetEmployeesByCriteria(string gender, string lastNameStartsWith);
}
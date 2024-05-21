using Npgsql;

namespace EmployeeDirectory;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly string _connectionString;

    public EmployeeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void CreateTable()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command =
                   new NpgsqlCommand(
                       "CREATE TABLE IF NOT EXISTS employees (id SERIAL PRIMARY KEY, fullname VARCHAR(255) NOT NULL, date_of_birth DATE NOT NULL, gender VARCHAR(10) NOT NULL)",
                       connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public void Add(Employee employee)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            employee.SaveToDatabase(connection);
        }
    }

    public List<Employee> GetAllEmployees()
    {
        List<Employee> employees = new List<Employee>();
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command =
                   new NpgsqlCommand(
                       "SELECT fullname, date_of_birth, gender FROM employees ORDER BY fullname, date_of_birth",
                       connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee(
                            reader.GetString(0),
                            reader.GetDateTime(1),
                            reader.GetString(2)
                        ));
                    }
                }
            }
        }

        return employees;
    }

    public void PopulateDatabase(int count, int maleCountWithFLastName = 0)
    {
        List<Employee> employees = new List<Employee>();
        Random random = new Random();

        for (int i = 0; i < count; i++)
        {
            string firstName = GenerateRandomName();
            string lastName = GenerateRandomName();
            string fullName = $"{lastName} {firstName}";
            string gender = random.Next(2) == 0 ? "Male" : "Female";
            DateTime dateOfBirth = GenerateRandomDateOfBirth();
            employees.Add(new Employee(fullName, dateOfBirth, gender));
        }

        for (int i = 0; i < maleCountWithFLastName; i++)
        {
            string firstName = GenerateRandomName();
            string lastName = $"F{GenerateRandomName().Substring(1)}";
            string fullName = $"{lastName} {firstName}";
            DateTime dateOfBirth = GenerateRandomDateOfBirth();
            employees.Add(new Employee(fullName, dateOfBirth, "Male"));
        }

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            Employee.SaveToDatabase(connection, employees);
        }
    }

    public List<Employee> GetEmployeesByCriteria(string gender, string lastNameStartsWith)
    {
        List<Employee> employees = new List<Employee>();
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command =
                   new NpgsqlCommand(
                       "SELECT fullname, date_of_birth, gender FROM employees WHERE gender = @gender AND fullname LIKE @lastNameStartsWith",
                       connection))
            {
                command.Parameters.AddWithValue("@gender", gender);
                command.Parameters.AddWithValue("@lastNameStartsWith", $"{lastNameStartsWith}%");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        (
                            reader.GetString(0),
                            reader.GetDateTime(1),
                            reader.GetString(2)
                        ));
                    }
                }
            }
        }

        return employees;
    }

    private static string GenerateRandomName()
    {
        Random random = new Random();
        string[] names =
        {
            "John", "Jane", "David", "Mary", "Peter", "Susan", "Michael", "Elizabeth", "William", "Margaret",
            "Richard", "Jennifer", "Charles", "Linda", "Joseph", "Barbara"
        };
        return names[random.Next(names.Length)];
    }

    private static DateTime GenerateRandomDateOfBirth()
    {
        Random random = new Random();
        int year = random.Next(1950, 2005);
        int month = random.Next(1, 13);
        int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);
        return new DateTime(year, month, day);
    }
}

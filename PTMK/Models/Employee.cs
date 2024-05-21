using Npgsql;

namespace EmployeeDirectory;

public class Employee
{
    public Employee(string fullName, DateTime dateOfBirth, string gender)
    {
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
    }

    public string FullName { get; }
    public DateTime DateOfBirth { get; }
    public string Gender { get;}

    // Метод для расчета возраста
    public int CalculateAge()
    {
        return DateTime.Now.Year - DateOfBirth.Year - (DateTime.Now.Month < DateOfBirth.Month ||
                                                       (DateTime.Now.Month == DateOfBirth.Month &&
                                                        DateTime.Now.Day < DateOfBirth.Day)
            ? 1
            : 0);
    }

    // Метод для сохранения сотрудника в базу данных
    public void SaveToDatabase(NpgsqlConnection connection)
    {
        using (var command =
               new NpgsqlCommand(
                   "INSERT INTO employees (fullname, date_of_birth, gender) VALUES (@fullname, @date_of_birth, @gender)",
                   connection))
        {
            command.Parameters.AddWithValue("@fullname", FullName);
            command.Parameters.AddWithValue("@date_of_birth", DateOfBirth);
            command.Parameters.AddWithValue("@gender", Gender);
            command.ExecuteNonQuery();
        }
    }

    // Метод для пакетной отправки данных в БД
    public static void SaveToDatabase(NpgsqlConnection connection, List<Employee> employees)
    {
        using (var transaction = connection.BeginTransaction())
        {
            using (var command =
                   new NpgsqlCommand(
                       "INSERT INTO employees (fullname, date_of_birth, gender) VALUES (@fullname, @date_of_birth, @gender)",
                       connection))
            {
                foreach (var employee in employees)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@fullname", employee.FullName);
                    command.Parameters.AddWithValue("@date_of_birth", employee.DateOfBirth);
                    command.Parameters.AddWithValue("@gender", employee.Gender);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }
    }
}
using System.Diagnostics;

namespace EmployeeDirectory;

public class EmployeeDirectoryApp
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeDirectoryApp(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public void Run(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Необходимо указать режим работы приложения.");
            return;
        }

        int mode = int.Parse(args[0]);

        switch (mode)
        {
            case 1:
                _employeeRepository.CreateTable();
                Console.WriteLine("Таблица сотрудников создана.");
                break;
            case 2:
                if (args.Length != 4)
                {
                    Console.WriteLine("Некорректные аргументы для создания записи.");
                    return;
                }

                string fullName = args[1];

                if (DateTime.TryParse(args[2], out DateTime dateOfBirth) == false)
                {
                    return;
                }

                string gender = args[3];
                _employeeRepository.Add(new Employee
                (
                    fullName,
                    dateOfBirth,
                    gender
                ));
                break;
            case 3:
                var employees = _employeeRepository.GetAllEmployees();
                foreach (var employee in employees)
                {
                    Console.WriteLine($"ФИО: {employee.FullName}");
                    Console.WriteLine($"Дата рождения: {employee.DateOfBirth}");
                    Console.WriteLine($"Пол: {employee.Gender}");
                    Console.WriteLine($"Возраст: {employee.CalculateAge()}");
                    Console.WriteLine("--------------------");
                }

                break;
            case 4:
                _employeeRepository.PopulateDatabase(1000000, 100);
                break;
            case 5:
                Stopwatch stopwatch = Stopwatch.StartNew();
                var filteredEmployees = _employeeRepository.GetEmployeesByCriteria("Male", "F");
                stopwatch.Stop();
                Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
                foreach (var employee in filteredEmployees)
                {
                    Console.WriteLine($"ФИО: {employee.FullName}");
                    Console.WriteLine($"Дата рождения: {employee.DateOfBirth}");
                    Console.WriteLine($"Пол: {employee.Gender}");
                    Console.WriteLine("--------------------");
                }
                break;
            default:
                Console.WriteLine("Неверный режим работы приложения.");
                break;
        }
    }
}
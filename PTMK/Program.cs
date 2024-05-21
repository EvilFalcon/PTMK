namespace EmployeeDirectory
{
    class Program
    {
        static string ConnectionString =
            "Server=localhost;Database=postgres1;Username=postgres;Password=123456789;";

        static void Main()
        {
            string[] args = ["1", "Sokolov Maxim Ilish", "27.02.1998", "Male"];
            IEmployeeRepository employeeRepository = new EmployeeRepository(ConnectionString);
            EmployeeDirectoryApp app = new EmployeeDirectoryApp(employeeRepository);
            app.Run(args);
        }
    }
}
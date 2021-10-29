namespace SoftUni
{
    using SoftUni.Data;
    using SoftUni.Models;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext db = new SoftUniContext();
            //change only method
            string result = AddNewAddressToEmployee(db);

            Console.WriteLine(result);

        }
        //3.Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
            {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .OrderBy(e=>e.EmployeeId)
                .Select(e=> new {e.FirstName,e.LastName, e.MiddleName, e.JobTitle,e.Salary})
             .ToList();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} {emp.MiddleName} {emp.JobTitle} {emp.Salary:F2}");
            }

            return sb.ToString().Trim();

            }

        //4.	Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employee = context.Employees
                .OrderBy(x => x.FirstName)
                .Where(x => x.Salary > 50000)
                .ToList();

            foreach (var e in employee)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:F2}");
            }

            return sb.ToString().Trim();
        }


        //5.	Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(s => s.Salary)
                .ThenByDescending(f => f.FirstName)
                .Select(e => new { e.FirstName, e.LastName, DepartmentName=e.Department.Name, e.Salary })
                .ToList();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:F2}");
            }

            return sb.ToString().Trim();
        }


        //6.	Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employee = context.Employees
                .FirstOrDefault(e => e.LastName == "Nakov");

            employee.Address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4,
            };

            context.SaveChanges();

            var addresses = context.Addresses
                .OrderByDescending(e => e.AddressId)
                .Select(e=>e.AddressText)
                .Take(10)
                .ToList();


            foreach (var a in addresses)
            {
                sb.AppendLine(a);

             }

            return sb.ToString().Trim();
        }

        //7.	Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.EmployeesProjects
                .Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                .Select(e => new { e.FirstName,
                    e.LastName, 
                    managerFirstName = e.Manager.FirstName,
                    managerLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects.Select(ep => ep.Project)})
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.managerFirstName} {e.managerLastName}");

                foreach (var p in e.Projects)
                {

                }
            }
        }
    }
}


namespace SoftUni
{
    using SoftUni.Data;
    using SoftUni.Models;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext db = new SoftUniContext();
            //change only method
            string result = RemoveTown(db);

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
                    string startDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US"));
                    string endDate = p.EndDate is null
                        ? "not finished"
                        : p.EndDate?.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US"));

                    sb.AppendLine($"--{p.Name} - {startDate} - {endDate}");
                }
            }

            return sb.ToString().Trim();
        }


        //8.	Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .Select(e => new { e.AddressText, townName = e.Town.Name, employeeCount = e.Employees.Count })
                .OrderByDescending(e => e.employeeCount)
                .ThenBy(e => e.townName)
                .ThenBy(e => e.AddressText)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in addresses)
            {
                sb.AppendLine($"{e.AddressText}, {e.townName} - {e.employeeCount} employees");
            }

            return sb.ToString().Trim();
        }

        //9.	Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees.FirstOrDefault(e => e.EmployeeId == 147);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            var projects = context.EmployeesProjects
                .Where(e => e.EmployeeId == 147)
                .Select(e => e.Project)
                .OrderBy(e => e.Name)
                .ToList();

            foreach (var e in projects)
            {
                sb.AppendLine($"{e.Name}");
            }

            return sb.ToString().Trim();
        }


        //10.	Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var department = context.Departments
                .Where(e=> e.Employees.Count > 5)
                .OrderBy(e => e.Employees.Count)
                .ThenBy(d=>d.Name)
                .Select(d  => new { d.Name,d.Manager,d.Employees })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var d in department)
            {
                sb.AppendLine($"{d.Name} - {d.Manager.FirstName} {d.Manager.LastName}");

                var orderedEmployees = context.Employees
                   .Select(e => new { e.FirstName, e.LastName, e.JobTitle })
                   .OrderBy(e => e.FirstName)
                   .ThenBy(e => e.LastName)
                   .ToList();



                foreach (var e in orderedEmployees)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().Trim();
        }

        //11.	Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .Select(e => new { e.Name, e.Description, e.StartDate })
                .OrderByDescending(e => e.StartDate)
                .Take(10)
                .OrderBy(e => e.Name)
                .ToList();
            StringBuilder sb = new StringBuilder();

            foreach (var p in projects)
            {
                var formatDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                sb.AppendLine($"{p.Name}");
                sb.AppendLine($"{p.Description}");
                sb.AppendLine($"{formatDate}");
            }

            return sb.ToString().Trim();
        }

        //12.	Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employee = context.Employees
                .Where(e => e.Department.Name == "Engineering" ||
                e.Department.Name == "Tool Design"
                || e.Department.Name == "Marketing"
                || e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            employee.ForEach(e => e.Salary *= 1.12m);

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employee)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }

            return sb.ToString().Trim();

        }


        //13.	Find Employees by First Name Starting with "Sa"
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new { e.FirstName, e.LastName, e.JobTitle, e.Salary })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return sb.ToString().Trim();
        }

        //14.	Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var employeeProject = context.EmployeesProjects
                .Where(e => e.ProjectId == 2);


            context.EmployeesProjects.RemoveRange(employeeProject);

            var project = context.Projects.Find(2);

            context.Projects.Remove(project);

            context.SaveChanges();

            var projects = context.Projects
            .Take(10)
            .ToList();


            StringBuilder sb = new StringBuilder();


            foreach (var p in projects)
            {
                sb.AppendLine($"{p.Name}");
            }

            return sb.ToString().Trim();

        }

        //15.	Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            //Seattle id is 4

            var employeesOfTown = context.Employees
                .Where(e => e.Address.Town.Name == "Seattle")
                .ToList();

            employeesOfTown.ForEach(e => e.AddressId = null);

            var addresses = context.Addresses
                .Where(e => e.Town.Name == "Seattle")
                .ToList();

            context.RemoveRange(addresses);

            var towns = context.Towns
                .FirstOrDefault(t => t.Name == "Seattle");

            context.Remove(towns);

            context.SaveChanges();

            return $"{addresses.Count()} addresses in Seattle were deleted";
                
        }
    }
}


namespace SoftUni
{
    using SoftUni.Data;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext db = new SoftUniContext();
            //change only method
            string result = GetEmployeesWithSalaryOver50000(db);

            Console.WriteLine(result);

        }
            //3.Employees Full Information
            public static string GetEmployeesFullInformation(SoftUniContext context)
            {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .OrderBy(e=>e.EmployeeId)
                .Select(e=> new {e.FirstName,e.MiddleName,e.LastName,e.JobTitle,e.Salary})
             .ToList();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.MiddleName} {emp.LastName} {emp.JobTitle} {emp.Salary:F2}");
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
                sb.AppendLine($"{e.FirstName} {e.Salary:F2}");
            }

            return sb.ToString().Trim();
        }
    }
}


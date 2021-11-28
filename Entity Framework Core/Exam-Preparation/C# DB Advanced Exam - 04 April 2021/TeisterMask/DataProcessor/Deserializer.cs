namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectXmlImportModel[]),
                new XmlRootAttribute("Projects"));


            var projectsDtos = (ProjectXmlImportModel[])xmlSerializer.Deserialize(new StringReader(xmlString));
            List<Project> projects = new List<Project>();

            foreach (var projectDto in projectsDtos)
            {
                if (!IsValid(projectDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime openDate;

                bool isValidOpenDate = DateTime.TryParseExact(projectDto.OpenDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,DateTimeStyles.None, out openDate);//out DateTime openDate works too

                if (!isValidOpenDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? dueDate = null;

                if (!String.IsNullOrWhiteSpace(projectDto.DueDate))
                {
                    DateTime dueDateDto;

                    bool isDueDateValid = DateTime.TryParseExact(projectDto.DueDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDateDto);


                    if (!isDueDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    //if valid set it 
                    dueDate = dueDateDto;
                }


                var p = new Project
                {
                    Name= projectDto.Name,
                    OpenDate = openDate,
                    DueDate = dueDate
                };

                foreach (var taskDto in projectDto.Tasks)
                {
                    if (!IsValid(taskDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isOpenDateTaskValid = DateTime.TryParseExact(taskDto.OpenDate, "dd/MM/yyyy",
                   CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime openDateTask);


                    if (!isOpenDateTaskValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isDueDateTaskValid = DateTime.TryParseExact(taskDto.DueDate, "dd/MM/yyyy",
                   CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDateTask);


                    if (!isDueDateTaskValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (openDate < openDateTask)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (dueDate.HasValue && dueDateTask > dueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Task t = new Task
                    {
                        Name = taskDto.Name,
                        OpenDate = openDateTask,
                        DueDate = dueDateTask,
                        ExecutionType =(ExecutionType)taskDto.ExecutionType,//cast it if its int in properties type
                        LabelType =(LabelType)taskDto.LabelType,
                    };
                    p.Tasks.Add(t);

                }
                projects.Add(p);
                sb.AppendLine(String.Format(SuccessfullyImportedProject, p.Name, p.Tasks.Count));
            }
            context.Projects.AddRange(projects);
            context.SaveChanges();


            return sb.ToString().Trim();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {




            return "TODO";
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
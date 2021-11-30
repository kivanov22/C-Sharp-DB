namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportDepartmentCellDto[] departmentsDtos = JsonConvert.DeserializeObject<ImportDepartmentCellDto[]>(jsonString);

            List<Department> departments = new List<Department>();

            foreach (var departmentDto in departmentsDtos)
            {
                if (!IsValid(departmentDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                //if (String.IsNullOrWhiteSpace(departmentDto.Name))
                //{
                //    sb.AppendLine("Invalid Data");
                //    continue;
                //}
                var d = new Department
                {
                    Name = departmentDto.Name,
                };

                bool isCellValid = true;

                foreach (var cellDto in departmentDto.Cells)
                {
                    if (!IsValid(cellDto))
                    {
                        isCellValid = false;
                        break;
                    }
                    d.Cells.Add(new Cell
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow
                    });

                }

                if (!isCellValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (d.Cells.Count == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                departments.Add(d);
                sb.AppendLine($"Imported {d.Name} with {d.Cells.Count} cells");
            }
            context.Departments.AddRange(departments);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportPrisonerMailDto[] prisonerDtos = JsonConvert.DeserializeObject<ImportPrisonerMailDto[]>(jsonString);

            List<Prisoner> prisoners = new List<Prisoner>();

            foreach (var prisonerDto in prisonerDtos)
            {
                if (!IsValid(prisonerDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                DateTime incarcerationDate;

                bool isIncarcerationDatevalid = DateTime
                    .TryParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out incarcerationDate);


                if (!isIncarcerationDatevalid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                DateTime? releaseDate = null;

                if (!String.IsNullOrEmpty(prisonerDto.ReleaseDate))
                {
                    DateTime releaseDateCheck;

                    bool isReleaseDateValid = DateTime
                        .TryParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDateCheck);

                    if (!isReleaseDateValid)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }
                    releaseDate = releaseDateCheck;
                }

                var p = new Prisoner()
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    Bail = prisonerDto.Bail,
                    CellId = prisonerDto.CellId,
                };

                bool isValidMail = true;

                foreach (var mailDto in prisonerDto.Mails)
                {
                    if (!IsValid(mailDto))
                    {
                        isValidMail = false;
                        continue;
                    }

                    p.Mails.Add(new Mail()
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address,
                    });
                }

                if (!isValidMail)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                prisoners.Add(p);
                sb.AppendLine($"Imported {p.FullName} {p.Age} years old");
            }
            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        //public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        //{
        //    var sb = new StringBuilder();

        //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportOfficerAndPrisonerDto[]),
        //        new XmlRootAttribute("Officers"));

        //    var officersDtos = (ImportOfficerAndPrisonerDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

        //    List<Officer> officers = new List<Officer>();

        //    foreach (var officerDto in officersDtos)
        //    {
        //        if (!IsValid(officerDto))
        //        {
        //            sb.AppendLine("Invalid Data");
        //            continue;
        //        }

        //        object positionObj;

        //        bool isPositionValid = Enum.TryParse(typeof(Position), officerDto.Position, out positionObj);

        //        if (!isPositionValid)
        //        {
        //            sb.AppendLine("Invalid Data");
        //            continue;
        //        }

        //        object weaponObj;

        //        bool isWeaponValid = Enum.TryParse(typeof(Weapon), officerDto.Weapon, out weaponObj);

        //        if (!isWeaponValid)
        //        {
        //            sb.AppendLine("Invalid Data");
        //            continue;
        //        }

        //        var o = new Officer()
        //        {
        //            FullName = officerDto.Name,
        //            Salary = officerDto.Money,
        //            Position = (Position)positionObj,
        //            Weapon = (Weapon)weaponObj,
        //            DepartmentId = officerDto.DepartmentId,
        //        };


        //        foreach (var prisonerDto in officerDto.Prisoners)
        //        {
        //            o.OfficerPrisoners.Add(new OfficerPrisoner
        //            {
        //                Officer = o,
        //                PrisonerId = prisonerDto.PrisonerId
        //            });
        //        }

        //        officers.Add(o);
        //        sb.AppendLine($"Imported {o.FullName} ({o.OfficerPrisoners.Count} prisoners)");
        //    }
        //    context.Officers.AddRange(officers);
        //    context.SaveChanges();

        //    return sb.ToString().Trim();
        //}

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}
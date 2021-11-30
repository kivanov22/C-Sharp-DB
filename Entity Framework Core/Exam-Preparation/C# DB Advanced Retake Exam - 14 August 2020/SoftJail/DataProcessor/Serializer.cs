namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context
                .Prisoners
                .ToArray()
                .Where(x => ids.Contains(x.Id))
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers
                    .Select(o => new
                    {
                        OfficerName = o.Officer.FullName,
                        Department = o.Officer.Department.Name,

                    })
                    .OrderBy(x => x.OfficerName)
                    .ToArray(),
                    TotalOfficerSalary = Math.Round(p.PrisonerOfficers.Sum(s => s.Officer.Salary),2)
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToArray();



            return JsonConvert.SerializeObject(prisoners,Formatting.Indented);
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportPrisonersInboxDto[]), new XmlRootAttribute("Prisoners"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter sw = new StringWriter(sb);

            string[] formatNames = prisonersNames.Split(",").ToArray();


            var prisoners = context.Prisoners
                .ToArray()
                .Where(x => formatNames.Contains(x.FullName))
                .Select(p => new ExportPrisonersInboxDto()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Mails = p.Mails.Select(m => new ExportEncryptedMessageDto()
                    {
                        Description = String.Join("", m.Description.Reverse())
                    })
                  .ToArray()
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToArray();

            xmlSerializer.Serialize(sw, prisoners, namespaces);

            return sb.ToString().Trim();
        }
    }
}
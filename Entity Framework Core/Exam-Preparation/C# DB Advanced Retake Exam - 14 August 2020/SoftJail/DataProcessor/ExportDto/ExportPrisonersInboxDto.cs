﻿using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ExportDto
{
    [XmlType("Prisoner")]

    public class ExportPrisonersInboxDto
    {
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("IncarcerationDate")]
        public string IncarcerationDate { get; set; }

        [XmlArray("EncryptedMessages")]
        public  ExportEncryptedMessageDto[] Mails { get; set; }
    }
}

namespace FuMilltime2.DataBase
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using FuMilltime2.Controls.ProjectInput;

    public class LogFileDataStorage : IFuMilltimeDataSource
    {
        private static string CreateFileName(DateTime date)
        {
            return string.Format("FuMilltime-{0:yyMMdd}.log", date);
        }

        public IReadOnlyList<TimeRecord> GetRecords(DateTime date)
        {
            try
            {
                var data = File.ReadAllText(CreateFileName(date));
                var root = XElement.Parse(data);

                var result = new List<TimeRecord>();

                var customerProjects = root.Element("customerProjects");
                var commonProjects = root.Element("commonProjects");

                foreach (var record in customerProjects.Elements("record"))
                {
                    result.Add(new TimeRecord{Name = record.Element("name").Value, TimeStamp = record.Element("timeStamp").Value});
                }

                foreach (var record in commonProjects.Elements("record"))
                {
                    result.Add(new TimeRecord{Common = true, Name = record.Element("name").Value, TimeStamp = record.Element("timeStamp").Value});
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SaveRecords(DateTime date, IReadOnlyList<TimeRecord> data)
        {
            var logFileName = CreateFileName(date);

            var root = new XElement("projects");

            var customerProjects = new XElement("customerProjects");
            var commonProjects = new XElement("commonProjects");

            root.Add(customerProjects);
            root.Add(commonProjects);

            foreach (var projectInfo in data)
            {
                if (string.IsNullOrWhiteSpace(projectInfo.Name) && string.IsNullOrWhiteSpace(projectInfo.TimeStamp))
                {
                    continue;
                }

                var record = new XElement("record");
                record.Add(new XElement("name") { Value = projectInfo.Name});
                record.Add(new XElement("timeStamp") { Value = projectInfo.TimeStamp });
                if (projectInfo.Common)
                {
                    commonProjects.Add(record);
                }
                else
                {
                    customerProjects.Add(record);
                }
            }

            File.WriteAllText(logFileName, root.ToString());
        }
    }
}

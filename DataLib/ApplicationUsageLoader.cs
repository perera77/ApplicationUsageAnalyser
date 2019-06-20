using CsvHelper;
using CsvHelper.Configuration;
using ModelLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    /// <summary>
    /// Application Usage Data loading utility
    /// Use CsvHelper NuGet package to parse the CSV file
    /// </summary>
    public class ApplicationUsageLoader
    {
        static public IEnumerable<ApplicationUsageModel> LoadFile(string csvFilePath)
        {
            try
            {
                using (CsvReader csv = new CsvReader(File.OpenText(csvFilePath)))
                {                    
                    var records = new List<ApplicationUsageModel>();
                    csv.Read();
                    csv.ReadHeader();
                    
                    while (csv.Read())
                    {
                        var record = new ApplicationUsageModel
                        {
                            ComputerID = csv.GetField<int>("ComputerID"),
                            UserID = csv.GetField<int>("UserID"),
                            ApplicationID = csv.GetField<int>("ApplicationID"),
                            ComputerType = csv.GetField("ComputerType").Equals(ComputerType.DESKTOP.ToString(), StringComparison.CurrentCultureIgnoreCase) ? ComputerType.DESKTOP : ComputerType.LAPTOP
                        };

                        records.Add(record);
                    }
                    return records;
                }
            }
            catch (Exception)
            {
                throw new Exception("Unable to load the data file!");
            }
        }
    }
}

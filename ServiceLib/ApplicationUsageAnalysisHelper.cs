using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib
{
    /// <summary>
    /// Application Usage Analysis Helper
    /// Implements licence counting businss logic.
    /// </summary>
    public class ApplicationUsageAnalysisHelper
    {
        private Action<int> ProressUpdate = null;

        public ApplicationUsageAnalysisHelper(Action<int> progressUpdate)
        {
            ProressUpdate = progressUpdate;
        }

        /// <summary>
        /// Analyze usag data and returns licence requiremnts for individual applications.
        /// </summary>
        /// <param name="usageData"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUsageAnalysisModel> AnalyzeUsageData(IEnumerable<ApplicationUsageModel> usageData)
        {
            var analysedRecords = new List<ApplicationUsageAnalysisModel>();

            //group usage data on application id
            var appGroups = usageData.GroupBy(x => x.ApplicationID);
            int progressCount = 0;
            foreach (var appGroup in appGroups)
            {
                if (ProressUpdate != null)
                {
                    ProressUpdate.Invoke(progressCount++ * 100 / appGroups.Count());
                }

                // Remove duplicate entries - delayd cleanup after grouping to minimise key lookups
                var records = new Dictionary<Tuple<int, int>, ApplicationUsageModel>(); //clean usage records of the application
                foreach (var x in appGroup)
                {
                    try
                    {
                        records.Add(Tuple.Create(x.ComputerID, x.UserID), x);
                    }
                    catch (Exception)
                    { } //ignore duplicats
                }

                var appUsage = records.Values.ToList();
                ApplicationUsageAnalysisModel record = new ApplicationUsageAnalysisModel
                {
                    ApplicationID = appGroup.Key,
                    Desktops = appUsage.Where(r => r.ComputerType == ComputerType.DESKTOP).Count(),
                    Laptops = appUsage.Where(r => r.ComputerType == ComputerType.LAPTOP).Count()
                };

                var userUsagList = appUsage.GroupBy(r => r.UserID);
                record.Users = userUsagList.Count();

                // Licence counting business logic
                //      Each copy of the application (ID 374) allows the user to install the
                //      application on to two computers if at least one of them is a laptop.
                int requiredLicences = 0;
                foreach (var userUsage in userUsagList)
                {
                    int desktops = userUsage.Where(r => r.ComputerType == ComputerType.DESKTOP).Count();
                    int laptops = userUsage.Count() - desktops;
                    if (desktops >= laptops)
                        requiredLicences += desktops;
                    else
                        requiredLicences += (desktops + (laptops + 1 - desktops) / 2);

                }
                record.RequiredCopies = requiredLicences;
                analysedRecords.Add(record);

            }
            return analysedRecords;
        }
    }
}

using DataLib;
using ModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib
{
    /// <summary>
    /// Licence usage analysis servic.
    /// </summary>
    public class ApplicationUsageAnalysisService
    {
        private int progress = 0;
        private string serviceStatus;

        public ApplicationUsageAnalysisService()
        {  }

        /// <summary>
        /// Property: Progress of usag analysis %
        /// </summary>
        public int Progress => progress;

        /// <summary>
        /// Property: The task the servic performing
        /// </summary>
        public string Status => serviceStatus;


        private void updateProgressCallBack(int update)
        {
            progress = update;
        }
        
        /// <summary>
        /// Ayncronous usage data analysing function
        /// </summary>
        /// <param name="filePath">Input: CSV usage data file path</param>
        /// <returns></returns>
        public async Task<IEnumerable<ApplicationUsageAnalysisModel>> AnalyseDataFileAsync(string filePath)
        {
            // loading data asyncronously
            serviceStatus = "Data loading";
            progress = 0;
            var dataLoadTask = Task.Run(() => loadUsageData(filePath));
            var usageData = await dataLoadTask;

            // Initializing the data analysis helpr passing progress report callback action
            ApplicationUsageAnalysisHelper helper = new ApplicationUsageAnalysisHelper(updateProgressCallBack);
            var task = Task.Run(() => helper.AnalyzeUsageData(usageData));
            serviceStatus = "Analysing usage";
            return await task;
        }

       
        static private IEnumerable<ApplicationUsageModel> loadUsageData(string filePath)
        {
            return ApplicationUsageLoader.LoadFile(filePath);
        }
    }
}

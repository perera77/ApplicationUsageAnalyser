using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLib;
using ServiceLib;

namespace UnitTestProject
{
    [TestClass]
    public class ApplicationUsageAnalysisHelperUnitTest
    {
        [TestMethod]
        public void TestExample1()
        {
            var usageData = new List<ApplicationUsageModel>(new ApplicationUsageModel[]
            {
                new ApplicationUsageModel {ComputerID=1, UserID=1, ApplicationID=374, ComputerType= ComputerType.LAPTOP},
                new ApplicationUsageModel {ComputerID=2, UserID=1, ApplicationID=374, ComputerType= ComputerType.DESKTOP }
            });

            var helper = new ApplicationUsageAnalysisHelper(null);
            var results = helper.AnalyzeUsageData(usageData);
            Assert.AreEqual(1, results.Count(), "Only one application is analysing");
            Assert.AreEqual(1, results.First().RequiredCopies, "Only one copy of the application is required as the user has installed it on two computers, " +
                                                "with one of them being a laptop");
        }

        [TestMethod]
        public void TestExample2()
        {
            var usageData = new List<ApplicationUsageModel>(new ApplicationUsageModel[]
            {
                new ApplicationUsageModel {ComputerID=1, UserID=1, ApplicationID=374, ComputerType= ComputerType.LAPTOP},
                new ApplicationUsageModel {ComputerID=2, UserID=1, ApplicationID=374, ComputerType= ComputerType.DESKTOP},
                new ApplicationUsageModel {ComputerID=3, UserID=2, ApplicationID=374, ComputerType= ComputerType.DESKTOP},
                new ApplicationUsageModel {ComputerID=4, UserID=2, ApplicationID=374, ComputerType= ComputerType.DESKTOP }
            });

            var helper = new ApplicationUsageAnalysisHelper(null);
            var results = helper.AnalyzeUsageData(usageData);
            Assert.AreEqual(1, results.Count(), "Only one application is analysing");
            Assert.AreEqual(3, results.First().RequiredCopies, "Three copies of the application are required as UserID 2 has installed the application on two computers" +
                                                "but neither of them is a laptop and thus both computers require a purchase of the application");
        }

        [TestMethod]
        public void TestExample3()
        {
            var usageData = new List<ApplicationUsageModel>(new ApplicationUsageModel[]
            {
                new ApplicationUsageModel {ComputerID=1, UserID=1, ApplicationID=374, ComputerType= ComputerType.LAPTOP},
                new ApplicationUsageModel {ComputerID=2, UserID=2, ApplicationID=374, ComputerType= ComputerType.DESKTOP},
                new ApplicationUsageModel {ComputerID=2, UserID=2, ApplicationID=374, ComputerType= ComputerType.DESKTOP}
            });

            var helper = new ApplicationUsageAnalysisHelper(null);
            var results = helper.AnalyzeUsageData(usageData);
            Assert.AreEqual(1, results.Count(), "Only one application is analysing");
            Assert.AreEqual(1, results.First().Desktops, "Only one desktop as the data from the second and third rows are duplicates");
            Assert.AreEqual(2, results.First().RequiredCopies, "Only two copies of the application are required as the data from the second and third rows are duplicates");
        }

        [TestMethod]
        public void TestMultipleApplications()
        {
            var usageData = new List<ApplicationUsageModel>(new ApplicationUsageModel[]
            {
                new ApplicationUsageModel {ComputerID=1, UserID=1, ApplicationID=374, ComputerType= ComputerType.LAPTOP},
                new ApplicationUsageModel {ComputerID=2, UserID=2, ApplicationID=374, ComputerType= ComputerType.DESKTOP},
                new ApplicationUsageModel {ComputerID=3, UserID=2, ApplicationID=374, ComputerType= ComputerType.DESKTOP},

                new ApplicationUsageModel {ComputerID=4, UserID=1, ApplicationID=375, ComputerType= ComputerType.LAPTOP},
                new ApplicationUsageModel {ComputerID=5, UserID=1, ApplicationID=375, ComputerType= ComputerType.LAPTOP},

                new ApplicationUsageModel {ComputerID=1, UserID=1, ApplicationID=376, ComputerType= ComputerType.LAPTOP},
                new ApplicationUsageModel {ComputerID=2, UserID=2, ApplicationID=376, ComputerType= ComputerType.DESKTOP},
                new ApplicationUsageModel {ComputerID=3, UserID=2, ApplicationID=376, ComputerType= ComputerType.LAPTOP}
            });

            var helper = new ApplicationUsageAnalysisHelper(null);
            var results = helper.AnalyzeUsageData(usageData).ToList();
            Assert.AreEqual(3, results.Count(), "Three applications ar analysing");
            Assert.AreEqual(2, results[0].Desktops, "Two desktops using 374");
            Assert.AreEqual(3, results[0].RequiredCopies, "Three copies of 374 required, user 2 need two licencess to use on two desktops");

            Assert.AreEqual(2, results[1].Laptops, "Two laptops using 375");
            Assert.AreEqual(1, results[1].RequiredCopies, "Only one coppy of 375 required, sam user using two laptops");

            Assert.AreEqual(2, results[2].Laptops, "Two laptops using 376");
            Assert.AreEqual(1, results[2].Desktops, "One desktop using 376");
            Assert.AreEqual(2, results[2].Users, "Two users of 376");
            Assert.AreEqual(2, results[2].RequiredCopies, "Two copies of 376 required, one licence is enoungh for usr 2");
        }

        [TestMethod]
        public void TestMultipleApplicationsWithDuplicates()
        {
            var usageData = new List<ApplicationUsageModel>(new ApplicationUsageModel[]
            {
                new ApplicationUsageModel {ComputerID=1, UserID=1, ApplicationID=374, ComputerType= ComputerType.LAPTOP},
                new ApplicationUsageModel {ComputerID=2, UserID=2, ApplicationID=374, ComputerType= ComputerType.DESKTOP},
                new ApplicationUsageModel {ComputerID=2, UserID=2, ApplicationID=374, ComputerType= ComputerType.DESKTOP},
                new ApplicationUsageModel {ComputerID=2, UserID=2, ApplicationID=374, ComputerType= ComputerType.DESKTOP},
                new ApplicationUsageModel {ComputerID=3, UserID=2, ApplicationID=374, ComputerType= ComputerType.DESKTOP},

                new ApplicationUsageModel {ComputerID=4, UserID=1, ApplicationID=375, ComputerType= ComputerType.LAPTOP},
                new ApplicationUsageModel {ComputerID=5, UserID=1, ApplicationID=375, ComputerType= ComputerType.LAPTOP},
                new ApplicationUsageModel {ComputerID=5, UserID=1, ApplicationID=375, ComputerType= ComputerType.LAPTOP},

                new ApplicationUsageModel {ComputerID=1, UserID=1, ApplicationID=376, ComputerType= ComputerType.LAPTOP},
                new ApplicationUsageModel {ComputerID=2, UserID=2, ApplicationID=376, ComputerType= ComputerType.DESKTOP},
                new ApplicationUsageModel {ComputerID=1, UserID=1, ApplicationID=376, ComputerType= ComputerType.LAPTOP},
                new ApplicationUsageModel {ComputerID=2, UserID=2, ApplicationID=376, ComputerType= ComputerType.DESKTOP},
                new ApplicationUsageModel {ComputerID=3, UserID=2, ApplicationID=376, ComputerType= ComputerType.LAPTOP}
            });

            var helper = new ApplicationUsageAnalysisHelper(null);
            var results = helper.AnalyzeUsageData(usageData).ToList();
            Assert.AreEqual(3, results.Count(), "Three applications ar analysing");
            Assert.AreEqual(2, results[0].Desktops, "Two desktops using 374");
            Assert.AreEqual(3, results[0].RequiredCopies, "Three copies of 374 required, user 2 need two licencess to use on two desktops");

            Assert.AreEqual(2, results[1].Laptops, "Two laptops using 375");
            Assert.AreEqual(1, results[1].RequiredCopies, "Only one coppy of 375 required, sam user using two laptops");

            Assert.AreEqual(2, results[2].Laptops, "Two laptops using 376");
            Assert.AreEqual(1, results[2].Desktops, "One desktop using 376");
            Assert.AreEqual(2, results[2].Users, "Two users of 376");
            Assert.AreEqual(2, results[2].RequiredCopies, "Two copies of 376 required, one licence is enoungh for usr 2");
        }
    }
}

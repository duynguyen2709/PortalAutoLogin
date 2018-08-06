using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace SeleniumTest
{
    [TestClass]
    public class UnitTest1
    {
        static IWebDriver driverGC;

        [AssemblyInitialize]
        public static void SetUp(TestContext context) => driverGC = new ChromeDriver();
        
        [TestMethod]
        public void TestMethod1()
        {
        driverGC.Navigate().GoToUrl("https://www.google.com.vn/");
        driverGC.FindElement(By.Id("lst-ib")).SendKeys("HCMUS");
        driverGC.FindElement(By.Id("lst-ib")).SendKeys(Keys.Enter);
        driverGC.Quit();
        }

        


    }
}

using System;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;

namespace SFA.DAS.ForecastingTool.Web.BrowserTests.Driver
{
    //
    // Summary:
    //     Defines driver interface through which the user controls the browser.
    public interface IEmfWebDriver : IDisposable
    {
        //
        // Summary:
        //      Get or Set the RemoteWebdriver instance.
        RemoteWebDriver webDriver { get; set; }
        //
        // Summary:
        //      Get runtime browser instance name.
        string BrowserName { get; }
        //
        // Summary:
        //     Load a new web page in the current browser window.
        //
        // Parameters:
        //   url:
        //     The URL to load. It is best to use a fully qualified URL
        void GoToURL(string url);
        //
        // Summary:
        //      Wait until default timeout in seconds for the current document readyState to be completed.
        void WaitUntilDocIsReady();

        //
        // Summary:
        //      Wait until default timeout in seconds for the element to be displayed and enabled.
        //      On Timeout raise WebDriver Timeout Exception.
        void WaitforElementTobeDisplayedAndEnabled(IWebElement webelement);

        //
        // Summary:
        //     Clicks this element.
        //Remarks:
        //      Click this element. Before clicking, this method will wait until default timeout in seconds or
        //      the element is Displayed and Enabled.
        void Click(IWebElement webelement);
        
        //
        // Summary:
        //     Gets a OpenQA.Selenium.Screenshot object representing the image of the page on
        //     the screen.
        //
        // Parameters:
        //     fileName:
        //      Screenshot FileName. It is best to use full path 
        void TakeScreenshot(string fileName);
    }
}

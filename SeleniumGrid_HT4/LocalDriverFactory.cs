using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace SeleniumGrid_HT4;

public class LocalDriverFactory
{
	public static IWebDriver CreateInstance(Tests.BrowserType browserType)
        {
            IWebDriver driver = null;
 
            switch (browserType)
            {
                case Tests.BrowserType.ChromeLocal:
                case Tests.BrowserType.Chrome:
                    driver = new ChromeDriver();
                    break;
                case Tests.BrowserType.Edge:
                    var options = new EdgeOptions();
                    driver = new EdgeDriver(options);
                    break;
                case Tests.BrowserType.Firefox:
                    driver = new FirefoxDriver();
                    break;
                case Tests.BrowserType.Opera:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(browserType), browserType, null);
            }
 
            return driver;
        }
 
        public static IWebDriver CreateInstance(Tests.BrowserType browserType, string hubUrl)
        {
            IWebDriver driver = null;
            TimeSpan timeSpan = new TimeSpan(0, 3, 0);
 
            switch (browserType)
            {
                case Tests.BrowserType.Chrome:
                    ChromeOptions chromeOptions = new ChromeOptions();
                    driver = GetWebDriver(hubUrl, chromeOptions.ToCapabilities());
                    break;
                case Tests.BrowserType.Edge:
                    EdgeOptions options = new EdgeOptions();
                    driver = GetWebDriver(hubUrl, options.ToCapabilities());
                    break;
                case Tests.BrowserType.Firefox:
                    FirefoxOptions firefoxOptions = new FirefoxOptions();
                    driver = GetWebDriver(hubUrl, firefoxOptions.ToCapabilities());
                    break;
            }
 
            return driver;
        }
 
        private static IWebDriver GetWebDriver(string hubUrl, ICapabilities capabilities)
        {
            TimeSpan timeSpan = new TimeSpan(0, 3, 0);
            return new RemoteWebDriver(
                        new Uri(hubUrl),
                        capabilities,
                        timeSpan
                    );
        }
}

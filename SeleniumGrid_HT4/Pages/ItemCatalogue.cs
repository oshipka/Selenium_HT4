using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;

namespace SeleniumGrid_HT4;

public class ItemCatalogue
{
	private IWebDriver _driver;
	private WebDriverWait _wait;

	private Dictionary<string, string> Categories = new()
	{
		{ "Pots", @"https://rozetka.com.ua/ua/prigotovlenie-pishi/c4626599/" },
		{ "Candles", @"https://rozetka.com.ua/ua/cvechi/c4636135/" },
		{ "Dogfood", @"https://rozetka.com.ua/ua/food_for_dogs/c1461202/" },
		{ "AnimalToys", @"https://rozetka.com.ua/ua/toys_for_pets/c3439219/" },
		{ "Webcams", @"https://rozetka.com.ua/ua/web_cameras/c180143/" },
		{ "Monitors", @"https://hard.rozetka.com.ua/ua/monitors/c80089/" },
	};
	
	public ItemCatalogue(IWebDriver driver)
	{
		_driver = driver;
		_wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
		PageFactory.InitElements(driver, this);
	}

	public void GoToPage(string key)
	{
		if (!Categories.ContainsKey(key))
		{
			key = "Pots";
		}

		_driver.Navigate().GoToUrl(Categories[key]);
	}

	public void GoToPage(int key)
	{
		if (Categories.Count < key)
		{
			key = Categories.Count - 1;
		}

		_driver.Navigate().GoToUrl(Categories[Categories.Keys.ToList()[key]]);
	}
	
	
	[CacheLookup]
	[FindsBy(How.XPath, @"//app-buy-button/button[not(contains(@class, 'in-cart'))]")]
	public IList<IWebElement> AddToCartButtons;

	[CacheLookup]
	[FindsBy(How.XPath, @"//app-buy-button/button[not(contains(@class, 'in-cart'))]/../../../../a[contains(@class, 'heading')]")]
	public IList<IWebElement> ItemNames;
	
	
	[FindsBy(How.XPath, @"//rz-cart/button")]
	public IWebElement CartBtn;

	public void ClosePromo()
	{
		_wait.Timeout = TimeSpan.FromSeconds(20);
		var button = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(@"//span[contains(@class, 'exponea-close ')]")));
		button.Click();
		_wait.Timeout = TimeSpan.FromSeconds(10);
	}

	public bool AddToCartIsVisible(int numberOnPage)
	{
		var actionBuilder = new Actions(_driver);
		actionBuilder.MoveToElement(AddToCartButtons[numberOnPage], 5, 5).Perform();
		return AddToCartButtons[numberOnPage].Displayed;
	}

	public void ClickAddToCartBtn(int numberOnPage)
	{
		var actionBuilder = new Actions(_driver);
		actionBuilder.MoveToElement(AddToCartButtons[numberOnPage], 5, 5).Perform();
		AddToCartButtons[numberOnPage].Click();
	}

	public string GetName(int numberOnPage)
	{
		return ItemNames[numberOnPage].GetAttribute("title");
	}

	public void ClickCartBtn()
	{
		CartBtn.Click();
	}

	public void WaitForCart(bool waitForItems)
	{
		if (_driver.Url == @"https://rozetka.com.ua/ua/cart/")
		{
			var el1 = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(@"//h1[contains(@class, 'heading')]")));
			if (waitForItems)
			{
				var el = _wait.Until(
					ExpectedConditions.ElementIsVisible(By.XPath(@"//a[@class='cart-product__title']")));
			}
		}

		else

		{
			var el = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(@"//h3[@class='modal__heading']")));

			if (waitForItems)
			{
				var el2 = _wait.Until(
					ExpectedConditions.ElementIsVisible(By.XPath(@"//a[@class='cart-product__title']")));
			}
		}
	}
}

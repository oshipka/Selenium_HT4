using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;

namespace SeleniumGrid_HT4;

public class CartModal
{
	private IWebDriver _driver;
	private WebDriverWait _wait;


	public CartModal(IWebDriver driver)
	{
		_driver = driver;
		_wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
		PageFactory.InitElements(driver, this);
	}
	
	
	[FindsBy(How.XPath, @"//rz-cart-counter/div/input")]
	public IList<IWebElement> InputQuantities;

	[FindsBy(How.XPath, @"//p[contains(@class, 'product__price')]")]
	public IList<IWebElement> Prices;

	[FindsBy(How.XPath, @"//a[contains(@class, 'product__title')]")]
	public IList<IWebElement> ItemNames;

	[FindsBy(How.XPath, @"//div[@class='cart-receipt__sum-price']/span[1]")]
	public IWebElement TotalDisplayed;

	[FindsBy(How.XPath, @"//rz-popup-menu/button")]
	public IList<IWebElement> DotsMenu;
	
	
	public string GetName(int numberInCart)
	{
		return ItemNames[numberInCart].GetAttribute("innerHTML");
	}

	public void InputQuantity(int inputNumber, int numberToInput)
	{
		var action = new Actions(_driver);
		
		action.MoveToElement(InputQuantities[inputNumber]).DoubleClick().SendKeys(numberToInput.ToString()).Build().Perform();
		Thread.Sleep(1000);
		PageFactory.InitElements(_driver, this);
		
	}

	public List<int> GetPrices()
	{
		return (Prices.Select(price => price.GetAttribute("innerHTML"))
			.Select(ih => ih.Split("<")[0].Replace("&nbsp;", ""))
			.Select(nstr => int.Parse(nstr))
			).ToList();
	}

	public int CalculateTotalPrice(int[] quantities)
	{
		var prices = GetPrices();
		if (prices.Count != quantities.Length)
		{
			Assert.Fail();
		}

		var iMax = prices.Count;
		var total = 0;
		for (var i = 0; i < iMax; i++)
		{
			total += prices[i] * quantities[i];
		}

		return total;
	}

	public int GetDisplayedTotal()
	{
		return int.Parse(TotalDisplayed.GetAttribute("innerHTML").Split(" ")[0]);
	}

	public void RemoveItemFromCart(int itemNumber)
	{
		DotsMenu[itemNumber].Click();
		_wait.Until(ExpectedConditions.ElementToBeClickable( By.XPath(@"//rz-trash-icon/button"))).Click();
		Thread.Sleep(1000);
		PageFactory.InitElements(_driver, this);
	}

}

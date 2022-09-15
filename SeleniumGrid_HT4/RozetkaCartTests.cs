using OpenQA.Selenium;

namespace SeleniumGrid_HT4;

[Parallelizable(ParallelScope.Fixtures)]
//[TestFixture(BrowserType.ChromeLocal)]
[TestFixture(BrowserType.Chrome)]
[TestFixture(BrowserType.Edge)]
[TestFixture(BrowserType.Firefox)]
public class Tests
{
	public enum BrowserType
	{
		Chrome, Edge, Firefox,
		Opera, ChromeLocal
	}

	public Tests(BrowserType browser)
	{
		_browserTypeForTheRun = browser;
	}

	private IWebDriver _driver;
	private IDictionary<string, object> Vars { get; set; }
	private IJavaScriptExecutor _js;
	private readonly BrowserType _browserTypeForTheRun;
	string _hubUrl = @"http://localhost:4444";

	[SetUp]
	public void Setup()
	{
		Vars = new Dictionary<string, object>();
		_driver = _browserTypeForTheRun == BrowserType.ChromeLocal ?
			LocalDriverFactory.CreateInstance(_browserTypeForTheRun) :
			LocalDriverFactory.CreateInstance(_browserTypeForTheRun, _hubUrl);
		_js = (IJavaScriptExecutor)_driver;
		_driver.Manage().Window.Maximize();
	}

	[TearDown]
	protected void TearDown()
	{
		_driver.Quit();
	}

	[Test]
	[TestCase(0, new[] { 0 })]
	[TestCase(1, new[] { 1, 2 })]
	[TestCase(3, new[] { 0, 1, 3, 4 })]
	public void CanAddItemToCart(int category, int[] onPageNumber)
	{
		var catalogue = new ItemCatalogue(_driver);
		catalogue.GoToPage(category);
		var names = new List<string>();
		catalogue.ClosePromo();

		foreach (var i in onPageNumber)
		{
			Assert.That(catalogue.AddToCartIsVisible(i));
			catalogue.ClickAddToCartBtn(i);
			names.Add(catalogue.GetName(i));
		}

		catalogue.ClickCartBtn();
		catalogue.WaitForCart(true);
		var cart = new CartModal(_driver);
		Assert.That(onPageNumber, Has.Length.EqualTo(cart.ItemNames.Count));
		names.Reverse();

		for (var i = 0; i < names.Count; i++)
		{
			Assert.That(cart.GetName(i), Does.Contain(names[i]));
		}
	}

	[Test]
	[TestCase(0, new[] { 0 }, new[] { 0 })]
	[TestCase(1, new[] { 1, 2 }, new[] { 1 })]
	[TestCase(3, new[] { 0, 1, 3, 4 }, new[] { 1, 1, 0 })]
	public void CanRemoveItemFromCart(int category, int[] onPageNumber, int[] itemsToRemove)
	{
		var catalogue = new ItemCatalogue(_driver);
		catalogue.GoToPage(category);
		var names = new List<string>();
		catalogue.ClosePromo();

		foreach (var i in onPageNumber)
		{
			Assert.That(catalogue.AddToCartIsVisible(i));
			catalogue.ClickAddToCartBtn(i);
			names.Add(catalogue.GetName(i));
		}

		catalogue.ClickCartBtn();
		catalogue.WaitForCart(true);
		var cart = new CartModal(_driver);

		foreach (var t in itemsToRemove)
		{
			cart.RemoveItemFromCart(t);
		}

		Assert.That(cart.ItemNames, Has.Count.EqualTo(names.Count - itemsToRemove.Length));
	}

	[Test]
	[TestCase(0, new[] { 0 }, new[] { 5 })]
	[TestCase(5, new[] { 1, 2 }, new[] { 3, 6 })]
	[TestCase(3, new[] { 0, 1, 3, 4 }, new[] { 2, 1, 3, 1 })]
	public void CanChangeItemCountInCart(int category, int[] onPageNumber, int[] quantitiesToEnter)
	{
		var catalogue = new ItemCatalogue(_driver);
		catalogue.GoToPage(category);
		catalogue.ClosePromo();

		foreach (var i in onPageNumber)
		{
			Assert.That(catalogue.AddToCartIsVisible(i));
			catalogue.ClickAddToCartBtn(i);
		}

		catalogue.ClickCartBtn();
		catalogue.WaitForCart(true);
		var cart = new CartModal(_driver);
		var oldPrices = cart.GetPrices();

		for (var i = 0; i < quantitiesToEnter.Length; i++)
		{
			cart.InputQuantity(i, quantitiesToEnter[i]);
		}

		var newPrices = cart.GetPrices();
		var expectedPrices = new List<int>();

		for (int i = 0; i < quantitiesToEnter.Length; i++)
		{
			expectedPrices.Add(oldPrices[i] * quantitiesToEnter[i]);
		}
		
		Assert.That(newPrices, Is.EqualTo(expectedPrices));
	}

	[Test]
	public void CanOpenCartModal()
	{
		var catalogue = new ItemCatalogue(_driver);
		catalogue.GoToPage("Monitors");
		catalogue.ClosePromo();
		catalogue.ClickCartBtn();
		catalogue.WaitForCart(false);
	}
}

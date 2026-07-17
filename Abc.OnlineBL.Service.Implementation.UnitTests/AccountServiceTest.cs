using Abc.AIS.Service.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Abc.AIS.Entities;

namespace Abc.AIS.Service.Implementation.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for AccountServiceTest and is intended
    ///to contain all AccountServiceTest Unit Tests
    ///</summary>
	[TestClass()]
	public class AccountServiceTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for GetCommOrderInvoice
		///</summary>
		[TestMethod()]
		public void GetCommOrderInvoiceTest()
		{
			//AccountService target = new AccountService(); 
			//int cusId = 760;
			//int invID = 364156; 
			//CommOrder actual;
			//actual = target.GetCommOrderInvoice(cusId, invID);
			//Assert.AreEqual(975.61, actual.Due);
		}
	}
}

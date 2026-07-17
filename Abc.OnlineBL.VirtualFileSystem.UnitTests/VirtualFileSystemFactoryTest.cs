using Abc.AIS.VirtualFileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Abc.AIS.VirtualFileSystem.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for VirtualFileSystemFactoryTest and is intended
    ///to contain all VirtualFileSystemFactoryTest Unit Tests
    ///</summary>
	[TestClass()]
	public class VirtualFileSystemFactoryTest
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
		///A test for GetFile
		///</summary>
		[TestMethod()]
		public void GetFileTest()
		{
			IFile actual;
			actual = VirtualFileSystemFactory.GetFile();

			Assert.IsNotNull(actual, "IFile Returned Null");

			bool fileFound = actual.Exists(@"\\192.168.1.2\test\Document.txt");

			Assert.IsTrue(fileFound, @"IFile Returned File '\\192.168.1.2\test\Document.txt' Not Found");
		}		
	}
}

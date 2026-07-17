using Abc.AIS.VirtualFileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Abc.AIS.VirtualFileSystem.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for LocalFileProviderTest and is intended
    ///to contain all LocalFileProviderTest Unit Tests
    ///</summary>
	[TestClass()]
	public class LocalFileProviderTest
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
		///A test for WriteAllText
		///</summary>
		[TestMethod()]
		public void WriteAllTextTest()
		{
			LocalFileProvider target = new LocalFileProvider(); // TODO: Initialize to an appropriate value
			string filePath = string.Empty; // TODO: Initialize to an appropriate value
			string data = string.Empty; // TODO: Initialize to an appropriate value
			target.WriteAllText(filePath, data);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for WriteAllBytes
		///</summary>
		[TestMethod()]
		public void WriteAllBytesTest()
		{
			LocalFileProvider target = new LocalFileProvider(); // TODO: Initialize to an appropriate value
			string filePath = string.Empty; // TODO: Initialize to an appropriate value
			byte[] data = null; // TODO: Initialize to an appropriate value
			target.WriteAllBytes(filePath, data);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for ReadAllText
		///</summary>
		[TestMethod()]
		public void ReadAllTextTest()
		{
			LocalFileProvider target = new LocalFileProvider(); // TODO: Initialize to an appropriate value
			string filePath = string.Empty; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.ReadAllText(filePath);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ReadAllBytes
		///</summary>
		[TestMethod()]
		public void ReadAllBytesTest()
		{
			LocalFileProvider target = new LocalFileProvider(); // TODO: Initialize to an appropriate value
			string filePath = string.Empty; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = target.ReadAllBytes(filePath);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Exists
		///</summary>
		[TestMethod()]
		public void ExistsTest()
		{
			LocalFileProvider target = new LocalFileProvider(); // TODO: Initialize to an appropriate value
			string filePath = string.Empty; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = target.Exists(filePath);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Delete
		///</summary>
		[TestMethod()]
		public void DeleteTest()
		{
			LocalFileProvider target = new LocalFileProvider(); // TODO: Initialize to an appropriate value
			string filePath = string.Empty; // TODO: Initialize to an appropriate value
			target.Delete(filePath);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Copy
		///</summary>
		[TestMethod()]
		public void CopyTest()
		{
			LocalFileProvider target = new LocalFileProvider(); // TODO: Initialize to an appropriate value
			string srcPath = string.Empty; // TODO: Initialize to an appropriate value
			string destPath = string.Empty; // TODO: Initialize to an appropriate value
			bool overwrite = false; // TODO: Initialize to an appropriate value
			target.Copy(srcPath, destPath, overwrite);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for LocalFileProvider Constructor
		///</summary>
		[TestMethod()]
		public void LocalFileProviderConstructorTest()
		{
			LocalFileProvider target = new LocalFileProvider();
			Assert.Inconclusive("TODO: Implement code to verify target");
		}
	}
}

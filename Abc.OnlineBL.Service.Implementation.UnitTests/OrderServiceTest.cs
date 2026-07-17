using Abc.AIS.Service.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Abc.AIS.Entities.Enums;
using Abc.AIS.Entities.Model;
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Abc.AIS.Service.Implementation.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for OrderServiceTest and is intended
    ///to contain all OrderServiceTest Unit Tests
    ///</summary>
	[TestClass()]
	public class OrderServiceTest
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
		/// Upload the RED_PROPERTY_PHOTO.
		/// </summary>
		[TestMethod()]
		public void UploadPhotoTest_RED_PROPERTY_PHOTO()
		{
			OrderService target = new OrderService();
			string originalFileName = "Sunset.jpg";

			string imageFile = GetImageFileFullPath(originalFileName);

			if (!File.Exists(imageFile))
			{
				Assert.Fail("Photo Image is not available to test");
				return;
			}

			string ext = Path.GetExtension(imageFile).ToLower();
			string uncFilePath = Path.Combine(Properties.Settings.Default.TEMPORARY_PHOTO_OUTPUT_FOLDER, Guid.NewGuid().ToString("N") + ext);

			File.Copy(imageFile, uncFilePath);

			List<UploadPhotoRequest> reqs = new List<UploadPhotoRequest>();
			UploadPhotoRequest req = new UploadPhotoRequest();
			req.UncFilePath = uncFilePath;
			req.FileName = originalFileName;
			req.FileSelectMode = UploadedFileType.PropertyPhoto;
			req.AgentContactName = String.Empty;
			reqs.Add(req);

			int orderId = 14722;
			OrderTrackingEventParameter ev = new OrderTrackingEventParameter();
			ev.OrderId = orderId;
			ev.LoggedBy = Environment.UserDomainName + "\\" + Environment.UserName + " - Outlook Image Extractor";
			ev.Message = "Finished Extracting Images";
			ev.AllItemsSelected = true;


			List<UploadPhotoResponse> actuals;
			actuals = target.UploadPhoto(reqs, ev);
			Assert.AreEqual(UploadedFileQualityType.RED, actuals[0].Quality);
			Assert.IsTrue(actuals[0].FileProcesed);
			Assert.IsTrue(File.Exists(Path.Combine(AISConfig.RED_PROPERTY_PHOTO_OUTPUT_FOLDER, string.Format("{0}_{1}", orderId, originalFileName))));
		}

		/// <summary>
		///Upload the GREEN_PROPERTY_PHOTO.
		///</summary>
		[TestMethod()]
		public void UploadPhotoTest_GREEN_PROPERTY_PHOTO()
		{
			OrderService target = new OrderService();
			string originalFileName = "Green.jpg";

			string imageFile = GetImageFileFullPath(originalFileName);

			if (!File.Exists(imageFile))
			{
				Assert.Fail("Photo Image is not available to test");
				return;
			}

			string ext = Path.GetExtension(imageFile).ToLower();
			string uncFilePath = Path.Combine(Properties.Settings.Default.TEMPORARY_PHOTO_OUTPUT_FOLDER, Guid.NewGuid().ToString("N") + ext);

			File.Copy(imageFile, uncFilePath);

			List<UploadPhotoRequest> reqs = new List<UploadPhotoRequest>();
			UploadPhotoRequest req = new UploadPhotoRequest();
			req.UncFilePath = uncFilePath;
			req.FileName = originalFileName;
			req.FileSelectMode = UploadedFileType.PropertyPhoto;
			req.AgentContactName = String.Empty;
			reqs.Add(req);

			int orderId = 14722;
			OrderTrackingEventParameter ev = new OrderTrackingEventParameter();
			ev.OrderId = orderId;
			ev.LoggedBy = Environment.UserDomainName + "\\" + Environment.UserName + " - Outlook Image Extractor";
			ev.Message = "Finished Extracting Images";
			ev.AllItemsSelected = true;

			List<UploadPhotoResponse> actuals;
			actuals = target.UploadPhoto(reqs, ev);
			Assert.AreEqual(UploadedFileQualityType.GREEN, actuals[0].Quality);
			Assert.IsTrue(actuals[0].FileProcesed);
			Assert.IsTrue(File.Exists(Path.Combine(AISConfig.GREEN_PROPERTY_PHOTO_OUTPUT_FOLDER, string.Format("{0}_{1}", orderId, originalFileName))));
		}

		/// <summary>
		/// Upload the RED_AGENT_PHOTO.
		/// </summary>
		[TestMethod()]
		public void UploadPhotoTest_RED_AGENT_PHOTO()
		{
			OrderService target = new OrderService();
			string originalFileName = "Sunset.jpg";

			string imageFile = GetImageFileFullPath(originalFileName);

			if (!File.Exists(imageFile))
			{
				Assert.Fail("Photo Image is not available to test");
				return;
			}

			string ext = Path.GetExtension(imageFile).ToLower();
			string uncFilePath = Path.Combine(Properties.Settings.Default.TEMPORARY_PHOTO_OUTPUT_FOLDER, Guid.NewGuid().ToString("N") + ext);

			File.Copy(imageFile, uncFilePath);

			List<UploadPhotoRequest> reqs = new List<UploadPhotoRequest>();
			UploadPhotoRequest req = new UploadPhotoRequest();
			req.UncFilePath = uncFilePath;
			req.FileName = originalFileName;
			req.FileSelectMode = UploadedFileType.AgentPhoto;
			req.AgentContactName = "TestTest";
			reqs.Add(req);

			int orderId = 14722;
			OrderTrackingEventParameter ev = new OrderTrackingEventParameter();
			ev.OrderId = orderId;
			ev.LoggedBy = Environment.UserDomainName + "\\" + Environment.UserName + " - Outlook Image Extractor";
			ev.Message = "Finished Extracting Images";
			ev.AllItemsSelected = true;

			List<UploadPhotoResponse> actuals;
			actuals = target.UploadPhoto(reqs, ev);
			Assert.AreEqual(UploadedFileQualityType.RED, actuals[0].Quality);
			Assert.IsTrue(actuals[0].FileProcesed);
		}

		/// <summary>
		///Upload the GREEN_AGENT_PHOTO.
		///</summary>
		[TestMethod()]
		public void UploadPhotoTest_GREEN_AGENT_PHOTO()
		{
			OrderService target = new OrderService();
			string originalFileName = "Green.jpg";

			string imageFile = GetImageFileFullPath(originalFileName);

			if (!File.Exists(imageFile))
			{
				Assert.Fail("Photo Image is not available to test");
				return;
			}

			string ext = Path.GetExtension(imageFile).ToLower();
			string uncFilePath = Path.Combine(Properties.Settings.Default.TEMPORARY_PHOTO_OUTPUT_FOLDER, Guid.NewGuid().ToString("N") + ext);

			File.Copy(imageFile, uncFilePath);
			List<UploadPhotoRequest> reqs = new List<UploadPhotoRequest>();
			UploadPhotoRequest req = new UploadPhotoRequest();
			req.UncFilePath = uncFilePath;
			req.FileName = originalFileName;
			req.FileSelectMode = UploadedFileType.AgentPhoto;
			req.AgentContactName = "TestTest";
			reqs.Add(req);

			int orderId = 14722;
			OrderTrackingEventParameter ev = new OrderTrackingEventParameter();
			ev.OrderId = orderId;
			ev.LoggedBy = Environment.UserDomainName + "\\" + Environment.UserName + " - Outlook Image Extractor";
			ev.Message = "Finished Extracting Images";
			ev.AllItemsSelected = true;

			List<UploadPhotoResponse> actuals;
			actuals = target.UploadPhoto(reqs, ev);
			Assert.AreEqual(UploadedFileQualityType.GREEN, actuals[0].Quality);
			Assert.IsTrue(actuals[0].FileProcesed);
		}

		/// <summary>
		///Upload the GRAPHICS_PHOTO.
		///</summary>
		[TestMethod()]
		public void UploadPhotoTest_GRAPHICS_PHOTO()
		{
			OrderService target = new OrderService();
			string originalFileName = "Green.jpg";

			string imageFile = GetImageFileFullPath(originalFileName);

			if (!File.Exists(imageFile))
			{
				Assert.Fail("Photo Image is not available to test");
				return;
			}

			string ext = Path.GetExtension(imageFile).ToLower();
			string uncFilePath = Path.Combine(Properties.Settings.Default.TEMPORARY_PHOTO_OUTPUT_FOLDER, Guid.NewGuid().ToString("N") + ext);

			File.Copy(imageFile, uncFilePath);
			List<UploadPhotoRequest> reqs = new List<UploadPhotoRequest>();
			UploadPhotoRequest req = new UploadPhotoRequest();
			req.UncFilePath = uncFilePath;
			req.FileName = originalFileName;
			req.FileSelectMode = UploadedFileType.GraphicsPhoto;
			req.AgentContactName = String.Empty;
			reqs.Add(req);

			int orderId = 14722;
			OrderTrackingEventParameter ev = new OrderTrackingEventParameter();
			ev.OrderId = orderId;
			ev.LoggedBy = Environment.UserDomainName + "\\" + Environment.UserName + " - Outlook Image Extractor";
			ev.Message = "Finished Extracting Images";
			ev.AllItemsSelected = true;

			List<UploadPhotoResponse> actuals;
			actuals = target.UploadPhoto(reqs, ev);
			Assert.AreEqual(UploadedFileQualityType.GRAPHICS, actuals[0].Quality);
			Assert.IsTrue(actuals[0].FileProcesed);
			Assert.IsTrue(File.Exists(Path.Combine(AISConfig.GRAPHICS_PROPERTY_PHOTO_OUTPUT_FOLDER, string.Format("{0}_{1}", orderId, originalFileName))));
		}

		/// <summary>
		///Upload the IMAGING_PHOTO.
		///</summary>
		[TestMethod()]
		public void UploadPhotoTest_IMAGING_PHOTO()
		{
			OrderService target = new OrderService();
			string originalFileName = "Sunset.jpg";

			string imageFile = GetImageFileFullPath(originalFileName);

			if (!File.Exists(imageFile))
			{
				Assert.Fail("Photo Image is not available to test");
				return;
			}

			string ext = Path.GetExtension(imageFile).ToLower();
			string uncFilePath = Path.Combine(Properties.Settings.Default.TEMPORARY_PHOTO_OUTPUT_FOLDER, Guid.NewGuid().ToString("N") + ext);

			File.Copy(imageFile, uncFilePath);
			List<UploadPhotoRequest> reqs = new List<UploadPhotoRequest>();
			UploadPhotoRequest req = new UploadPhotoRequest();
			req.UncFilePath = uncFilePath;
			req.FileName = originalFileName;
			req.FileSelectMode = UploadedFileType.ImagingPhoto;
			req.AgentContactName = String.Empty;
			reqs.Add(req);

			int orderId = 14722;
			OrderTrackingEventParameter ev = new OrderTrackingEventParameter();
			ev.OrderId = orderId;
			ev.LoggedBy = Environment.UserDomainName + "\\" + Environment.UserName + " - Outlook Image Extractor";
			ev.Message = "Finished Extracting Images";
			ev.AllItemsSelected = true;

			List<UploadPhotoResponse> actuals;
			actuals = target.UploadPhoto(reqs, ev);
			Assert.AreEqual(UploadedFileQualityType.IMAGING, actuals[0].Quality);
			Assert.IsTrue(actuals[0].FileProcesed);
			Assert.IsTrue(File.Exists(Path.Combine(AISConfig.IMAGING_PROPERTY_PHOTO_OUTPUT_FOLDER, string.Format("{0}_{1}", orderId, originalFileName))));
		}

		private static string GetImageFileFullPath(string originalFileName)
		{
			string currentPath = Path.GetDirectoryName(Environment.CurrentDirectory);
			string imageFile = currentPath.Substring(0, currentPath.LastIndexOf("TestResults")) + @"Abc.AIS.Service.Implementation.UnitTests\ImageTest\" + originalFileName;
			//string imageFile = Path.Combine(Assembly.GetExecutingAssembly().Location, originalFileName);
			return imageFile;

		}

	}
}

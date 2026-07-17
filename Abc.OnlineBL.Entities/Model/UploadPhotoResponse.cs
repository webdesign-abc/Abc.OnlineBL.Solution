using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities.Enums;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
	/// <summary>
	/// A Response class to be sent back to caller with the details of file processing
	/// </summary>
	[Serializable]
	[DataContract]
	public class UploadPhotoResponse
	{
		private UploadedFileQualityType quality;
		private bool fileProcesed;
		private string errorMessage;
		private string messageForAbc;
		private string messageForClients;
		private string fileName;

		/// <summary>
		/// Initializes a new instance of the <see cref="UploadPhotoResponse"/> class.
		/// </summary>
		public UploadPhotoResponse()
		{
		}

		[DataMember]
		public UploadedFileType FileSelectMode { get; set; }

		/// <summary>
		/// Gets or sets the error message.
		/// </summary>
		/// <value>The error message.</value>
		[DataMember]
		public string ErrorMessage
		{
			get { return errorMessage; }
			set { errorMessage = value; }
		}

		/// <summary>
		/// This property will contain the message that needs to displayed to ABC users. For e.g. if it was called
		/// by Abc Outlook Plugin then the UI will show this message
		/// </summary>
		/// <value>The message for abc.</value>
		[DataMember]
		public string MessageForAbc
		{
			get { return messageForAbc; }
			set { messageForAbc = value; }
		}

		/// <summary>
		/// Messages for Clients. i.e. display in Cmyabc web interface
		/// </summary>
		/// <value>The message for clients.</value>
		[DataMember]
		public string MessageForClients
		{
			get { return messageForClients; }
			set { messageForClients = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [file procesed].
		/// </summary>
		/// <value><c>true</c> if [file procesed]; otherwise, <c>false</c>.</value>
		[DataMember]
		public bool FileProcesed
		{
			get { return fileProcesed; }
			set { fileProcesed = value; }
		}

		/// <summary>
		/// Gets or sets the quality.
		/// </summary>
		/// <value>The quality and where the file went after processing</value>
		[DataMember]
		public UploadedFileQualityType Quality
		{
			get { return quality; }
			set { quality = value; }
		}

		[DataMember]
		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		[DataMember]
		public string Notes { get; set; }

		[DataMember]
		public bool ForBoards { get; set; }

		[DataMember]
		public bool ForBrochures { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Mail;

using log4net.Layout;
using log4net.Core;
using log4net.Util;
using System.Deployment.Application;
using System.Windows.Forms;
using System.Web;
using System.Reflection;

namespace Abc.OnlineBL.Utility
{
	/// <summary>
	/// AbcSmtpAppender is the custom appender of SmtpAppender.
	/// This class is exactly the same as log4net.Appender.AbcSmtpAppender.
	/// The reason this class is here because flourine lib was compiled with
	/// the original log4net.dll which was signed.
	/// </summary>
	public class AbcSmtpAppender : log4net.Appender.BufferingAppenderSkeleton
	{
		#region SmtpAuthentication Enum
		/// <summary>
		/// Values for the <see cref="AbcSmtpAppender.Authentication"/> property.
		/// </summary>
		/// <remarks>
		/// <para>
		/// SMTP authentication modes.
		/// </para>
		/// </remarks>
		public enum SmtpAuthentication
		{
			/// <summary>
			/// No authentication
			/// </summary>
			None,

			/// <summary>
			/// Basic authentication.
			/// </summary>
			/// <remarks>
			/// Requires a username and password to be supplied
			/// </remarks>
			Basic,

			/// <summary>
			/// Integrated authentication
			/// </summary>
			/// <remarks>
			/// Uses the Windows credentials from the current thread or process to authenticate.
			/// </remarks>
			Ntlm
		}
		#endregion // SmtpAuthentication Enum

		#region Private Instance Fields
		private string m_to;
		private string m_from;
		private string m_subject;
		private string m_smtpHost;

		// authentication fields
		private SmtpAuthentication m_authentication = SmtpAuthentication.None;
		private string m_username;
		private string m_password;

		// server port, default port 25
		private int m_port = 25;
		private MailPriority m_mailPriority = MailPriority.Normal;

		private string m_projectName;
		#endregion // Private Instance Fields

		#region Constructors
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <remarks>
		/// <para>
		/// Default constructor
		/// </para>
		/// </remarks>
		public AbcSmtpAppender() : base() { }
		#endregion // Public Instance Constructors

		#region Public Properties
		/// <summary>
		/// Gets or sets a semicolon-delimited list of recipient e-mail addresses.
		/// </summary>
		/// <value>
		/// A semicolon-delimited list of e-mail addresses.
		/// </value>
		/// <remarks>
		/// <para>
		/// A semicolon-delimited list of recipient e-mail addresses.
		/// </para>
		/// </remarks>
		public string To
		{
			get { return m_to; }
			set { m_to = value; }
		}

		/// <summary>
		/// Gets or sets the e-mail address of the sender.
		/// </summary>
		/// <value>
		/// The e-mail address of the sender.
		/// </value>
		/// <remarks>
		/// <para>
		/// The e-mail address of the sender.
		/// </para>
		/// </remarks>
		public string From
		{
			get { return m_from; }
			set { m_from = value; }
		}

		/// <summary>
		/// Gets or sets the subject line of the e-mail message.
		/// </summary>
		/// <value>
		/// The subject line of the e-mail message.
		/// </value>
		/// <remarks>
		/// <para>
		/// The subject line of the e-mail message.
		/// </para>
		/// </remarks>
		public string Subject
		{
			get { return m_subject; }
			set { m_subject = value; }
		}

		/// <summary>
		/// Gets or sets the name of the SMTP relay mail server to use to send 
		/// the e-mail messages.
		/// </summary>
		/// <value>
		/// The name of the e-mail relay server. If SmtpServer is not set, the 
		/// name of the local SMTP server is used.
		/// </value>
		/// <remarks>
		/// <para>
		/// The name of the e-mail relay server. If SmtpServer is not set, the 
		/// name of the local SMTP server is used.
		/// </para>
		/// </remarks>
		public string SmtpHost
		{
			get { return m_smtpHost; }
			set { m_smtpHost = value; }
		}

		/// <summary>
		/// Obsolete
		/// </summary>
		/// <remarks>
		/// Use the BufferingAppenderSkeleton Fix methods instead 
		/// </remarks>
		/// <remarks>
		/// <para>
		/// Obsolete property.
		/// </para>
		/// </remarks>
		[Obsolete("Use the BufferingAppenderSkeleton Fix methods")]
		public bool LocationInfo
		{
			get { return false; }
			set { ; }
		}

		/// <summary>
		/// The mode to use to authentication with the SMTP server
		/// </summary>
		/// <remarks>
		/// <note type="caution">Authentication is only available on the MS .NET 1.1 runtime.</note>
		/// <para>
		/// Valid Authentication mode values are: <see cref="SmtpAuthentication.None"/>, 
		/// <see cref="SmtpAuthentication.Basic"/>, and <see cref="SmtpAuthentication.Ntlm"/>. 
		/// The default value is <see cref="SmtpAuthentication.None"/>. When using 
		/// <see cref="SmtpAuthentication.Basic"/> you must specify the <see cref="Username"/> 
		/// and <see cref="Password"/> to use to authenticate.
		/// When using <see cref="SmtpAuthentication.Ntlm"/> the Windows credentials for the current
		/// thread, if impersonating, or the process will be used to authenticate. 
		/// </para>
		/// </remarks>
		public SmtpAuthentication Authentication
		{
			get { return m_authentication; }
			set { m_authentication = value; }
		}

		/// <summary>
		/// The username to use to authenticate with the SMTP server
		/// </summary>
		/// <remarks>
		/// <note type="caution">Authentication is only available on the MS .NET 1.1 runtime.</note>
		/// <para>
		/// A <see cref="Username"/> and <see cref="Password"/> must be specified when 
		/// <see cref="Authentication"/> is set to <see cref="SmtpAuthentication.Basic"/>, 
		/// otherwise the username will be ignored. 
		/// </para>
		/// </remarks>
		public string Username
		{
			get { return m_username; }
			set { m_username = value; }
		}

		/// <summary>
		/// The password to use to authenticate with the SMTP server
		/// </summary>
		/// <remarks>
		/// <note type="caution">Authentication is only available on the MS .NET 1.1 runtime.</note>
		/// <para>
		/// A <see cref="Username"/> and <see cref="Password"/> must be specified when 
		/// <see cref="Authentication"/> is set to <see cref="SmtpAuthentication.Basic"/>, 
		/// otherwise the password will be ignored. 
		/// </para>
		/// </remarks>
		public string Password
		{
			get { return m_password; }
			set { m_password = value; }
		}

		/// <summary>
		/// The port on which the SMTP server is listening
		/// </summary>
		/// <remarks>
		/// <note type="caution">Server Port is only available on the MS .NET 1.1 runtime.</note>
		/// <para>
		/// The port on which the SMTP server is listening. The default
		/// port is <c>25</c>. The Port can only be changed when running on
		/// the MS .NET 1.1 runtime.
		/// </para>
		/// </remarks>
		public int Port
		{
			get { return m_port; }
			set { m_port = value; }
		}

		/// <summary>
		/// Gets or sets the priority of the e-mail message
		/// </summary>
		/// <value>
		/// One of the <see cref="MailPriority"/> values.
		/// </value>
		/// <remarks>
		/// <para>
		/// Sets the priority of the e-mails generated by this
		/// appender. The default priority is <see cref="MailPriority.Normal"/>.
		/// </para>
		/// <para>
		/// If you are using this appender to report errors then
		/// you may want to set the priority to <see cref="MailPriority.High"/>.
		/// </para>
		/// </remarks>
		public MailPriority Priority
		{
			get { return m_mailPriority; }
			set { m_mailPriority = value; }
		}


		/// <summary>
		/// Gets or sets the name of the project.
		/// </summary>
		/// <value>The name of the project.</value>
		public string ProjectName
		{
			get { return m_projectName; }
			set { m_projectName = value; }
		}
		#endregion // Public Instance Properties

		#region Override implementation of AppenderSkeleton
		/// <summary>
		/// Gets a value indicating whether [requires layout].
		/// </summary>
		/// <value><c>true</c> if [requires layout]; otherwise, <c>false</c>.</value>
		protected override bool RequiresLayout
		{
			get { return false; }
		}
		#endregion // Override implementation of AppenderSkeleton

		#region SendBuffer
		/// <summary>
		/// Sends the contents of the cyclic buffer as an e-mail message.
		/// </summary>
		/// <param name="events">The logging events to send.</param>
		protected override void SendBuffer(LoggingEvent[] events)
		{
			// Note: this code already owns the monitor for this
			// appender. This frees us from needing to synchronize again.
			try
			{
				string content = string.Empty;
				foreach (LoggingEvent loggingEvent in events)
				{
					if (this.Evaluator.IsTriggeringEvent(loggingEvent))
					{
						SendEmail(loggingEvent, content);
						content = string.Empty;
					}
					else
					{
						content += GetAppInfoHtmlTable(loggingEvent);
						content += GetWebInfoHtmlTable();
						content += "<br /><br />";
					}
				}
			}
			catch (Exception e)
			{
				ErrorHandler.Error("Error occurred while sending e-mail notification.", e);
			}
		}
		#endregion

		#region SendEmail
		/// <summary>
		/// Sends the email.
		/// </summary>
		/// <param name="loggingEvent">The logging event.</param>
		/// <param name="attachmentContent">Content of the attachment.</param>
		protected virtual void SendEmail(LoggingEvent loggingEvent, string attachmentContent)
		{
			string html = GetHtmlDocument(loggingEvent);
			string file = string.Empty;

			if (!string.IsNullOrEmpty(attachmentContent))
			{
				file = Path.GetTempFileName();
				File.WriteAllText(file, GetHtmlDocument(attachmentContent));
			}

			try
			{
				if (File.Exists(file))
					SendEmail(html, file);
				else
					SendEmail(html);
			}
			catch (Exception ex)
			{
				ErrorHandler.Error("Error occurred while sending e-mail notification.", ex);
			}

			if (File.Exists(file))
				File.Delete(file);
		}

		/// <summary>
		/// Sends the email.
		/// </summary>
		/// <param name="messageBody">The message body.</param>
		protected virtual void SendEmail(string messageBody)
		{
			SendEmail(messageBody, null);
		}

		/// <summary>
		/// Send the email message
		/// </summary>
		/// <param name="messageBody">the body text to include in the mail</param>
		/// <param name="attachmentFile">The attachment file.</param>
		protected virtual void SendEmail(string messageBody, string attachmentFile)
		{
			// .NET 2.0 has a new API for SMTP email System.Net.Mail
			// This API supports credentials and multiple hosts correctly.
			// The old API is deprecated.

			// Create and configure the smtp client
			SmtpClient smtpClient = new SmtpClient();
			if (m_smtpHost != null && m_smtpHost.Length > 0)
			{
				smtpClient.Host = m_smtpHost;
			}
			smtpClient.Port = m_port;
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

			if (m_authentication == SmtpAuthentication.Basic)
			{
				// Perform basic authentication
				smtpClient.Credentials = new System.Net.NetworkCredential(m_username, m_password);
			}
			else if (m_authentication == SmtpAuthentication.Ntlm)
			{
				// Perform integrated authentication (NTLM)
				smtpClient.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
			}

			MailMessage mailMessage = new MailMessage();
			mailMessage.Body = messageBody;
			mailMessage.From = new MailAddress(m_from);
			mailMessage.To.Add(m_to);
			mailMessage.Subject = m_subject;
			mailMessage.Priority = m_mailPriority;
			mailMessage.IsBodyHtml = true;

			// TODO: Consider using SendAsync to send the message without blocking. This would be a change in
			// behaviour compared to .NET 1.x. We would need a SendCompletedCallback to log errors.
			if (attachmentFile != null && File.Exists(attachmentFile))
			{
				using (System.IO.FileStream fileStream = new System.IO.FileStream(attachmentFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					mailMessage.Attachments.Add(new Attachment(fileStream, "Log.html"));
					smtpClient.Send(mailMessage);
					fileStream.Close();
				}
			}
			else
			{
				smtpClient.Send(mailMessage);
			}
		}

		#endregion // Protected Methods

		#region GetHtmlDocument
		private string GetHtmlDocument(LoggingEvent loggingEvent)
		{
			string body = GetAppInfoHtmlTable(loggingEvent);
			body += GetWebInfoHtmlTable();
			return GetHtmlDocument(body);
		}

		private string GetHtmlDocument(string body)
		{
			string styles = " body {margin:0px; padding:0px;}";
			string tableClass = ".tableClass {width:100%;}";
			string headingClass = ".headingClass {padding:5px; text-align:right; background-color:#CACAFF; font-weight:bold; font-size:15px;}";
			string leftColClass = ".leftColClass {vertical-align:top; padding:5px; width:125px; background-color:#CACAFF; font-weight:bold; font-size:13px;}";
			string rightColClass = ".rightColClass {vertical-align:top; padding:5px; background-color:#E1E1FF; font-weight:normal; font-size:13px;}";

			string html = "<html>\n<head>\n<title>Error Log</title>\n";
			html += "<style type=\"text/css\">\n";
			html += string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n", tableClass, headingClass, leftColClass, rightColClass, styles);
			html += "</style>\n</head>\n";
			html += string.Format("<body>\n{0}\n</body>\n</html>", body);
			return html;
		}
		#endregion

		#region GetAppInfoHtmlTable
		private string GetAppInfoHtmlTable(LoggingEvent loggingEvent)
		{
			Dictionary<string, string> rows = new Dictionary<string, string>();
			string projectName = string.Empty, sub = string.Empty, user = string.Empty;

			if (HttpContext.Current != null)
			{
				if (HttpContext.Current.User.Identity.Name != null)
					user = HttpContext.Current.User.Identity.Name;

				projectName = HttpContext.Current.Request.ApplicationPath;
			}
			else
			{
				if (string.IsNullOrEmpty(m_projectName))
				{
					projectName = System.AppDomain.CurrentDomain.SetupInformation.ApplicationName;
					if (string.IsNullOrEmpty(projectName))
					{
						if (Assembly.GetEntryAssembly() != null)
						{
							projectName = Assembly.GetEntryAssembly().FullName;
						}
						else if (Assembly.GetCallingAssembly() != null)
						{
							projectName = Assembly.GetCallingAssembly().FullName;
						}
					}
				}
				else
				{
					projectName = m_projectName;
				}
			}

			if (string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(loggingEvent.UserName))
				user = loggingEvent.UserName;

			sub = String.Format("[{0}] [Project: {1}] [User: {2}]", loggingEvent.Level.DisplayName, projectName, user);
			//sub = String.Format("[{0}] [Project: {1}] [User: {2}]", loggingEvent.Level.DisplayName, "Abc OnlineBL Service", user);

			this.Subject = sub;

			rows.Add("DateTime", DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToShortTimeString());

			string version = string.Empty;
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				version = String.Format("v{0}.{1}.{2}", ApplicationDeployment.CurrentDeployment.CurrentVersion.Major,
				 ApplicationDeployment.CurrentDeployment.CurrentVersion.Minor,
				 ApplicationDeployment.CurrentDeployment.CurrentVersion.Revision);
			}
			else
			{
				version = String.Format("v{0}", Application.ProductVersion);
			}

			string machineCol1 = "Machine Name<br>Username<br>Application Name<br>Version<br>Memory Usage";
			string machineCol2 = String.Format("<b>{0}</b><br>{1}<br>{2}<br>{3}<br>{4} KB",
				System.Environment.MachineName, user, projectName, version, System.Environment.WorkingSet / 1024);

			rows.Add(machineCol1, machineCol2);
			rows.Add("Message Type", loggingEvent.Level.DisplayName);

			string redStyle = "style=\"color:#CC0000;\"";
			string greenStyle = "style=\"color:Green;\"";

			if (loggingEvent.ExceptionObject != null)
			{
				Exception ex = loggingEvent.ExceptionObject;
				if (!string.IsNullOrEmpty(ex.Source))
					rows.Add("Source", ex.Source);
				if (!string.IsNullOrEmpty(ex.Message))
					rows.Add("Message", string.Format("<div {0}><pre>{1}</pre></div>", redStyle, ex.Message));
				if (!string.IsNullOrEmpty(loggingEvent.RenderedMessage))
					rows.Add("Message Info", string.Format("<div {0}><pre>{1}</pre></div>", greenStyle, loggingEvent.RenderedMessage));
				if (ex.InnerException != null)
					rows.Add("Inner Exception", ex.InnerException.ToString());
				if (ex.TargetSite != null)
					rows.Add("Target Site", ex.TargetSite.ToString());
				rows.Add("ToString()", ex.ToString());
			}
			else
			{
				if (loggingEvent.Level >= Level.Error)
					rows.Add("Message Info", string.Format("<div {0}><pre>{1}<pre></div>", redStyle, loggingEvent.RenderedMessage));
				else
					rows.Add("Message Info", string.Format("<div {0}><pre>{1}</pre></div>", greenStyle, loggingEvent.RenderedMessage));
			}

			return CreateHtmlTable(rows, "AppInfo");
		}
		#endregion

		#region GetWebInfoHtmlTable
		private string GetWebInfoHtmlTable()
		{
			if (HttpContext.Current == null) return string.Empty;

			Dictionary<string, string> rows = new Dictionary<string, string>();

			if (HttpContext.Current.User.Identity.Name != null)
				rows.Add("User", HttpContext.Current.User.Identity.Name);

			rows.Add("App Path", HttpContext.Current.Request.ApplicationPath);

			if (HttpContext.Current.Request != null)
			{
				StringBuilder sb = new StringBuilder();
				System.Collections.Specialized.NameValueCollection col = HttpContext.Current.Request.Params;
				foreach (string key in col.Keys)
				{
					sb.AppendFormat("{0} = {1}<br/>\n", key, col[key]);
				}

				rows.Add("Request Details", sb.ToString());
			}

			return CreateHtmlTable(rows, "WebInfo");
		}
		#endregion

		#region CreateHtmlTable
		private string CreateHtmlTable(Dictionary<string, string> rows, string heading)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string key in rows.Keys)
			{
				sb.AppendFormat("<tr><td class=\"leftColClass\">{0}</td><td class=\"rightColClass\">{1}</td></tr>\n", key, rows[key]);
			}

			string headingRow = string.Format("<tr><td class=\"leftColClass\" style=\"color:#CACAFF;\">_________________<td class=\"headingClass\">{0}</td></tr>\n", heading);
			string table = String.Format("\n<table class=\"tableClass\" cellpadding=\"0\" cellspacing=\"2\">\n{0}\n{1}</table>\n", headingRow, sb.ToString());

			return table;
		}
		#endregion
	}
}

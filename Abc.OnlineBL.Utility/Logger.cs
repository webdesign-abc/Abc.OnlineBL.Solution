using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

[assembly: log4net.Config.XmlConfigurator(Watch = true, ConfigFile="log4net.config")]

namespace Abc.OnlineBL
{
	/// <summary>
	/// Main Logger Class
	/// </summary>
	public sealed class Logger
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Logger));
		private static bool IsInfoEnabled = true;
		private static bool IsWarnEnabled = true;
		private static bool IsDebugEnabled = true;
		private static bool IsErrorEnabled = true;
		private static bool IsFatalEnabled = true;

		public static TimeSpan ExecutionTime;
		public static int Counter;

		private Logger()
		{
			IsInfoEnabled = log.IsInfoEnabled;
			IsWarnEnabled = log.IsWarnEnabled;
			IsDebugEnabled = log.IsDebugEnabled;
			IsErrorEnabled = log.IsErrorEnabled;
			IsFatalEnabled = log.IsFatalEnabled;
		}

		public static void Info(string message)
		{
			if (IsInfoEnabled)
				log.Info(message);
		}

		public static void Info(string format, params object[] args)
		{
			if (IsInfoEnabled)
				log.InfoFormat(format, args);
		}

		public static void Warn(string message)
		{
			if (IsWarnEnabled)
				log.Warn(message);
		}
		public static void Warn(string format, params object[] args)
		{
			if (IsWarnEnabled)
				log.WarnFormat(format, args);
		}

		public static void Debug(string message)
		{
			if (IsDebugEnabled)
				log.Debug(message);
		}
		public static void Debug(string format, params object[] args)
		{
			if (IsDebugEnabled)
				log.DebugFormat(format, args);
		}

		public static void Error(string message)
		{
			if (IsErrorEnabled)
				log.Error(message);
		}
		public static void Error(string format, params object[] args)
		{
			if (IsErrorEnabled)
				log.ErrorFormat(format, args);
		}

		public static void Fatal(string message)
		{
			if (IsFatalEnabled)
				log.Fatal(message);
		}

		public static void Fatal(string format, params object[] args)
		{
			if (IsFatalEnabled)
				log.FatalFormat(format, args);
		}

		public static void Exception(Exception exception, string message)
		{
			if (IsErrorEnabled)
				log.Error(message, exception);
		}

		public static void Exception(Exception exception, string format, params object[] args)
		{
			if (IsErrorEnabled)
				log.Error(string.Format(format, args), exception);
		}

		public static void EmailException(string msg)
		{
			SmtpClient client = new SmtpClient();
			client.Send("errors@photosigns.com.au", "errors@photosigns.com.au", "Error", msg);
		}
	}
}

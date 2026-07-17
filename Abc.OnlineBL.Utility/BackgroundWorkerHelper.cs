using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Abc.OnlineBL.Utility
{
	/// <summary>
	/// A Helper class to use the background worker thread using lambda expressions
	/// </summary>
	/// <example>
	/// BackgroundWorkerHelper.DoWork<string>(() =>
	/// {	
	///    return myProxy.SayHello("My name");
	/// }, (args) =>
	/// {
	///    if (args.Error != null)
	///        richTextBox.AppendText("Returned Result from SayHello2: Exception:" + args.Error.ToString() + "\r\n");
	///    else
	///        richTextBox.AppendText("Returned Result from SayHello2: " + args.Result + "\r\n");
	/// });
	/// </example>
	public static class BackgroundWorkerHelper
	{
		/// <summary>
		/// Run the Provided Function using Background Worker
		/// </summary>
		/// <typeparam name="Tin">The type of the worker input argument.</typeparam>
		/// <typeparam name="Tout">The type of the output we are expecting from the long running op.</typeparam>
		/// <param name="inputArgument">The input argument.</param>
		/// <param name="doWork">The do work function which will be called by the background worker.</param>
		/// <param name="workerCompleted">The worker completed callback function.</param>
		public static void DoWork<Tin, Tout>(
				Tin inputArgument,
				Func<DoWorkArgument<Tin>, Tout> doWork,
				Action<WorkerResult<Tout>> workerCompleted)
		{
			BackgroundWorker bw = new BackgroundWorker();
			bw.WorkerReportsProgress = false;
			bw.WorkerSupportsCancellation = false;
			bw.DoWork += (sender, args) =>
			{
				if (doWork != null)
				{
					args.Result = doWork(new DoWorkArgument<Tin>((Tin)args.Argument));
				}
			};
			bw.RunWorkerCompleted += (sender, args) =>
			{
				if (workerCompleted != null)
				{
					Tout result = default(Tout);
					try
					{
						result = (Tout)args.Result;
					}
					catch (System.Reflection.TargetInvocationException)
					{
					}
					workerCompleted(new WorkerResult<Tout>(result, args.Error, args.Cancelled));
				}
			};
			bw.RunWorkerAsync(inputArgument);
		}

		/// <summary>
		/// Run the Provided Function using Background Worker
		/// </summary>
		/// <typeparam name="Tin">The type of the worker input argument.</typeparam>
		/// <typeparam name="Tout">The type of the output we are expecting from the long running op.</typeparam>
		/// <param name="inputArgument">The input argument.</param>
		/// <param name="doWork">The do work function which will be called by the background worker.</param>
		/// <param name="workerCompleted">The worker completed callback function.</param>
		/// <param name="progressChanged">The progress changed callback function.</param>
		public static void DoWork<Tin, Tout>(
				Tin inputArgument,
				Func<DoWorkArgument<Tin>, Tout> doWork,
				Action<WorkerResult<Tout>> workerCompleted,
			Action<ProgressChangedEventArgs> progressChanged)
		{
			BackgroundWorker bw = new BackgroundWorker();
			bw.WorkerReportsProgress = (progressChanged!=null);
			bw.WorkerSupportsCancellation = false;
			bw.DoWork += (sender, args) =>
			{
				if (doWork != null)
				{
					args.Result = doWork(new DoWorkArgument<Tin>((Tin)args.Argument));
				}
			};
			bw.RunWorkerCompleted += (sender, args) =>
			{
				if (workerCompleted != null)
				{
					Tout result = default(Tout);
					try
					{
						result = (Tout)args.Result;
					}
					catch (System.Reflection.TargetInvocationException)
					{
					}
					workerCompleted(new WorkerResult<Tout>(result, args.Error, args.Cancelled));
				}
			};
			bw.ProgressChanged += (sender, args) =>
			{
				if (progressChanged != null)
				{
					progressChanged(args);
				}
			};
			bw.RunWorkerAsync(inputArgument);
		}

		/// <summary>
		/// Run the Provided Function using Background Worker
		/// </summary>
		/// <typeparam name="Tout">The type of the output we are expecting from the long running op.</typeparam>
		/// <param name="doWork">The do work function which will be called by the background worker.</param>
		/// <param name="workerCompleted">The worker completed callback function.</param>
		public static void DoWork<Tout>(
			 Func<Tout> doWork,
			 Action<WorkerResult<Tout>> workerCompleted)
		{
			BackgroundWorker bw = new BackgroundWorker();
			bw.WorkerReportsProgress = false;
			bw.WorkerSupportsCancellation = false;
			bw.DoWork += (sender, args) =>
			{
				if (doWork != null)
				{
					args.Result = doWork();
				}
			};
			bw.RunWorkerCompleted += (sender, args) =>
			{
				if (workerCompleted != null)
				{
					Tout result = default(Tout);
					try
					{
						result = (Tout)args.Result;
					}
					catch (System.Reflection.TargetInvocationException)
					{
					}
					workerCompleted(new WorkerResult<Tout>(result, args.Error, args.Cancelled));
				}
			};
			bw.RunWorkerAsync();
		}

		/// <summary>
		/// Run the Provided Function using Background Worker
		/// </summary>
		/// <typeparam name="Tout">The type of the output we are expecting from the long running op.</typeparam>
		/// <param name="doWork">The do work function which will be called by the background worker.</param>
		/// <param name="workerCompleted">The worker completed callback function.</param>
		/// <param name="progressChanged">The progress changed callback function.</param>
		public static void DoWork<Tout>(
			 Func<Tout> doWork,
			 Action<WorkerResult<Tout>> workerCompleted,
			Action<ProgressChangedEventArgs> progressChanged)
		{
			BackgroundWorker bw = new BackgroundWorker();
			bw.WorkerReportsProgress = (progressChanged != null);
			bw.WorkerSupportsCancellation = false;
			bw.DoWork += (sender, args) =>
			{
				if (doWork != null)
				{
					args.Result = doWork();
				}
			};
			bw.RunWorkerCompleted += (sender, args) =>
			{
				if (workerCompleted != null)
				{
					Tout result = default(Tout);
					try
					{
						result = (Tout)args.Result;
					}
					catch (System.Reflection.TargetInvocationException)
					{
					}
					workerCompleted(new WorkerResult<Tout>(result, args.Error, args.Cancelled));
				}
			};
			bw.ProgressChanged += (sender, args) =>
			{
				if (progressChanged != null)
				{
					progressChanged(args);
				}
			};
			bw.RunWorkerAsync();
		}
	}

	/// <summary>
	/// Background Worker Helper Function Input Argument Class
	/// </summary>
	public class DoWorkArgument
	{
		public DoWorkArgument()
		{
		}
	}

	/// <summary>
	/// Background Worker Helper Function Input Argument Class
	/// </summary>
	/// <typeparam name="T">The input type for Background Worker</typeparam>
	public class DoWorkArgument<T>
	{
		public DoWorkArgument(T argument)
		{
			this.Argument = argument;
		}

		public T Argument { get; private set; }
	}

	/// <summary>
	/// The return type from Background Worker
	/// </summary>
	/// <typeparam name="T">The result type</typeparam>
	public class WorkerResult<T>
	{
		public WorkerResult(T result, Exception error, bool cancelled)
		{
			this.Result = result;
			this.Error = error;
			this.Cancelled = cancelled;
		}

		public T Result { get; private set; }
		public Exception Error { get; private set; }
		public bool Cancelled { get; private set; }
	}
}

using System;

namespace Abc.OnlineBL.Utility.Configuration
{
	/// <summary>
	/// AbcConfigException custom exception
	/// </summary>
	public class AbcConfigException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbcConfigException"/> class.
		/// </summary>
		public AbcConfigException() : base(){}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbcConfigException"/> class.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		public AbcConfigException(string msg) : base(msg){}
	}

	/// <summary>
	/// AbcConfigParsingException
	/// </summary>
	public class AbcConfigParsingException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbcConfigParsingException"/> class.
		/// </summary>
		public AbcConfigParsingException() : base(){}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbcConfigParsingException"/> class.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		public AbcConfigParsingException(string msg) : base(msg){}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbcConfigParsingException"/> class.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		/// <param name="inner">The inner.</param>
		public AbcConfigParsingException(string msg, Exception inner) : base(msg, inner){}
	}

	/// <summary>
	/// AbcConfigActiveProfileNotFoundException
	/// </summary>
	public class AbcConfigActiveProfileNotFoundException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbcConfigActiveProfileNotFoundException"/> class.
		/// </summary>
		public AbcConfigActiveProfileNotFoundException() : base(){}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbcConfigActiveProfileNotFoundException"/> class.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		public AbcConfigActiveProfileNotFoundException(string msg) : base(string.Format("Active Profile `{0}` NotFound",msg)){}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abc.OnlineBL.Entities.Enums
{
	// Summary:
	//     An enum representation of the 'QueueStatus' table. [No description found
	//     in the database]
	[Serializable]
	public enum AOP_QueueStatusList
	{		
		/// <summary>
		/// While the job hasn't been started yet
		/// </summary>
		Waiting_In_Queue = 0,
		/// <summary>
		/// Job Started
		/// </summary>
		Job_In_Progress = 1,
		/// <summary>
		/// While exporting preview to jpg and pdf, as soon as jpg is done the status will change to this
		/// </summary>
		Jpeg_Preview_Complete_Waiting_For_Pdf = 2,
		/// <summary>
		/// Job is done
		/// </summary>
		Job_Completed_Successfully = 10,
		/// <summary>
		/// Job maybe incomplete and completed with error/exception
		/// </summary>
		Job_Completed_with_Error = 100,
	}
}

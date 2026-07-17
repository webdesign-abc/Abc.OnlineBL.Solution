using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abc.OnlineBL.Entities
{
	public partial class AOP_JobDocument
	{
		/// <summary>
		/// Return the Job id if not null else returns 0
		/// </summary>
		/// <value>The job id.</value>
		public int SafeJobId
		{
			get
			{
				return (JobId.HasValue ? JobId.Value : 0);
			}
		}
	}
}

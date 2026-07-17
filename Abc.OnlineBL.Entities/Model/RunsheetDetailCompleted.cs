using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
	[DataContract]
	[Serializable]
	public class RunsheetDetailCompleted
	{
		[DataMember]
		public int OrderID { get; set; }

		[DataMember]
		public int RunID { get; set; }

		[DataMember]
		public bool IsRemoval { get; set; }

		[DataMember]
		public string PhotoAttachmentPath { get; set; }

		[DataMember]
		public bool IsBoardNotFound { get; set; }

        [DataMember]
        public bool IsDamagedBoard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is board unable to remove.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is board unable to remove; otherwise, <c>false</c>.
        /// </value>
		[DataMember]
		public bool IsBoardUnableToRemove { get; set; }

		[DataMember]
		public bool IsBoardUnableToErect { get; set; }

		[DataMember]
		public bool IsRequireTwoMen { get; set; }

		[DataMember]
		public bool IsRequireCherryPicker { get; set; }

		[DataMember]
		public bool IsRequireHighInstall { get; set; }


		[DataMember]
		public string EmailNote { get; set; }

		/// <summary>
		/// If a spotlight is not found when trying to remove a board then notify via Event
		/// </summary>
		[DataMember]
		public bool SpotlightNotFound { get; set; }

        [DataMember]
        public DateTime TabLocalDateTime { get; set; }

	}
}

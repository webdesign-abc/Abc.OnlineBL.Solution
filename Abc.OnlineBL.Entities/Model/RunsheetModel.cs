
namespace Abc.OnlineBL.Entities.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Entitty Class to Define the Properties
    /// </summary>
    [Serializable]
    [DataContract]
    public class RunsheetModel
    {
        /// <summary>
        /// Gets or sets the runsheet ID.
        /// </summary>
        /// <value>The runsheet ID.</value>
        [DataMember]
        public int RunsheetId { get; set; }

        /// <summary>
        /// Gets or sets the manager ID.
        /// </summary>
        /// <value>The manager ID.</value>
        [DataMember]
        public string ManagerId { get; set; }

        /// <summary>
        /// Gets or sets the truck ID.
        /// </summary>
        /// <value>The truck ID.</value>
        [DataMember]
        public int TruckId { get; set; }

        /// <summary>
        /// Gets or sets the runsheet date.
        /// </summary>
        /// <value>The runsheet date.</value>
        [DataMember]
        public string RunsheetDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [marked off].
        /// </summary>
        /// <value>The datetime when the runsheet is marked off</value>
        [DataMember]
        public string MarkedOff { get; set; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        [DataMember]
        public List<RunsheetDetail> Details { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="RunsheetModel"/> class.
        /// </summary>
        /// <param name="rs">The rs.</param>
        public RunsheetModel(Abc.OnlineBL.Entities.RunSheet rs)
        {
            this.RunsheetId = rs.RID;
            this.ManagerId = rs.ManID;
            this.TruckId = rs.TruckID;
            this.RunsheetDate = rs.DateFor.ToString("yyyy-MM-dd HH:mm:ss");

            if (rs.MarkedOff.HasValue)
            {
                this.MarkedOff = rs.MarkedOff.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                this.MarkedOff = "";
            }

            this.Details = new List<RunsheetDetail>();

            foreach (var item in rs.RunSheetDetails)
            {
                this.Details.Add(new RunsheetDetail(item));
            }

            foreach (var item in rs.RunsheetDetailsSBs)
            {
                this.Details.Add(new RunsheetDetail(item));
            }

            foreach (var item in rs.RunsheetDetailsNotes)
            {
                this.Details.Add(new RunsheetDetail(item));
            }

            this.Details.Sort();
        }
    }
}

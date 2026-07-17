using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Abc.OnlineBL.Entities.Model
{
	[DataContract]
	[Serializable]
	public class RunsheetDetail : IComparable<RunsheetDetail>
	{
		/// <summary>
		/// Gets or sets the type of the detail.
		/// </summary>
		/// <value>The type of the detail.</value>
		[DataMember]
		public int DetailType { set; get; }

		/// <summary>
		/// Gets or sets the runsheet ID.
		/// </summary>
		/// <value>The runsheet ID.</value>
		[DataMember]
		public int Rid { set; get; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is removal.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is removal; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool IsRemoval { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance  IsSolarPanelRemovalOnly.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is removal; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsSolarPanelRemovalOnly { set; get; }

		/// <summary>
		/// Gets or sets the serial.
		/// </summary>
		/// <value>The serial.</value>
		[DataMember]
		public int Serial { set; get; }

		/// <summary>
		/// Gets or sets the notes.
		/// </summary>
		/// <value>The notes.</value>
		[DataMember]
		public string Notes { set; get; }

		/// <summary>
		/// Gets or sets the date board erected.
		/// </summary>
		/// <value>The date board erected.</value>
		[DataMember]
		public string DateBoardErected { set; get; }

		/// <summary>
		/// Gets or sets the date board removed.
		/// </summary>
		/// <value>The date board removed.</value>
		[DataMember]
		public string DateBoardRemoved { set; get; }

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>The location.</value>
		[DataMember]
		public string Location { set; get; }

		/// <summary>
		/// Gets or sets the address.
		/// </summary>
		/// <value>The address.</value>
		[DataMember]
		public string Address { set; get; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>The state.</value>
		[DataMember]
		public string State { set; get; }

		/// <summary>
		/// Gets or sets the caption.
		/// </summary>
		/// <value>The caption.</value>
		[DataMember]
		public string Caption { set; get; }

		/// <summary>
		/// Gets or sets the order id.
		/// </summary>
		/// <value>The order id.</value>
		[DataMember]
		public int OrderId { set; get; }

		/// <summary>
		/// Gets or sets the agent.
		/// </summary>
		/// <value>The agent.</value>
		[DataMember]
		public string Agent { set; get; }

		/// <summary>
		/// Gets or sets the board.
		/// </summary>
		/// <value>The board.</value>
		[DataMember]
		public string Board { set; get; }

		/// <summary>
		/// Gets or sets the UBD_ map_ ref.
		/// </summary>
		/// <value>The UBD_ map_ ref.</value>
		[DataMember]
		public string UBD_Map_Ref { set; get; }

		/// <summary>
		/// Gets or sets the agent phone no.
		/// </summary>
		/// <value>The agent phone no.</value>
		[DataMember]
		public string AgentPhoneNo { set; get; }


		/// <summary>
		/// Gets or sets a value indicating whether this instance has installation notes.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has installation notes; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool HasInstallationNotes { get; set; }

		/// <summary>
		/// Gets or sets the name of the region.
		/// </summary>
		/// <value>
		/// The name of the region.
		/// </value>
		[DataMember]
		public string RegionName { get; set; }

		/// <summary>
		/// Gets or sets the boards despatched.
		/// </summary>
		/// <value>
		/// The boards despatched.
		/// </value>
		[DataMember]
		public string BoardsDespatched { get; set; }

		/// <summary>
		/// Gets or sets the date removal requested.
		/// </summary>
		/// <value>
		/// The date removal requested.
		/// </value>
		[DataMember]
		public string DateRemovalRequested { get; set; }

        /// <summary>
        /// Gets or sets the date re-erection requested.
        /// </summary>
        /// <value>
        /// The date re-erection requested.
        /// </value>
        [DataMember]
        public string DateReErectionRequested { get; set; }


        /// <summary>
        /// Gets or sets the date solar power removal requested.
        /// </summary>
        /// <value>
        /// The date solar power removal requested.
        /// </value>
        [DataMember]
        public string DateSolarPowerRemovalRequested { get; set; }

        /// <summary>
        /// Gets or sets the date approved.
        /// </summary>
        /// <value>
        /// The date approved
        /// </value>
        [DataMember]
        public string DateApproved { get; set; }

        /// <summary>
        /// Gets or sets the date approved.
        /// </summary>
        /// <value>
        /// The date approved
        /// </value>
        [DataMember]
        public string PreferredErectionDate { get; set; }

        /// <summary>
        /// Gets or sets the date approved.
        /// </summary>
        /// <value>
        /// The date approved
        /// </value>
        [DataMember]
        public string PreferredRemovalDate { get; set; }

		/// <summary>
		/// Gets or sets the clients ref no.
		/// </summary>
		/// <value>
		/// The clients ref no.
		/// </value>
		[DataMember]
		public string ClientsRefNo
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this order has spotlight.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this order has spotlight; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public string SpotlightProductNames
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether [urgent erection].
		/// </summary>
		/// <value><c>true</c> if [urgent erection]; otherwise, <c>false</c>.</value>
		[DataMember]
		public bool UrgentErection
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether [urgent removal].
		/// </summary>
		/// <value><c>true</c> if [urgent removal]; otherwise, <c>false</c>.</value>
		[DataMember]
		public bool UrgentRemoval
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether [require cherry picker].
		/// </summary>
		/// <value><c>true</c> if [require cherry picker]; otherwise, <c>false</c>.</value>
		[DataMember]
		public bool RequireCherryPicker
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether [require two men].
		/// </summary>
		/// <value><c>true</c> if [require two men]; otherwise, <c>false</c>.</value>
		[DataMember]
		public bool RequireTwoMen
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance has signshop product.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has signshop product; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool HasSignshopProduct
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether [job more than day old].
		/// </summary>
		/// <value><c>true</c> if [job more than day old]; otherwise, <c>false</c>.</value>
		[DataMember]
		public bool JobMoreThanDayOld
		{
			get;
			set;
		}

		[DataMember]
		public bool HighInstall
		{
			get;
			set;
		}


        /// <summary>
        /// Gets or sets a value indicating whether this is a light board product (requires additional photo).
        /// </summary>
        /// <value><c>true</c> if light board product; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool IsLightBoard
        {
            get;
            set;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="RunsheetDetail"/> class.
		/// </summary>
		public RunsheetDetail()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RunsheetDetail"/> class.
		/// </summary>
		/// <param name="item">The item.</param>
		public RunsheetDetail(Abc.OnlineBL.Entities.RunSheetDetail item)
		{
			this.DetailType = 1;
			this.Rid = item.RID;
			this.IsRemoval = item.IsRemoval;
			this.Serial = item.Sl;
			this.OrderId = item.OrderID;

			if (item.DespatchDetail != null)
			{
				if (this.IsRemoval == false)
				{
					if (item.DespatchDetail.Order.Urgent)
						this.UrgentErection = true;
					else
						this.UrgentErection = item.DespatchDetail.UrgentErection.HasValue ? item.DespatchDetail.UrgentErection.Value : false;
				}
				else
				{
					this.UrgentRemoval = item.DespatchDetail.UrgentRemoval;
				}

                IsSolarPanelRemovalOnly = item.SolarPanelRemoval ?? false;

                this.RequireCherryPicker = item.DespatchDetail.RequireCherryPicker.HasValue ? item.DespatchDetail.RequireCherryPicker.Value : false;
				this.RequireTwoMen = item.DespatchDetail.RequireTwoMen.HasValue ? item.DespatchDetail.RequireTwoMen.Value : false;
				this.HighInstall = item.DespatchDetail.HighInstallRunOnly.HasValue ? item.DespatchDetail.HighInstallRunOnly.Value : false;

                this.IsLightBoard = item.DespatchDetail.Order.OrderDetails.Any(x => x.Product.FrameType == "Light Board");

				var signshopProduct = (from p in item.DespatchDetail.Order.OrderDetails
                                       where p.Product.TypeID == ProductTypes.SignShop
											  select p).FirstOrDefault();
				if (signshopProduct != null)
				{
					this.HasSignshopProduct = true;
				}

				if (item.DespatchDetail.Order != null)
					if (item.DespatchDetail.Order.OrderOtherDetail != null)
					{
						if (!string.IsNullOrEmpty(item.DespatchDetail.Order.OrderOtherDetail.InstallFile))
							this.HasInstallationNotes = true;
					}
				if (item.DespatchDetail.DateBoardErected.HasValue)
				{
					this.DateBoardErected = item.DespatchDetail.DateBoardErected.Value.ToString("yyyy-MM-dd HH:mm:ss");
				}
				else
				{
					this.DateBoardErected = string.Empty;
				}

				if (!OnlineBLConfig.IS_NZ)
				{
					this.BoardsDespatched = GetDateAlsoCheckIfOld(item.DespatchDetail.BoardsDespatched);
				}
				else
				{
					this.BoardsDespatched = item.DespatchDetail.BoardsDespatched.HasValue ? item.DespatchDetail.BoardsDespatched.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
				}

				if (item.DespatchDetail.DateBoardRemoved.HasValue)
				{
					this.DateBoardRemoved = item.DespatchDetail.DateBoardRemoved.Value.ToString("yyyy-MM-dd HH:mm:ss");
				}
				else
				{
					this.DateBoardRemoved = string.Empty;
				}

				if (!OnlineBLConfig.IS_NZ)
				{
					this.DateRemovalRequested = GetDateAlsoCheckIfOld(item.DespatchDetail.DateRemovalRequested);
				}
				else
				{
					this.DateRemovalRequested = item.DespatchDetail.DateRemovalRequested.HasValue ? item.DespatchDetail.DateRemovalRequested.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
				}


                if (!OnlineBLConfig.IS_NZ)
                {
                    this.DateReErectionRequested = GetDateAlsoCheckIfOld(item.DespatchDetail.ReErectionRequested);
                }
                else
                {
                    this.DateReErectionRequested = item.DespatchDetail.ReErectionRequested.HasValue ? item.DespatchDetail.ReErectionRequested.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
                }

                if (!OnlineBLConfig.IS_NZ)
                {
                    this.PreferredErectionDate = GetDateAlsoCheckIfOld(item.DespatchDetail.PreferredErectionDate);
                    this.PreferredRemovalDate = GetDateAlsoCheckIfOld(item.DespatchDetail.PreferredRemovalDate);
                }
                else
                {
                    this.PreferredErectionDate = item.DespatchDetail.PreferredErectionDate.HasValue ? item.DespatchDetail.PreferredErectionDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
                    this.PreferredRemovalDate = item.DespatchDetail.PreferredRemovalDate.HasValue ? item.DespatchDetail.PreferredRemovalDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
                }
               
				if (item.DespatchDetail.Order != null)
				{
					if (item.IsRemoval)
					{
						this.Notes = item.DespatchDetail.Order.RemovalNotes;
                        if(!string.IsNullOrEmpty(item.DespatchDetail.Order.ErectionNotes))
                        {
                            this.Notes = this.Notes + " *** Installation Notes: " + item.DespatchDetail.Order.ErectionNotes; 
                        }
						//check for ABC Spotlight Solar and LED products in Order
						if (item.DespatchDetail.Order.OrderDetails != null)
						{
							var products = (from od in item.DespatchDetail.Order.OrderDetails
												 where od.Product.Name.Equals("Spotlight LED - ABC") || od.Product.Name.Equals("Solar Light - ABC")
												 select od).FirstOrDefault();
							if (products != null)
							{
								if (products.Product != null)
								{
									this.SpotlightProductNames = products.Product.Name;
								}
							}
						}
					}
					else
					{
						this.Notes = item.DespatchDetail.Order.ErectionNotes;
					}

					this.Caption = item.DespatchDetail.Order.Caption;
					this.Address = item.DespatchDetail.Order.PropertyAddress;

					if (item.DespatchDetail.Order.Location != null)
					{
						this.State = item.DespatchDetail.Order.Location.State;
						this.Location = item.DespatchDetail.Order.Location.Location1;

						/// Get Region Name
						int driverRegionId = 0;
						if (item.DespatchDetail.Order.Location.DriverRegionID != null)
						{
							driverRegionId = (int)item.DespatchDetail.Order.Location.DriverRegionID;
						}

						if (driverRegionId > 0)
						{
							if (item.DespatchDetail.Order.Location.DriverRegions != null)
								this.RegionName = item.DespatchDetail.Order.Location.DriverRegions.RegionName;
						}
					}
					//Retrive Client/Agent name
					if (item.DespatchDetail.Order.Client != null)
					{
						this.Agent = item.DespatchDetail.Order.Client.ClientName;
						if (!String.IsNullOrEmpty(item.DespatchDetail.Order.Client.ClientsDisplayInfo.Office))
						{
							this.Agent += " / " + item.DespatchDetail.Order.Client.ClientsDisplayInfo.Office;
						}
						if (!String.IsNullOrEmpty(item.DespatchDetail.Order.Client.ClientsDisplayInfo.Phone))
						{
							this.AgentPhoneNo = item.DespatchDetail.Order.Client.ClientsDisplayInfo.Phone;
						}
						else
							this.AgentPhoneNo = item.DespatchDetail.Order.Client.Phone;
					}

					if (item.DespatchDetail.Order.ClientsRefNo != null && !string.IsNullOrEmpty(item.DespatchDetail.Order.ClientsRefNo))
					{
						ClientsRefNo = item.DespatchDetail.Order.ClientsRefNo;
					}

                    if (IsSolarPanelRemovalOnly)
                    {
                        if (item.DespatchDetail.Order.OrderOtherInfo != null && item.DespatchDetail.Order.OrderOtherInfo.SolarPowerRemovalRequested.HasValue)
                        {
                            this.DateSolarPowerRemovalRequested = item.DespatchDetail.Order.OrderOtherInfo.SolarPowerRemovalRequested.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            this.DateSolarPowerRemovalRequested = string.Empty;
                        }
                    }
                    else
                    {
                        this.DateSolarPowerRemovalRequested = string.Empty;
                    }

                    if (item.DespatchDetail.Order.ProofDetail != null && item.DespatchDetail.Order.ProofDetail.DateApproved.HasValue)
                    {
                        this.DateApproved = item.DespatchDetail.Order.ProofDetail.DateApproved.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        this.DateApproved = string.Empty;
                    }
				}
			}
		}

		/// <summary>
		/// Gets the date also check if older than 1 day then returns the string with ^^ in front to that we can use this delimeter in android tabs to highlight in red
		/// </summary>
		/// <param name="dateData">The date data.</param>
		/// <returns></returns>
		private string GetDateAlsoCheckIfOld(DateTime? dateData)
		{
			string ret = string.Empty;
			DateTime today = DateTime.Today;
			if (today.DayOfWeek == DayOfWeek.Monday)
			{
				today = today.AddDays(-2);
			}
			if (dateData.HasValue)
			{
				if ((today - dateData.Value.Date).TotalDays > 1)
				{
					ret = dateData.Value.ToString("^^yyyy-MM-dd HH:mm:ss");
					this.JobMoreThanDayOld = true;
				}
				else
				{
					ret = dateData.Value.ToString("yyyy-MM-dd HH:mm:ss");
				}
			}

			return ret;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="RunsheetDetail"/> class.
		/// </summary>
		/// <param name="item">The item.</param>
		public RunsheetDetail(Abc.OnlineBL.Entities.RunsheetDetailsSB item)
		{
			this.DetailType = 2;
			this.Rid = item.RID;
			this.IsRemoval = item.IsRemoval;
			this.Serial = item.Sl;
			this.OrderId = item.OrderID;

			if (item.SB_Order != null)
			{
				if (item.SB_Order.DateBoardErected.HasValue)
				{
					this.DateBoardErected = item.SB_Order.DateBoardErected.Value.ToString("yyyy-MM-dd HH:mm:ss");
				}
				else
				{
					this.DateBoardErected = string.Empty;
				}

				if (item.SB_Order.DateBoardRemoved.HasValue)
				{
					this.DateBoardRemoved = item.SB_Order.DateBoardRemoved.Value.ToString("yyyy-MM-dd HH:mm:ss");
				}
				else
				{
					this.DateBoardRemoved = string.Empty;
				}

				//Retrive Client name
				if (item.SB_Order.Client != null)
				{
					this.Agent = item.SB_Order.Client.ClientName;
					if (!String.IsNullOrEmpty(item.SB_Order.Client.ClientsDisplayInfo.Office))
					{
						this.Agent += " / " + item.SB_Order.Client.ClientsDisplayInfo.Office;
					}

					if (!String.IsNullOrEmpty(item.SB_Order.Client.ClientsDisplayInfo.Phone))
					{
						this.AgentPhoneNo = item.SB_Order.Client.ClientsDisplayInfo.Phone;
					}
				}

				this.Caption = item.SB_Order.Caption;
                this.Notes = item.SB_Order.Notes;
				this.Address = item.SB_Order.PropertyAddress;
				this.State = item.SB_Order.State;
				this.Location = item.SB_Order.Location;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RunsheetDetail"/> class.
		/// </summary>
		/// <param name="item">The item.</param>
		public RunsheetDetail(Abc.OnlineBL.Entities.RunsheetDetailsNote item)
		{
			this.DetailType = 3;
			this.Rid = item.RID;
            this.OrderId = -item.NoteID; // Giving this a unique orderno in the negative range so we can persist it on the devices.
			this.Serial = item.Sl;
			this.Notes = item.Notes;
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
		/// Value
		/// Meaning
		/// Less than zero
		/// This object is less than the <paramref name="other"/> parameter.
		/// Zero
		/// This object is equal to <paramref name="other"/>.
		/// Greater than zero
		/// This object is greater than <paramref name="other"/>.
		/// </returns>
		public int CompareTo(RunsheetDetail other)
		{
			return this.Serial.CompareTo(other.Serial);
		}
	}
}

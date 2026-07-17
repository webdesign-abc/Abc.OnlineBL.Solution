using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.OnlineOrder;

namespace Abc.OnlineBL.Orders.Workflow.Model
{
	public class OnlineOrder
	{
		public static string GetOrderNotes(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder PropertyOrder)
		{
			string addNote1 = string.Empty;
			string addNote2 = string.Empty;
			string addNoteAll = string.Empty;

			if (!string.IsNullOrEmpty(PropertyOrder.Notes))
				addNoteAll = PropertyOrder.Notes + "\r\n";

			if (PropertyOrder.CommonDetails.IsInsertAbcRealEstateLink)
			{
				addNote1 += "USE ABCRealestate.com.au Link";
			}

			foreach (var item in PropertyOrder.Cart)
			{
				if (item.TypeId == ProductTypes.BoardPackages || item.TypeId == ProductTypes.OtherPackages || item.TypeId == ProductTypes.Packages)
				{
					addNote2 += item.ProductName + ",";
				}
			}

			if (addNote2.EndsWith(","))
			{
				addNote2 = addNote2.TrimEnd(',') + "\r\n";
			}

			if (addNote1.Length > 0)
			{
				addNoteAll = addNoteAll + addNote1 + "\r\n";
			}
			//if (!string.IsNullOrEmpty(addNote2))
			//{
			//	addNoteAll = addNoteAll + "\r\nPackages: " + addNote2;
			//}

			return addNoteAll;
		}

		public static string GetErectionNotes(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder PropertyOrder)
		{
			if (PropertyOrder.BoardInstallationType == BoardInstallationType.High ||
					PropertyOrder.BoardInstallationType == BoardInstallationType.HigherThanFirstLevel ||
				PropertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed500mmOfTheGround ||
				PropertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed1000mmOfTheGround ||
				PropertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed1250mmOfTheGround ||
				PropertyOrder.BoardInstallationType == BoardInstallationType.BoardToBeROnlineBLed2000mmOfTheGround
				)
			{
                if (!string.IsNullOrEmpty(PropertyOrder.ErectionNotes))
                {
                    if (PropertyOrder.ErectionNotes.IndexOf(GetBoardErectionDescription(PropertyOrder.BoardInstallationType)) < 0)
                        PropertyOrder.ErectionNotes = string.Format("{0}\r\n{1}", GetBoardErectionDescription(PropertyOrder.BoardInstallationType), PropertyOrder.ErectionNotes);
                }
                else
                {
                    PropertyOrder.ErectionNotes = GetBoardErectionDescription(PropertyOrder.BoardInstallationType);
                }
			}
			return PropertyOrder.ErectionNotes;
		}

		public static bool IsPhotoExists(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder PropertyOrder)
		{
			foreach (CartItem anItem in PropertyOrder.Cart)
			{
				if (anItem.TypeId == ProductTypes.Photography)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsStockBoardExists(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder PropertyOrder)
		{
			foreach (CartItem anItem in PropertyOrder.Cart)
			{
				if (anItem.TypeId == ProductTypes.Stockboard)
				{
					return true;
				}
			}
			return false;
		}

		private static string GetBoardErectionDescription(BoardInstallationType installationType)
		{
			string level = string.Empty;

			if (installationType == BoardInstallationType.Standard)
				level = "Standard Installation";
			else if (installationType == BoardInstallationType.High)
				level = "High Installation";
			else if (installationType == BoardInstallationType.HigherThanFirstLevel)
				level = "Higher than 1st Level Installation";
			else if (installationType == BoardInstallationType.BoardToBeROnlineBLed500mmOfTheGround)
				level = "Board to be rOnlineBLed 500mm of the ground $225";
			else if (installationType == BoardInstallationType.BoardToBeROnlineBLed1000mmOfTheGround)
				level = "Board to be rOnlineBLed 1000mm of the ground $325";
			else if (installationType == BoardInstallationType.BoardToBeROnlineBLed1250mmOfTheGround)
				level = "Board to be rOnlineBLed 1250mm of the ground $525";
			else if (installationType == BoardInstallationType.BoardToBeROnlineBLed2000mmOfTheGround)
				level = "Board to be install on wall or higher than 2000mm to be quoted";

			return string.Format("Erection Fee: {0}", level);
		}
	}
}

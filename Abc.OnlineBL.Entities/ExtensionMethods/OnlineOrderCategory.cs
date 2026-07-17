using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abc.OnlineBL.Entities
{
	public partial class OnlineOrderCategory
	{
		public List<int> RelatedProductList
		{
			get
			{
				List<int> ret = new List<int>();

				if (!string.IsNullOrEmpty(this.RelatedProductIds))
				{
					string[] parts = this.RelatedProductIds.Split(',', ';', '|');
					foreach (var item in parts)
					{
						int id = 0;
						if (int.TryParse(item, out id))
						{
							if (id > 0)
								ret.Add(id);
						}
					}
				}

				return ret;
			}
		}
	}
}

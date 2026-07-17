using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.DataStore;

namespace Abc.OnlineBL.Service.Implementation.BusinessLogic
{
	public class ProductAttributeMapper
	{
		public static void MapAttributes(AbcDataContext ctx, Product product, OnlineProduct onlineProduct, string attributeList)
		{
			if (attributeList == null)
				return;

			string[] fields = attributeList.ToUpper().Split(',', '|', ';');
			foreach (var field in fields)
			{
				switch (field)
				{
					case "SIZECODE":
						if (product.ProductSizeCode != null)
						{
							string sizeCode;

                            sizeCode = (from pd in ctx.ProductSizeCodeDetails
                                        where pd.ProductTypeId == product.TypeID && pd.SizeCode == product.SizeCode
                                        select pd.SizeCodeOnWeb).FirstOrDefault();
							if (sizeCode!=null)
							{
								onlineProduct.Attributes.Add(new AbcKeyValuePair(){ Key = "SizeCode", Value = sizeCode });
							}
						}
						break;
					case "CONTENTTYPE":
						if (!string.IsNullOrEmpty(product.ContentType))
						{
							onlineProduct.Attributes.Add(new AbcKeyValuePair() { Key = "ContentType", Value = product.ContentType });
						}
						break;
					case "FRAMETYPE":
						if (!string.IsNullOrEmpty(product.FrameType))
						{
							onlineProduct.Attributes.Add(new AbcKeyValuePair() { Key = "FrameType", Value = product.FrameType });
						}
						break;
					case "QTY":
						if (product.Qty.HasValue)
						{
							onlineProduct.Attributes.Add(new AbcKeyValuePair() { Key = "Qty", Value = product.Qty.ToString() });
						}
						break;
					case "CUSTOMNAME":
						if (!string.IsNullOrEmpty(product.CustomName))
						{
							onlineProduct.Attributes.Add(new AbcKeyValuePair() { Key = "CustomName", Value = product.CustomName });
						}
						break;
					case "FORMAT":
						if (!string.IsNullOrEmpty(product.Format))
						{
							onlineProduct.Attributes.Add(new AbcKeyValuePair() { Key = "Format", Value = product.Format });
						}
						break;
					default:
						break;
				}
			}
		}
	}
}

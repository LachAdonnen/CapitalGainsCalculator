using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	public static class TradeListUtilities
	{

		public static List<TaxEvent> GetTaxEventList(List<SellOrder> trades)
		{
			List<TaxEvent> taxList = new List<TaxEvent>();
			foreach (SellOrder sell in trades)
			{
				taxList.Add(sell.GetTaxEvent());
			}
			return taxList;
		}
	}
}

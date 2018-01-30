using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class OrderHistory<TBaseOrder> : List<TBaseOrder>
		where TBaseOrder : BaseOrder
	{
		public OrderHistory() { }

		public void Sort(OrderSortType sortBy, ListSortDirection sortDir)
		{
			BaseOrder.SetComparisonParameters(sortBy, sortDir, true);
			Sort();
		}

		//public List<T> Filter<T>(List<TradeOrder> trades, TradeFilter filter)
		//	where T : TradeOrder
		//{
		//	List<T> results = new List<T>();
		//	foreach (TradeOrder trade in trades)
		//	{
		//		if (trade.Filter(filter))
		//		{
		//			T castTrade = trade as T;
		//			if (castTrade != null)
		//			{ results.Add(castTrade); }
		//		}
		//	}
		//	return results;
		//}

		public OrderHistory<TBaseOrder> Filter(OrderFilter filter)
		{
			OrderHistory<TBaseOrder> results = new OrderHistory<TBaseOrder>();
			foreach (TBaseOrder trade in this)
			{
				if (trade.Filter(filter))
				{
					results.Add(trade);
				}
			}
			return results;
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class TradeHistory<TTradeOrder> : List<TTradeOrder>
		where TTradeOrder : TradeOrder
	{
		public TradeHistory() { }

		public void Sort(TradeSortType sortBy, ListSortDirection sortDir)
		{
			TradeOrder.SetComparisonParameters(sortBy, sortDir, true);
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

		public TradeHistory<TTradeOrder> Filter(TradeFilter filter)
		{
			TradeHistory<TTradeOrder> results = new TradeHistory<TTradeOrder>();
			foreach (TTradeOrder trade in this)
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

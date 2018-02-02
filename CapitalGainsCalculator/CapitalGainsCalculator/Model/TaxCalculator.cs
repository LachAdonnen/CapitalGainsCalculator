using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	public static class TaxCalculator
	{
		private static TaxCalculationType s_type;

		public static void GenerateTaxEvents(TaxCalculationType type, OrderHistory history)
		{
			s_type = type;
			TaxLedger ledger = new TaxLedger();
			ledger.AddOrderHistory(history);
			ledger.SortOrders();

			foreach (KeyValuePair<Currency, List<TaxableBaseOrder>> kvp in ledger)
			{
				GenerateTaxEvents(kvp.Value);
			}
		}

		private static void GenerateTaxEvents(List<TaxableBaseOrder> taxOrders)
		{
			TaxableBaseOrder order;
			for (int sellIndex = 0; sellIndex < taxOrders.Count; sellIndex++)
			{
				order = taxOrders[sellIndex];
				if (order.Type != OrderType.Sell) { continue; }
				TaxableSellOrder sellOrder = order as TaxableSellOrder;

				int buyIndex = sellIndex; // Initialize loop start point
				while (!sellOrder.TaxLine.TaxEvent.IsComplete &&
					buyIndex >= 0)
				{
					buyIndex = GetBuyOrderIndex(buyIndex, taxOrders);
					if (buyIndex >= 0)
					{
						(taxOrders[buyIndex] as TaxableBuyOrder).AddToTaxEvent(sellOrder.TaxLine.TaxEvent);
					}
				}
			}
		}

		private static int GetBuyOrderIndex(int startIndex, List<TaxableBaseOrder> taxOrders)
		{
			switch (s_type)
			{
				case TaxCalculationType.LIFO:
					return GetBuyOrderIndexLifo(startIndex, taxOrders);
				default:
					return -1;
			}
		}

		private static int GetBuyOrderIndexLifo(int startIndex, List<TaxableBaseOrder> taxOrders)
		{
			int resultIndex = -1;
			for (int buyIndex = startIndex - 1; buyIndex >= 0; buyIndex--)
			{
				TaxableBaseOrder order = taxOrders[buyIndex];
				if (order.Type != OrderType.Buy) { continue; }
				TaxableBuyOrder buyOrder = order as TaxableBuyOrder;
				if (buyOrder.IsFullyTaxed) { continue; }
				resultIndex = buyIndex;
				break;
			}
			return resultIndex;
		}

		public static void ResetTaxEvents(OrderHistory history)
		{
			foreach (ExchangeOrder order in history)
			{
				ResetTaxEvents(order);
			}
		}

		public static void ResetTaxEvents(ExchangeOrder order)
		{

		}
	}
}

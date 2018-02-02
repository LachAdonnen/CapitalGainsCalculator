using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	public class TaxLedger : Dictionary<Currency, List<TaxableBaseOrder>>
	{
		public TaxLedger()
			: base()
		{ }

		public void AddOrder(ExchangeOrder order)
		{
			order.SplitIntoTaxableOrders();
			GetCurrencyList(order.TradeCurrency).AddRange(
				new TaxableBaseOrder[]
				{
					order.TaxableSell,
					order.TaxableBuy
				});
		}

		public void AddOrderHistory(OrderHistory history)
		{
			foreach (ExchangeOrder order in history)
			{
				if (order.Type == OrderType.Buy ||
					order.Type == OrderType.Sell)
				{
					AddOrder(order);
				}
			}
		}

		private List<TaxableBaseOrder> GetCurrencyList(Currency currency)
		{
			List<TaxableBaseOrder> list;
			if (!TryGetValue(currency, out list))
			{
				list = new List<TaxableBaseOrder>();
				Add(currency, list);
			}
			return list;
		}

		public void SortOrders()
		{
			foreach (KeyValuePair<Currency, List<TaxableBaseOrder>> kvp in this)
			{
				kvp.Value.Sort();
			}
		}
	}
}

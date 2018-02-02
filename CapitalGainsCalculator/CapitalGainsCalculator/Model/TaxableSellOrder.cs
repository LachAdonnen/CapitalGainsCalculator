using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class TaxableSellOrder : TaxableBaseOrder
	{
		#region Properties
		public TaxLine TaxLine { get; private set; }

		public override OrderType Type { get { return OrderType.Sell; } }
		#endregion

		#region Constructors
		public TaxableSellOrder(ExchangeOrder order)
			: base(order)
		{
			if (order.Type == OrderType.Buy)
			{
				TradeAmount = order.BaseAmount;
				TradeCurrency = order.BaseCurrency;
			}
			else if (order.Type == OrderType.Sell)
			{
				TradeAmount = order.TradeAmount;
				TradeCurrency = order.TradeCurrency;
			}

			TaxLine = new TaxLine(this, TradeAmount);
			TaxLine.CreateTaxEvent();
		}
		#endregion

		protected override int OnCompareTo(TaxableBaseOrder other)
		{ return -1; } // If the source order is the same, sort the Sell first
	}
}

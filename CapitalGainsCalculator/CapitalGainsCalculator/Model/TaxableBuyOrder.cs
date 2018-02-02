using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class TaxableBuyOrder : TaxableBaseOrder
	{
		#region Properties
		public List<TaxLine> TaxLines { get; private set; }
		public decimal TaxableAmountRemaining
		{
			get
			{
				decimal remaining = TradeAmount;
				foreach (TaxLine taxLine in TaxLines)
				{
					remaining -= taxLine.TradeAmount;
				}
				if (remaining < 0) { remaining = 0; }
				return remaining;
			}
		}

		public override OrderType Type { get { return OrderType.Buy; } }

		public bool IsFullyTaxed
		{
			get
			{
				return TaxableAmountRemaining == 0;
			}
		}
		#endregion

		#region Constructors
		public TaxableBuyOrder(ExchangeOrder order)
			: base(order)
		{
			if (order.Type == OrderType.Buy)
			{
				TradeAmount = order.TradeAmount;
				TradeCurrency = order.TradeCurrency;
			}
			else if (order.Type == OrderType.Sell)
			{
				TradeAmount = order.BaseAmount;
				TradeCurrency = order.BaseCurrency;
			}

			TaxLines = new List<TaxLine>();
		}
		#endregion

		protected override int OnCompareTo(TaxableBaseOrder other)
		{ return 1; } // If the source order is the same, sort the Buy last

		public void AddToTaxEvent(TaxEvent taxEvent)
		{
			AddTaxLine(taxEvent.AmountRemaining)?.AddToTaxEvent(taxEvent);
		}

		private TaxLine AddTaxLine(decimal tradeAmount)
		{
			decimal remaining = TaxableAmountRemaining;
			if (tradeAmount == 0 || remaining == 0) { return null; }

			TaxLine newLine = null;
			if (tradeAmount <= remaining)
			{
				newLine = new TaxLine(this, tradeAmount);
			}
			else
			{
				newLine = new TaxLine(this, TradeAmount);
			}
			this.TaxLines.Add(newLine);
			return newLine;
		}

		public List<TaxEvent> GetTaxEvents()
		{
			List<TaxEvent> taxEvents = new List<TaxEvent>();
			foreach (TaxLine line in TaxLines)
			{
				TaxEvent taxEvent = line.TaxEvent;
				if (!taxEvents.Contains(taxEvent))
				{
					taxEvents.Add(taxEvent);
				}
			}
			return taxEvents;
		}
	}
}

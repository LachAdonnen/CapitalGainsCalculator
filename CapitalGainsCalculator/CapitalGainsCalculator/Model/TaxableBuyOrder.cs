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

		public bool IsCompletelyTaxed
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
		{ }
		#endregion

		public TaxLine AddTaxLine(decimal tradeAmount)
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

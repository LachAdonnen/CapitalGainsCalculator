using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class TaxEvent
	{
		public TaxLine SellTaxLine { get; private set; }
		public List<TaxLine> BuyTaxLines { get; private set; }

		public decimal AmountRemaining
		{
			get
			{
				if (SellTaxLine == null) { return 0; }

				decimal remaining = SellTaxLine.TradeAmount;
				foreach (TaxLine buy in BuyTaxLines)
				{
					remaining -= buy.TradeAmount;
				}
				if (remaining < 0) { remaining = 0; }
				return remaining;
			}
		}

		public bool IsComplete
		{
			get
			{
				return AmountRemaining == 0;
			}
		}

		public TaxEvent(TaxLine sellTaxLine)
		{
			if (sellTaxLine.TradeOrder.Type != TradeType.Sell)
			{
				throw new TradeTypeMismatchException();
			}
			sellTaxLine.TaxEvent = this;
			this.SellTaxLine = sellTaxLine;
			BuyTaxLines = new List<TaxLine>();
		}

		public void AddBuyTaxLine(TaxLine buyTaxLine)
		{
			if (buyTaxLine.TradeOrder.Type != TradeType.Buy)
			{
				throw new TradeTypeMismatchException();
			}
			buyTaxLine.TaxEvent = this;
			BuyTaxLines.Add(buyTaxLine);
		}
	}
}

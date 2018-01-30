using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class SellOrder : ExchangeOrder
	{
		#region Properties
		public TaxLine TaxLine { get; private set; }
		public bool IsTaxed
		{
			get
			{
				return TaxLine != null;
			}
		}
		#endregion

		#region Constructors
		public SellOrder()
			: base()
		{ }

		public SellOrder(int orderId)
			: base(orderId)
		{ }

		protected override void OnInitializeTrade()
		{
			Type = TradeType.Sell;
		}
		#endregion

		public bool TryAddTaxLine(out TaxLine taxLine)
		{
			if (IsTaxed)
			{
				taxLine = TaxLine;
				return false;
			}
			else
			{
				TaxLine = new TaxLine(this, TradeAmount);
				taxLine = TaxLine;
				return true;
			}
		}
		
		public TaxEvent GetTaxEvent()
		{
			return TaxLine.TaxEvent;
		}
	}
}

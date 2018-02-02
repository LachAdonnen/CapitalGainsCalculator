using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class TaxLine
	{
		public decimal TradeAmount { get; private set; }
		public TaxableBaseOrder TaxOrder { get; private set; }

		private TaxEvent _taxEvent;
		public TaxEvent TaxEvent
		{
			get { return _taxEvent; }
			private set
			{
				if (_taxEvent != null)
				{
					throw new DuplicateTaxEventEcxeption();
				}
				_taxEvent = value;
			}
		}

		public bool IsBuyOrder { get { return TaxOrder is TaxableBuyOrder; } }
		public bool IsSellOrder { get { return TaxOrder is TaxableSellOrder; } }

		public TaxLine(TaxableBaseOrder order, decimal amount)
		{
			TradeAmount = amount;
			TaxOrder = order;
		}

		public void CreateTaxEvent()
		{
			if (!IsSellOrder)
			{
				throw new OrderTypeMismatchException();
			}

			TaxEvent = new TaxEvent(this);
		}

		public void AddToTaxEvent(TaxEvent taxEvent)
		{
			if (!IsBuyOrder)
			{
				throw new OrderTypeMismatchException();
			}

			TaxEvent = taxEvent;
			taxEvent.AddBuyTaxLine(this);
		}
	}
}

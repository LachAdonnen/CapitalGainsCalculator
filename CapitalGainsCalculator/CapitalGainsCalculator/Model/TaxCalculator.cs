using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	public class TaxCalculator
	{
		private TaxCalculationType _calcType;
		
		private TradeHistory<BuyOrder> _buyTrades;
		private TradeHistory<SellOrder> _sellTrades;

		public TaxCalculator(TaxCalculationType type, TradeHistory<TradeOrder> allTrades)
		{
			_calcType = type;
		}
	}
}

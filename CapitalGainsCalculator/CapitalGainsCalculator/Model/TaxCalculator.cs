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

		public TaxCalculator(TaxCalculationType type, OrderHistory<BaseOrder> allTrades)
		{
			_calcType = type;
		}
	}
}

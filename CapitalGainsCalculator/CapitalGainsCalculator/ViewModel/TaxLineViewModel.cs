using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapitalGainsCalculator.Model;

namespace CapitalGainsCalculator.ViewModel
{
	public class TaxLineViewModel : ViewModelBase
	{
		private TaxLine _taxLine;
		public TaxLineViewModel(TaxLine taxLine)
		{
			_taxLine = taxLine;
			RaisePropertyChangedEvent(nameof(TradeAmount));
			RaisePropertyChangedEvent(nameof(Type));
		}

		public decimal TradeAmount
		{
			get { return _taxLine.TradeAmount; }
		}

		public OrderType Type
		{
			get
			{
				if (_taxLine.IsBuyOrder) { return OrderType.Buy; }
				else { return OrderType.Sell; }
			}
		}
	}
}

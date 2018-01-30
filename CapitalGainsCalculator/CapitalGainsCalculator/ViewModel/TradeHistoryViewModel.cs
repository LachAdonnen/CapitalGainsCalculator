using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapitalGainsCalculator.Model;

namespace CapitalGainsCalculator.ViewModel
{
	public class TradeHistoryViewModel : ViewModelBase
	{
		protected ObservableCollection<TradeOrderViewModel> _tradesVM;
		protected TradeFilter _filter;

		public ObservableCollection<TradeOrderViewModel> Trades
		{
			get { return _tradesVM; }
		}

		public TradeHistoryViewModel()
		{
			Initialize();
			InitializeViewModels();
		}

		private void Initialize()
		{
			_tradesVM = new ObservableCollection<TradeOrderViewModel>();
			_filter = new TradeFilter();
			OnInitialize();
		}

		protected virtual void OnInitialize()
		{ }

		protected void InitializeViewModels()
		{
		}
	}
}

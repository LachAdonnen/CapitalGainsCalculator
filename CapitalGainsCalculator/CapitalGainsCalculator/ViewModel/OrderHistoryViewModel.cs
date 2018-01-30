using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapitalGainsCalculator.Model;

namespace CapitalGainsCalculator.ViewModel
{
	public class OrderHistoryViewModel : ViewModelBase
	{
		protected ObservableCollection<OrderViewModel> _ordersVM;
		protected OrderFilter _filter;

		public ObservableCollection<OrderViewModel> Orders
		{
			get { return _ordersVM; }
		}

		public OrderHistoryViewModel()
		{
			Initialize();
			InitializeViewModels();
		}

		private void Initialize()
		{
			_ordersVM = new ObservableCollection<OrderViewModel>();
			_filter = new OrderFilter();
			OnInitialize();
		}

		protected virtual void OnInitialize()
		{ }

		protected void InitializeViewModels()
		{
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapitalGainsCalculator.Model;

namespace CapitalGainsCalculator.ViewModel
{
	[Serializable]
	public class OrderViewModel : ViewModelBase, IComparable<OrderViewModel>
	{
		private BaseOrder _order;
		public BaseOrder Order
		{
			get
			{
				return _order;
			}
		}

		public OrderViewModel()
		{
			InitializeViewModels();
		}

		public OrderViewModel(BaseOrder order)
			: this()
		{
			_order = order;
		}

		public OrderViewModel(OrderType type)
			: this(BaseOrder.CreateOrder(type))
		{ }

		public OrderViewModel(string importData)
			: this(ImportOrder(importData))
		{ }

		public OrderViewModel(OrderViewModel orderVM)
			: this(BaseOrder.CreateOrder(orderVM.Order))
		{ }
		
		private void InitializeViewModels()
		{
		}

		public int CompareTo(OrderViewModel other)
		{
			return _order.CompareTo(other._order);
		}

		#region Pass-through properties for TradeOrder
		public int OrderId
		{
			get { return _order.OrderId; }
		}

		public DateTime OrderInstant
		{
			get { return _order.OrderInstant; }
			set
			{
				_order.OrderInstant = value;
				RaisePropertyChangedEvent(nameof(OrderInstant));
			}
		}

		public OrderType Type
		{
			get { return _order.Type; }
			set
			{
				_order.Type = value;
				RaisePropertyChangedEvent(nameof(Type));
			}
		}

		public Location Location
		{
			get { return _order.Location; }
			set
			{
				_order.Location = value;
				RaisePropertyChangedEvent(nameof(Location));
			}
		}

		public Currency TradeCurrency
		{
			get { return _order.TradeCurrency; }
			set
			{
				_order.TradeCurrency = value;
				RaisePropertyChangedEvent(nameof(TradeCurrency));
			}
		}

		public decimal TradeAmount
		{
			get { return _order.TradeAmount; }
			set
			{
				_order.TradeAmount = value;
				RaisePropertyChangedEvent(nameof(TradeAmount));
			}
		}

		public string DisplayTradeAmount
		{
			get { return _order.DisplayTradeAmount; }
		}

		public Currency BaseCurrency
		{
			get
			{
				ExchangeOrder order = _order as ExchangeOrder;
				if (order !=  null)
				{
					return order.BaseCurrency;
				}
				else { return Currency.None; }
			}
			set
			{
				ExchangeOrder order = _order as ExchangeOrder;
				if (order != null)
				{
					order.BaseCurrency = value;
					RaisePropertyChangedEvent(nameof(BaseCurrency));
				}
			}
		}

		public decimal BaseAmount
		{
			get
			{
				ExchangeOrder order = _order as ExchangeOrder;
				if (order != null)
				{
					return order.BaseAmount;
				}
				else { return default(decimal); }
			}
			set
			{
				ExchangeOrder order = _order as ExchangeOrder;
				if (order != null)
				{
					order.BaseAmount = value;
					RaisePropertyChangedEvent(nameof(BaseAmount));
				}
			}
		}

		public string DisplayBaseAmount
		{
			get
			{
				ExchangeOrder order = _order as ExchangeOrder;
				if (order != null)
				{
					return order.DisplayBaseAmount;
				}
				else { return string.Empty; }
			}
		}

		public decimal BaseFee
		{
			get
			{
				ExchangeOrder order = _order as ExchangeOrder;
				if (order != null)
				{
					return order.BaseFee;
				}
				else { return default(decimal); }
			}
			set
			{
				ExchangeOrder order = _order as ExchangeOrder;
				if (order != null)
				{
					order.BaseFee = value;
					RaisePropertyChangedEvent(nameof(BaseFee));
				}
			}
		}
		#endregion

		#region Import Helper Methods
		private static BaseOrder ImportOrder(string importString)
		{
			string[] orderData = importString.Split(new char[] { ',' }, StringSplitOptions.None);
			BaseOrder newOrder = CreateOrder(orderData[1], !string.IsNullOrWhiteSpace(orderData[3]));

			Currency coinParse;
			decimal amountParse;
			Location locationParse;

			// Parse the order instant
			newOrder.OrderInstant = DateTime.Parse(orderData[0]);

			// Parse the trading location
			if (!Enum.TryParse<Location>(orderData[7], out locationParse))
			{
				throw new OrderImportDataException();
			}
			newOrder.Location = locationParse;

			// Parse the trade currency type
			if (!Enum.TryParse<Currency>(orderData[4], out coinParse))
			{
				throw new OrderImportDataException();
			}
			newOrder.TradeCurrency = coinParse;

			// Parse the trade amount (used for deposits/withdrawals as well)
			if (!decimal.TryParse(orderData[5], out amountParse))
			{
				throw new OrderImportDataException();
			}
			newOrder.TradeAmount = amountParse;

			// Extra parsing for buy/sell orders
			if (newOrder is ExchangeOrder)
			{
				ExchangeOrder newExchangeOrder = (ExchangeOrder)newOrder;

				// Parse the base currency type
				if (!Enum.TryParse<Currency>(orderData[2], out coinParse))
				{
					throw new OrderImportDataException();
				}
				newExchangeOrder.BaseCurrency = coinParse;

				// Parse the base amount
				if (!decimal.TryParse(orderData[3], out amountParse))
				{
					throw new OrderImportDataException();
				}
				newExchangeOrder.BaseAmount = amountParse;

				// Parse the fee amount (stored in the base currency)
				if (!decimal.TryParse(orderData[6], out amountParse))
				{
					throw new OrderImportDataException();
				}
				newExchangeOrder.BaseFee = amountParse;
			}

			return newOrder;
		}

		private static BaseOrder CreateOrder(string tradeType, bool hasBaseAmount)
		{
			OrderType typeParse;
			if (!Enum.TryParse<OrderType>(tradeType, out typeParse))
			{
				throw new OrderImportDataException();
			}

			if (typeParse == OrderType.Buy)
			{
				if (!hasBaseAmount)
				{
					typeParse = OrderType.Deposit;
				}
			}
			else if (typeParse == OrderType.Sell)
			{
				if (!hasBaseAmount)
				{
					typeParse = OrderType.Withdraw;
				}
			}
			else { throw new OrderImportDataException(); }
			return BaseOrder.CreateOrder(typeParse);
		}
		#endregion
	}

	public class OrderVMComparer : IComparer<OrderViewModel>
	{
		public OrderVMComparer(OrderSortType sortBy, ListSortDirection sortDir, bool uniqueSort)
		{
			BaseOrder.SetComparisonParameters(sortBy, sortDir, uniqueSort);
		}

		public int Compare(OrderViewModel x, OrderViewModel y)
		{
			return x.CompareTo(y);
		}
	}
}

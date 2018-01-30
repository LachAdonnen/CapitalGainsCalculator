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
	public class TradeOrderViewModel : ViewModelBase, IComparable<TradeOrderViewModel>
	{
		private TradeOrder _trade;
		public TradeOrder Trade
		{
			get
			{
				return _trade;
			}
		}

		public TradeOrderViewModel()
		{
			InitializeViewModels();
		}

		public TradeOrderViewModel(TradeOrder trade)
			: this()
		{
			_trade = trade;
		}

		public TradeOrderViewModel(TradeType type)
			: this(TradeOrder.CreateTradeOrder(type))
		{ }

		public TradeOrderViewModel(string importData)
			: this(ImportTradeOrder(importData))
		{ }

		public TradeOrderViewModel(TradeOrderViewModel tradeVM)
			: this(TradeOrder.CreateTradeOrder(tradeVM.Trade))
		{ }
		
		private void InitializeViewModels()
		{
		}

		public int CompareTo(TradeOrderViewModel other)
		{
			return _trade.CompareTo(other._trade);
		}

		private int CompareToTrades(TradeOrderViewModel other)
		{
			return _trade.CompareTo(other._trade);
		}

		#region Pass-through properties for TradeOrder
		public int OrderId
		{
			get { return _trade.OrderId; }
		}

		public DateTime OrderInstant
		{
			get { return _trade.OrderInstant; }
			set
			{
				_trade.OrderInstant = value;
				RaisePropertyChangedEvent(nameof(OrderInstant));
			}
		}

		public TradeType Type
		{
			get { return _trade.Type; }
			set
			{
				_trade.Type = value;
				RaisePropertyChangedEvent(nameof(Type));
			}
		}

		public TradingLocation Location
		{
			get { return _trade.Location; }
			set
			{
				_trade.Location = value;
				RaisePropertyChangedEvent(nameof(Location));
			}
		}

		public CoinType TradeCurrency
		{
			get { return _trade.TradeCurrency; }
			set
			{
				_trade.TradeCurrency = value;
				RaisePropertyChangedEvent(nameof(TradeCurrency));
			}
		}

		public decimal TradeAmount
		{
			get { return _trade.TradeAmount; }
			set
			{
				_trade.TradeAmount = value;
				RaisePropertyChangedEvent(nameof(TradeAmount));
			}
		}

		public string DisplayTradeAmount
		{
			get { return _trade.DisplayTradeAmount; }
		}

		public CoinType BaseCurrency
		{
			get
			{
				ExchangeOrder trade = _trade as ExchangeOrder;
				if (trade !=  null)
				{
					return trade.BaseCurrency;
				}
				else { return CoinType.None; }
			}
			set
			{
				ExchangeOrder trade = _trade as ExchangeOrder;
				if (trade != null)
				{
					trade.BaseCurrency = value;
					RaisePropertyChangedEvent(nameof(BaseCurrency));
				}
			}
		}

		public decimal BaseAmount
		{
			get
			{
				ExchangeOrder trade = _trade as ExchangeOrder;
				if (trade != null)
				{
					return trade.BaseAmount;
				}
				else { return default(decimal); }
			}
			set
			{
				ExchangeOrder trade = _trade as ExchangeOrder;
				if (trade != null)
				{
					trade.BaseAmount = value;
					RaisePropertyChangedEvent(nameof(BaseAmount));
				}
			}
		}

		public string DisplayBaseAmount
		{
			get
			{
				ExchangeOrder trade = _trade as ExchangeOrder;
				if (trade != null)
				{
					return trade.DisplayBaseAmount;
				}
				else { return string.Empty; }
			}
		}

		public decimal BaseFee
		{
			get
			{
				ExchangeOrder trade = _trade as ExchangeOrder;
				if (trade != null)
				{
					return trade.BaseFee;
				}
				else { return default(decimal); }
			}
			set
			{
				ExchangeOrder trade = _trade as ExchangeOrder;
				if (trade != null)
				{
					trade.BaseFee = value;
					RaisePropertyChangedEvent(nameof(BaseFee));
				}
			}
		}
		#endregion

		#region Import Helper Methods
		private static TradeOrder ImportTradeOrder(string importString)
		{
			string[] tradeData = importString.Split(new char[] { ',' }, StringSplitOptions.None);
			TradeOrder newOrder = CreateTradeOrder(tradeData[1], !string.IsNullOrWhiteSpace(tradeData[3]));

			CoinType coinParse;
			decimal amountParse;
			TradingLocation locationParse;

			// Parse the order instant
			newOrder.OrderInstant = DateTime.Parse(tradeData[0]);

			// Parse the trading location
			if (!Enum.TryParse<TradingLocation>(tradeData[7], out locationParse))
			{
				throw new TradeImportDataException();
			}
			newOrder.Location = locationParse;

			// Parse the trade currency type
			if (!Enum.TryParse<CoinType>(tradeData[4], out coinParse))
			{
				throw new TradeImportDataException();
			}
			newOrder.TradeCurrency = coinParse;

			// Parse the trade amount (used for deposits/withdrawals as well)
			if (!decimal.TryParse(tradeData[5], out amountParse))
			{
				throw new TradeImportDataException();
			}
			newOrder.TradeAmount = amountParse;

			// Extra parsing for buy/sell orders
			if (newOrder is ExchangeOrder)
			{
				ExchangeOrder newExchangeOrder = (ExchangeOrder)newOrder;

				// Parse the base currency type
				if (!Enum.TryParse<CoinType>(tradeData[2], out coinParse))
				{
					throw new TradeImportDataException();
				}
				newExchangeOrder.BaseCurrency = coinParse;

				// Parse the base amount
				if (!decimal.TryParse(tradeData[3], out amountParse))
				{
					throw new TradeImportDataException();
				}
				newExchangeOrder.BaseAmount = amountParse;

				// Parse the fee amount (stored in the base currency)
				if (!decimal.TryParse(tradeData[6], out amountParse))
				{
					throw new TradeImportDataException();
				}
				newExchangeOrder.BaseFee = amountParse;
			}

			return newOrder;
		}

		private static TradeOrder CreateTradeOrder(string tradeType, bool hasBaseAmount)
		{
			TradeType typeParse;
			if (!Enum.TryParse<TradeType>(tradeType, out typeParse))
			{
				throw new TradeImportDataException();
			}

			if (typeParse == TradeType.Buy)
			{
				if (!hasBaseAmount)
				{
					typeParse = TradeType.Deposit;
				}
			}
			else if (typeParse == TradeType.Sell)
			{
				if (!hasBaseAmount)
				{
					typeParse = TradeType.Withdraw;
				}
			}
			else { throw new TradeImportDataException(); }
			return TradeOrder.CreateTradeOrder(typeParse);
		}
		#endregion
	}

	public class TradeVMComparer : IComparer<TradeOrderViewModel>
	{
		public TradeVMComparer(TradeSortType sortBy, ListSortDirection sortDir, bool uniqueSort)
		{
			TradeOrder.SetComparisonParameters(sortBy, sortDir, uniqueSort);
		}

		public int Compare(TradeOrderViewModel x, TradeOrderViewModel y)
		{
			return x.CompareTo(y);
		}
	}
}

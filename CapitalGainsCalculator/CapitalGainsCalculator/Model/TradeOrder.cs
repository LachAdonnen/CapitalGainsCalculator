using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public abstract class TradeOrder : IComparable<TradeOrder>
	{
		#region Properties
		private static TradeComparer s_comparer;
		private static int s_nextId = 1;

		public int OrderId { get; private set; }

		[Required()]
		public DateTime OrderInstant { get; set; }

		[Required()]
		public TradeType Type { get; set; }

		[Required()]
		public TradingLocation Location { get; set; }

		[Required()]
		public CoinType TradeCurrency { get; set; }

		[Required()]
		public decimal TradeAmount { get; set; }

		public string DisplayTradeAmount
		{
			get
			{
				return FormatCurrencyAmount(TradeAmount, TradeCurrency);
			}
		}

		protected string FormatCurrencyAmount(decimal amount, CoinType currency)
		{
			return string.Format("{0:F2} {1}", amount, currency.ToString());
		}
		#endregion

		#region Constructors
		public TradeOrder()
			: this(-1)
		{ }

		public TradeOrder(int orderId)
		{
			if (orderId < 0)
			{
				this.OrderId = s_nextId++;
			}
			else
			{
				this.OrderId = orderId;
				if (orderId >= s_nextId)
				{
					s_nextId = orderId + 1;
				}
			}

			OnInitializeTrade();
		}

		protected void InitializeTrade(int orderId)
		{

		}

		protected virtual void OnInitializeTrade() { }
		#endregion

		public static TradeOrder CreateTradeOrder(TradeType type)
		{
			return CreateTradeOrder(type, -1);
		}

		private static TradeOrder CreateTradeOrder(TradeType type, int orderId)
		{
			TradeOrder trade = null;
			switch (type)
			{
				case TradeType.Buy:
					trade = new BuyOrder(orderId);
					break;
				case TradeType.Sell:
					trade = new SellOrder(orderId);
					break;
				case TradeType.Deposit:
					trade = new DepositOrder(orderId);
					break;
				case TradeType.Withdraw:
					trade = new WithdrawOrder(orderId);
					break;
			}
			return trade;
		}

		public static TradeOrder CreateTradeOrder(TradeOrder copyFrom)
		{
			TradeOrder newOrder = TradeOrder.CreateTradeOrder(copyFrom.Type, copyFrom.OrderId);
			newOrder.CopyFrom(copyFrom);
			return newOrder;
		}

		private void CopyFrom(TradeOrder copyFrom)
		{
			OnCopyFrom(copyFrom);
		}

		protected virtual void OnCopyFrom(TradeOrder copyFrom)
		{
			this.OrderInstant = new DateTime(copyFrom.OrderInstant.Ticks);
			this.Location = copyFrom.Location;
			this.TradeCurrency = copyFrom.TradeCurrency;
			this.TradeAmount = copyFrom.TradeAmount;
		}

		public bool Filter(TradeFilter filter)
		{
			if (filter.HasEarliestTime &&
				!OnFilterEarliestTime(filter.EarliestTime))
			{ return false; }

			if (filter.HasLatestTime &&
				!OnFilterLatestTime(filter.LatestTime))
			{ return false; }

			if (filter.HasAllowedTypes &&
				!OnFilterType(filter.AllowedTypes))
			{ return false; }

			if (filter.HasAllowedLocations &&
				!OnFilterLocation(filter.AllowedLocations))
			{ return false; }

			if (filter.HasAllowedCurrencies &&
				!OnFilterCurrency(filter.AllowedCurrencies))
			{ return false; }

			return true;
		}

		protected virtual bool OnFilterEarliestTime(DateTime earliestTime)
		{
			return OrderInstant >= earliestTime;
		}

		protected virtual bool OnFilterLatestTime(DateTime latestTime)
		{
			return OrderInstant <= latestTime;
		}

		protected virtual bool OnFilterLocation(TradingLocation[] allowedLocations)
		{
			return allowedLocations.Contains(Location);
		}

		protected virtual bool OnFilterType(TradeType[] allowedTypes)
		{
			return allowedTypes.Contains(Type);
		}

		protected virtual bool OnFilterCurrency(CoinType[] allowedCurrencies)
		{
			return allowedCurrencies.Contains(TradeCurrency);
		}

		public static void SetComparisonParameters(TradeSortType sortBy, ListSortDirection sortDir,
			bool uniqueSort)
		{
			s_comparer = new TradeComparer(sortBy, sortDir, uniqueSort);
		}

		public int CompareTo(TradeOrder other)
		{
			return s_comparer.Compare(this, other);
		}
	}

	public class TradeComparer : IComparer<TradeOrder>
	{
		private TradeSortType sortBy;
		private ListSortDirection sortDir;
		private bool uniqueSort;

		public TradeComparer(TradeSortType sortBy, ListSortDirection sortDir, bool uniqueSort)
		{
			this.sortBy = sortBy;
			this.sortDir = sortDir;
			this.uniqueSort = uniqueSort;
		}

		public int Compare(TradeOrder x, TradeOrder y)
		{
			int testComp = 0;

			// First pass comparison using primary criterion
			switch (sortBy)
			{
				case TradeSortType.Location:
					testComp = CompareLocation(x, y);
					break;
				case TradeSortType.Type:
					testComp = CompareType(x, y);
					break;
				case TradeSortType.TradeCurrency:
					testComp = CompareTradeCurrency(x, y);
					break;
				case TradeSortType.Date:
					testComp = CompareDate(x, y);
					break;
				case TradeSortType.ID:
				default:
					testComp = CompareID(x, y);
					break;
			}

			// Check fallback sorting criteria for a comparison such that now two orders are
			// considered equal.
			if (uniqueSort && testComp == 0)
			{
				// Use the date fallback if applicable
				if (sortBy == TradeSortType.Location ||
					sortBy == TradeSortType.Type ||
					sortBy == TradeSortType.TradeCurrency)
				{
					// Default to descending date order
					testComp = (-1) * CompareDate(x, y);
				}

				// Use the ID as a final fallback
				if (testComp == 0)
				{
					testComp = CompareID(x, y);
				}
			}

			if (sortDir == ListSortDirection.Descending)
			{
				testComp *= -1; // Invert for descending sort
			}
			return testComp;
		}

		private int CompareDate(TradeOrder x, TradeOrder y)
		{
			return x.OrderInstant.CompareTo(y.OrderInstant);
		}

		private int CompareType(TradeOrder x, TradeOrder y)
		{
			return x.Type.ToString().CompareTo(y.Type.ToString());
		}

		private int CompareLocation(TradeOrder x, TradeOrder y)
		{
			return x.Location.ToString().CompareTo(y.Location.ToString());
		}

		private int CompareTradeCurrency(TradeOrder x, TradeOrder y)
		{
			return x.TradeCurrency.ToString().CompareTo(y.TradeCurrency.ToString());
		}

		private int CompareID(TradeOrder x, TradeOrder y)
		{
			return x.OrderId.CompareTo(y.OrderId);
		}

		public static TradeSortType ParseHeaderTag(string tag)
		{
			switch (tag)
			{
				case "Date":
					return TradeSortType.Date;
				case "Type":
					return TradeSortType.Type;
				case "Location":
					return TradeSortType.Location;
				default:
					return TradeSortType.ID;
			}
		}
	}

	public class TradeFilter
	{
		public DateTime EarliestTime { get; set; }
		public bool HasEarliestTime
		{
			get
			{
				return EarliestTime != null;
			}
		}

		public DateTime LatestTime { get; set; }
		public bool HasLatestTime
		{
			get
			{
				return LatestTime != null;
			}
		}

		public TradeType[] AllowedTypes { get; set; }
		public bool HasAllowedTypes
		{
			get
			{
				return (AllowedTypes != null && AllowedTypes.Count() > 0);
			}
		}

		public TradingLocation[] AllowedLocations { get; set; }
		public bool HasAllowedLocations
		{
			get
			{
				return (AllowedLocations != null && AllowedLocations.Count() > 0);
			}
		}

		public CoinType[] AllowedCurrencies { get; set; }
		public bool HasAllowedCurrencies
		{
			get
			{
				return (AllowedCurrencies != null && AllowedCurrencies.Count() > 0);
			}
		}

	}
}
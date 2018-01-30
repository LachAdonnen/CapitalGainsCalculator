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
	public abstract class BaseOrder : IComparable<BaseOrder>
	{
		#region Properties
		private static OrderComparer s_comparer;
		private static int s_nextId = 1;

		public int OrderId { get; private set; }

		[Required()]
		public DateTime OrderInstant { get; set; }

		[Required()]
		public OrderType Type { get; set; }

		[Required()]
		public Location Location { get; set; }

		[Required()]
		public Currency TradeCurrency { get; set; }

		[Required()]
		public decimal TradeAmount { get; set; }

		public string DisplayTradeAmount
		{
			get
			{
				return FormatCurrencyAmount(TradeAmount, TradeCurrency);
			}
		}

		protected string FormatCurrencyAmount(decimal amount, Currency currency)
		{
			return string.Format("{0:F2} {1}", amount, currency.ToString());
		}
		#endregion

		#region Constructors
		public BaseOrder()
			: this(-1)
		{ }

		public BaseOrder(int orderId)
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

			OnInitializeOrder();
		}

		protected virtual void OnInitializeOrder() { }
		#endregion

		public static BaseOrder CreateOrder(OrderType type)
		{
			return CreateOrder(type, -1);
		}

		private static BaseOrder CreateOrder(OrderType type, int orderId)
		{
			BaseOrder trade = null;
			switch (type)
			{
				case OrderType.Buy:
				case OrderType.Sell:
					trade = new ExchangeOrder(orderId) { Type = type };
					break;
				case OrderType.Deposit:
				case OrderType.Withdraw:
					trade = new TransferOrder(orderId) { Type = type };
					break;
			}
			return trade;
		}

		public static BaseOrder CreateOrder(BaseOrder copyFrom)
		{
			BaseOrder newOrder = BaseOrder.CreateOrder(copyFrom.Type, copyFrom.OrderId);
			newOrder.CopyFrom(copyFrom);
			return newOrder;
		}

		private void CopyFrom(BaseOrder copyFrom)
		{
			OnCopyFrom(copyFrom);
		}

		protected virtual void OnCopyFrom(BaseOrder copyFrom)
		{
			this.OrderInstant = new DateTime(copyFrom.OrderInstant.Ticks);
			this.Location = copyFrom.Location;
			this.TradeCurrency = copyFrom.TradeCurrency;
			this.TradeAmount = copyFrom.TradeAmount;
		}

		public bool Filter(OrderFilter filter)
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

		protected virtual bool OnFilterLocation(Location[] allowedLocations)
		{
			return allowedLocations.Contains(Location);
		}

		protected virtual bool OnFilterType(OrderType[] allowedTypes)
		{
			return allowedTypes.Contains(Type);
		}

		protected virtual bool OnFilterCurrency(Currency[] allowedCurrencies)
		{
			return allowedCurrencies.Contains(TradeCurrency);
		}

		public static void SetComparisonParameters(OrderSortType sortBy, ListSortDirection sortDir,
			bool uniqueSort)
		{
			s_comparer = new OrderComparer(sortBy, sortDir, uniqueSort);
		}

		public int CompareTo(BaseOrder other)
		{
			return s_comparer.Compare(this, other);
		}
	}

	public class OrderComparer : IComparer<BaseOrder>
	{
		private OrderSortType sortBy;
		private ListSortDirection sortDir;
		private bool uniqueSort;

		public OrderComparer(OrderSortType sortBy, ListSortDirection sortDir, bool uniqueSort)
		{
			this.sortBy = sortBy;
			this.sortDir = sortDir;
			this.uniqueSort = uniqueSort;
		}

		public int Compare(BaseOrder x, BaseOrder y)
		{
			int testComp = 0;

			// First pass comparison using primary criterion
			switch (sortBy)
			{
				case OrderSortType.Location:
					testComp = CompareLocation(x, y);
					break;
				case OrderSortType.Type:
					testComp = CompareType(x, y);
					break;
				case OrderSortType.TradeCurrency:
					testComp = CompareTradeCurrency(x, y);
					break;
				case OrderSortType.Date:
					testComp = CompareDate(x, y);
					break;
				case OrderSortType.ID:
				default:
					testComp = CompareID(x, y);
					break;
			}

			// Check fallback sorting criteria for a comparison such that now two orders are
			// considered equal.
			if (uniqueSort && testComp == 0)
			{
				// Use the date fallback if applicable
				if (sortBy == OrderSortType.Location ||
					sortBy == OrderSortType.Type ||
					sortBy == OrderSortType.TradeCurrency)
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

		private int CompareDate(BaseOrder x, BaseOrder y)
		{
			return x.OrderInstant.CompareTo(y.OrderInstant);
		}

		private int CompareType(BaseOrder x, BaseOrder y)
		{
			return x.Type.ToString().CompareTo(y.Type.ToString());
		}

		private int CompareLocation(BaseOrder x, BaseOrder y)
		{
			return x.Location.ToString().CompareTo(y.Location.ToString());
		}

		private int CompareTradeCurrency(BaseOrder x, BaseOrder y)
		{
			return x.TradeCurrency.ToString().CompareTo(y.TradeCurrency.ToString());
		}

		private int CompareID(BaseOrder x, BaseOrder y)
		{
			return x.OrderId.CompareTo(y.OrderId);
		}

		public static OrderSortType ParseHeaderTag(string tag)
		{
			switch (tag)
			{
				case "Date":
					return OrderSortType.Date;
				case "Type":
					return OrderSortType.Type;
				case "Location":
					return OrderSortType.Location;
				default:
					return OrderSortType.ID;
			}
		}
	}

	public class OrderFilter
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

		public OrderType[] AllowedTypes { get; set; }
		public bool HasAllowedTypes
		{
			get
			{
				return (AllowedTypes != null && AllowedTypes.Count() > 0);
			}
		}

		public Location[] AllowedLocations { get; set; }
		public bool HasAllowedLocations
		{
			get
			{
				return (AllowedLocations != null && AllowedLocations.Count() > 0);
			}
		}

		public Currency[] AllowedCurrencies { get; set; }
		public bool HasAllowedCurrencies
		{
			get
			{
				return (AllowedCurrencies != null && AllowedCurrencies.Count() > 0);
			}
		}

	}
}
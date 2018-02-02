using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class ExchangeOrder : BaseOrder, IComparable<ExchangeOrder>
	{
		#region Properties
		private static OrderComparer s_comparer;

		private static int s_nextId = 1;
		public int OrderId { get; private set; }

		[Required()]
		public OrderType Type { get; set; }

		public TaxableBuyOrder TaxableBuy { get; private set; }
		public TaxableSellOrder TaxableSell { get; private set; }

		public List<TaxLine> TaxLines
		{
			get
			{
				List<TaxLine> taxLines = new List<TaxLine>();
				taxLines.Add(TaxableSell.TaxLine);
				taxLines.AddRange(TaxableBuy.TaxLines);
				return taxLines;
			}
		}
		#endregion

		#region Constructors
		public ExchangeOrder()
				: this(-1)
		{ }

		private ExchangeOrder(int orderId)
			: base()
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
		}
		#endregion

		public void SplitIntoTaxableOrders()
		{
			TaxableBuy = new TaxableBuyOrder(this);
			TaxableSell = new TaxableSellOrder(this);
		}

		public ExchangeOrder DeepCopy()
		{
			ExchangeOrder newOrder = new ExchangeOrder(this.OrderId)
			{
				Type = this.Type,
				OrderInstant = new DateTime(this.OrderInstant.Ticks),
				OrderExchange = this.OrderExchange,
				TradeCurrency = this.TradeCurrency,
				TradeAmount = this.TradeAmount,
				BaseCurrency = this.BaseCurrency,
				BaseAmount = this.BaseAmount,
				BaseFee = this.BaseFee,
				TaxableBuy = this.TaxableBuy,
				TaxableSell = this.TaxableSell
			};
			return newOrder;
		}
		
		public bool Filter(OrderFilter filter)
		{
			if (filter.HasEarliestTime &&
				!FilterEarliestTime(filter.EarliestTime))
			{ return false; }

			if (filter.HasLatestTime &&
				!FilterLatestTime(filter.LatestTime))
			{ return false; }

			if (filter.HasAllowedTypes &&
				!FilterType(filter.AllowedTypes))
			{ return false; }

			if (filter.HasAllowedLocations &&
				!FilterLocation(filter.AllowedLocations))
			{ return false; }

			if (filter.HasAllowedCurrencies &&
				!FilterCurrency(filter.AllowedCurrencies))
			{ return false; }

			return true;
		}

		protected virtual bool FilterEarliestTime(DateTime earliestTime)
		{
			return OrderInstant >= earliestTime;
		}

		protected virtual bool FilterLatestTime(DateTime latestTime)
		{
			return OrderInstant <= latestTime;
		}

		protected virtual bool FilterLocation(Exchange[] allowedLocations)
		{
			return allowedLocations.Contains(OrderExchange);
		}

		protected virtual bool FilterType(OrderType[] allowedTypes)
		{
			return allowedTypes.Contains(Type);
		}

		protected virtual bool FilterCurrency(Currency[] allowedCurrencies)
		{
			return (allowedCurrencies.Contains(TradeCurrency) ||
					allowedCurrencies.Contains(BaseCurrency));
		}

		public static void SetComparisonParameters(OrderSortType sortBy, ListSortDirection sortDir,
			bool uniqueSort)
		{
			s_comparer = new OrderComparer(sortBy, sortDir, uniqueSort);
		}

		public int CompareTo(ExchangeOrder other)
		{
			return s_comparer.Compare(this, other);
		}
	}

	public class OrderComparer : IComparer<ExchangeOrder>
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

		public int Compare(ExchangeOrder x, ExchangeOrder y)
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

		private int CompareDate(ExchangeOrder x, ExchangeOrder y)
		{
			return x.OrderInstant.CompareTo(y.OrderInstant);
		}

		private int CompareType(ExchangeOrder x, ExchangeOrder y)
		{
			return x.Type.ToString().CompareTo(y.Type.ToString());
		}

		private int CompareLocation(ExchangeOrder x, ExchangeOrder y)
		{
			return x.OrderExchange.ToString().CompareTo(y.OrderExchange.ToString());
		}

		private int CompareTradeCurrency(ExchangeOrder x, ExchangeOrder y)
		{
			return x.TradeCurrency.ToString().CompareTo(y.TradeCurrency.ToString());
		}

		private int CompareID(ExchangeOrder x, ExchangeOrder y)
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

		public Exchange[] AllowedLocations { get; set; }
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
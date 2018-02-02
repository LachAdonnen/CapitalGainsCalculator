using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public abstract class TaxableBaseOrder : BaseOrder, IComparable<TaxableBaseOrder>
	{
		#region Properties
		public ExchangeOrder SourceOrder { get; private set; }

		public bool IsDummy { get { return TradeCurrency == Currency.USD; } }

		public virtual OrderType Type { get; }
		#endregion

		#region Constructors
		public TaxableBaseOrder(ExchangeOrder order)
		{
			if (order.Type == OrderType.Withdraw ||
				order.Type == OrderType.Deposit)
			{
				throw new OrderTypeMismatchException();
			}

			SourceOrder = order;
			OrderInstant = order.OrderInstant;
			OrderExchange = order.OrderExchange;
			BaseCurrency = Currency.USD;
			BaseAmount = ConvertToUsd(order.BaseAmount, order.BaseCurrency);
			BaseFee = ConvertToUsd(order.BaseFee, order.BaseCurrency);
		}
		#endregion

		private static decimal ConvertToUsd(decimal amount, Currency currency)
		{
			//TODO: Need to account for instant as well
			decimal currencyWorth;
			switch (currency)
			{
				case Currency.USDT:
					currencyWorth = 1;
					break;
				case Currency.BTC:
					currencyWorth = 10000;
					break;
				case Currency.ETH:
					currencyWorth = 500;
					break;
				default:
					currencyWorth = 1;
					break;
			}
			return amount * currencyWorth;
		}

		public void ResetTaxLines()
		{
			OnResetTaxLines();
		}

		protected virtual void OnResetTaxLines() { }

		public int CompareTo(TaxableBaseOrder other)
		{
			int result = 0;

			result = OrderInstant.CompareTo(other.OrderInstant);
			if (result != 0) { return result; }

			result = SourceOrder.OrderId.CompareTo(other.SourceOrder.OrderId);
			if (result != 0) { return result; }

			result = OnCompareTo(other);
			return result;
		}

		protected virtual int OnCompareTo(TaxableBaseOrder other)
		{
			return 0;
		}
	}
}

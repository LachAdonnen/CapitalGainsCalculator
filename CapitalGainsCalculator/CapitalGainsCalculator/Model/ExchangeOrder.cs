using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class ExchangeOrder : BaseOrder
	{
		#region Properties
		[Required()]
		public Currency BaseCurrency { get; set; }

		[Required()]
		public decimal BaseAmount { get; set; }

		[Required()]
		public decimal BaseFee { get; set; }

		public string DisplayBaseAmount
		{
			get
			{
				return FormatCurrencyAmount(BaseAmount, BaseCurrency);
			}
		}

		public TaxableBuyOrder TaxableBuy { get; private set; }
		public TaxableSellOrder TaxableSell { get; private set; }
		#endregion

		#region Constructors
		protected ExchangeOrder()
				: base()
		{ }

		public ExchangeOrder(int orderId)
			: base(orderId)
		{ }

		protected override void OnInitializeOrder()
		{
			// Split trade into the buy/sell components
		}
		#endregion

		protected override void OnCopyFrom(BaseOrder copyFrom)
		{
			base.OnCopyFrom(copyFrom);
			ExchangeOrder copyExchange = copyFrom as ExchangeOrder;
			this.BaseCurrency = copyExchange.BaseCurrency;
			this.BaseAmount = copyExchange.BaseAmount;
			this.BaseFee = copyExchange.BaseFee;
		}

		protected override bool OnFilterCurrency(Currency[] allowedCurrencies)
		{
			return (allowedCurrencies.Contains(TradeCurrency) ||
					allowedCurrencies.Contains(BaseCurrency));
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public abstract class ExchangeOrder : TradeOrder
	{
		#region Properties
		[Required()]
		public CoinType BaseCurrency { get; set; }

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
		#endregion

		#region Constructors
		protected ExchangeOrder()
			: base()
		{ }

		public ExchangeOrder(int orderId)
			: base(orderId)
		{ }
		#endregion

		protected override void OnCopyFrom(TradeOrder copyFrom)
		{
			base.OnCopyFrom(copyFrom);
			ExchangeOrder copyExchange = copyFrom as ExchangeOrder;
			this.BaseCurrency = copyExchange.BaseCurrency;
			this.BaseAmount = copyExchange.BaseAmount;
			this.BaseFee = copyExchange.BaseFee;
		}

		protected override bool OnFilterCurrency(CoinType[] allowedCurrencies)
		{
			return (allowedCurrencies.Contains(TradeCurrency) ||
					allowedCurrencies.Contains(BaseCurrency));
		}
	}
}

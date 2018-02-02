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
	public abstract class BaseOrder
	{
		#region Properties
		[Required()]
		public DateTime OrderInstant { get; set; }

		[Required()]
		public Exchange OrderExchange { get; set; }

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

		protected string FormatCurrencyAmount(decimal amount, Currency currency)
		{
			if (amount == 0) { return string.Empty; }
			return string.Format("{0:F2} {1}", amount, currency.ToString());
		}
		#endregion

		#region Constructors
		public BaseOrder()
		{ }
		#endregion
	}
}
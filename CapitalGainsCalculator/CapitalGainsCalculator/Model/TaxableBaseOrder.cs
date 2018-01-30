using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public abstract class TaxableBaseOrder : BaseOrder
	{
		#region Properties
		public decimal BaseAmount { get; set; } // Always in USD

		public ExchangeOrder SourceOrder { get; private set; }
		#endregion

		#region Constructors
		public TaxableBaseOrder(ExchangeOrder order)
		{
			SourceOrder = order;
			OnTaxableAddSource();
		}

		protected override void OnInitializeOrder()
		{
			Type = OrderType.Taxable;
		}

		protected virtual void OnTaxableAddSource()
		{ }
		#endregion

		private static decimal ConvertCurrency()
		{
			return 0;
		}
	}
}

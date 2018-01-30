using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class DepositOrder : TransferOrder
	{
		public DepositOrder()
			: base()
		{ }

		public DepositOrder(int orderId)
			: base(orderId)
		{ }

		protected override void OnInitializeTrade()
		{
			Type = TradeType.Deposit;
		}
	}
}

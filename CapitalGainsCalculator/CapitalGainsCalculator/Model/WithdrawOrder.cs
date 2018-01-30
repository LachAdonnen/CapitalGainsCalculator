using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class WithdrawOrder : TransferOrder
	{
		public WithdrawOrder()
			: base()
		{ }

		public WithdrawOrder(int orderId)
			: base(orderId)
		{ }

		protected override void OnInitializeTrade()
		{
			Type = TradeType.Withdraw;
		}
	}
}

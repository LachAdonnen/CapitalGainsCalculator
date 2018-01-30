using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class TransferOrder : BaseOrder
	{
		#region Constructors
		public TransferOrder()
			: base()
		{ }

		public TransferOrder(int orderId)
			: base(orderId)
		{ }
		#endregion
	}
}

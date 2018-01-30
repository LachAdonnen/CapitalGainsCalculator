using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	internal class TradeTypeMismatchException : Exception
	{
		public TradeTypeMismatchException()
			: base("Invalid trade type used.")
		{ }
	}
}

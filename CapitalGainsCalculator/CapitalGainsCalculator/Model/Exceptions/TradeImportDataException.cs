using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	internal class TradeImportDataException : Exception
	{
		public TradeImportDataException()
			: base("Attempted to import invalid trade data.")
		{ }
	}
}

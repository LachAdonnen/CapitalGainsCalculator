using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	internal class OrderImportDataException : Exception
	{
		public OrderImportDataException()
			: base("Attempted to import invalid order data.")
		{ }
	}
}

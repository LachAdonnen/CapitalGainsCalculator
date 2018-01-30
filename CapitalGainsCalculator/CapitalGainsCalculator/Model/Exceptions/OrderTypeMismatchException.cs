using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	internal class OrderTypeMismatchException : Exception
	{
		public OrderTypeMismatchException()
			: base("Invalid order type used.")
		{ }
	}
}

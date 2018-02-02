using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	internal class DuplicateTaxEventEcxeption : Exception
	{
		public DuplicateTaxEventEcxeption()
			: base("Attempted to add a tax line to multiple tax events.")
		{ }
	}
}

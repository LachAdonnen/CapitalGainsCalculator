﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGainsCalculator.Model
{
	[Serializable]
	public class TaxLine
	{
		public decimal TradeAmount { get; set; }
		public BaseOrder TradeOrder { get; set; }
		public TaxEvent TaxEvent { get; set; }
		
		public TaxLine(BaseOrder tradeOrder, decimal tradeAmount)
		{
			this.TradeOrder = tradeOrder;
			this.TradeAmount = tradeAmount;
		}
	}
}

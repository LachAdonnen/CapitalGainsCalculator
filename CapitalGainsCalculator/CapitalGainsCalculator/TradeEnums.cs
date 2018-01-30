using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace CapitalGainsCalculator
{
	public enum CoinType
	{
		USD, BTC, ETH, LTC,
		FST, ADA, APX, ARK,
		BAY, BCN, BLITZ, BTS, CVC,
		DGB, EMC2, FIRST, GAS, GRC,
		GRS, IOC, LSK, NAV,
		NEO, NXC, NXS, OMG,
		OMNI, SC, SHIFT, SJCX,
		SLR, STR, TKS, VRM,
		VTC, XCP, XRP, ZEC,
		USDT,
		None
	}

	public enum TradingLocation
	{
		Coinbase,
		GDAX,
		Bittrex,
		Poloniex,
		Bitfinex
	}

	public enum TradeType
	{
		Buy,
		Sell,
		Deposit,
		Withdraw,
	}

	public enum TradeSortType
	{
		ID,
		Date,
		Type,
		Location,
		TradeCurrency
	}

	public enum TaxCalculationType
	{
		LIFO, FIFO
	}
}

﻿using CheapLoc;

namespace PriceCheck
{
	public class Result
	{
		public static Result Success = new Result(Loc.Localize("SellOnMarketboard", "Sell on marketboard"), 45);
		public static Result FailedToGetData = new Result(Loc.Localize("FailedToGetData", "Failed to get data"), 17);
		public static Result NoDataAvailable = new Result(Loc.Localize("NoDataAvailable", "No data available"), 17);
		public static Result NoRecentDataAvailable = new Result(Loc.Localize("NoRecentDataAvailable", "No recent data"), 17);
		public static Result BelowVendor = new Result(Loc.Localize("BelowVendor", "Sell to vendor"), 25);
		public static Result BelowMinimum = new Result(Loc.Localize("BelowMinimum", "Below minimum price"), 25);
		private readonly ushort _colorKey;
		private readonly string _description;
		
		private Result(string resultDesc, ushort colorKey)
		{
			_description = resultDesc;
			_colorKey = colorKey;
		}

		public override string ToString()
		{
			return _description;
		}

		public ushort ColorKey()
		{
			return _colorKey;
		}

		public static void UpdateLanguage()
		{
			Success = new Result(Loc.Localize("SellOnMarketboard", "Sell on marketboard"), 45);
			FailedToGetData = new Result(Loc.Localize("FailedToGetData", "Failed to get data"), 17);
			NoDataAvailable = new Result(Loc.Localize("NoDataAvailable", "No data available"), 17);
			NoRecentDataAvailable = new Result(Loc.Localize("NoRecentDataAvailable", "No recent data"), 17);
			BelowVendor = new Result(Loc.Localize("BelowVendor", "Sell to vendor"), 25);
			BelowMinimum = new Result(Loc.Localize("BelowMinimum", "Below minimum price"), 25);
		}
	}
}
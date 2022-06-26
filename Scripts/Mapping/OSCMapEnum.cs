/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

namespace extOSC.Mapping
{
	public enum OSCMapType
	{
		Float,

		Int,

		FloatToBool,

		BoolToFloat,

		IntToBool,

		BoolToInt
	}

	public enum OSCMapLogic
	{
		GreaterOrEquals,

		Greater,

		LessOrEquals,

		Less,

		Equals
	}
}
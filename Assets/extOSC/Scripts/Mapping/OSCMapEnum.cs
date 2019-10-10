/* Copyright (c) 2019 ExT (V.Sigalkin) */

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
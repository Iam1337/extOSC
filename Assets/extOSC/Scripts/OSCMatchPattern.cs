/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

namespace extOSC
{
	public class OSCMatchPattern
	{
		#region Public Methods

		public OSCValueType[] Types { get; }

		#endregion

		#region Public Methods

		public OSCMatchPattern(params OSCValueType[] types)
		{
			Types = types;
		}

		#endregion
	}
}
/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using extOSC.Core.Events;

namespace extOSC.Core
{
	public interface IOSCBindBundle
	{
		#region Public Vars

		OSCEventBundle Callback { get; }

		#endregion
	}
}
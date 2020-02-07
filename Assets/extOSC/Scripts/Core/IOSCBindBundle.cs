/* Copyright (c) 2020 ExT (V.Sigalkin) */

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
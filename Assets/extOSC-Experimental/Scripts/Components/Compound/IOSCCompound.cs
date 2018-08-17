/* Copyright (c) 2018 ExT (V.Sigalkin) */

namespace extOSC.Components.Compounds
{
    public interface IOSCCompound
	{
		#region Vars

		#endregion

		#region Methods

		OSCCompoundElement[] GetElements();

		void AddElement(OSCCompoundElement element);

		void RemoveElement(OSCCompoundElement element);

		#endregion
	}
}
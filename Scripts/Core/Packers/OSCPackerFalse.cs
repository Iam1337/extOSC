/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

namespace extOSC.Core.Packers
{
	internal class OSCPackerFalse : OSCPacker<bool>
	{
		#region Public Methods

		public override OSCValueType PackerType => OSCValueType.False;

		#endregion

		#region Protected Methods

		protected override bool BytesToValue(byte[] buffer, ref int index) => false;

		protected override void ValueToBytes(byte[] buffer, ref int index, bool value)
		{ }

		#endregion
	}
}
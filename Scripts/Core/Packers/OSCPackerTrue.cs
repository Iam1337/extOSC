/* Copyright (c) 2019 ExT (V.Sigalkin) */

namespace extOSC.Core.Packers
{
	internal class OSCPackerTrue : OSCPacker<bool>
	{
		#region Public Methods

		public override OSCValueType PackerType => OSCValueType.True;

		#endregion

		#region Protected Methods

		protected override bool BytesToValue(byte[] buffer, ref int index) => true;

		protected override void ValueToBytes(byte[] buffer, ref int index, bool value)
		{ }

		#endregion
	}
}
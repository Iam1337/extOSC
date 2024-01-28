/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Core.Packers
{
	internal class OSCPackerColor : OSCPacker<Color>
	{
		#region Public Methods

		public override OSCValueType PackerType => OSCValueType.Color;

		#endregion

		#region Protected Methods

		protected override Color BytesToValue(byte[] buffer, ref int index)
		{
			index += 4;

			return new Color32(buffer[index - 4],
							   buffer[index - 3],
							   buffer[index - 2],
							   buffer[index - 1]);
		}

		protected override void ValueToBytes(byte[] buffer, ref int index, Color value)
		{
			var color = (Color32) value;

			buffer[index++] = color.r;
			buffer[index++] = color.g;
			buffer[index++] = color.b;
			buffer[index++] = color.a;
		}

		#endregion
	}
}
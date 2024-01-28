/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using System;

namespace extOSC.Mapping
{
	[Serializable]
	public class OSCMapValue
	{
		#region Public Vars

		public OSCMapType Type;

		public float InputMin;

		public float InputMax = 1;

		public float OutputMin;

		public float OutputMax = 1;

		public bool Clamp = true;

		public float Value = 1;

		public float FalseValue;

		public float TrueValue = 1;

		public OSCMapLogic Logic;

		#endregion

		#region Public Methods

		public OSCValue Map(OSCValue value)
		{
			//FLOAT MAP
			if (Type == OSCMapType.Float)
			{
				value.FloatValue = OSCUtilities.Map(value.FloatValue, InputMin, InputMax, OutputMin, OutputMax, Clamp);
			}

			// FLOAT TO BOOL MAP
			else if (Type == OSCMapType.FloatToBool)
			{
				if (Logic == OSCMapLogic.GreaterOrEquals)
					return OSCValue.Bool(value.FloatValue >= Value);
				if (Logic == OSCMapLogic.Greater)
					return OSCValue.Bool(value.FloatValue > Value);
				if (Logic == OSCMapLogic.LessOrEquals)
					return OSCValue.Bool(value.FloatValue <= Value);
				if (Logic == OSCMapLogic.Less)
					return OSCValue.Bool(value.FloatValue < Value);
				if (Logic == OSCMapLogic.Equals)
					return OSCValue.Bool(Math.Abs(value.FloatValue - Value) <= float.Epsilon);
			}

			// BOOL TO FLOAT MAP
			else if (Type == OSCMapType.BoolToFloat)
			{
				return OSCValue.Float(value.BoolValue ? TrueValue : FalseValue);
			}

			// INT MAP
			else if (Type == OSCMapType.Int)
			{
				value.IntValue = (int) OSCUtilities.Map(value.IntValue, InputMin, InputMax, OutputMin, OutputMax, Clamp);
			}

			// INT TO BOOL MAP
			else if (Type == OSCMapType.IntToBool)
			{
				if (Logic == OSCMapLogic.GreaterOrEquals)
					return OSCValue.Bool(value.IntValue >= Value);
				if (Logic == OSCMapLogic.Greater)
					return OSCValue.Bool(value.IntValue > Value);
				if (Logic == OSCMapLogic.LessOrEquals)
					return OSCValue.Bool(value.IntValue <= Value);
				if (Logic == OSCMapLogic.Less)
					return OSCValue.Bool(value.IntValue < Value);
				if (Logic == OSCMapLogic.Equals)
					return OSCValue.Bool(Math.Abs(value.IntValue - Value) <= float.Epsilon);
			}

			// BOOL TO INT MAP
			else if (Type == OSCMapType.BoolToInt)
			{
				return OSCValue.Int((int) (value.BoolValue ? TrueValue : FalseValue));
			}

			return value;
		}

		#endregion
	}
}
/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using System;

using extOSC.Core.Reflection;

namespace extOSC.Components.Compounds
{
	[Serializable]
	public class OSCCompoundElement
	{
		#region Public Vars

		public OSCValueType ValueType
		{
			get { return valueType; }
			set
			{
				if (valueType == value)
					return;

				valueType = value;

				UpdateCachedReferences();
			}
		}

		public OSCReflectionMember ValueSource
		{
			get { return valueSource; }
			set
			{
				if (valueSource == value)
					return;

				valueSource = value;

				UpdateCachedReferences();
			}
		}

		#endregion

		#region Protected Vars

		[SerializeField]
		protected OSCValueType valueType;

		[SerializeField]
		protected OSCReflectionMember valueSource;

		#endregion

		#region Private Vars

		private OSCReflectionProperty _cachedProperty;

		#endregion

		#region Public Vars

		public void UpdateCachedReferences()
		{
			if (valueSource != null && valueSource.IsValid())
			{
				var property = valueSource.GetProperty();
				if (property != null && property.PropertyType == OSCValue.GetType(valueType))
				{
					_cachedProperty = property;
					return;
				}
			}

			_cachedProperty = null;
		}

		public OSCReflectionProperty GetProperty()
		{
			if (_cachedProperty == null)
			{
				UpdateCachedReferences();
			}

			return _cachedProperty;
		}

		#endregion
	}
}
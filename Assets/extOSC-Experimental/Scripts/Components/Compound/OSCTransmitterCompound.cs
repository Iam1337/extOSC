/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

namespace extOSC.Components.Compounds
{
	[AddComponentMenu("extOSC/Components/Transmitter/Transmitter Compound Message")]
	public class OSCTransmitterCompound : OSCTransmitterComponent
	{
		#region Public Vars

		public bool SendOnChanged
		{
			get { return sendOnChanged; }
			set { sendOnChanged = value; }
		}

		public float SendInterval
		{
			get { return sendInterval; }
			set { sendInterval = Mathf.Max(value, 0); }
		}

		#endregion

		#region Protected Vars

		[SerializeField]
		protected List<OSCCompoundElement> elements = new List<OSCCompoundElement>();

		[SerializeField]
		protected bool sendOnChanged = true;

		[SerializeField]
		protected float sendInterval = 0;

		#endregion

		#region Private Vars

		private List<object> _cachedValues = new List<object>();

		private float _sendTimer;

		#endregion

		#region Unity Methods

		protected virtual void Update()
		{
			if (sendOnChanged)
			{
				var changed = false;

				for (var i = 0; i < elements.Count; i++)
				{
					var element = elements[i];
					var currentValue = GetPropertyValue(element);

					if (!currentValue.Equals(_cachedValues[i]))
					{
						_cachedValues[i] = currentValue;
						changed = true;
					}
				}
				
				if (changed)
					Send();
			}
			else
			{
				sendInterval = Mathf.Max(sendInterval, 0);
				if (sendInterval < float.Epsilon)
				{
					Send();
				}
				else
				{
					_sendTimer += Time.deltaTime;

					if (sendInterval < _sendTimer)
					{
						Send();

						_sendTimer = 0;
					}
				}
			}
		}

		protected void OnEnable()
		{
			UpdateCachedReferences();
		}

#if UNITY_EDITOR
		protected void OnValidate()
		{
			UpdateCachedReferences();
		}
#endif

		#endregion

		#region Public Methods

		public OSCCompoundElement[] GetElements()
		{
			return elements.ToArray();
		}

		public void AddElement(OSCCompoundElement element)
		{
			if (elements.Contains(element))
				return;
			
			elements.Add(element);

			UpdateCachedReferences();
		}

		public void RemoveElement(OSCCompoundElement element)
		{
			if (!elements.Contains(element))
				return;

			elements.Remove(element);

			UpdateCachedReferences();
		}

		#endregion

		#region Protected Methods

		protected override bool FillMessage(OSCMessage message)
		{
			foreach (var element in elements)
			{
				message.AddValue(CreateValue(element));
			}

			return true;
		}

		#endregion

		#region Private Methods

		private void UpdateCachedReferences()
		{
			_cachedValues.Clear();

			foreach (var element in elements)
			{
				// Refresh Property
				element.UpdateCachedReferences();

				// Get value
				_cachedValues.Add(GetPropertyValue(element));
			}
		}

		private OSCValue CreateValue(OSCCompoundElement element)
		{
			if (element.ValueType == OSCValueType.Unknown ||
			    element.ValueType == OSCValueType.Array ||
			    element.ValueType == OSCValueType.Null)
			{
				return OSCValue.Null();
			}

			if (element.ValueType == OSCValueType.Impulse)
			{
				return OSCValue.Impulse();
			}

			var property = element.GetProperty();
			if (property == null) return OSCValue.Null();

			return new OSCValue(element.ValueType, property.Value); // That not safe.
		}

		private object GetPropertyValue(OSCCompoundElement element)
		{
			if (element.ValueType == OSCValueType.Unknown ||
			    element.ValueType == OSCValueType.Impulse ||
				element.ValueType == OSCValueType.Array ||
			    element.ValueType == OSCValueType.Null)
			{
				return null;
			}

			var property = element.GetProperty();
			if (property == null) return null;

			return property.Value; // That not safe.
		}

		#endregion
	}
}
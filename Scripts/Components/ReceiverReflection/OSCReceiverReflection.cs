/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.Serialization;

using System;
using System.Collections.Generic;

using extOSC.Core.Reflection;

namespace extOSC.Components.ReceiverReflections
{
	public abstract class OSCReceiverReflection : OSCReceiverComponent
	{
		#region Public Vars

		public abstract Type ReceiverType { get; }

		#endregion

		#region Protected Vars

		[SerializeField]
		[FormerlySerializedAs("reflectionMembers")]
		private List<OSCReflectionMember> _reflectionMembers = new List<OSCReflectionMember>();

		protected readonly Dictionary<OSCReflectionMember, OSCReflectionProperty> _cachedProperties = new Dictionary<OSCReflectionMember, OSCReflectionProperty>();

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			UpdateCachedReferences();
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			UpdateCachedReferences();
		}
#endif

		#endregion

		#region Public Methods

		public OSCReflectionMember[] GetMembers()
		{
			return _reflectionMembers.ToArray();
		}

		public void AddMember(OSCReflectionMember member)
		{
			if (_reflectionMembers.Contains(member))
				return;

			_reflectionMembers.Add(member);

			UpdateCachedReferences();
		}

		public void RemoveMember(OSCReflectionMember member)
		{
			if (!_reflectionMembers.Contains(member))
				return;

			_reflectionMembers.Remove(member);

			UpdateCachedReferences();
		}

		public void UpdateMembers()
		{
			UpdateCachedReferences();
		}

		#endregion

		#region Private Methods

		private void UpdateCachedReferences()
		{
			_cachedProperties.Clear();

			foreach (var reflectionMember in _reflectionMembers)
			{
				if (reflectionMember == null)
					continue;

				if (_cachedProperties.ContainsKey(reflectionMember))
					_cachedProperties.Add(reflectionMember, null);

				if (reflectionMember.IsValid())
					_cachedProperties[reflectionMember] = reflectionMember.GetProperty();
				else
					_cachedProperties[reflectionMember] = null;
			}
		}

		#endregion
	}

	public abstract class OSCReceiverReflection<T> : OSCReceiverReflection
	{
		#region Public Vars

		public override Type ReceiverType => typeof(T);

		#endregion

		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (ProcessMessage(message, out var value))
			{
				foreach (var property in _cachedProperties.Values)
				{
					if (property != null)
						property.SetValue(value);
				}
			}
		}

		protected abstract bool ProcessMessage(OSCMessage message, out T value);

		#endregion
	}
}
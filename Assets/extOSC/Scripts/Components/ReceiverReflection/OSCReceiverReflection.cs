/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

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
        protected List<OSCReflectionMember> reflectionMembers = new List<OSCReflectionMember>();

        protected Dictionary<OSCReflectionMember, OSCReflectionProperty> cachedProperties = new Dictionary<OSCReflectionMember, OSCReflectionProperty>();

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
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
			return reflectionMembers.ToArray();
		}

		public void AddMember(OSCReflectionMember member)
        {
            if (reflectionMembers.Contains(member))
                return;

            reflectionMembers.Add(member);

            UpdateCachedReferences();
        }

        public void RemoveMember(OSCReflectionMember member)
        {
            if (!reflectionMembers.Contains(member))
                return;

            reflectionMembers.Remove(member);

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
            cachedProperties.Clear();

            foreach (var reflectionMember in reflectionMembers)
            {
                if (reflectionMember == null)
                    continue;

                if (cachedProperties.ContainsKey(reflectionMember))
                    cachedProperties.Add(reflectionMember, null);

                if (reflectionMember.IsValid())
                    cachedProperties[reflectionMember] = reflectionMember.GetProperty();
                else
                    cachedProperties[reflectionMember] = null;
            }
        }

        #endregion
    }

    public abstract class OSCReceiverReflection<T> : OSCReceiverReflection
    {
        #region Public Vars

        public override Type ReceiverType
        {
            get { return typeof(T); }
        }

        #endregion

        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            T value;

            if (ProcessMessage(message, out value))
            {
                foreach (var property in cachedProperties.Values)
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
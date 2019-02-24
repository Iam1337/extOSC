/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Reflection;

namespace extOSC.Core.Reflection
{
    [Serializable]
    public class OSCReflectionMember
    {
        #region Public Vars

        public Component Target;

        public string MemberName;

        #endregion

        #region Public Methods

        public bool IsValid()
        {
            return GetMemberInfo() != null;
        }

        public Type GetMemberType()
        {
            var memberInfo = GetMemberInfo();
            if (memberInfo != null)
            {
                if (memberInfo is FieldInfo)
                    return ((FieldInfo)memberInfo).FieldType;

                if (memberInfo is PropertyInfo)
                    return ((PropertyInfo)memberInfo).PropertyType;

                if (memberInfo is MethodInfo)
                    return ((MethodInfo)memberInfo).ReturnType;
            }

            return null;
        }

        public OSCReflectionType GetReflectionType()
        {
            var memberInfo = GetMemberInfo();
            if (memberInfo != null)
            {
                if (memberInfo is FieldInfo)
                    return OSCReflectionType.Field;
                if (memberInfo is PropertyInfo)
                    return OSCReflectionType.Property;
                if (memberInfo is MethodInfo)
                    return OSCReflectionType.Method;
            }

            return OSCReflectionType.Unknown;
        }

        public Type GetInputType()
        {
            var memberInfo = GetMemberInfo();
            if (memberInfo != null)
            {
                if (memberInfo is FieldInfo)
                    return ((FieldInfo)memberInfo).FieldType;

                if (memberInfo is PropertyInfo)
                    return ((PropertyInfo)memberInfo).PropertyType;

                if (memberInfo is MethodInfo)
                {
                    var parameters = ((MethodInfo)memberInfo).GetParameters();
                    if (parameters.Length > 0) return parameters[0].ParameterType;
                }
            }

            return null;
        }

        public OSCReflectionProperty GetProperty()
        {
            return OSCReflectionProperty.Create(Target, GetMemberInfo());
        }

        public MemberInfo GetMemberInfo()
        {
            if (Target == null || string.IsNullOrEmpty(MemberName))
                return null;

            var members = Target.GetType().GetMember(MemberName);
            return members.Length > 0 ? members[0] : null;
        }

        #endregion
    }
}
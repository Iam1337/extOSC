/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace extOSC.Core.Reflection
{
	public static class OSCReflection
	{
		#region Static Private Vars

		private static readonly BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.Instance;

		#endregion

		#region Static Public Methods

		public static MemberInfo[] GetMembers(Type targetType, OSCReflectionType memberTypes)
		{
			var members = new List<MemberInfo>();
			var typeMembers = targetType.GetMembers(_bindingFlags);

			foreach (var memberInfo in typeMembers)
			{
				if (memberInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any())
					continue;

				if ((memberTypes & OSCReflectionType.Field) == OSCReflectionType.Field && memberInfo is FieldInfo)
				{
					members.Add(memberInfo);
				}
				else if ((memberTypes & OSCReflectionType.Property) == OSCReflectionType.Property && memberInfo is PropertyInfo)
				{
					members.Add(memberInfo);
				}
				else if ((memberTypes & OSCReflectionType.Method) == OSCReflectionType.Method && memberInfo is MethodInfo)
				{
					var methodInfo = (MethodInfo) memberInfo;
					if (methodInfo.IsSpecialName) continue;

					members.Add(memberInfo);
				}
			}

			return members.ToArray();
		}

		public static MemberInfo[] GetMembersByType(object memberTarget, Type valueType, OSCReflectionAccess valueAccess, OSCReflectionType memberTypes)
		{
			return GetMembersByType(memberTarget.GetType(), valueType, valueAccess, memberTypes);
		}

		public static MemberInfo[] GetMembersByType(Type targetType, Type valueType, OSCReflectionAccess valueAccess, OSCReflectionType memberTypes)
		{
			var members = new List<MemberInfo>();
			var targetMembers = GetMembers(targetType, memberTypes);

			foreach (var memberInfo in targetMembers)
			{
				if (memberInfo is FieldInfo fieldInfo)
				{
					if (fieldInfo.FieldType == valueType)
						members.Add(fieldInfo);
				}
				else if (memberInfo is PropertyInfo propertyInfo)
				{
					if (!CheckAccess(propertyInfo, valueAccess))
						continue;

					if (propertyInfo.PropertyType == valueType)
						members.Add(propertyInfo);
				}
				else if (memberInfo is MethodInfo methodInfo)
				{
					if (!CheckAccess(methodInfo, valueAccess))
						continue;

					if (valueAccess == OSCReflectionAccess.Any)
					{
						if (GetMethodReadType(methodInfo) == valueType ||
							GetMethodWriteType(methodInfo) == valueType)
							members.Add(methodInfo);

						continue;
					}

					if (valueAccess == OSCReflectionAccess.ReadWrite)
					{
						if (GetMethodReadType(methodInfo) == valueType ||
							GetMethodWriteType(methodInfo) == valueType)
							members.Add(methodInfo);

						continue;
					}

					if (valueAccess == OSCReflectionAccess.Read)
					{
						if (GetMethodReadType(methodInfo) == valueType)
							members.Add(methodInfo);

						continue;
					}

					if (valueAccess == OSCReflectionAccess.Write)
					{
						if (GetMethodWriteType(methodInfo) == valueType)
							members.Add(methodInfo);
					}
				}

			}

			return members.ToArray();
		}

		public static bool CheckAccess(PropertyInfo propertyInfo, OSCReflectionAccess access)
		{
			if (access == OSCReflectionAccess.ReadWrite && propertyInfo.CanWrite && propertyInfo.CanRead)
				return true;
			if (access == OSCReflectionAccess.Read && propertyInfo.CanRead)
				return true;
			if (access == OSCReflectionAccess.Write && propertyInfo.CanWrite)
				return true;
			if (access == OSCReflectionAccess.Any)
				return true;

			return false;
		}

		public static bool CheckAccess(MethodInfo methodInfo, OSCReflectionAccess access)
		{
			if (access == OSCReflectionAccess.Read)
				return !(methodInfo.ReturnType == typeof(void)) && GetMethodWriteType(methodInfo) == null;
			if (access == OSCReflectionAccess.Write)
				return GetMethodWriteType(methodInfo) != null;
			if (access == OSCReflectionAccess.ReadWrite)
				return false;
			if (access == OSCReflectionAccess.Any)
				return CheckAccess(methodInfo, OSCReflectionAccess.Read) || CheckAccess(methodInfo, OSCReflectionAccess.Write);

			return false;
		}

		public static Type GetMethodReadType(MethodInfo methodInfo)
		{
			return methodInfo.ReturnType;
		}

		public static Type GetMethodWriteType(MethodInfo methodInfo)
		{
			var methodParameters = methodInfo.GetParameters();
			if (methodParameters.Length == 1)
				return methodParameters[0].ParameterType;

			return null;
		}

		//public static MemberInfo[] GetMembers(object memberTarget, OSCReflectionType memberTypes)
		//{
		//    return GetMembers(memberTarget.GetType(), memberTypes);
		//}

		//public static MemberInfo[] GetMembersByAccess(object target, OSCReflectionAccess valueAccess, OSCReflectionType memberTypes)
		//{
		//    return GetMembersByAccess(target.GetType(), valueAccess, memberTypes);
		//}

		//public static MemberInfo[] GetMembersByAccess(Type targetType, OSCReflectionAccess valueAccess, OSCReflectionType memberTypes)
		//{
		//    var members = new List<MemberInfo>();
		//    var targetMembers = GetMembers(targetType, memberTypes);

		//    foreach (var memberInfo in targetMembers)
		//    {
		//        if (memberInfo is FieldInfo)
		//        {
		//            members.Add(memberInfo);
		//        }
		//        else if (memberInfo is PropertyInfo)
		//        {
		//            var propertyInfo = (PropertyInfo)memberInfo;

		//            if (!CheckAccess(propertyInfo, valueAccess))
		//                continue;

		//            members.Add(memberInfo);
		//        }
		//        else if (memberInfo is MethodInfo)
		//        {
		//            var methodInfo = memberInfo as MethodInfo;

		//            if (!CheckAccess(methodInfo, valueAccess))
		//                continue;

		//            members.Add(memberInfo);
		//        }
		//    }

		//    return members.ToArray();
		//}

		//public static bool CheckAccess(MemberInfo memberInfo, OSCReflectionAccess access)
		//{
		//    if (memberInfo is FieldInfo)
		//        return true;
		//    if (memberInfo is PropertyInfo)
		//        return CheckAccess((PropertyInfo)memberInfo, access);
		//    if (memberInfo is MethodInfo)
		//        return CheckAccess((MethodInfo)memberInfo, access);

		//    return false;
		//}

		#endregion
	}
}
/* Copyright (c) 2020 ExT (V.Sigalkin) */

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
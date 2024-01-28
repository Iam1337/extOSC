/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using System;
using System.Reflection;

namespace extOSC.Core.Reflection
{
	public class OSCReflectionProperty
	{
		#region Static Public Methods

		public static OSCReflectionProperty Create(object target, MemberInfo memberInfo)
		{
			if (memberInfo == null)
				return null;

			if (memberInfo is FieldInfo fieldInfo)
				return new OSCReflectionProperty(target, fieldInfo);
			if (memberInfo is PropertyInfo propertyInfo)
				return new OSCReflectionProperty(target, propertyInfo);
			if (memberInfo is MethodInfo methodInfo)
				return new OSCReflectionProperty(target, methodInfo);

			return null;
		}

		#endregion

		#region Public Vars

		public object Value
		{
			get => GetValue();
			set => SetValue(value);
		}

		public Type ValueType => _valueType;

		#endregion

		#region Private Vars

		private readonly object _target;

		private readonly OSCReflectionType _propertyType;

		private readonly Type _valueType;

		// FIELD
		private readonly FieldInfo _fieldInfo;

		// PROPERTY
		private readonly MethodInfo _propertyGetter;

		private readonly MethodInfo _propertySetter;

		// Method
		private readonly MethodInfo _methodInfo;

		private readonly Type _methodWriteType;

		#endregion

		#region Public Methods

		public object GetValue()
		{
			object value = null;

			if (_propertyType == OSCReflectionType.Field)
				value = GetFieldValue();
			else if (_propertyType == OSCReflectionType.Property)
				value = GetPropertyValue();
			else if (_propertyType == OSCReflectionType.Method)
				value = GetMethodValue();

			return value;
		}

		public void SetValue(object value)
		{
			if (_propertyType == OSCReflectionType.Field)
				SetFieldValue(value);
			else if (_propertyType == OSCReflectionType.Property)
				SetPropertyValue(value);
			else if (_propertyType == OSCReflectionType.Method)
				SetMethodValue(value);
		}

		#endregion

		#region Private Methods

		private OSCReflectionProperty(object target, FieldInfo fieldInfo)
		{
			_target = target;
			_propertyType = OSCReflectionType.Field;

			_fieldInfo = fieldInfo;

			_valueType = fieldInfo.FieldType;
		}

		private OSCReflectionProperty(object target, PropertyInfo propertyInfo)
		{
			_target = target;
			_propertyType = OSCReflectionType.Property;

			_propertyGetter = propertyInfo.GetGetMethod();
			_propertySetter = propertyInfo.GetSetMethod();

			_valueType = propertyInfo.PropertyType;
		}

		private OSCReflectionProperty(object target, MethodInfo methodInfo)
		{
			_target = target;
			_propertyType = OSCReflectionType.Method;

			_methodInfo = methodInfo;
			_methodWriteType = OSCReflection.GetMethodWriteType(methodInfo);

			_valueType = methodInfo.ReturnType;
		}

		// FIELD
		private object GetFieldValue()
		{
			return _fieldInfo == null ? null : _fieldInfo.GetValue(_target);
		}

		private void SetFieldValue(object value)
		{
			if (_fieldInfo == null || _fieldInfo.IsLiteral)
				return;

			if (value == null || _valueType.IsInstanceOfType(value))
			{
				_fieldInfo.SetValue(_target, value);
			}
			else
			{
				_fieldInfo.SetValue(_target, Convert.ChangeType(value, _valueType));
			}
		}

		// PROPERTY
		private object GetPropertyValue()
		{
			if (_propertyGetter == null)
				return null;

			return _propertyGetter.Invoke(_target, null);
		}

		private void SetPropertyValue(object value)
		{
			if (_propertySetter == null)
				return;

			var parameters = new object[1];

			if (value == null || _valueType.IsInstanceOfType(value))
			{
				parameters[0] = value;
			}
			else
			{
				parameters[0] = Convert.ChangeType(value, _valueType);
			}

			_propertySetter.Invoke(_target, parameters);
		}

		// METHOD
		private object GetMethodValue()
		{
			if (_methodInfo == null || !OSCReflection.CheckAccess(_methodInfo, OSCReflectionAccess.Read))
				return null;

			return _methodInfo.Invoke(_target, null);
		}

		private void SetMethodValue(object value)
		{
			if (_methodInfo == null || !OSCReflection.CheckAccess(_methodInfo, OSCReflectionAccess.Write))
				return;

			var parameters = new object[1];

			if (value == null || _methodWriteType.IsInstanceOfType(value))
			{
				parameters[0] = value;
			}
			else
			{
				parameters[0] = Convert.ChangeType(value, _methodWriteType);
			}

			_methodInfo.Invoke(_target, parameters);
		}

		#endregion
	}
}
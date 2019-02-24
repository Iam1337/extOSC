/* Copyright (c) 2019 ExT (V.Sigalkin) */

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
            if (memberInfo is FieldInfo)
                return new OSCReflectionProperty(target, (FieldInfo)memberInfo);
            if (memberInfo is PropertyInfo)
                return new OSCReflectionProperty(target, (PropertyInfo)memberInfo);
            if (memberInfo is MethodInfo)
                return new OSCReflectionProperty(target, (MethodInfo)memberInfo);

            return null;
        }

        #endregion

        #region Public Vars

        public object Target
        {
            get { return _valueType; }
        }

        public object Value
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        public Type PropertyType
        {
            get { return _valueType; }
        }

        public Type MethodWriteType
        {
            get { return _methodWriteType; }
        }

        public Type MethodReadType
        {
            get { return _methodReadType; }
        }

        public MemberInfo MemberInfo
        {
            get { return GetMemberInfo(); }
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        #endregion

        #region Private Vars

        private OSCReflectionType _propertyType;

        private Type _valueType;

        private object _target;

        private string _propertyName;

        // FIELD
        private FieldInfo _fieldInfo;

        // PROPERTY
        private PropertyInfo _propertyInfo;

        private MethodInfo _propertyGetter;

        private MethodInfo _propertySetter;

        // Method
        private MethodInfo _methodInfo;

        private Type _methodReadType;

        private Type _methodWriteType;

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
            _propertyName = fieldInfo.Name;

            _fieldInfo = fieldInfo;

            _valueType = fieldInfo.FieldType;
        }

        private OSCReflectionProperty(object target, PropertyInfo propertyInfo)
        {
            _target = target;
            _propertyType = OSCReflectionType.Property;
            _propertyName = propertyInfo.Name;

            _propertyInfo = propertyInfo;
            _propertyGetter = propertyInfo.GetGetMethod();
            _propertySetter = propertyInfo.GetSetMethod();

            _valueType = propertyInfo.PropertyType;
        }

        private OSCReflectionProperty(object target, MethodInfo methodInfo)
        {
            _target = target;
            _propertyType = OSCReflectionType.Method;
            _propertyName = methodInfo.Name;

            _methodInfo = methodInfo;
            _methodWriteType = OSCReflection.GetMethodWriteType(methodInfo);
            _methodReadType = OSCReflection.GetMethodReadType(methodInfo);

	        _valueType = _methodInfo.ReturnType;
        }

        private OSCReflectionProperty() { }

        private MemberInfo GetMemberInfo()
        {
            if (_propertyType == OSCReflectionType.Field)
                return _fieldInfo;
            if (_propertyType == OSCReflectionType.Property)
                return _propertyInfo;
            if (_propertyType == OSCReflectionType.Method)
                return _methodInfo;

            return null;
        }

        // FIELD
        private object GetFieldValue()
        {
            if (_fieldInfo == null)
                return null;

            return _fieldInfo.GetValue(_target);
        }

        private void SetFieldValue(object value)
        {
            if (_fieldInfo == null)
                return;

            if (_fieldInfo.IsLiteral)
                return;

            if (value == null || _valueType.IsAssignableFrom(value.GetType()))
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

            if (value == null || _valueType.IsAssignableFrom(value.GetType()))
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
            if (_methodInfo == null)
                return null;

            if (!OSCReflection.CheckAccess(_methodInfo, OSCReflectionAccess.Read))
                return null;

            return _methodInfo.Invoke(_target, null);
        }

        private void SetMethodValue(object value)
        {
            if (_methodInfo == null)
                return;

            if (!OSCReflection.CheckAccess(_methodInfo, OSCReflectionAccess.Write))
                return;

            var parameters = new object[1];

            if (value == null || _methodWriteType.IsAssignableFrom(value.GetType()))
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
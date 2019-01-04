/* Copyright (c) 2019 ExT (V.Sigalkin) */

#if !NETFX_CORE
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using extOSC.Core.Reflection;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerAttribute : OSCPacker<object>
    {
        #region Protected Static Methods

        protected static IEnumerable<MemberInfo> GetMembers(Type type)
        {
            return type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                             .Where(member => Attribute.IsDefined(member, typeof(OSCSerializeAttribute)));
        }

        #endregion

        #region Public Methods

        public override Type GetPackerType()
        {
            return typeof(object);
        }

        #endregion

        #region Protected Methods

        protected override object OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            var members = GetMembers(type);
            var value = Activator.CreateInstance(type);

            foreach (var memberInfo in members)
            {
                var oscValue = values[start];

                var property = OSCReflectionProperty.Create(value, memberInfo);
                if (property != null && !oscValue.IsNull)
                {
                    property.SetValue(OSCSerializer.Unpack(values, ref start, property.PropertyType));
                }
                else
                {
                    start++;
                }
            }

            return value;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, object value)
        {
            var members = GetMembers(value.GetType());

            foreach (var memberInfo in members)
            {
                var property = OSCReflectionProperty.Create(value, memberInfo);
                if (property != null)
                {
                    var propertyValue = property.GetValue();
                    if (propertyValue == null)
                    {
                        values.Add(OSCValue.Null());

                        continue;
                    }

                    OSCSerializer.Pack(values, propertyValue);
                }
                else
                {
                    throw new NullReferenceException("Property not found!");
                }
            }
        }

        protected Type GetMemberType(MemberInfo member)
        {
            var fieldInfo = member as FieldInfo;

            if (fieldInfo != null)
                return fieldInfo.FieldType;

            var propertyInfo = member as PropertyInfo;

            if (propertyInfo != null)
                return propertyInfo.PropertyType;

            return null;
        }

        #endregion
    }
}
#endif
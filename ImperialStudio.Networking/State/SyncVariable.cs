using System;
using System.Reflection;

namespace ImperialStudio.Networking.State
{
    internal class SyncVariable
    {
        private readonly FieldInfo m_FieldInfo;
        private readonly MethodInfo m_SetMethod;
        private readonly MethodInfo m_GetMethod;

        public Type Type { get; }

        public SyncVariable(PropertyInfo propertyInfo)
        {
            Type = propertyInfo.PropertyType;

            m_GetMethod = propertyInfo.GetGetMethod(true);
            m_SetMethod = propertyInfo.GetSetMethod(true);
        }

        public SyncVariable(FieldInfo fieldInfo)
        {
            m_FieldInfo = fieldInfo;
            Type = fieldInfo.FieldType;
        }

        public object GetValue(object instance)
        {
            return m_FieldInfo != null 
                ? m_FieldInfo.GetValue(instance) 
                : m_GetMethod.Invoke(instance, Array.Empty<object>());
        }


        public void SetValue(object instance, object value)
        {
            if (m_FieldInfo != null)
                m_FieldInfo.SetValue(instance, value);
            else
                m_SetMethod.Invoke(instance, Array.Empty<object>());
        }
    }
}
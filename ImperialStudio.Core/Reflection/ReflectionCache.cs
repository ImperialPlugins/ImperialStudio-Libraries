using System.Reflection;

namespace ImperialStudio.Core.Reflection
{
    // ReSharper disable StaticMemberInGenericType
    public static class ReflectionCache<T>
    {
        private static PropertyInfo[] m_Properties;
        private static FieldInfo[] m_Fields;

        public static PropertyInfo[] GetProperties(BindingFlags bindingFlags)
        {
            m_Properties = typeof(T).GetProperties(bindingFlags);
            return m_Properties; //todo
        }

        public static FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
            m_Fields = typeof(T).GetFields(bindingFlags);
            return m_Fields; //todo
        }
    }
}
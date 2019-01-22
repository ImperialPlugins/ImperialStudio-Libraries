using System;
using System.Collections.Generic;
using System.Reflection;

namespace ImperialStudio.Core.Eventing
{
    /// <inheritdoc />
    public class Event : IEvent
    {
        protected Event()
        {
            List<string> names = EventBus.GetEventNames(GetType());

            Names = names;
        }

        /// <summary>
        ///     <inheritdoc /><br /><br />
        ///     In this implementation it contains the properties of the class with their respective values.
        /// </summary>
        public Dictionary<string, object> Arguments
        {
            get
            {
                Dictionary<string, object> args = new Dictionary<string, object>();
                PropertyInfo[] props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in props)
                {
                    MethodInfo getter = prop.GetGetMethod(false);
                    if (getter == null) continue;

                    args.Add(prop.Name.ToLower(), this);
                }

                return args;
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> Names { get; }
    }
}
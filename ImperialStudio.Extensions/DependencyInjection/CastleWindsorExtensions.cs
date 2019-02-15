using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Windsor;

namespace ImperialStudio.Extensions.DependencyInjection
{
    public static class CastleWindsorExtensions
    {
        public static T Activate<T>(this IWindsorContainer container, params object[] args) where T : class
        {
            return (T) Activate(container, typeof(T), args);
        }

        public static object Activate(this IWindsorContainer container, Type type, params object[] args)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            foreach (var constructor in constructors)
            {
                if (TryActivate(constructor, container, args, out object result))
                    return result;
            }

            throw new Exception("Activation failed; no matching constructor was found");
        }


        private static bool TryActivate(ConstructorInfo c, IWindsorContainer container, object[] args, out object result)
        {
            result = null;

            List<object> parameters = new List<object>();
            List<object> remainingArgs = new List<object>(args);

            bool useArgs = false;

            foreach (var parameter in c.GetParameters())
            {
                var type = parameter.ParameterType;
                bool isOptional = parameter.GetCustomAttributes(typeof(OptionalAttribute), false).Any();

                if (!container.Kernel.HasComponent(type))
                {
                    if (isOptional)
                    {
                        parameters.Add(null);
                        continue;
                    }

                    if (args.Length == 0)
                        return false;

                    useArgs = true;
                }

                if (useArgs)
                {
                    if (remainingArgs.Count == 0)
                    {
                        return false;
                    }

                    var nextArg = remainingArgs[0];
                    if (nextArg.GetType() != type)
                        return false;

                    parameters.Add(nextArg);
                    remainingArgs.RemoveAt(0);
                    continue;
                }

                parameters.Add(container.Resolve(type));
            }

            result = Activator.CreateInstance(c.DeclaringType, parameters.ToArray());
            return true;
        }
    }
}
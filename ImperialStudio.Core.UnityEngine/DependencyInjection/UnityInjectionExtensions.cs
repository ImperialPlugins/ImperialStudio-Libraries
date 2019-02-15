using System;
using System.Linq;
using System.Reflection;
using Castle.Windsor;
using ImperialStudio.Core.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace ImperialStudio.Core.UnityEngine.DependencyInjection
{
    public static class UnityInjectionExtensions
    {
        public static T AddComponentWithInjection<T>(this GameObject o, IWindsorContainer container) where T : Component
        {
            o.SetActive(false);
            T addedComponent = o.AddComponent<T>();

            foreach (var property in ReflectionCache<T>.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                InjectProperty(property, container, addedComponent);
            }

            foreach (var field in ReflectionCache<T>.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                InjectField(field, container, addedComponent);
            }

            o.SetActive(true);
            return addedComponent;
        }

        public static void SetupInjections<T>(this Component component, IWindsorContainer container)
        {
            GameObject o = component.gameObject;
            o.SetActive(false);
            foreach (var property in ReflectionCache<T>.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                InjectProperty(property, container, component);
            }

            foreach (var field in ReflectionCache<T>.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                InjectField(field, container, component);
            }
            o.SetActive(true);
        }

        public static Component AddComponentWithInjection(this GameObject o, Type type, IWindsorContainer container)
        {
            o.SetActive(false);
            Component addedComponent = o.AddComponent(type);

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                InjectProperty(property, container, addedComponent);
            }

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                InjectField(field, container, addedComponent);
            }

            o.SetActive(true);
            return addedComponent;
        }

        private static void InjectProperty([NotNull] PropertyInfo property, [NotNull] IWindsorContainer container, [NotNull] Component component)
        {
            if (property.GetCustomAttributes().Any(d => d is AutoInjectAttribute))
            {
                var instance = container.Resolve(property.PropertyType);

                var setMethod = property.GetSetMethod(true);
                if (setMethod == null)
                {
                    throw new Exception($"Could not AutoInject property \"{property.Name}\" in type \"{component.GetType().FullName}\": Setter was not accessible.");
                }
                property.GetSetMethod(true).Invoke(component, new[] { instance });
            }
        }

        private static void InjectField([NotNull] FieldInfo field, [NotNull] IWindsorContainer container, [NotNull] Component component)
        {

            if (field.GetCustomAttributes().Any(d => d is AutoInjectAttribute))
            {
                var instance = container.Resolve(field.FieldType);
                field.SetValue(component, instance);
            }
        }
    }
}
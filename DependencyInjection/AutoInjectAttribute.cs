using System;

namespace ImperialStudio.Core.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AutoInjectAttribute : Attribute
    {
        
    }
}
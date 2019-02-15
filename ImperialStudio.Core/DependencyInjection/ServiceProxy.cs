using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;

namespace ImperialStudio.Core.DependencyInjection
{
    public class ServiceProxy<T> where T: class
    {
        public IReadOnlyCollection<T> ProxiedServices => Container.ResolveAll<T>().Where(c => c != this).ToList();

        protected IWindsorContainer Container { get; }

        public ServiceProxy(IWindsorContainer container)
        {
            Container = container;
        }
    }
}
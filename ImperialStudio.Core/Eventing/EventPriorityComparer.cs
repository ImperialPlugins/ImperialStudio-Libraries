using System.Collections.Generic;
using System.Linq;

namespace ImperialStudio.Core.Eventing
{
    public class EventPriorityComparer
    {

        public static int Compare(EventPriority a, EventPriority b, bool highestFirst)
            => highestFirst ? Compare(b, a) : Compare(a, b);

        public static int Compare(EventPriority a, EventPriority b)
            => ((int)a).CompareTo((int)b);

        public static void Sort<T>(List<T> objects)
        {
            Sort(objects, false);
        }

        public static void Sort<T>(List<T> objects, bool highestFirst)
        {
            objects.Sort((a, b) => Compare(GetPriority(a), GetPriority(b), highestFirst));
        }

        public static EventPriority GetPriority(object a)
        {
            EventHandler serviceAttribute = (EventHandler)
                                                        a.GetType()
                                                            .GetCustomAttributes(
                                                                typeof(EventHandler), true)
                                                            .FirstOrDefault()
                                                        ?? new EventHandler
                                                        {
                                                            Priority = EventPriority.Normal
                                                        };

            return serviceAttribute.Priority;
        }
    }
}
using System;

namespace Devdog.Rucksack.Editor
{
    public class EditorPageAttribute : Attribute
    {
        public string path;
        public int order;
        public int priority;

        public const int DefaultPriority = 50;
        
        public EditorPageAttribute(string path, int order, int priority = DefaultPriority)
        {
            this.path = path;
            this.order = order;
            this.priority = priority;
        }
    }
}
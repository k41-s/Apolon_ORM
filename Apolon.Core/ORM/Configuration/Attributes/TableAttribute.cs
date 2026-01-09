using System;

namespace Apolon.Core.ORM.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; }
        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}

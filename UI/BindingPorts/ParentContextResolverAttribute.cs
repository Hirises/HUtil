using System;

namespace HUtil.UI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ParentContextResolverAttribute : Attribute
    {
        public string Setter { get; set; }

        public ParentContextResolverAttribute(string setter)
        {
            Setter = setter;
        }
    }
}
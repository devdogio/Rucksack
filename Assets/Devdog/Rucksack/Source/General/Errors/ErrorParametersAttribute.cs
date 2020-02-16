using System;

namespace Devdog.Rucksack
{
    // TODO: Experimental concept
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ErrorParametersAttribute : Attribute
    {
        public string[] parameters { get; set; }

        public ErrorParametersAttribute(params string[] p)
        {
            this.parameters = p;
        }
    }
}
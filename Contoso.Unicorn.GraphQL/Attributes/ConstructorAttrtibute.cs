using System;

namespace Contoso.Unicorn.GraphQL.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ConstructorAttribute : Attribute
    { }
}
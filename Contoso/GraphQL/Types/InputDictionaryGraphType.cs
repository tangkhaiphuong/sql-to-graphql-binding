using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace Contoso.GraphQL.Types
{
    /// <inheritdoc />
    public class InputDictionaryGraphType<T> : InputObjectGraphType<IDictionary<string, object>>
    {
        public FieldBuilder<T, TProperty> Field<TProperty>(
            Expression<Func<T, TProperty>> expression,

            bool nullable = false,
            Type type = null)
        {
            string name;
            try
            {
                name = expression.NameOf();
            }
            catch
            {
                throw new ArgumentException("Cannot infer a Field name from the expression: '" + expression?.Body + "' on parent GraphQL type: '" + (Name ?? GetType().Name) + "'.");
            }
            return Field(name, expression, nullable, type);
        }

        public FieldBuilder<T, TProperty> Field<TProperty>(
            string name,
            Expression<Func<T, TProperty>> expression,
            bool nullable = false,
            Type type = null)
        {
            try
            {
                if (type == null)
                    type = typeof(TProperty).GetGraphTypeFromType(nullable);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentException("The GraphQL type for Field: '" + name + "' on parent type: '" + (Name ?? GetType().Name) + "' could not be derived implicitly. \n", ex);
            }

            var defaultValueOf = expression.DefaultValueOf();
            var fieldBuilder = FieldBuilder.Create<T, TProperty>(type).Name(name).Resolve(new ExpressionFieldResolver<T, TProperty>(expression)).Description(expression.DescriptionOf()).DeprecationReason(expression.DeprecationReasonOf());

            if (!nullable)
                fieldBuilder = fieldBuilder.DefaultValue(defaultValueOf != null ? (TProperty)defaultValueOf : default);

            AddField(fieldBuilder.FieldType);

            return fieldBuilder;
        }
    }
}
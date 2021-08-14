using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Contoso.Unicorn.GraphQL.Mutations;
using Contoso.Unicorn.GraphQL.Queries;
using Fluid;
using Fluid.Ast;
using GraphQL.Types;

namespace Contoso.Unicorn.GraphQL
{
    public class UnicornSchema : Schema
    {
        public UnicornSchema(
            IServiceProvider serviceProvider,
            UnicornQuery unicornQuery,
            UnicornMutation unicornMutation) : base(serviceProvider)
        {
            Query = unicornQuery;
            Mutation = unicornMutation;
        }

        internal static async ValueTask<Completion> HandleStopTag(Expression expression, TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            var value = (await expression.EvaluateAsync(context).ConfigureAwait(false)).ToBooleanValue();
            context.SetValue("stop", value);
            await writer.WriteAsync("").ConfigureAwait(false);
            return Completion.Normal;
        }
    }
}

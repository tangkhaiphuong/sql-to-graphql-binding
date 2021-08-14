﻿

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated.
// 
using System.Linq;
using Contoso.Unicorn.GraphQL.Attributes;
using Contoso.Unicorn.GraphQL.Types;
using GraphQL;
using GraphQL.Types;
using Contoso.Unicorn.GraphQL.Proxies;

namespace Contoso.Unicorn.GraphQL.Mutations
{
    /// <inheritdoc />
    [GraphQLAuthorize("default")]
    public sealed partial class UnicornMutation : ObjectGraphType
    {
        /// <inheritdoc />
        public UnicornMutation()
        {
            Connection<CategoryType>()
                .Name("categories")
                .Description("Mutate category base on criteria.")
                .Argument<StringGraphType>("predicate", "An expression string to test each element for a condition.")
                .Argument<ListGraphType<StringGraphType>>("args", "List of arguments.")
                .Argument<NonNullGraphType<EnumerationGraphType<MutationAction>>>("action", "Action of mutation.")
                .Argument<CategoryInputType>("node", "Input of single node.")
                .Argument<StringGraphType>("template", "Input of single node.")
                .Argument<BooleanGraphType>("ignore", "Reverse action if predicate match.")
                .Argument<BooleanGraphType>("skip", "Skip action if predicate match.")
                .Argument<BooleanGraphType>("multiple", "Running predicate many time for each node.")
                .Argument<ListGraphType<CategoryInputType>>("nodes", "Input of multi node.")
                .Argument<ListGraphType<NonNullGraphType<StringGraphType>>>("fields", "Field include.")
                .Argument<StringGraphType>("ordering", "Sorts the elements of a sequence in ascending or descending order according to a key.")
                .ResolveAsync(ResolveMutation(_ => _.Categories, _ => new CategoryProxy(_)));

            Connection<CustomerType>()
                .Name("customers")
                .Description("Mutate customer base on criteria.")
                .Argument<StringGraphType>("predicate", "An expression string to test each element for a condition.")
                .Argument<ListGraphType<StringGraphType>>("args", "List of arguments.")
                .Argument<NonNullGraphType<EnumerationGraphType<MutationAction>>>("action", "Action of mutation.")
                .Argument<CustomerInputType>("node", "Input of single node.")
                .Argument<StringGraphType>("template", "Input of single node.")
                .Argument<BooleanGraphType>("ignore", "Reverse action if predicate match.")
                .Argument<BooleanGraphType>("skip", "Skip action if predicate match.")
                .Argument<BooleanGraphType>("multiple", "Running predicate many time for each node.")
                .Argument<ListGraphType<CustomerInputType>>("nodes", "Input of multi node.")
                .Argument<ListGraphType<NonNullGraphType<StringGraphType>>>("fields", "Field include.")
                .Argument<StringGraphType>("ordering", "Sorts the elements of a sequence in ascending or descending order according to a key.")
                .ResolveAsync(ResolveMutation(_ => _.Customers, _ => new CustomerProxy(_)));

            Connection<EmployeeType>()
                .Name("employees")
                .Description("Mutate employee base on criteria.")
                .Argument<StringGraphType>("predicate", "An expression string to test each element for a condition.")
                .Argument<ListGraphType<StringGraphType>>("args", "List of arguments.")
                .Argument<NonNullGraphType<EnumerationGraphType<MutationAction>>>("action", "Action of mutation.")
                .Argument<EmployeeInputType>("node", "Input of single node.")
                .Argument<StringGraphType>("template", "Input of single node.")
                .Argument<BooleanGraphType>("ignore", "Reverse action if predicate match.")
                .Argument<BooleanGraphType>("skip", "Skip action if predicate match.")
                .Argument<BooleanGraphType>("multiple", "Running predicate many time for each node.")
                .Argument<ListGraphType<EmployeeInputType>>("nodes", "Input of multi node.")
                .Argument<ListGraphType<NonNullGraphType<StringGraphType>>>("fields", "Field include.")
                .Argument<StringGraphType>("ordering", "Sorts the elements of a sequence in ascending or descending order according to a key.")
                .ResolveAsync(ResolveMutation(_ => _.Employees, _ => new EmployeeProxy(_)));

            Connection<OrderType>()
                .Name("orders")
                .Description("Mutate order base on criteria.")
                .Argument<StringGraphType>("predicate", "An expression string to test each element for a condition.")
                .Argument<ListGraphType<StringGraphType>>("args", "List of arguments.")
                .Argument<NonNullGraphType<EnumerationGraphType<MutationAction>>>("action", "Action of mutation.")
                .Argument<OrderInputType>("node", "Input of single node.")
                .Argument<StringGraphType>("template", "Input of single node.")
                .Argument<BooleanGraphType>("ignore", "Reverse action if predicate match.")
                .Argument<BooleanGraphType>("skip", "Skip action if predicate match.")
                .Argument<BooleanGraphType>("multiple", "Running predicate many time for each node.")
                .Argument<ListGraphType<OrderInputType>>("nodes", "Input of multi node.")
                .Argument<ListGraphType<NonNullGraphType<StringGraphType>>>("fields", "Field include.")
                .Argument<StringGraphType>("ordering", "Sorts the elements of a sequence in ascending or descending order according to a key.")
                .ResolveAsync(ResolveMutation(_ => _.Orders, _ => new OrderProxy(_)));

            Connection<OrderDetailType>()
                .Name("orderDetails")
                .Description("Mutate order detail base on criteria.")
                .Argument<StringGraphType>("predicate", "An expression string to test each element for a condition.")
                .Argument<ListGraphType<StringGraphType>>("args", "List of arguments.")
                .Argument<NonNullGraphType<EnumerationGraphType<MutationAction>>>("action", "Action of mutation.")
                .Argument<OrderDetailInputType>("node", "Input of single node.")
                .Argument<StringGraphType>("template", "Input of single node.")
                .Argument<BooleanGraphType>("ignore", "Reverse action if predicate match.")
                .Argument<BooleanGraphType>("skip", "Skip action if predicate match.")
                .Argument<BooleanGraphType>("multiple", "Running predicate many time for each node.")
                .Argument<ListGraphType<OrderDetailInputType>>("nodes", "Input of multi node.")
                .Argument<ListGraphType<NonNullGraphType<StringGraphType>>>("fields", "Field include.")
                .Argument<StringGraphType>("ordering", "Sorts the elements of a sequence in ascending or descending order according to a key.")
                .ResolveAsync(ResolveMutation(_ => _.OrderDetails, _ => new OrderDetailProxy(_)));

            Connection<ProductType>()
                .Name("products")
                .Description("Mutate product base on criteria.")
                .Argument<StringGraphType>("predicate", "An expression string to test each element for a condition.")
                .Argument<ListGraphType<StringGraphType>>("args", "List of arguments.")
                .Argument<NonNullGraphType<EnumerationGraphType<MutationAction>>>("action", "Action of mutation.")
                .Argument<ProductInputType>("node", "Input of single node.")
                .Argument<StringGraphType>("template", "Input of single node.")
                .Argument<BooleanGraphType>("ignore", "Reverse action if predicate match.")
                .Argument<BooleanGraphType>("skip", "Skip action if predicate match.")
                .Argument<BooleanGraphType>("multiple", "Running predicate many time for each node.")
                .Argument<ListGraphType<ProductInputType>>("nodes", "Input of multi node.")
                .Argument<ListGraphType<NonNullGraphType<StringGraphType>>>("fields", "Field include.")
                .Argument<StringGraphType>("ordering", "Sorts the elements of a sequence in ascending or descending order according to a key.")
                .ResolveAsync(ResolveMutation(_ => _.Products, _ => new ProductProxy(_)));

            Connection<ShipperType>()
                .Name("shippers")
                .Description("Mutate shipper base on criteria.")
                .Argument<StringGraphType>("predicate", "An expression string to test each element for a condition.")
                .Argument<ListGraphType<StringGraphType>>("args", "List of arguments.")
                .Argument<NonNullGraphType<EnumerationGraphType<MutationAction>>>("action", "Action of mutation.")
                .Argument<ShipperInputType>("node", "Input of single node.")
                .Argument<StringGraphType>("template", "Input of single node.")
                .Argument<BooleanGraphType>("ignore", "Reverse action if predicate match.")
                .Argument<BooleanGraphType>("skip", "Skip action if predicate match.")
                .Argument<BooleanGraphType>("multiple", "Running predicate many time for each node.")
                .Argument<ListGraphType<ShipperInputType>>("nodes", "Input of multi node.")
                .Argument<ListGraphType<NonNullGraphType<StringGraphType>>>("fields", "Field include.")
                .Argument<StringGraphType>("ordering", "Sorts the elements of a sequence in ascending or descending order according to a key.")
                .ResolveAsync(ResolveMutation(_ => _.Shippers, _ => new ShipperProxy(_)));

            Connection<SupplierType>()
                .Name("suppliers")
                .Description("Mutate supplier base on criteria.")
                .Argument<StringGraphType>("predicate", "An expression string to test each element for a condition.")
                .Argument<ListGraphType<StringGraphType>>("args", "List of arguments.")
                .Argument<NonNullGraphType<EnumerationGraphType<MutationAction>>>("action", "Action of mutation.")
                .Argument<SupplierInputType>("node", "Input of single node.")
                .Argument<StringGraphType>("template", "Input of single node.")
                .Argument<BooleanGraphType>("ignore", "Reverse action if predicate match.")
                .Argument<BooleanGraphType>("skip", "Skip action if predicate match.")
                .Argument<BooleanGraphType>("multiple", "Running predicate many time for each node.")
                .Argument<ListGraphType<SupplierInputType>>("nodes", "Input of multi node.")
                .Argument<ListGraphType<NonNullGraphType<StringGraphType>>>("fields", "Field include.")
                .Argument<StringGraphType>("ordering", "Sorts the elements of a sequence in ascending or descending order according to a key.")
                .ResolveAsync(ResolveMutation(_ => _.Suppliers, _ => new SupplierProxy(_)));

            var methods = GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(ConstructorAttribute), false).Length > 0);

            foreach (var item in methods) item.Invoke(this, null);
        }
    }
}
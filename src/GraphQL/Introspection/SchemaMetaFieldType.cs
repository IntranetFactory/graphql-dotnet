using System.Linq;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.Introspection
{
    public class SchemaMetaFieldType : FieldType
    {
        public SchemaMetaFieldType()
        {
            Name = "__schema";
            Type = typeof (__Schema);
            Description = "Access the current type schema of this server.";
            Resolver = new FuncFieldResolver<ISchema>(context => context.Schema);
        }
    }

    public class __Schema : ObjectGraphType<object>
    {
        public __Schema()
        {
            Name = "__Schema";
            Description =
                "A GraphQL Schema defines the capabilities of a GraphQL server. It " +
                "exposes all available types and directives on the server, as well as " +
                "the entry points for query, mutation, and subscription operations.";

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<__Type>>>>(
                "types",
                "A list of all types supported by this server.",
                resolve: context =>
                {
                    return context.Schema.AllTypes.Where(x => context.Schema.Filter.Type(x)).ToList();
                });

            Field<NonNullGraphType<__Type>>(
                "queryType",
                "The type that query operations will be rooted at.",
                resolve: context =>
                {
                    return context.Schema.Query;
                });

            Field<__Type>(
                "mutationType",
                "If this server supports mutation, the type that mutation operations will be rooted at.",
                resolve: context =>
                {
                    if(context.Schema.Filter.Type(context.Schema.Mutation))
                    {
                        return context.Schema.Mutation;
                    }
                    return null;
                });

            Field<__Type>(
                "subscriptionType",
                "If this server supports subscription, the type that subscription operations will be rooted at.",
                resolve: context =>
                {
                    if(context.Schema.Filter.Type(context.Schema.Subscription))
                    {
                        return context.Schema.Subscription;
                    }
                    return null;
                });

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<__Directive>>>>(
                "directives",
                "A list of all directives supported by this server.",
                resolve: context =>
                {
                    return context.Schema.Directives.Where(d => context.Schema.Filter.Directive(d));
                });
        }
    }
}

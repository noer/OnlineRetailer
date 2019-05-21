using CustomerApi.Models;
using CustomerApi.Data;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CustomerApi.Models
{
    public class CustomerQuery : ObjectGraphType
    {
        public CustomerQuery(IRepository<Customer> repository)
        {
            Field<ListGraphType<CustomerType>>(
                "customers",
                resolve: context => repository.GetAll());

            Field<CustomerType>(
                "customer",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "customerId" }),
                resolve: context => repository.Get(context.GetArgument<int>("customerId")));
        }
    }
    
    public class CustomerMutation : ObjectGraphType
    {
        public CustomerMutation(IRepository<Customer> repository)
        {
            Name = "AddCustomerMutation";

            Field<CustomerType>(
                "addCustomer",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<CustomerInputType>> {Name = "customer"}),
                resolve: context =>
                {
                    var customer = context.GetArgument<Customer>("customer");
                    return repository.Add(customer);
                });
            
            Field<CustomerType>(
                "editCustomer",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<CustomerInputType>> {Name = "customer"}),
                resolve: context =>
                {
                    var customer = context.GetArgument<Customer>("customer");
                    repository.Edit(customer);
                    return customer;
                });
            
            Field<IntGraphType>(
                "deleteCustomer",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> {Name = "customerId"}),
                resolve: context =>
                {
                    var customerId = int.Parse(context.GetArgument<IntGraphType>("customerId").ToString());
                    repository.Remove(customerId);
                    return customerId;
                });
        }
    }
}
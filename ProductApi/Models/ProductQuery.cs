using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using ProductApi.Data;

namespace ProductApi.Models
{
    public class ProductQuery : ObjectGraphType
    {
        public ProductQuery(IRepository<Product> repository)
        {
            Field<ListGraphType<ProductType>>(
                "products",
                resolve: context => repository.GetAll(),
                description: "Lists all products.");

            Field<ProductType>(
                "product",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "productId" }),
                resolve: context => repository.Get(context.GetArgument<int>("productId")),
                description: "Lists a product. Takes a productId as parameter.");
        }
    }

    public class ProductMutation : ObjectGraphType
    {
        public ProductMutation(IRepository<Product> repository)
        {
            Name = "AddProductMutations";

            Field<ProductType>(
                "addProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }),
                resolve: context =>
                {
                    var product = context.GetArgument<Product>("product");
                    return repository.Add(product);
                },
                description: "Adds a product.");

            Field<ProductType>(
                "editProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }),
                resolve: context =>
                {
                    var product = context.GetArgument<Product>("product");

                    repository.Edit(product);
                    return product;
                },
                description: "Edits a product.");

            Field<IntGraphType>(
                "deleteProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "productId" }),
                resolve: (context) =>
                {
                    var productId = context.GetArgument<int>("productId");
                    repository.Remove(productId);
                    return productId;
                }, 
                description: "Deletes a product. Takes a productId as parameter.");
        }
    }
}

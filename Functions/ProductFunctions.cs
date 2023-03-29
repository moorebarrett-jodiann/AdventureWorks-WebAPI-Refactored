using AdventureWorksApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorksApi.Functions
{
    public class ProductFunctions
    {
        public static IResult CreateProduct(AdventureWorksLt2019Context context, Product product)
        {
            
            context.Products.Add(product);
            context.SaveChanges();

            return Results.Created($"/Products", product);

        }

        public static IResult ReadProduct(AdventureWorksLt2019Context context, int? id)
        {
            if (id == null || id == -1)
            {
                return Results.Ok(context.Products.ToList());
            }

            Product? product = context.Products.Find(id);

            if (product == null)
            {
                return Results.NotFound();
            } else
            {
                return Results.Ok(product);
            }
                
        }

        public static IResult UpdateProduct(AdventureWorksLt2019Context context, int id, Product inputProduct)
        {
            Product? product = context.Products.Find(id);
            
            if (product == null)
            {
                return Results.NotFound();
            }

            
            var jsonString = JsonSerializer.Serialize(inputProduct);
            
            product = JsonSerializer.Deserialize<Product>(jsonString);

            context.SaveChanges();

            return Results.Ok(product);
           
        }

        public static IResult DeleteProduct(AdventureWorksLt2019Context context, int id)
        {
            
            Product? product = context.Products
                .Include(p => p.SalesOrderDetails)
                .FirstOrDefault(p => p.ProductId == id);
                
            if (product != null)
            {
                context.Products.Remove(product);
                context.SaveChanges();
                return Results.Ok(product);
            } else
            {
                return Results.NotFound();
            }
        }
    }
}

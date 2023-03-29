using AdventureWorksApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorksApi.Functions
{
    public static class ProductFunctions
    {

        public static IResult CreateProduct(AdventureWorksLt2019Context context, Product product)
        {
            product.Rowguid = Guid.NewGuid();
            product.ThumbNailPhoto = null;
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

            inputProduct.Rowguid = Guid.NewGuid();
            inputProduct.ThumbNailPhoto = null;
            if (product == null)
            {
                
                
                context.Products.Add(inputProduct);
                context.SaveChanges();

                return Results.Created($"/Products", inputProduct);
            }

            
            product.Name = inputProduct.Name;
            product.ProductNumber = inputProduct.ProductNumber;
            product.Color = inputProduct.Color;
            product.StandardCost= inputProduct.StandardCost;
            product.ListPrice = inputProduct.ListPrice;
            product.Size = inputProduct.Size;
            product.Weight = inputProduct.Weight;
            product.ProductCategoryId = inputProduct.ProductCategoryId;
            product.ProductModelId= inputProduct.ProductModelId;
            product.SellStartDate= inputProduct.SellStartDate;
            product.SellEndDate = inputProduct.SellEndDate;
            product.DiscontinuedDate= inputProduct.DiscontinuedDate;
            product.ThumbNailPhoto= inputProduct.ThumbNailPhoto;
            product.ThumbnailPhotoFileName= inputProduct.ThumbnailPhotoFileName;
            product.Rowguid = inputProduct.Rowguid;
            product.ModifiedDate = DateTime.Now;



                /*
            var jsonString = JsonSerializer.Serialize(inputProduct);
            
            product = JsonSerializer.Deserialize<Product>(jsonString);
                */
            context.SaveChanges();

            return Results.Ok(context.Products.Find(product.ProductId));
           
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

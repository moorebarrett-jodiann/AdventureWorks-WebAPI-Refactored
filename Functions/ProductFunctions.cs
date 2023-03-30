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

            return Results.Created($"/Product/Read", product);
        }

        public static IResult ReadProduct(AdventureWorksLt2019Context context, int? id)
        {
            if (id == null)
            {
                return Results.Ok(context.Products.ToList());
            }

            Product? product = context.Products.Find(id);

            if (product == null)
            {
                return Results.BadRequest();
            } 
               
            return Results.Ok(product);
        }

        public static IResult UpdateProduct( int id, Product inputProduct, AdventureWorksLt2019Context context)
        {
            Product? product = context.Products.Find(id);
            
            inputProduct.ThumbNailPhoto = null;
            if (product == null)
            {
                inputProduct.Rowguid = Guid.NewGuid();
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
            product.ModifiedDate = DateTime.Now;

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
            } 

            return Results.BadRequest();
            
        }

        public static IResult Details(int id, AdventureWorksLt2019Context context)
        {
            Product? product = null;
            
                 product = context.Products
                .Include(p => p.ProductCategory).ThenInclude(pc => pc.ParentProductCategory)
                .Include(p => p.ProductModel).ThenInclude(pm => pm.ProductModelProductDescriptions)
                .ThenInclude(d => d.ProductDescription)
                .FirstOrDefault(p => p.ProductId == id);


            if (product == null)
            {
                return Results.BadRequest();
            }

            string? description = null;


            ProductModelProductDescription? relationalObject = product.ProductModel.ProductModelProductDescriptions.FirstOrDefault(d => d.Culture.Trim() == "en");

            if(relationalObject != null)
            {
                description = context.ProductDescriptions.Find(relationalObject.ProductDescriptionId).Description;
            }

            if (description == null)
            {
                description = "na";
            }

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            return Results.Json(new {
                ProductId = product.ProductId,
                Name = product.Name,
                ProductNumber = product.ProductNumber,
                Color = product.Color,
                StandardCost = product.StandardCost,
                ListPrice = product.ListPrice,
                Size = product.Size,
                Weight = product.Weight,
                
                ProductCategory = product.ProductCategory.Name,
                ParentCategory = product.ProductCategory.ParentProductCategory.Name,

                ProductModel = product.ProductModel.Name,
                Description = description
            }
            , options) ;
        }
    }
}

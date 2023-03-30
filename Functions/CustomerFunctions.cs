using AdventureWorksApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace AdventureWorksApi.Functions
{
    public class CustomerFunctions
    {
               
        public static IResult CreateCustomer(AdventureWorksLt2019Context context, Customer customer)
        {
            if (context.Addresses.Any(a => a.Rowguid == customer.Rowguid))
            {
                customer.Rowguid = Guid.NewGuid();
            }


            context.Customers.Add(customer);
            context.SaveChanges();

            return Results.Created($"/Customers", customer);
        }

        public static IResult ReadCustomer(AdventureWorksLt2019Context context, int? id)
        {
         
     
            if (id == null)
            {
                return Results.Ok(context.Customers.ToList());
            }

            Customer? customer = context.Customers.FirstOrDefault(a => a.CustomerId == id);

            if (customer == null)
            {
                return Results.BadRequest();
            }else
            {
                return Results.Ok(customer);
            }

        }

        public static IResult UpdateCustomer(AdventureWorksLt2019Context context, int id, Customer updateCustomer)
        {

            Customer? customer = context.Customers.Find(id);
            if (customer == null)
            {
                context.Customers.Add(updateCustomer);
                context.SaveChanges();
                return Results.Created("/Customers", updateCustomer);
            }
            else
            {
                customer.Title= updateCustomer.Title;
                customer.FirstName= updateCustomer.FirstName;
                customer.MiddleName = updateCustomer.LastName;
                customer.CompanyName = updateCustomer.CompanyName;
                customer.SalesPerson = updateCustomer.SalesPerson;
                customer.EmailAddress = updateCustomer.EmailAddress;
                customer.Phone = updateCustomer.Phone;
                customer.Rowguid = updateCustomer.Rowguid;
                context.SaveChanges();
                return Results.Ok(customer);
            }


            /*if (id != customer.CustomerId)
            {
                return Results.BadRequest();
            }

            context.Entry(customer).State = EntityState.Modified;

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(context, id))
                {
                    return Results.NotFound();
                }
                else
                {
                    throw;
                }
            }

           return Results.NoContent();*/
        }
            
        public static IResult DeleteCustomer(AdventureWorksLt2019Context context, int id)
        {
            var customer = context.Customers.Find(id);

            if (customer == null)
            {
                return Results.NotFound();
            }

            context.Customers.Remove(customer);
            context.SaveChanges();

            return Results.Ok(customer);
        }

        public static IResult CustomerDetails(int CustomerId, AdventureWorksLt2019Context context)
        {
            var customer = context.Customers.Where(a => a.CustomerId == CustomerId).Select(b => new
            {
                b.CustomerId,
                b.Title,
                b.FirstName,
                b.MiddleName,
                b.LastName,
                b.CompanyName,
                b.SalesPerson,
                b.EmailAddress,
                b.Phone,
                Address = b.CustomerAddresses.Select(c => new
                {
                    c.Address.AddressId,
                    c.Address.AddressLine1,
                    c.Address.AddressLine2,
                    c.Address.City,
                    c.Address.StateProvince,
                    c.Address.CountryRegion,
                    c.Address.PostalCode,
                    c.Address.Rowguid
                }).ToList()
            }).FirstOrDefault();

            if (customer == null)
            {
                return Results.NotFound();
            }

            return Results.Json(customer, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                IncludeFields = true
            });
        }

        private static bool CustomerExists(AdventureWorksLt2019Context context, int id)
        {
            return context.Customers.Any(e => e.CustomerId == id);
        }


    }
}

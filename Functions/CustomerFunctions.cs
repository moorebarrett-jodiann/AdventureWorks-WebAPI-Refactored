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
                updateCustomer.Rowguid = Guid.NewGuid();
                context.Customers.Add(updateCustomer);
                context.SaveChanges();
                return Results.Created("/Customers", updateCustomer);
            }
            
            customer.Title= updateCustomer.Title;
            customer.NameStyle = updateCustomer.NameStyle;
            customer.FirstName= updateCustomer.FirstName;
            customer.MiddleName = updateCustomer.MiddleName;
            customer.LastName = updateCustomer.LastName;
            customer.Suffix = updateCustomer.Suffix;
            customer.CompanyName = updateCustomer.CompanyName;
            customer.SalesPerson = updateCustomer.SalesPerson;
            customer.EmailAddress = updateCustomer.EmailAddress;
            customer.Phone = updateCustomer.Phone;
            
            context.SaveChanges();

            return Results.Ok(customer);
            
        }
            
        public static IResult DeleteCustomer(AdventureWorksLt2019Context context, int id)
        {
            Customer? customer = context.Customers.Find(id);

            if (customer == null)
            {
                return Results.BadRequest();
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
                return Results.BadRequest();
            }

            return Results.Json(customer, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                IncludeFields = true
            });
        }

        
        public static IResult CustomerAddToAddress(AdventureWorksLt2019Context db, int customerId, int addressId)
        {
            Customer? selectedCustomer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            Address? address = db.Addresses.FirstOrDefault(a => a.AddressId == addressId);

            if (selectedCustomer == null)
            {
                return Results.BadRequest($"Customer of {customerId} was not found");
            }

            if (address == null)
            {
                return Results.BadRequest($"Address of {addressId} was not found");
            }

            if (db.CustomerAddresses.Any(ca => ca.CustomerId == customerId && ca.AddressId == addressId))
            {
                return Results.BadRequest($"ERROR: customerId: {customerId} already on address {addressId}");
            }

            CustomerAddress customerAddress = new CustomerAddress
            {
                CustomerId = selectedCustomer.CustomerId,
                AddressId = address.AddressId,
                AddressType = "Main Office",
                Rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.Now
            };

            db.CustomerAddresses.Add(customerAddress);
            db.SaveChanges();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            return Results.Json(customerAddress, options);
        }

    }
}

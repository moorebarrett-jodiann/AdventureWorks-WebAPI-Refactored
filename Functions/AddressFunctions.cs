using AdventureWorksApi.Models;
using AdventureWorksApi.Functions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using System.Net;
using System.Text.Json.Serialization;

namespace AdventureWorksApi.Functions
{
    public static class AddressFunctions
    {

        public static IResult CreateAddress(AdventureWorksLt2019Context context, Address address)
        {
            if (context.Addresses.Any(a => a.Rowguid == address.Rowguid))
            {
                address.Rowguid = Guid.NewGuid();
            }

            context.Addresses.Add(address);
            context.SaveChanges();

            return Results.Created($"/Addresses", address);
        }

        public static IResult ReadAddress(AdventureWorksLt2019Context context, int id)
        {
            if (id == null || id == -1)
            {
                return Results.Ok(context.Addresses.ToList());
            }

            Address address = context.Addresses.FirstOrDefault(a => a.AddressId == id);

            if (address == null)
            {
                return Results.NotFound();
            }
            else
            {
                return Results.Ok(address);
            };


        }

        public static IResult UpdateAddress(int AddressId, AdventureWorksLt2019Context context, Address updatedAddress)
        {
            Address address = context.Addresses.Find(AddressId);
            if (address == null)
            {
                address = new Address();

                context.Addresses.Add(address);
                context.SaveChanges();
                return Results.Ok("Address is added successful");
            }
            else
            {
                address.AddressLine1 = updatedAddress.AddressLine1;
                address.AddressLine2 = updatedAddress.AddressLine2;
                address.City = updatedAddress.City;
                address.CountryRegion = updatedAddress.CountryRegion;
                address.StateProvince = updatedAddress.StateProvince;
                address.PostalCode = updatedAddress.PostalCode;
                address.Rowguid = updatedAddress.Rowguid;
                address.ModifiedDate = updatedAddress.ModifiedDate;

                return Results.Ok("Address updated successfully");
            }

        }

        public static IResult DeleteAddress(int AddressId, AdventureWorksLt2019Context context)
        {
            Address address = context.Addresses.Find(AddressId);


            if (address == null)
            {

                return Results.BadRequest("Address does not exist.");
            }
            else
            {
                IQueryable<CustomerAddress> relatedCustomerAddresses = context.CustomerAddresses.Where(ca => ca.AddressId == AddressId);

                foreach (CustomerAddress customerAddress in relatedCustomerAddresses)
                {
                    context.CustomerAddresses.Remove(customerAddress);
                }

                context.Addresses.Remove(address);
                context.SaveChanges();

                return Results.Ok("The address is successfully deleted.");
            }
        }

        public static IResult CustomerDetails (int CustomerId, AdventureWorksLt2019Context context)
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
                return Results.BadRequest("Customer does not exist.");
            }

            string serializer = JsonSerializer.Serialize(customer, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                IncludeFields = true
            });

            return Results.Ok(serializer);
        }

        public static IResult AddressDetails(int AddressId, AdventureWorksLt2019Context context)
        {
            var address = context.Addresses.Where(a => a.AddressId == AddressId).Select(b => new
            {
                b.AddressId,
                b.AddressLine1,
                b.AddressLine2,
                b.City,
                b.StateProvince,
                b.CountryRegion,
                b.PostalCode,
                b.Rowguid,
                Customer = b.CustomerAddresses.Select(c => new
                {
                    c.Customer.CustomerId,
                    c.Customer.FirstName,
                    c.Customer.LastName,
                    c.Customer.Phone,
                    c.Customer.EmailAddress,
                    c.Customer.CompanyName,
                    c.Customer.Rowguid
                }).ToList()
            }).FirstOrDefault();

            if (address == null)
            {
                return Results.BadRequest("Address does not exist.");
            };

            string serializer = JsonSerializer.Serialize(address, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                IncludeFields = true
            });

            return Results.Ok(serializer);
        }


    }
}

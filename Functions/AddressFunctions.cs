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

            return Results.Created($"/Address", address);
        }

        public static IResult ReadAddress(AdventureWorksLt2019Context context, int? id)
        {
            if (id == null || id == -1)
            {
                return Results.Ok(context.Addresses.ToList());
            }

            Address? address = context.Addresses.FirstOrDefault(a => a.AddressId == id);

            if (address == null)
            {
                return Results.BadRequest();
            }
            
            return Results.Ok(address);
        }

        public static IResult UpdateAddress(int AddressId, AdventureWorksLt2019Context context, Address updateAddress)
        {
            Address? address = context.Addresses.Find(AddressId);
            if (address == null)
            {
                updateAddress.Rowguid= Guid.NewGuid();

                context.Addresses.Add(updateAddress);
                context.SaveChanges();
                return Results.Created("/Address", updateAddress);
            }
            
            address.AddressLine1 = updateAddress.AddressLine1;
            address.AddressLine2 = updateAddress.AddressLine2;
            address.City = updateAddress.City;
            address.CountryRegion = updateAddress.CountryRegion;
            address.StateProvince = updateAddress.StateProvince;
            address.PostalCode = updateAddress.PostalCode;
            address.ModifiedDate = DateTime.Now;

            context.SaveChanges();

            return Results.Ok(address);
        }

        public static IResult DeleteAddress(int AddressId, AdventureWorksLt2019Context context)
        {
            bool hasReference = context.CustomerAddresses.Any(ca => ca.AddressId == AddressId);

            if (hasReference)
            {
                List<CustomerAddress> references = context.CustomerAddresses.Where(ca => ca.AddressId == AddressId).ToList();
                context.CustomerAddresses.RemoveRange(references);
            }


            Address? address = context.Addresses.FirstOrDefault(a => a.AddressId == AddressId);

            if (address != null)
            {
                context.Addresses.Remove(address);
                context.SaveChanges();
                return Results.Ok(address);
            } else
            {
                return Results.BadRequest();
            }
            
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
                return Results.BadRequest();
            };

            return Results.Json(address, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                IncludeFields = true
            });
        }
    }
}

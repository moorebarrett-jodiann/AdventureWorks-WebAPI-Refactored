using AdventureWorksApi.Models;


namespace AdventureWorksApi.Function
{
    public class Address
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

    }
}

using AdventureWorksApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorksApi.Functions
{
    public static class OrderFunctions
    {
        public static IResult CreateOrder(AdventureWorksLt2019Context context, SalesOrderHeader order)
        {
            order.Rowguid = Guid.NewGuid();
            context.SalesOrderHeaders.Add(order);
            context.SaveChanges();

            return Results.Created($"/Order/Read?int={order.SalesOrderId}", order);
        }

        public static IResult ReadOrder(AdventureWorksLt2019Context context, int? id)
        {
            if (id == null || id == -1)
            {
                return Results.Ok(context.SalesOrderHeaders.ToList());
            }

            SalesOrderHeader? order = context.SalesOrderHeaders.Find(id);

            if (order == null)
            {
                return Results.BadRequest();
            }
               
            return Results.Ok(order);
        }

        public static IResult UpdateOrder(AdventureWorksLt2019Context context, int id, SalesOrderHeader inputOrder)
        {
            SalesOrderHeader? order = context.SalesOrderHeaders.Find(id);

            if (order == null)
            {
                inputOrder.Rowguid = Guid.NewGuid();
                context.SalesOrderHeaders.Add(inputOrder);
                context.SaveChanges();
                return Results.Created("/Orders", order);
            }

            order.RevisionNumber = inputOrder.RevisionNumber;
            order.OrderDate= inputOrder.OrderDate;
            order.DueDate= inputOrder.DueDate;
            order.ShipDate= inputOrder.ShipDate;
            order.Status= inputOrder.Status;
            order.OnlineOrderFlag= inputOrder.OnlineOrderFlag;
            order.SalesOrderNumber= inputOrder.SalesOrderNumber;   
            order.PurchaseOrderNumber= inputOrder.PurchaseOrderNumber;
            order.AccountNumber= inputOrder.AccountNumber;
            order.CustomerId= inputOrder.CustomerId;
            order.ShipToAddressId= inputOrder.ShipToAddressId;
            order.BillToAddressId= inputOrder.BillToAddressId;
            order.ShipMethod= inputOrder.ShipMethod;
            order.CreditCardApprovalCode= inputOrder.CreditCardApprovalCode;
            order.SubTotal = inputOrder.SubTotal;
            order.TaxAmt= inputOrder.TaxAmt;
            order.Freight= inputOrder.Freight;
            order.TotalDue= inputOrder.TotalDue;
            order.Comment= inputOrder.Comment;
            order.ModifiedDate = DateTime.Now;

            context.SaveChanges();
            return Results.Ok(context.SalesOrderHeaders.Find(order.SalesOrderId));
        }

        public static IResult DeleteOrder(AdventureWorksLt2019Context context, int id)
        {
            SalesOrderHeader? order = context.SalesOrderHeaders
                .FirstOrDefault(o => o.SalesOrderId == id);

            if (order != null)
            {
                context.SalesOrderHeaders.Remove(order);
                context.SaveChanges();
                return Results.Ok(order);
            }
            else
            {
                return Results.BadRequest();
            }
        }
    }
}

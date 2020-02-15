// Creates an OData REST service that accepts GET requests, 
// in order to retrieve data from XOrders.xml and XCustomers.xml.
//
// Name: Jacob Zaidi
// Filename: src.svc.fs

namespace A4FS
open System
open System.Collections.Generic
open System.Data.Services
open System.Data.Services.Common
open System.Data.Services.Providers
open System.Linq
open System.ServiceModel
open System.ServiceModel.Web
open System.Web
open System.Xml.Linq
type [<AllowNullLiteral(); DataServiceKey("CustomerID")>] 
  Customer () = class  // public
    member val CustomerID: string = null with get, set
    member val CompanyName: string = null with get, set
    member val ContactName: string = null with get, set
    member val Orders: seq<Order> = null with get, set
    end
and [<AllowNullLiteral(); DataServiceKey("OrderID")>] 
  Order () = class  // public
    member val OrderID: int = 0 with get, set 
    member val CustomerID: string = null with get, set
    member val OrderDate: Nullable<DateTime> = Nullable<DateTime>() with get, set
    member val ShippedDate: Nullable<DateTime> = Nullable<DateTime>() with get, set
    member val Freight: Nullable<decimal> = Nullable<decimal>() with get, set
    member val ShipName: string = null with get, set
    member val ShipCity: string = null with get, set
    member val ShipCountry: string = null with get, set
    member val Customer: Customer = null with get, set
    end
type public MyDataSource () = class
    static let FOLDER = HttpContext.Current.Server.MapPath ("/data") 
    // @"C:\usertmp\";
    static let mutable _Customers: seq<Customer> = null
    static let mutable _Orders: seq<Order> = null
    static let xn = XName.Get
    static do  // MyDataSource ()
        _Customers <- XElement
            .Load(FOLDER + @"\XCustomers.xml")
            .Elements(xn "Customer")
            .Select(fun x -> 
                new Customer (
                    CustomerID = x.Element(xn "CustomerID").Value,
                    CompanyName = x.Element(xn "CompanyName").Value,
                    ContactName = x.Element(xn "ContactName").Value))
            .ToArray()
        _Orders <- XElement
            .Load(FOLDER + @"\XOrders.xml")
            .Elements(xn "Order")
            .Select(fun x -> 
                new Order (
                    OrderID = Int32.Parse(x.Element(xn "OrderID").Value),
                    CustomerID = x.Element(xn "CustomerID").Value,
                    OrderDate = (if x.Elements(xn "OrderDate").Any() && x.Element(xn "OrderDate").Value <> "" then Nullable (DateTime.Parse(x.Element(xn "OrderDate").Value)) else Nullable<DateTime>()), 
                    ShippedDate = (if x.Elements(xn "ShippedDate").Any() && x.Element(xn "ShippedDate").Value <> "" then Nullable (DateTime.Parse(x.Element(xn "ShippedDate").Value)) else Nullable<DateTime>()), 
                    Freight = (if x.Elements(xn "Freight").Any() && x.Element(xn "Freight").Value <> "" then Nullable (decimal (x.Element(xn "Freight").Value)) else Nullable<decimal>()), 
                    ShipName = x.Element(xn "ShipName").Value,
                    ShipCity = x.Element(xn "ShipCity").Value,
                    ShipCountry = x.Element(xn "ShipCountry").Value))
            .ToArray();
        let _orders_lookup = _Orders.ToLookup (fun o -> o.CustomerID)
        let _customers_dict = _Customers.ToDictionary (fun c -> c.CustomerID)
        for o in _Orders do o.Customer <- _customers_dict.[o.CustomerID]
        for c in _Customers do c.Orders <- _orders_lookup.[c.CustomerID]
    member this.Customers
        with get (): IQueryable<Customer> = _Customers.AsQueryable()
    member this.Orders
        with get (): IQueryable<Order> = _Orders.AsQueryable()
    end
[<ServiceBehavior(IncludeExceptionDetailInFaults = true)>]
type public A4FS () = class 
    inherit DataService<MyDataSource> ()
    static member InitializeService (config: DataServiceConfiguration): unit =
        config.SetEntitySetAccessRule ("*", EntitySetRights.AllRead)
        // config.SetServiceOperationAccessRule ("MyServiceOperation", ServiceOperationRights.All)
        config.DataServiceBehavior.MaxProtocolVersion <- DataServiceProtocolVersion.V3;
        config.UseVerboseErrors <- true;
    end    
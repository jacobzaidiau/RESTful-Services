## About ODATA Rest Services

This project uses functional programming techniques to create an OData REST service that accepts GET requests, in order to retrieve data from XOrders.xml and XCustomers.xml.

## Instructions for how to develop, use, and test the code.

1. Run `\CS\_Compile A4CS.bat` or `\FS\_Compile A4FS.bat`.

2. Run `_Service_IISExpress_x86_8181.bat` or `_Service_IISExpress_x64_8181.bat`.

3. Go to the link provided below. If everything succeeds then you will receive a response.

```
http://localhost:8181/A4CS.svc/Orders()?$format=json&$orderby=OrderID&$filter=Freight eq null&$expand=Customer&$select=OrderID,CustomerID,OrderDate,ShippedDate,Freight
```

Note: use A4FS.svc if using the FS project.

4. Play around with the query and have fun!

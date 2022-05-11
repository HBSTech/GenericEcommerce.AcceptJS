# XperienceCommunity.GenericEcommerce.AcceptJS

Accept JS Payment gateway for Generic Ecommerce module (XperienceCommunity.GenericEcomerce OR HBS.Kentico.Ecommerce)

To use make sure your application is already using the Generic Ecommerce solution.

1. Create a Payment Option Provider in the Kentico Admin with a Code Name of "AcceptJS"
2. Install [HBS.Kentico.Ecommerce](https://www.nuget.org/packages/HBS.Kentico.Ecommerce/) (future will be `XperienceCommunity.GenericEcommerce`, working on refactoring)
3. Initialize via the `Startups.cs` service command:
``` csharp 
services.RegisterAcceptJS(new AcceptJSConfiguration("<clientKey>", "<apiLoginID>", "<ApiTransactionKey>"));
```

These keys are found through Authorize.Net

You can optionally provide your own acceptjs view using the PayentGatewayView property of the AcceptJSConfiguration.

# GenericEcommerce.AcceptJS
Accept JS Payment gateway for Generic Ecommerce module

To use make sure your application is already using the Generic.Ecommerce solution.  

Initialize via the Startups.cs service command 
services.RegisterAcceptJS(new AcceptJSConfiguration("7ae4gSdBP6pbLVkAXeRLSmFyuWxR9Ku23j7a8wUQv37RYsH8B7w36573W5sZb8vG", "2LE2uEtp4rq", "74bg98n65MEh4S8x"));

You can optionally provide your own acceptjs view using the 
PayentGatewayView property of the AcceptJSConfiguration.
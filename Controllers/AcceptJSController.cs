using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using CMS.Ecommerce;
using System.Linq;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Core;
using XperienceCommunity.GenericEcommerce.AcceptJS.Models;

namespace XperienceCommunity.GenericEcommerce.AcceptJS.Controllers
{
    public class AcceptJSController : Controller
    {
        public IOrderInfoProvider OrderInfoProvider { get; }
        public IOrderItemInfoProvider OrderItemInfoProvider { get; }
        public ICustomerInfoProvider CustomerInfoProvider { get; }
        public IStateInfoProvider StateInfoProvider { get; }
        public IAcceptJSOptions AcceptJSOptions { get; }
        public ICurrencyInfoProvider CurrencyInfoProvider { get; }
        public ICountryInfoProvider CountryInfoProvider { get; }

        public IExchangeRateInfoProvider ExchangeRateInfoProvider;

        public AcceptJSController(IOrderInfoProvider orderInfoProvider, IOrderItemInfoProvider orderItemInfoProvider, ICustomerInfoProvider customerInfoProvider, IStateInfoProvider stateInfoProvider, IAcceptJSOptions acceptJSOptions, IExchangeRateInfoProvider exchangeRateInfoProvider, ICurrencyInfoProvider currencyInfoProvider, ICountryInfoProvider countryInfoProvider)
        {
            OrderInfoProvider = orderInfoProvider;
            OrderItemInfoProvider = orderItemInfoProvider;
            CustomerInfoProvider = customerInfoProvider;
            StateInfoProvider = stateInfoProvider;
            AcceptJSOptions = acceptJSOptions;
            ExchangeRateInfoProvider = exchangeRateInfoProvider;
            CurrencyInfoProvider = currencyInfoProvider;
            CountryInfoProvider = countryInfoProvider;
        }

        public async Task<IActionResult> GetAuthorization()
        {
            var model = new AuthorizationDataModel
            {
                ClientKey = AcceptJSOptions.AcceptJSClientKey(),
                ApiLoginID = AcceptJSOptions.AcceptJSApiLoginID()
            };
            return new JsonResult(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment([FromBody] AcceptJSDataModel model)
        {
            if (AcceptJSOptions.TestMode())
            {
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
            } 
            else
            {
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            }

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = AcceptJSOptions.AcceptJSApiLoginID(),
                ItemElementName = ItemChoiceType.transactionKey,
                Item = AcceptJSOptions.AcceptJSApiTransactionKey()
            };

            var opaqueData = new opaqueDataType() {
                dataDescriptor = model.DataDescriptor,
                dataValue = model.DataValue
            };

            var paymentType = new paymentType() { Item = opaqueData };

            var order = (await OrderInfoProvider.Get().WhereEquals(nameof(OrderInfo.OrderGUID), model.OrderGUID).TopN(1).GetEnumerableTypedResultAsync()).FirstOrDefault();

            var orderItems = await OrderItemInfoProvider.Get().WhereEquals(nameof(OrderItemInfo.OrderItemOrderID), order.OrderID).GetEnumerableTypedResultAsync();

            var customer = await CustomerInfoProvider.GetAsync(order.OrderCustomerID);

            var lineItems = new List<lineItemType>();

            foreach(var item in orderItems)
            {
                lineItems.Add(new lineItemType() { itemId = item.OrderItemID.ToString(), name = item.OrderItemSKUName, unitPrice = item.OrderItemUnitPrice, quantity = item.OrderItemUnitCount });
            }

            var state = await StateInfoProvider.GetAsync(order.OrderBillingAddress.AddressStateID);
            var country = await CountryInfoProvider.GetAsync(order.OrderBillingAddress.AddressCountryID);

            var billingAddress = new customerAddressType
            {
                firstName = customer.CustomerFirstName,
                lastName = customer.CustomerLastName,
                address = order.OrderBillingAddress.AddressLine1,
                city = order.OrderBillingAddress.AddressCity,
                zip = order.OrderBillingAddress.AddressZip,
                state = state.StateCode,
                country = country.CountryThreeLetterCode
            };
            customerAddressType shippingAddress = null;

            if (order.OrderShippingAddress != null)
            {
                state = await StateInfoProvider.GetAsync(order.OrderShippingAddress.AddressStateID);
                country = await CountryInfoProvider.GetAsync(order.OrderShippingAddress.AddressCountryID);

                shippingAddress = new customerAddressType
                {
                    firstName = customer.CustomerFirstName,
                    lastName = customer.CustomerLastName,
                    address = order.OrderShippingAddress.AddressLine1,
                    city = order.OrderShippingAddress.AddressCity,
                    zip = order.OrderShippingAddress.AddressZip,
                    state = state.StateCode,
                    country = country.CountryThreeLetterCode
                };
            }

            var mainCurrency = Service.Resolve<ISiteMainCurrencySource>().GetSiteMainCurrency(order.OrderSiteID);

            var orderCurrency = await CurrencyInfoProvider.GetAsync(order.OrderCurrencyID);

            var currencyConverter = Service.Resolve<ICurrencyConverterService>();

            var rateToMainCurrency = currencyConverter.GetExchangeRate(orderCurrency.CurrencyCode, mainCurrency.CurrencyCode, order.OrderSiteID);

            var roundingService = Service.Resolve<IRoundingServiceFactory>().GetRoundingService(order.OrderSiteID);

            var shipping = roundingService.Round(currencyConverter.ApplyExchangeRate(order.OrderTotalShipping, rateToMainCurrency), mainCurrency);

            var tax = roundingService.Round(currencyConverter.ApplyExchangeRate(order.OrderTotalTax, rateToMainCurrency), mainCurrency);

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),
                amount = order.OrderGrandTotalInMainCurrency,
                shipping = new extendedAmountType() { amount = shipping, name = "Shipping", description = "Total Order Shipping" },
                tax = new extendedAmountType() { amount = tax, name = "Tax", description = "Total Order Tax" },
                payment = paymentType,
                billTo = billingAddress,
                lineItems = lineItems.ToArray(),
                order = new orderType() { invoiceNumber = order.OrderID.ToString() },
                customer = new customerDataType() { email = customer.CustomerEmail }
            };

            if(shippingAddress != null)
            {
                transactionRequest.shipTo = shippingAddress;
            }

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            var constrollerResponse = new JsonResult(new { Message = new { Message = ResHelper.GetString("AcceptJS.ResponseNull"), Type = 1 } });

            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {
                        if(order != null && response.transactionResponse.responseCode == "1")
                        {
                            // Creates a payment result object that will be viewable in Xperience
                            PaymentResultInfo result = new PaymentResultInfo
                            {
                                PaymentDate = DateTime.Now,
                                PaymentDescription = "Successfully created transaction with Transaction ID: " + response.transactionResponse.transId,
                                PaymentIsCompleted = response.transactionResponse.responseCode == "1",
                                PaymentTransactionID = response.transactionResponse.transId,
                                PaymentStatusValue = $"Response Code: {response.transactionResponse.responseCode},  Message Code: {response.transactionResponse.messages[0].code}, Description: { response.transactionResponse.messages[0].description}",
                                PaymentMethodName = "AcceptJS"
                            };

                            // Saves the payment result to the database
                            order.UpdateOrderStatus(result);

                            return new JsonResult(new { PaymentSuccessful = true });
                        }
                        else
                        {
                            constrollerResponse = new JsonResult(new { Message = new { Message = $"Message Code: { response.transactionResponse.messages[0].code},  Description: {response.transactionResponse.messages[0].description}", Type = 1 } });
                        }
                    }
                    else
                    {
                        if (response.transactionResponse.errors != null)
                        {
                            constrollerResponse = new JsonResult(new { Message = new { Message = $"Failed Transaction. {response.transactionResponse.errors[0].errorCode}: {response.transactionResponse.errors[0].errorText}", Type = 1 } });
                        }
                        else
                        {
                            constrollerResponse = new JsonResult(new { Message = new { Message = "Failed Transaction.", Type = 1 } });
                        }
                    }
                }
                else
                {
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        constrollerResponse = new JsonResult(new { Message = new { Message = $"Failed Transaction. {response.transactionResponse.errors[0].errorCode}: {response.transactionResponse.errors[0].errorText}", Type = 1 } });
                    }
                    else
                    {
                        constrollerResponse = new JsonResult(new { Message = new { Message = $"Failed Transaction. {response.messages.message[0].code}: {response.messages.message[0].text}", Type = 1 } });
                    }
                }
            }
            return constrollerResponse;
        }
    }
}

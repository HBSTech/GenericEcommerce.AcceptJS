using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generic.AcceptJSPaymentGateway.Models
{
    public class AcceptJSViewModel
    {
        public string JsDomain
        {
            get
            {

                if (Development)
                {
                    return "jstest.authorize.net";
                }
                else
                {
                    return "js.authorize.net";
                }
            }
        }

        public bool Development { get; }

        public AcceptJSViewModel(bool development)
        {
            Development = development;
        }
    }
}
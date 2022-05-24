using System;
using System.Collections.Generic;
using System.Text;

namespace XperienceCommunity.GenericEcommerce.AcceptJS
{
    public class AcceptJSOptions : IAcceptJSOptions
    {
        public AcceptJSOptions(AcceptJSConfiguration acceptJSConfiguration)
        {
            AcceptJSConfiguration = acceptJSConfiguration;
        }

        public AcceptJSConfiguration AcceptJSConfiguration { get; }

        public string AcceptJSApiLoginID()
        {
            return AcceptJSConfiguration.AcceptJSApiLoginID;
        }

        public string AcceptJSApiTransactionKey()
        {
            return AcceptJSConfiguration.AcceptJSApiTransactionKey;
        }

        public string AcceptJSClientKey()
        {
            return AcceptJSConfiguration.AcceptJSClientKey;
        }

        public string PayentGatewayView()
        {
            return AcceptJSConfiguration.PayentGatewayView;
        }

        public string JsDomain()
        {
            return AcceptJSConfiguration.JsDomain;
        }
    }
}

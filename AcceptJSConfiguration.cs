namespace XperienceCommunity.GenericEcommerce.AcceptJS
{
    public class AcceptJSConfiguration
    {
        public AcceptJSConfiguration(string acceptJSClientKey, string acceptJSApiLoginID, string acceptJSApiTransactionKey)
        {
            AcceptJSClientKey = acceptJSClientKey;
            AcceptJSApiLoginID = acceptJSApiLoginID;
            AcceptJSApiTransactionKey = acceptJSApiTransactionKey;
        }

        /// <summary>
        /// Log into Authorize.net -> Account -> "Manage Public Client Key" and create new key
        /// </summary>
        public string AcceptJSClientKey { get; set; }

        /// <summary>
        /// APi LoginID: Log into Authorize.net -> Account -> API Credentials & Keys -> "API Login ID" is on the page.
        /// </summary>
        public string AcceptJSApiLoginID { get; set; }

        /// <summary>
        /// Log into Authorize.net -> Account -> API Credentials & Keys -> New Transaction Key (WARNING: THIS DISABLES CURRENT TRANSACTION KEYS)
        /// </summary>
        public string AcceptJSApiTransactionKey { get;set; }

        public string PayentGatewayView { get; set; } = "~/Components/AcceptJS/AcceptJS.cshtml";
    }
}
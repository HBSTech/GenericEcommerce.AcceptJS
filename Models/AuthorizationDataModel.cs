using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace XperienceCommunity.GenericEcommerce.AcceptJS.Models
{
    public class AuthorizationDataModel
    {
        [JsonPropertyName("clientKey")]
        public string ClientKey { get; set; }
        [JsonPropertyName("apiLoginID")]
        public string ApiLoginID { get; set; }
    }
}

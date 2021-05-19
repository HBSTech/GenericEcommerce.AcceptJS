using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Generic.AcceptJSPaymentGateway.Models
{
    public class AcceptJSDataModel
    {
        [JsonPropertyName("orderGUID")]
        public Guid OrderGUID { get; set; }

        [JsonPropertyName("dataDescriptor")]
        public string DataDescriptor { get; set; }

        [JsonPropertyName("dataValue")]
        public string DataValue { get; set; }
    }
}

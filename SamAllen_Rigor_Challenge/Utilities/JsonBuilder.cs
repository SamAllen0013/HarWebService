using Newtonsoft.Json.Linq;
using System;

namespace SamAllen_Rigor_Challenge.Utilities
{
    public static class JsonBuilder
    {
        public static JObject BuildJsonResponse(string response)
        {
            return new JObject(new JProperty("Response", response));
        }

        public static JObject BuildJsonResponse(Object returnObject)
        {
            return (JObject)returnObject;
        }
    }
}
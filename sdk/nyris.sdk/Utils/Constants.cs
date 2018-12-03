using System;

namespace Nyris.Sdk.Utils
{
    public static class Constants
    {
        public static string SDK_ID = "nyris.net";
        public static Uri DEFAULT_HOST_URL = new Uri("https://api.nyris.io");
        public static int DEFAULT_INTEGER = -1;
        public static int DEFAULT_LIMIT = 20;
        public static string DEFAULT_OUTPUT_FORMAT = "application/offers.complete+json";
        public static string DEFAULT_LANGUAGE = "*";
        public static int DEFAULT_NETWORK_CONNECTION_TIMEOUT = 30;
        public static int DEFAULT_HTTP_RETRY_COUNT = 3;
    }
}
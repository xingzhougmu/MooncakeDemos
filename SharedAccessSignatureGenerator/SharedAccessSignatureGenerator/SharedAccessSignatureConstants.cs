using System;

namespace SharedAccessSignatureGenerator
{
    public class SharedAccessSignatureConstants
    {
        public const int MaxKeyNameLength = 256;

        public const int MaxKeyLength = 256;

        public const string SharedAccessSignature = "SharedAccessSignature";

        public const string AudienceFieldName = "sr";

        public const string SignatureFieldName = "sig";

        public const string KeyNameFieldName = "skn";

        public const string ExpiryFieldName = "se";

        public const string SignedResourceFullFieldName = "SharedAccessSignature sr";

        public const string KeyValueSeparator = "=";

        public const string PairSeparator = "&";

        public readonly static DateTime EpochTime;

        public readonly static TimeSpan MaxClockSkew;

        static SharedAccessSignatureConstants()
        {
            SharedAccessSignatureConstants.EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            SharedAccessSignatureConstants.MaxClockSkew = TimeSpan.FromMinutes(5);
        }
    }
}

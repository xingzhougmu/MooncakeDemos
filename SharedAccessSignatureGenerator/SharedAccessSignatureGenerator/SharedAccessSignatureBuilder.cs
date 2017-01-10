using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SharedAccessSignatureGenerator
{
    public class SharedAccessSignatureBuilder
    {
        private string key;

        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                // StringValidationHelper.EnsureBase64String(value, "Key");
                this.key = value;
            }
        }

        public string KeyName
        {
            get;
            set;
        }

        public string Target
        {
            get;
            set;
        }

        public TimeSpan TimeToLive
        {
            get;
            set;
        }

        public string TargetService
        {
            get;
            set;
        }

        public SharedAccessSignatureBuilder()
        {
            this.TimeToLive = TimeSpan.FromMinutes(20);
            TargetService = "iothub";
        }

        private static string BuildExpiresOn(TimeSpan timeToLive)
        {
            DateTime dateTime = DateTime.UtcNow.Add(timeToLive);
            TimeSpan timeSpan = dateTime.Subtract(SharedAccessSignatureConstants.EpochTime);
            return Convert.ToString(Convert.ToInt64(timeSpan.TotalSeconds, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }

        private static string BuildSignature(string keyName, string key, string target, TimeSpan timeToLive, string targetService = "iothub")
        {
            string str = SharedAccessSignatureBuilder.BuildExpiresOn(timeToLive);
            string str1 = WebUtility.UrlEncode(target);
            List<string> strs = new List<string>()
            {
                str1,
                str
            };
            string str2 = SharedAccessSignatureBuilder.Sign(string.Join("\n", strs), key, targetService);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0} {1}={2}&{3}={4}&{5}={6}", new object[] { "SharedAccessSignature", "sr", str1, "sig", WebUtility.UrlEncode(str2), "se", WebUtility.UrlEncode(str) });
            if (!string.IsNullOrEmpty(keyName))
            {
                stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "&{0}={1}", new object[] { "skn", WebUtility.UrlEncode(keyName) });
            }
            return stringBuilder.ToString();
        }

        private static string Sign(string requestString, string key, string targetService)
        {
            string base64String;

            if (!string.IsNullOrEmpty(targetService) && targetService.ToLower() == "servicebus")
            {
                using (HMACSHA256 hMACSHA256 = new HMACSHA256(Encoding.UTF8.GetBytes(key))) // key is not decoded
                {
                    base64String = Convert.ToBase64String(hMACSHA256.ComputeHash(Encoding.UTF8.GetBytes(requestString)));
                }
            }
            else
            {
                using (HMACSHA256 hMACSHA256 = new HMACSHA256(Convert.FromBase64String(key))) // key is decoded
                {
                    base64String = Convert.ToBase64String(hMACSHA256.ComputeHash(Encoding.UTF8.GetBytes(requestString)));
                }
            }

            return base64String;
        }

        public string ToSignature()
        {
            return SharedAccessSignatureBuilder.BuildSignature(this.KeyName, this.Key, this.Target, this.TimeToLive, this.TargetService);
        }
    }
}
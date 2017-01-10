# SharedAccessSignatureGenerator
This tool can be used to generate SAS from Azure Service Bus and IoT Hub. The code is mainly taken from Microsoft.Azure.Devices.dll.

## **Usage**:

```csharp
using SharedAccessSignatureGenerator;
```
### Service Bus:

```csharp
// servicebus
string targetURL = "sb://<service bus namespace>.servicebus.chinacloudapi.cn";
string keyName = "DefaultFullSharedAccessSignature";
string key = <key>;
double ttlValue = 1;

var sasBuilder = new SharedAccessSignatureBuilder()
{
    TargetService = "servicebus",
    Target = targetURL,
    KeyName = keyName,
    Key = key,
    TimeToLive = TimeSpan.FromDays(ttlValue)
};

string sas = sasBuilder.ToSignature();
```
### IoT Hub:

```csharp
//iot hub
string targetURL = "<iot hub name>.azure-devices.cn/devices";
string keyName = <policy name>;
string key = <key>;

var sasBuilder1 = new SharedAccessSignatureBuilder()
{
    Target = targetURL,
    KeyName = keyName,
    Key = key,
    TimeToLive = TimeSpan.FromDays(ttlValue)
};
string sas1 = sasBuilder1.ToSignature();

var sasBuilder2 = new SharedAccessSignatureBuilder()
{
    TargetService="iothub", // optional
    Target = targetURL,
    KeyName = keyName,
    Key = key,
    TimeToLive = TimeSpan.FromDays(ttlValue)
};
string sas2 = sasBuilder1.ToSignature();
```
## References:

1. https://azure.microsoft.com/en-us/documentation/articles/iot-hub-devguide-security/

2. https://azure.microsoft.com/en-us/documentation/articles/service-bus-shared-access-signature-authentication/


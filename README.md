# AS4 Connect Business API Adapter

[Schleupen AS4 Connect](https://www.schleupen.de/loesungen/services/it-services/as4-connect) is a cloud based and managed solution for AS4
communication in the german energy market.
ERP systems can be connected to the of Schleupen AS4 Connect using adapters.

The AS4 Connect Business API Adapter in this repository can be used as a jumpstart to achieve this connection to
the [AS4 Connect - API](https://developer-campus.de/tracks/integration/as4-connect-api/).
This adapter sends messages based on the files in the file system.

The adapter supports the following AS4 services:

* **MP** (Marktprozesse): Edifact
* **FP** (Fahrplanmanagement): ESS, CIM

![image](https://github.com/schleupen/as4-connect-business-api-adapter/assets/68913205/55d9f9dd-f664-482d-8b6f-bb2106baf506)

## Develop & Testing

In order to develop and test an adapter, a [fakeserver](https://github.com/schleupen/as4-connect-business-api-fakeserver-releases) implementation of
the [AS4 Connect - API](https://developer-campus.de/tracks/integration/as4-connect-api/) is available.

![image](https://github.com/schleupen/as4-connect-business-api-adapter/assets/68913205/a37335ab-1ef5-4f4d-a559-33a250849bea)

Certificates for testing authentication will be provided by Schleupen SE.

## Compile & Publish

You will need the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

To build the solution use:

`dotnet build`

If you would like to prepare the solution to publish it, you can use the following command:

`dotnet publish`

This will produce for each Service (MP, FP) a single, self-contained executable that can be run.

## Run

To run the adapter you will need the [client certificates](#certificates) and a [configuration](#configuration) file for the specific service.

* **MP** (Marktprozesse): Schleupen.AS4.BusinessAdapter.MP.Console.exe
* **FP** (Fahrplanmanagement): Schleupen.AS4.BusinessAdapter.FP.Console.exe

### Certificates

For authentication we use mutual TLS (mTLS).
You require a client certificate (issued by Schleupen SE) for each market partner.
The certificate containing the keys to access your market partner data on our cloud system.

More information can be found under [AS4 Connect - API](https://developer-campus.de/tracks/integration/as4-connect-api/) (requires free registration).

### Configuration

The basic configuration of the Adapter is handled through the use of an `appsettings.json` files.

A simple example (only required values):

```
{
  "Adapter": {
    "As4ConnectEndpoint": "https://as4.connect.api",
    "Send": {
      "Directory": "C:\\Send"     
    },
    "Receive": {
      "Directory": "C:\\Receive",
    },
    "Marketpartners": [
      "9984617000002"
    ]
  },
    "EICMapping": {
    "9984617000002": [
      {
        "EIC": "5790000432752",
        "MarktpartnerTyp": "BDEW",
        "Bilanzkreis": "FINGRID",
        "FahrplanHaendlerTyp": "PPS"
      },
      {
        "EIC": "5790000432766",
        "MarktpartnerTyp": "BDEW",
        "Bilanzkreis": "FINGRID",
        "FahrplanHaendlerTyp": "TPS"
      }
    ]
  }
}
```

A full example:

```
{
  "Adapter": {
    "As4ConnectEndpoint": "https://as4.connect.api",
    "Marketpartners": [
      "9984617000002"
    ],
    "CertificateStoreLocation": "LocalMachine",
    "CertificateStoreName": "My"
  },
  "Send": {
      "Directory": "C:\\Send",
      "Retry": {
        "Count": 3,
        "SleepDuration": "00:00:10"
      },
      "MessageLimitCount": 1000,
      "SleepDuration" :  "00:01:00"
  },
  "Receive": {
      "Directory": "C:\\Receive",
      "Retry": {
        "Count": 3,
        "SleepDuration": "00:00:10"
      },
      "SleepDuration" :  "00:01:00",
      "MessageLimitCount": 1000
  },
    "EICMapping": {
    "9984617000002": [
      {
        "EIC": "5790000432752",
        "MarktpartnerTyp": "BDEW",
        "Bilanzkreis": "FINGRID",
        "FahrplanHaendlerTyp": "PPS"
      },
      {
        "EIC": "5790000432766",
        "MarktpartnerTyp": "BDEW",
        "Bilanzkreis": "FINGRID",
        "FahrplanHaendlerTyp": "TPS"
      }
    ],
    "9984616000003": [
      {
        "EIC": "10X000000000RTEM",
        "MarktpartnerTyp": "BDEW",
        "Bilanzkreis": "FINGRID",
        "FahrplanHaendlerTyp": "PPS"
      }
    ]
  }
}
```

| Key                         | .NET type                                                                                                                               | Default value  | Description                                                                                                                                                            |
|-----------------------------|-----------------------------------------------------------------------------------------------------------------------------------------|----------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Adapter:As4ConnectEndpoint  | [string](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0)                                                       | \<null\>       | The endpoint from [AS4 Connect - API](https://developer-campus.de/tracks/integration/as4-connect-api/)                                                                 |
| Adapter:Marketpartners              | [string](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0)[]                                                     | \<null\>       | An array with the identification numbers of your own market partners for which the adapter should send and receive messages.                                           |
| Adapter:CertificateStoreLocation    | [StoreLocation ](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storelocation?view=net-8.0) | CurrentUser    | The [location](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storelocation?view=net-8.0) of the certificate store to use. |
| Adapter:CertificateStoreName        | [StoreName](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storename?view=net-8.0)          | My             | The name of the [certificate store](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storename?view=net-8.0) to use.         |
|                             |                                                                                                                                         |
| Send:Directory              | [string](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0)                                                       | \<null\>       | Specifies where the messages that you want to send out are located.                                                                                                    |
| Send:SleepDuration          | [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-8.0)                                                   | 00:01:00       | The sleep duration before the next send iteration.                                                                                                                     |
| Send:MessageLimitCount      | [int](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types?redirectedfrom=MSDN)      | Int32.MaxValue | The maximum number of messages to send in one iteration.                                                                                                               |
| Send:Retry:Count            | [int](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types?redirectedfrom=MSDN)      | 3              | The maximum number of retries to perform per iteration for each message when sending messages.                                                                         |
| Send:Retry:SleepDuration    | [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-8.0)                                                   | 00:00:10       | The sleep duration between each retry. [only for service usage]                                                                                                       |
|                             |                                                                                                                                         |
| Receive:Directory           | [string](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0)                                                       | \<null\>       | Specifies where received where the messages should be stored.                                                                                                          |
| Receive:SleepDuration       | [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-8.0)                                                   | 00:01:00       | The sleep duration before the next receive iteration.                                                                                                                  |
| Receive:MessageLimitCount   | [int](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types?redirectedfrom=MSDN)      | 0              | The maximum number of messages to receive in one iteration. The default is 0 and means there is no limit.                                                              |
| Receive:Retry:Count         | [int](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types?redirectedfrom=MSDN)      | 3              | The maximum number of retries to perform per iteration for each message when receiving messages.                                                                       |
| Receive:Retry:SleepDuration | [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-8.0)                                                   | 00:00:10       | The sleep duration between each retry. [only for service usage]                                                                                                                               |

### FP configuration

The configuration of the FP Adapter required an additional `EICMapping`.
This maps the AS4 specific Party-Id (eg. 1000000001) to X-amount of EIC-Codes (eg. "11XYYYYYY-V----V") and Party-Type (BDEW, DVGW, GS1),
the FahrplanHaendlertyp(eg. PPS) and the Bilanzkreis (eg. FINGRID) and vice versa.

```
{
  // Adapter,Send,Receive
   "EICMapping": {
    "9984617000002": [
      {
        "EIC": "5790000432752",
        "MarktpartnerTyp": "BDEW",
        "Bilanzkreis": "FINGRID",
        "FahrplanHaendlerTyp": "PPS"
      },
      {
        "EIC": "5790000432766",
        "MarktpartnerTyp": "BDEW",
        "Bilanzkreis": "FINGRID",
        "FahrplanHaendlerTyp": "TPS"
      }
    ],
    "9984616000003": [
      {
        "EIC": "10X000000000RTEM",
        "MarktpartnerTyp": "BDEW",
        "Bilanzkreis": "FINGRID",
        "FahrplanHaendlerTyp": "PPS"
      }
    ]
  }
}
```

Currently tested platforms:

* Windows (requires .NET 8 Runtime)
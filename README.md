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

This repository contains the following executables, corresponding to the AS4 Service.

* **Marktprozesse (MP)**: Schleupen.AS4.BusinessAdapter.MP.Console.exe
* **Fahrplanmanagement (FP)**: Schleupen.AS4.BusinessAdapter.FP.Console.exe

Each executable supports the following commands:

* **send**: Send available messages in the Send.Directory to as4 connect. Will terminate after one iteration.
* **receive**: receives messages from as4 connect and saves them in Receive.Directory. Will terminate after one iteration.
* **service**: send and receives messages continuously.

The options are also documented in the executable itself. For more information execute with `-?`

`Schleupen.AS4.BusinessAdapter.MP.Console.exe -?`
`Schleupen.AS4.BusinessAdapter.FP.Console.exe send -?`

To run the adapter you will need the [client certificates](#certificates) and a [configuration](#configuration) file for the specific service.

Currently tested platforms:

* Windows (requires .NET 8 Runtime)

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
  }
}
```

A full example:

```
{
  "Adapter": {
    "As4ConnectEndpoint": "https://as4.connect.api",
    "Marketpartners": [
      "9900000000001"
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
  }
}
```

| Key                              | .NET type                                                                                                                               | Default value  | Description                                                                                                                                                            |
|----------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------|----------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Adapter:As4ConnectEndpoint       | [string](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0)                                                       | \<null\>       | The endpoint from [AS4 Connect - API](https://developer-campus.de/tracks/integration/as4-connect-api/)                                                                 |
| Adapter:Marketpartners           | [string](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0)[]                                                     | \<null\>       | An array with the identification numbers of your own market partners for which the adapter should send and receive messages.                                           |
| Adapter:CertificateStoreLocation | [StoreLocation ](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storelocation?view=net-8.0) | CurrentUser    | The [location](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storelocation?view=net-8.0) of the certificate store to use. |
| Adapter:CertificateStoreName     | [StoreName](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storename?view=net-8.0)          | My             | The name of the [certificate store](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storename?view=net-8.0) to use.         |
|                                  |                                                                                                                                         |
| Send:Directory                   | [string](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0)                                                       | \<null\>       | Specifies where the messages that you want to send out are located.                                                                                                    |
| Send:SleepDuration               | [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-8.0)                                                   | 00:01:00       | The sleep duration before the next send iteration.                                                                                                                     |
| Send:MessageLimitCount           | [int](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types?redirectedfrom=MSDN)      | Int32.MaxValue | The maximum number of messages to send in one iteration.                                                                                                               |
| Send:Retry:Count                 | [int](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types?redirectedfrom=MSDN)      | 3              | The maximum number of retries to perform per iteration for each message when sending messages.                                                                         |
| Send:Retry:SleepDuration         | [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-8.0)                                                   | 00:00:10       | The sleep duration between each retry. [only for service usage]                                                                                                        |
|                                  |                                                                                                                                         |
| Receive:Directory                | [string](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0)                                                       | \<null\>       | Specifies where received where the messages should be stored.                                                                                                          |
| Receive:SleepDuration            | [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-8.0)                                                   | 00:01:00       | The sleep duration before the next receive iteration.                                                                                                                  |
| Receive:MessageLimitCount        | [int](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types?redirectedfrom=MSDN)      | 0              | The maximum number of messages to receive in one iteration. The default is 0 and means there is no limit.                                                              |
| Receive:Retry:Count              | [int](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types?redirectedfrom=MSDN)      | 3              | The maximum number of retries to perform per iteration for each message when receiving messages.                                                                       |
| Receive:Retry:SleepDuration      | [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-8.0)                                                   | 00:00:10       | The sleep duration between each retry. [only for service usage]                                                                                                        |

### FP configuration

The configuration of the FP Adapter required an additional `EICMapping`.
This maps the EIC-Codes (e.g. `11XYYYYYY-V----V`) to the AS4 specific Party-Id (e.g. `9900000000001`) including the Party-Type (`BDEW`,`DVGW`, `GS1`)
and vice versa.

**Example:** The send directory contains a file with sender EIC `11XSBBBBBBB----N` and receiver EIC `10XDE-EON-NETZ-C`.
In that case the adapter looks up a client certificate for `9900000000001@BDEW` and sends a message to `4033872000058@GS1`.
It is possible to map multiple EIC Code (e.g. `11XSAAAAAAA----N` and `11XSBBBBBBB----N`) to the same Party-Id (e.g. `9900000000001`)

```
{
   // ...sections Adapter, Send, Receive see Configuration
   
  {
    "EICMapping": {
      "9900000000001": [
        {
          "EIC": "11XSAAAAAAA----N",
          "MarktpartnerTyp": "BDEW"
        },
        {
          "EIC": "11XSBBBBBBB----N",
          "MarktpartnerTyp": "BDEW"
        }
      ],
      "4033872000058": [
        {
          "EIC": "10XDE-EON-NETZ-C",
          "MarktpartnerTyp": "GS1"
        }
      ],
      "9911835000001": [
        {
          "EIC": "10XDE-ENBW--TNGX",
          "MarktpartnerTyp": "BDEW"
        }
      ]
    }
  }
}
```

#### FP Filename

The FP adapter expects the files that should be sent, to follow a specific convention:

| MessageTyp         | BDEWDocumentType | Filename                                                                                                     |
|--------------------|------------------|--------------------------------------------------------------------------------------------------------------|
| Acknowledge        | A17              | \<yyyyMMdd>\_\<TYP>\_\<EIC-NAME-BILANZKREIS>\_\<EIC-NAME-TSO>\_\<VVV>\_ACK\_\<yyyy-MM-ddThh-mm-ssZ>.XML</code> |
| AnomalyReport      | A16              | \<yyyyMMdd>\_\<TYP>\_\<EIC-NAME-BILANZKREIS>\_\<EIC-NAME-TSO>\_\<VVV>\_ANO\_\<yyyy-MM-ddThh-mm-ssZ>.XML</code>  |
| ConfirmationReport | A07, A08, A09    | \<yyyyMMdd>\_\<TYP>\_\<EIC-NAME-BILANZKREIS>\_\<EIC-NAME-TSO>\_\<VVV>\_CNF\_\<yyyy-MM-ddThh-mm-ssZ>.XML</code>  |
| StatusRequest      | A59              | \<yyyyMMdd>\_\<TYP>\_\<EIC-NAME-BILANZKREIS>\_\<EIC-NAME-TSO>\_CRQ.XML</code>                                  |
| Schedule           | A01              | \<yyyyMMdd>\_\<TYP>\_\<EIC-NAME-BILANZKREIS>\_\<EIC-NAME-TSO>\_\<VVV>.XML</code>                               |

The FP adapter will also save the received files in the same convention.

## Migration

### 1.0 to 3.0

* use 'Schleupen.AS4.BusinessAdapter.MP.Console.exe' instead of 'Schleupen.AS4.BusinessAdapter.Console.exe'
    * you will get the behaviour of 'Schleupen.AS4.BusinessAdapter.Console.exe' by using the command **service**
* migrate the config file to new format (see [Configuration](#Configuration))
    * configure the send behavior in the section **Send** (concerns the legacy values: SendDirectory, DeliveryRetryCount, DeliveryMessageLimitCount)
    * configure the receiving behavior in the section **Receive** (concerns the legacy values: ReceiveDirectory, ReceivingRetryCount,
      ReceivingMessageLimitCount)

## Changelog

### 3.0

* Support MP and FP Service in separate executables (Schleupen.AS4.BusinessAdapter.MP.Console.exe, Schleupen.AS4.BusinessAdapter.FP.Console.exe)
* Support **.net framework** runtime in core assemblies targeting .netstandard
* The format of the config files has been revised in an incompatible manner (see [Migration](#Migration))

### 1.0

* Supports MP Service in a single executable ( Schleupen.AS4.BusinessAdapter.Console.exe )
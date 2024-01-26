# AS4 Connect Business API Adapter

## Context
Schleupen AS4 Connect is a cloud based and managed solution for AS4 communication in the german energy market. ERP systems can be connected to the WebAPI of Schleupen AS4 Connect using adapters. This WebAPI is documented [on the Developer Campus website](https://developer-campus.de/tracks/integration/as4-connect-api/) (requires free registration).

The AS4 Connect Business API Adapter can be used as a jumpstart to achieve this connection to Schleupen AS4 Connect. It sends and receives EDIFACT messages by using folders on the filesystem. Certificates issued by Schleupen SE and mutual TLS are needed for authentication.
![image](https://github.com/schleupen/as4-connect-business-api-adapter/assets/68913205/55d9f9dd-f664-482d-8b6f-bb2106baf506)



## How to develop
In order to develop and test an adapter, a fakeserver implementation of the WebAPI is available. Certificates for testing authentication will be provided by Schleupen SE.

![image](https://github.com/schleupen/as4-connect-business-api-adapter/assets/68913205/a37335ab-1ef5-4f4d-a559-33a250849bea)


## How to compile
You can use the following command to build the solution (you need Microsoft .NET 8 SDK):

`dotnet build`

If you would like to prepare the solution to publish it, you can use the following command:

`dotnet publish`

This will produce a single, self-contained executable that can be run.

## How to run
You require a certificate for each market partner you want to use AS4 Connect for. These are the keys to access your market partner data on our cloud system and are issued by Schleupen SE. More information regarding AS4 Connect and its API can be found [on the Developer Campus website](https://developer-campus.de/tracks/integration/as4-connect-api/) (requires free registration).


Currently tested platforms:
* Windows (requires .NET 8 Runtime)

The basic configuration of the Adapter is handled through the use of an `appsettings.json` files.

```
{
    "Adapter": {
        "SendDirectory": "C:\\Send",
        "ReceiveDirectory": "C:\\Receive",
        "As4ConnectEndpoint": "https://erp.prod.as4.schleupen.cloud",
        "DeliveryRetryCount": 3,
        "DeliveryMessageLimitCount": 0,
        "ReceivingRetryCount": 3,
        "ReceivingMessageLimitCount": 0,
        "Marketpartners": [
            "9984617000002"
        ],
        "CertificateStoreLocation": "LocalMachine",
        "CertificateStoreName": "My"
    }
}
```

| Configuration value | Description |
| -------- | ------- |
| SendDirectory  | Specifies where the EDIFACT messages that you want to send out are located.|
| ReceiveDirectory | Specifies where received EDIFACT messages should be stored. |
| As4ConnectEndpoint | The endpoint of the AS4 Connect Cloud system. |
| DeliveryRetryCount | The maximum number of retries to perform per iteration for each message when sending messages. |
| DeliveryMessageLimitCount | The maxmimum number of messages to send in one iteration. |
| ReceivingRetryCount | The maximum number of retries to perform per iteration for each message when receiving messages. |
| ReceivingMessageLimitCount | The maximum number of messages to receive in one iteration. The default is 0 and means there is no limit. |
| Marketpartners | An array with the market partners for which the Adapter should handle incoming and outgoing messages. For each of these you required a valid certificate. |
| CertificateStoreLocation | The location of the certificate store to use. [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storelocation?view=net-8.0)    |
| CertificateStoreName | The name of the certificate store to use. [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storename?view=net-8.0) |



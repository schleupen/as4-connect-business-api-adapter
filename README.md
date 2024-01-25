# AS4 Connect Business API Adapter

## How to compile
In order to compile the AS4 Connect Business API Adapter, you need the dotnet 8 SDK. You can use the following command to build the solution:

`dotnet build`

If you would like to prepare the solution to publish it, you can use the following command:

`dotnet publish`

This will produce a single, self-contained executable that can be run.

## How to run
You require a certificate for each market partner you want to use AS4 Connect for. These are the keys to access your market partner data on our cloud system and are issued by Schleupen SE. More information regarding AS4 Connect and its API can be found [on the Developer Campus website](https://developer-campus.de/tracks/integration/as4-connect-api/) (requires free registration).

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



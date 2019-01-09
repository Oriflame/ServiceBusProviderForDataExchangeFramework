# Introduction 
Azure Service Bus Provider for [Data Exchange Framework (DEF)](https://doc.sitecore.com/developers/def/21/data-exchange-framework/en/data-exchange-framework.html) allows you to get data from the [Azure Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview) to Sitecore. By contrast to typical providers that are supporting both ways of data transport, this provider is limited just for the getting data from Service Bus to Sitecore.

Technically, this repository contains:
- source code of all supporting classes
- the necessary configuration files
- the required Sitecore items (in form of [TDS project](https://hedgehogdevelopment.github.io/tds/chapter1.html))
- the SC installation package of the provider

Logically, the provider consist of:
- ServiceBus endpoint
- troubleshooter
- processor for reading data from ServiceBus
- all the ServiceBus specific _business logic_ (to connect to, to create entities and to read data)

# Versions
The [**master**](https://oriflame.visualstudio.com/MarketingAutomation/_git/Ori.Providers.AzureServiceBus?version=GBmaster) branch holds the version suitable for DEF 2.1.0 (Sitecore 9.1). 

The version of this provider suitable for DEF 2.0.1 (Sitecore 9.0) is available [here](https://oriflame.visualstudio.com/MarketingAutomation/_git/MarketingAutomationSC9-Connectors?path=%2FAzureServiceBusProvider.md&version=GBmaster&_a=preview).


# Getting Started
To start using the provider you need to have installed already:
- [Sitecore 9.1](https://dev.sitecore.net/Downloads/Sitecore_Experience_Platform/91/Sitecore_Experience_Platform_91_Initial_Release.aspx)
- [Data Exchange Framework 2.1.0](https://dev.sitecore.net/Downloads/Data_Exchange_Framework/2x/Data_Exchange_Framework_210.aspx) (just the basic package called Data Exchange Framework)
- (not required) [Sitecore Powershell Extensions](https://marketplace.sitecore.net/Modules/Sitecore_PowerShell_console.aspx)

As the next step, simply install the package. TODO: add a link to the package.

Finally, configure the system and start using the provider.

## Endpoint configuration
### **Connection** section
* Connection String - a connection string to the selected Azure namespace from where the provider will read the data (the connection strings can be found in our [keepass](https://oriflamecosmetics.sharepoint.com/teams/MarketingAutomation/SitePages/Keepass---team-passwords-in-one-place.aspx))
* Topic - Azure topic from where the messages will be read (e.g. consultant specific data are sent into the *consultantdata* topic)
* Market - a two letter upper case abbreviation of the selected market (e.g. **CN** for China)
* Sender - identification of the sender from which the provider will consume the messages 

Every message should contain *Sender* and *Market* info stored in *meta data section* of the message (they are not mixed up with the data of the message itself).
According these two values the correct subscription is created by the provider: 
* Subscription name has this syntax: **SC9_MARKET_Sender**, e.g. SC9_CN_Orisales
* The proper **MarketingAutomation** filter is created (filtering the messages for selected market and sender only)

### **ReadingDataSettings** section
* Batch Size - Each time we are going to read messages from Service Bus, we are reading them in a batch of messages (rather than reading them one by one). This parameter specifies the size of this batch.
* Max Number of Messages - This is a maximum number of messages that are read during the one reading process.

For example, the following settings:
* BatchSize=100
* MaxNumberOfMessages=2000

mean that the system will try to read 2000 messages in 20 subsequent batches (by 100 messages).

# Continuous Integration & Continuous Deployment/Delivery
## Local development
In order to develop the code on your local machine you need to (in addition to all installation mentioned in the Getting Started section)
- install [Data Exchange Framework SDK](https://dev.sitecore.net/Downloads/Data_Exchange_Framework/2x/Data_Exchange_Framework_210.aspx) 
- synchronize items from the TDS project in the repository with your local Sitecore
- open solution and make your changes

When you are done with your changes you need to copy the changed assemblies to your wwwroot manually to test the result. 

## Versioning
Initial version of the provider is delivered in form of the standard Sitecore ZIP package. TODO: add a link to the package. 

The subsequent adjustments / changes should be delivered via Sitecore update packages. For this purpose the existing TDS project (**TDS.Master**) should be utilized. For more info see, for example, [this link](https://www.hhogdev.com/help/tds/proppackaging). 

**Important:** One of the basic responsibility of the developer who is updating the provider is to ensure that all affected environments / developers will be informed / updated. 

_Note: Please provide the new version of the provider each time the DEF is upgraded. Use a separate branch (or any other git instrument) to keep it in this repository._


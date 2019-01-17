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
The **master** branch holds the version suitable for DEF 2.1.0 (Sitecore 9.1, .NET Framework 4.7.1). 


# Getting Started
To start using the provider you need to have installed already:
- [Sitecore 9.1](https://dev.sitecore.net/Downloads/Sitecore_Experience_Platform/91/Sitecore_Experience_Platform_91_Initial_Release.aspx)
- [Data Exchange Framework 2.1.0](https://dev.sitecore.net/Downloads/Data_Exchange_Framework/2x/Data_Exchange_Framework_210.aspx) 
- (not required) [Sitecore Powershell Extensions](https://marketplace.sitecore.net/Modules/Sitecore_PowerShell_console.aspx)

As the next step, simply install the [package](https://github.com/Oriflame/ServiceBusProviderForDataExchangeFramework/blob/master/Azure%20Service%20Bus%20Provider-2.0.zip).

Finally, configure the system and start using the provider.

## Endpoint configuration
### **Connection** section
* Connection String - a connection string to the selected Azure namespace from where the provider will read the data 
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
- install [.NET Framework 4.7.1](https://dotnet.microsoft.com/download/visual-studio-sdks?utm_source=getdotnetsdk&utm_medium=referral)
- install Sitecore 9.1 on your local machine
- install [Data Exchange Framework 2.1.0](https://dev.sitecore.net/Downloads/Data_Exchange_Framework/2x/Data_Exchange_Framework_210.aspx)
- (not required, but recommended :)) [Sitecore Powershell Extensions](https://marketplace.sitecore.net/Modules/Sitecore_PowerShell_console.aspx)
- install [Data Exchange Framework SDK](https://dev.sitecore.net/Downloads/Data_Exchange_Framework/2x/Data_Exchange_Framework_210.aspx) 
- synchronize items from the TDS project in the repository with your local Sitecore
  - be careful when using TDS with _Lightning mode_: sometimes synchronization must be run twice, but with Lightning mode on you don't have to see the changes the second time :(
- open solution and make your changes

When you are done with your changes you need to copy the changed assemblies to your wwwroot manually to test the result. 

## Versioning
Initial version of the provider is delivered in form of the standard [Sitecore ZIP package](https://github.com/Oriflame/ServiceBusProviderForDataExchangeFramework/blob/master/Azure%20Service%20Bus%20Provider-2.0.zip). 

The subsequent adjustments / changes should be delivered via Sitecore update packages. For this purpose the existing TDS project (**[TDS.Master](https://github.com/Oriflame/ServiceBusProviderForDataExchangeFramework/tree/master/TDS.Master)**) should be utilized. For more info see, for example, [this link](https://www.hhogdev.com/help/tds/proppackaging). 

**Important:** One of the basic responsibility of the developer who is updating the provider is to ensure that all affected environments / developers will be informed / updated. 

_Note: Please provide the new version of the provider each time the DEF is upgraded. Use a separate branch (or any other git instrument) to keep it in this repository._


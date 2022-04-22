# Azure Blob Storage Archive Pipeline Component
The Azure Blob Storage Archive Pipeline Component is, as the name suggests, a component that enables you to archive incoming/outgoing messages from any adapters to an Azure Blog Storage account. 

This is the list of properties that you can set up on the archive pipeline component:

| Property Name  | Description  | Sample Values |
| -------------  | -----------  | ------------- |
| ContainerName  | Blob Container Name | biztalk-app-storage  |
| StorageAccntConnection | ConnectionString to the Storage Account | DefaultEndpointsProtocol=https;AccountName=<storage-account-name>;AccountKey=<account-key>;EndpointSuffix=core.windows.net |

Of course, to be able to use it we need to create a Storage Account in our Azure Subscription.

THIS PIPELINE COMPONENT IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND.

# About Us
**Sandro Pereira** | [DevScope](http://www.devscope.net/) | MVP & MCTS BizTalk Server 2010 | [https://blog.sandro-pereira.com/](https://blog.sandro-pereira.com/) | [@sandro_asp](https://twitter.com/sandro_asp)

**Diogo Formosinho** | DevScope | BizTalk Server Developer | https://www.linkedin.com/in/diogo-formosinho-242b221a2/
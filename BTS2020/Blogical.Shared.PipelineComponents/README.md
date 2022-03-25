# SQL Archive Pipeline Component
The SQL Archive Pipeline Component is a component created by [Johan Hedberg](https://www.linkedin.com/in/mvpjohanhedberg/?originalSubdomain=se) on his project Blogical.Shared.PipelineComponents.

This component has the availability to archive incoming/outgoing messages from any adapters to a SQL Server database. 

This is the list of properties that you can set up on the archive pipeline component:

| Property Name  | Description  | Sample Values |
| -------------  | -----------  | ------------- |
| Enabled  | Define if the archive is enable or not | true/false  |
| ConnectionString | ConnectionString to the archive database  | Provider=SQLNCLI11;Server=myServerAddress;Database=myDataBase;Trusted_Connection=yes; |

Because I recently used it I decide to migrate this amazing compinent to BizTalk Server 2020.

THIS PIPELINE COMPONENT IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND.

# About Us
*Johan Hedberg** | [LinkedIn](https://www.linkedin.com/in/mvpjohanhedberg/?originalSubdomain=se)

**Sandro Pereira** | [DevScope](http://www.devscope.net/) | MVP & MCTS BizTalk Server 2010 | [https://blog.sandro-pereira.com/](https://blog.sandro-pereira.com/) | [@sandro_asp](https://twitter.com/sandro_asp)
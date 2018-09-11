# BizTalk PipelineComponents Extensions UtilityPack
BizTalk PipelineComponents Extensions UtilityPack is a set of custom pipeline components (libraries) with several custom pipeline components that can be used in received and sent pipelines, which will provide an extension of BizTalk out-of-the-box pipeline capabilities.

![BizTalk PipelineComponents Extensions UtilityPack](media/BizTalk-PipelineComponents-Extensions-UtilityPack.png)

## Content-Based Routing
### CBRIdocOperationPromotionEncode

* Content Based Routing Component to promote IDOC Operation property.
  * This component requires one configuration that is the MessageType string to be ignored. Then it will take the last string (word) from the MessageType Message Context Property and promote it to the Operation Message Context Property.

### CBROperationPromotionEncode

* Content Based Routing Component to promote Operation property.
  * This component doesn't requires any configuration. Then it will take the value (word) which lies ahead of the cardinal (#) from the MessageType message context property and promote it to the Operation Message Context Property.

## Deploying Pipeline Components
All the .NET pipeline component assemblies (native and custom) must be located in the <installation directory>\Pipeline Components folder to be executed by the server. If the pipeline with a custom component will be deployed across several servers, the component's binaries must be present in the specified folder on every server.

You do not need to add a custom pipeline component to be used by the BizTalk Runtime to the Global Assembly Cache (GAC).

To know more about Deploying Pipeline Components, please see: [Deploying Pipeline Components](https://docs.microsoft.com/en-us/biztalk/core/deploying-pipeline-components)

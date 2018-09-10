# BizTalk PipelineComponents Extensions UtilityPack
BizTalk PipelineComponents Extensions UtilityPack is a set of custom pipeline components (libraries) with several custom pipeline components that can be used in received and sent pipelines, which will provide an extension of BizTalk out-of-the-box pipeline capabilities.

## Content-Based Routing
### CBRIdocOperationPromotionDecode

* Content Based Routing Component to promote IDOC Operation property.
* * This component requires one configuration that is the MessageType string to be ignored. Then it will take the last string (word) from the MessageType Message Context Property and promote it to the Operation Message Context Property.

### CBROperationPromotionDecode

* Content Based Routing Component to promote Operation property.
* * This component doesn't requires any configuration. Then it will take the value (word) which lies ahead of the cardinal (#) from the MessageType message context property and promote it to the Operation Message Context Property.

# Receive Location Name Property Promotion Pipeline Component

## Receive Location Name Property Promotion Pipeline Component
Receive Location Name Property Promotion Pipeline Component is a simple pipeline component to promote the Receive Location Name (ReceiveLocationName) property to the context of the message. Several BizTalk Server context properties are not promoted by default with BizTalk Server, which means that they are not available for routing. 

One such property is the ReceiveLocationName property. While the ReceivePortName property is available in the BTS namespace, the ReceiveLocationName property is not promoted. It cannot be used for routing nor access it from inside an orchestration.

My team and I kept that behavior creates this project as a proof-of-concept to explain how you can promote properties to the context of the message.

## BizTalk.Pipeline.Components.RcvLocationPromotion

Project that contains the Property Schema to be used with the Receive Location Name Property Promotion Pipeline Component.

THIS PIPELINE COMPONENT IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND.

# About Us
**Sandro Pereira** | [DevScope](http://www.devscope.net/) | MVP & MCTS BizTalk Server 2010 | [https://blog.sandro-pereira.com/](https://blog.sandro-pereira.com/) | [@sandro_asp](https://twitter.com/sandro_asp)

**Diogo Formosinho** | [DevScope](http://www.devscope.net/) | BizTalk Server Developer | [https://www.linkedin.com/in/diogo-formosinho-242b221a2/](https://www.linkedin.com/in/diogo-formosinho-242b221a2/)

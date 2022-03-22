# BizTalk PDF2Xml Pipeline Component
BizTalk PDF2Xml Pipeline Component is, as the name mentioned, a decode component that transforms the content of a PDF document to an XML message that BizTalk can understand and process. The component uses the itextsharp library to extract the PDF content. The original source code was available on the CodePlex (pdf2xmlbiztalk.codeplex.com). Still, I couldn't validate who was the original creator. So, the component first transforms the PDF content to HTML, and then using an external XSLT, will apply a transformation to convert the HTML into a know XML document that BizTalk Server can process.

My team and I kept that behavior, but we extended this component and added the capability also to, by default, convert it to a well know XML without the need for you to use an XSTL transformation directly on the pipeline.

Once you pass the PDF by this component and depending on how you configure it, the outcome can be:
* All PDF content in an HTML format;
* All PDF content in an XML format;
* Part of the PDF content on an XML format (if you apply a transformation)

Unfortunately, on my initial tests, this component works well with some PDF files, but others simply ignore its content. Nevertheless, I make it available as a prof-of-concept.

This is the list of properties that you can set up on the archive pipeline component:

| Property Name  | Description  | Sample Values |
| -------------  | -----------  | ------------- |
| InternalProcessToHTML  | Value to decide if you want the component to transform the PDF content to HTML or XML | True/False  |
| IsToApplyTrasnformation | Value to decide if you want to apply a transformation on the pipeline component or not | True/False |
| XsltFilePath | Path to an XSLT transformation file | C:\transf\mymap.xslt |

THIS PIPELINE COMPONENT IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND.

# About Us
**Sandro Pereira** | [DevScope](http://www.devscope.net/) | MVP & MCTS BizTalk Server 2010 | [https://blog.sandro-pereira.com/](https://blog.sandro-pereira.com/) | [@sandro_asp](https://twitter.com/sandro_asp)

**Diogo Formosinho** | [DevScope](http://www.devscope.net/) | BizTalk Server Developer | [https://www.linkedin.com/in/diogo-formosinho-242b221a2/](https://www.linkedin.com/in/diogo-formosinho-242b221a2/)

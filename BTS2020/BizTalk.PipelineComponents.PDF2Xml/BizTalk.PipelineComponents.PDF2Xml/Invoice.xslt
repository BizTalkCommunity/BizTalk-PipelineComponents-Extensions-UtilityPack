<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="/">
      <Invoice xmlns="http://pdf.transformed.dustin/document">
        <Header>
        <Supplier>
          <xsl:value-of select="/HTML/BODY/TABLE/TR[./TD='Organisationsnummer:'][2]/TD[4]"/>
        </Supplier>
        <SupplierVAT>
          <xsl:value-of select="/HTML/BODY/TABLE/TR[./TD='Vat-nummer:']/TD[5]"/>
        </SupplierVAT>
        <InvoiceDate>
          <xsl:value-of select="/HTML/BODY/TABLE/TR[3]/TD[2]"/>
        </InvoiceDate>
        <DueDate>
          <xsl:value-of select="/HTML/BODY/TABLE/TR[./TD='Förfallodatum:']/TD[2]"/>
        </DueDate>
        <DelivDate>
          <xsl:value-of select="/HTML/BODY/TABLE/TR[./TD='Leveransdatum:']/TD[2]"/>
        </DelivDate>
        <OrderNo>
          <xsl:value-of select="/HTML/BODY/TABLE/TR[3]/TD[3]"/>
        </OrderNo>
        <InvoiceNo>
          <xsl:value-of select="/HTML/BODY/TABLE/TR[3]/TD[4]"/>
        </InvoiceNo>
        <PurchaseNo>
          <xsl:value-of select="/HTML/BODY/TABLE/TR[./TD='Ert inköpsnummer:']/TD[2]"/>
        </PurchaseNo>
          <PaymentID>
            <xsl:value-of select="/HTML/BODY/TABLE/TR[./TD='OCR-nummer:']/TD[1]"/>
          </PaymentID>
          <InvoiceAmount>
            <xsl:value-of select="translate(/HTML/BODY/TABLE/TR[./TD='Summa inkl. moms:']/TD[2],'SEKDFI ','')"/>
          </InvoiceAmount>
        </Header>
        <InvoiceLines>
        
<!--<xsl:for-each select="/HTML/BODY/TABLE/TR[TD = 'Artikelnummer']/following-sibling::*[count(./TD) = 8]">-->
        <xsl:for-each select="/HTML/BODY/TABLE/TR[TD = 'Artikelnummer']/following-sibling::*[./following-sibling::*[./TD ='Nettobelopp']]">
          <Item>
          <ItemNo>
            <xsl:value-of select="./TD[1]"/>
          </ItemNo>
          <ItemDescription>
            <xsl:value-of select="./TD[2]"/>
          </ItemDescription>
          <Qty>
            <xsl:value-of select="./TD[3]"/>
          </Qty>
          <Price>
            <xsl:value-of select="./TD[5]"/>
          </Price>
          <Tax>
            <xsl:value-of select="./TD[6]"/>
          </Tax>
          <TaxAmount>
            <xsl:value-of select="./TD[7]"/>
          </TaxAmount>
          <LineAmount>
            <xsl:value-of select="./TD[8]"/>
          </LineAmount>
          </Item>
        </xsl:for-each>
          </InvoiceLines>
          
        
      </Invoice>
        
    </xsl:template>
</xsl:stylesheet>

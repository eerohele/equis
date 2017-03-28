<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:import href="stylesheet-b.xsl"/>

  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

  <xsl:param name="quux"/>

  <xsl:template match="c">
    <e>
      <xsl:value-of select="$quux"/>
    </e>
  </xsl:template>

</xsl:stylesheet>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

  <xsl:param name="foo"/>

  <xsl:template match="a">
    <c>
      <xsl:value-of select="$foo"/>
    </c>
  </xsl:template>

  <xsl:include href="stylesheet-b.xsl"/>

</xsl:stylesheet>

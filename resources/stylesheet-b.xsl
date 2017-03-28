<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

  <xsl:param name="baz"/>

  <xsl:template match="b">
    <d>
      <xsl:value-of select="$baz"/>
    </d>
  </xsl:template>

</xsl:stylesheet>

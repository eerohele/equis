<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="a">
    <b>
      <xsl:value-of select="$UNDEFINED"/>
    </b>
  </xsl:template>

</xsl:stylesheet>

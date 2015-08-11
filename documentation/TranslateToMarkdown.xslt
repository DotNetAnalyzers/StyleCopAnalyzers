<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

  <xsl:output method="text"/>

  <xsl:template match="/html">
    <xsl:variable name="TypeName" select="//table[1]/tr[1]/td[2]/p/text()"/>
    <xsl:variable name="CheckId" select="//table[1]/tr[2]/td[2]/p/text()"/>
    <xsl:variable name="Category" select="//table[1]/tr[3]/td[2]/p/text()"/>

    <xsl:text>## </xsl:text>
    <xsl:value-of select="$CheckId"/>
    <xsl:text>&#xa;&#xa;</xsl:text>
    <xsl:text><![CDATA[<table>
<tr>
  <td>TypeName</td>
  <td>]]></xsl:text>
    <xsl:value-of select="$CheckId"/>
    <xsl:value-of select="$TypeName"/>
    <xsl:text><![CDATA[</td>
</tr>
<tr>
  <td>CheckId</td>
  <td>]]></xsl:text>
    <xsl:value-of select="$CheckId"/>
    <xsl:text><![CDATA[</td>
</tr>
<tr>
  <td>Category</td>
  <td>]]></xsl:text>
    <xsl:value-of select="$Category"/>
    <xsl:text><![CDATA[</td>
</tr>
</table>]]></xsl:text>

    <xsl:apply-templates mode="block" select="//table[1]/following-sibling::node()"/>

    <xsl:text>&#xa;</xsl:text>
    <xsl:text>```csharp&#xa;</xsl:text>
    <xsl:text>#pragma warning disable </xsl:text>
    <xsl:value-of select="$CheckId"/>
    <xsl:text> // </xsl:text>
    <xsl:value-of select="$TypeName"/>
    <xsl:text>&#xa;</xsl:text>
    <xsl:text>#pragma warning restore </xsl:text>
    <xsl:value-of select="$CheckId"/>
    <xsl:text> // </xsl:text>
    <xsl:value-of select="$TypeName"/>
    <xsl:text>&#xa;```&#xa;</xsl:text>
  </xsl:template>

  <xsl:template mode="block" match="@* | node()">
    <xsl:text>???</xsl:text>
  </xsl:template>

  <xsl:template mode="inline" match="@* | node()">
    <xsl:text>???</xsl:text>
  </xsl:template>

  <xsl:template mode="table" match="@* | node()">
    <xsl:text>???</xsl:text>
  </xsl:template>

  <xsl:template mode="block" match="h2|H2">
    <xsl:text>&#xa;&#xa;</xsl:text>
    <xsl:text>## </xsl:text>
    <xsl:value-of select="text()"/>
  </xsl:template>

  <xsl:template mode="block" match="p|P">
    <xsl:choose>
      <xsl:when test="@style">
        <xsl:text>&#xa;</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text>&#xa;&#xa;</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:apply-templates mode="inline" select="node()"/>
  </xsl:template>

  <xsl:template mode="block" match="img|IMG">
    <xsl:text>&#xa;&#xa;</xsl:text>
    <xsl:text>![</xsl:text>
    <xsl:value-of select="@alt"/>
    <xsl:text>](</xsl:text>
    <xsl:value-of select="@src"/>
    <xsl:text>)</xsl:text>
  </xsl:template>

  <xsl:template mode="block" match="table|TABLE">
    <xsl:text>&#xa;&#xa;</xsl:text>
    <xsl:text><![CDATA[<table>]]></xsl:text>
    <xsl:apply-templates mode="table" select="node()"/>
    <xsl:text>&#xa;</xsl:text>
    <xsl:text><![CDATA[</table>]]></xsl:text>
  </xsl:template>

  <xsl:template mode="table" match="tbody|TBODY">
    <xsl:apply-templates mode="table" select="node()"/>
  </xsl:template>

  <xsl:template mode="table" match="tr|TR">
    <xsl:text>&#xa;</xsl:text>
    <xsl:text><![CDATA[  <tr>]]></xsl:text>
    <xsl:apply-templates mode="table" select="node()"/>
    <xsl:text>&#xa;</xsl:text>
    <xsl:text><![CDATA[  </tr>]]></xsl:text>
  </xsl:template>

  <xsl:template mode="table" match="td|TD">
    <xsl:text>&#xa;</xsl:text>
    <xsl:text><![CDATA[    <td>]]></xsl:text>
    <xsl:apply-templates mode="tableinline" select="node()"/>
    <xsl:text><![CDATA[</td>]]></xsl:text>
  </xsl:template>

  <xsl:template mode="tableinline" match="@* | node()">
    <xsl:apply-templates mode="inline" select="."/>
  </xsl:template>

  <xsl:template mode="tableinline" match="text()[translate(normalize-space(.), ' ', '')='']">
  </xsl:template>

  <xsl:template mode="tableinline" match="p|P">
    <xsl:apply-templates mode="inline" select="node()"/>
  </xsl:template>

  <xsl:template mode="block" match="pre">
    <xsl:text>&#xa;&#xa;</xsl:text>
    <xsl:text>```csharp&#xa;</xsl:text>
    <xsl:apply-templates mode="inline" select="@* | node()"/>
    <xsl:text>&#xa;```&#xa;</xsl:text>
  </xsl:template>

  <xsl:template mode="block" match="text()[translate(normalize-space(.), ' ', '')='']">
  </xsl:template>

  <xsl:template mode="table" match="text()">
  </xsl:template>

  <xsl:template mode="inline" match="a|A">
    <xsl:text>[</xsl:text>
    <xsl:apply-templates mode="inline" select="node()"/>
    <xsl:text>](</xsl:text>
    <xsl:value-of select="@href"/>
    <xsl:text>)</xsl:text>
  </xsl:template>

  <xsl:template mode="inline" match="a[count(node())=0]|A[count(node())=0]">
  </xsl:template>

  <xsl:template mode="inline" match="em|EM">
    <xsl:text>*</xsl:text>
    <xsl:apply-templates mode="inline" select="@* | node()"/>
    <xsl:text>*</xsl:text>
  </xsl:template>

  <xsl:template mode="inline" match="strong|STRONG">
    <xsl:text>**</xsl:text>
    <xsl:apply-templates mode="inline" select="@* | node()"/>
    <xsl:text>**</xsl:text>
  </xsl:template>

  <xsl:template mode="inline" match="img|IMG">
    <xsl:text>![</xsl:text>
    <xsl:value-of select="@alt"/>
    <xsl:text>](</xsl:text>
    <xsl:value-of select="@src"/>
    <xsl:text>)</xsl:text>
  </xsl:template>

  <xsl:template mode="inline" match="br|BR">
    <xsl:text>&#xa;</xsl:text>
  </xsl:template>

  <xsl:template mode="inline" match="text()">
    <xsl:copy/>
  </xsl:template>

  <xsl:template mode="inline" match="span|SPAN|font|FONT">
    <xsl:apply-templates mode="inline" select="node()"/>
  </xsl:template>
</xsl:stylesheet>

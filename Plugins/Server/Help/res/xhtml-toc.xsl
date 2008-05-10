<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://www.w3.org/1999/xhtml">                
  
  <!-- Use an HTML CSS stylesheet. -->
  <!--<xsl:param name="html.stylesheet">style.css</xsl:param>--><!-- SDK: doesn't work, can't get this to coexist with my CSS-->

  <!-- xhtml-toc.xsl Stylesheet to create a TOC client-side for h1 and h2 XHTML elements
       Daniel Schneider from tecfa.unig.ch (feb 2007)
       Get it from: http://tecfa.unige.ch/guides/xml/examples/xsl-toc/
       This code is freeware.
       Tested with Firefox 1.5x (linux) and 2.0x (win) and IE 6 (win)
       For IE compatibility, the XHMTL file must be called file.xml (even if the mime type is text/xml).
       If you change encoding you have to fix this file, i.e replace the &#160 and § characters
       XHTML should be real XML like below and h1 and h2 headers can be put in any place I think
       - - - - -
      <?xml version="1.0" encoding="ISO-8859-1" ?>
      <?xml-stylesheet href="xhtml-toc.xsl" version="1.0" type="text/xsl"?>
      <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
                             "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

        <html xmlns:h="http://www.w3.org/1999/xhtml">
          <head> ... </head><body> ... </body> ... </html>
       - - - - -
       Note: A less ugly solution is in xhtml-to-ns.xsl
             Bad solutions are files *bad* in the same directory
     -->

  <xsl:output
    method="xml"
    doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"
    doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN"
    indent="yes"
    encoding="iso-8859-1"
    />

  <xsl:template match="/*[local-name()='html']">
  <html xmlns="http://www.w3.org/1999/xhtml">
    <head>
      <title><xsl:value-of select="*[local-name()='title']"/></title>
    </head>
    <body bgcolor="#FFFFFF">
      <xsl:apply-templates select="*[local-name()='body']"/>
    </body>
  </html>
</xsl:template>

<xsl:template match="*[local-name()='body']">
  <strong><a name="toc">Contents</a></strong>
  <xsl:apply-templates select="*[local-name()='h1']|*[local-name()='h2']" mode="toc"/>
  <xsl:apply-templates />
</xsl:template>

<xsl:template match="*[local-name()='h1']" mode="toc">
  <br/>
  <a href="#h1_{generate-id(.)}"><xsl:value-of select="."/></a><br/>
</xsl:template>

<xsl:template match="*[local-name()='h2']" mode="toc">
  <span style="font-size:small;">
    <xsl:text> &#160;&#160;§ </xsl:text>
    <a href="#h2_{generate-id(.)}"><xsl:value-of select="."/></a>
  </span>
</xsl:template>

<xsl:template match="*[local-name()='h1']">
  <h1><a name="h1_{generate-id(.)}"><xsl:value-of select="."/></a> (<a href="#toc">&#171;up</a>)</h1>
</xsl:template>

<xsl:template match="*[local-name()='h2']">
  <h2><a name="h2_{generate-id(.)}"><xsl:value-of select="."/></a> (<a href="#toc">&#171;up</a>)</h2>
</xsl:template>


<!-- A default rule will just copy all the rest -->

<xsl:template match="*">
    <xsl:element name="{name(.)}">
      <xsl:for-each select="@*">
        <xsl:attribute name="{name(.)}">
          <xsl:value-of select="."/>
        </xsl:attribute>
      </xsl:for-each>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

</xsl:stylesheet>

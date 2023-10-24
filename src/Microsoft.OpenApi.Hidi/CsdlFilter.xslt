<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:edm="http://docs.oasis-open.org/odata/ns/edm"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:param name="entitySetOrSingleton" select="'serviceappointments'"></xsl:param>
    
    <xsl:template match="edm:EntityContainer">
        <xsl:copy>
            <xsl:apply-templates select="@* | edm:EntitySet[contains($entitySetOrSingleton,@Name)] | edm:Singleton[contains($entitySetOrSingleton,@Name)]"/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>

</xsl:stylesheet>

--- snmplib/snmpusm.c.orig	2018-07-16 16:33:40.000000000 +0200
+++ snmplib/snmpusm.c	2018-10-07 10:19:02.166439000 +0200
@@ -117,6 +117,8 @@
 oid    usmAESPrivProtocol[10] = { 1, 3, 6, 1, 6, 3, 10, 1, 2, 4 };
 /* backwards compat */
 oid    *usmAES128PrivProtocol = usmAESPrivProtocol;
+oid    *usmAES192PrivProtocol = usmAESPrivProtocol;
+oid    *usmAES256PrivProtocol = usmAESPrivProtocol;
 
 #ifdef NETSNMP_DRAFT_BLUMENTHAL_AES_04
     /* OIDs from http://www.snmp.com/eso/esoConsortiumMIB.txt */

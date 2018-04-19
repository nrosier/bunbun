Index: files/patch-lib_dns_openssldh__link.c
===================================================================
--- files/patch-lib_dns_openssldh__link.c	(nonexistent)
+++ files/patch-lib_dns_openssldh__link.c	(working copy)
@@ -0,0 +1,11 @@
+--- lib/dns/openssldh_link.c.orig	2018-03-25 00:15:52 UTC
++++ lib/dns/openssldh_link.c
+@@ -69,7 +69,7 @@ static isc_result_t openssldh_todns(cons
+ 
+ static BIGNUM *bn2, *bn768, *bn1024, *bn1536;
+ 
+-#if OPENSSL_VERSION_NUMBER < 0x10100000L || defined(LIBRESSL_VERSION_NUMBER)
++#if OPENSSL_VERSION_NUMBER < 0x10100000L || ( defined(LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20700000L )
+ /*
+  * DH_get0_key, DH_set0_key, DH_get0_pqg and DH_set0_pqg
+  * are from OpenSSL 1.1.0.

Property changes on: files/patch-lib_dns_openssldh__link.c
___________________________________________________________________
Added: fbsd:nokeywords
## -0,0 +1 ##
+yes
\ No newline at end of property
Added: svn:eol-style
## -0,0 +1 ##
+native
\ No newline at end of property
Added: svn:mime-type
## -0,0 +1 ##
+text/plain
\ No newline at end of property
Index: files/patch-lib_dns_openssldsa__link.c
===================================================================
--- files/patch-lib_dns_openssldsa__link.c	(nonexistent)
+++ files/patch-lib_dns_openssldsa__link.c	(working copy)
@@ -0,0 +1,11 @@
+--- lib/dns/openssldsa_link.c.orig	2018-03-25 00:16:57 UTC
++++ lib/dns/openssldsa_link.c
+@@ -49,7 +49,7 @@
+ 
+ static isc_result_t openssldsa_todns(const dst_key_t *key, isc_buffer_t *data);
+ 
+-#if OPENSSL_VERSION_NUMBER < 0x10100000L || defined(LIBRESSL_VERSION_NUMBER)
++#if OPENSSL_VERSION_NUMBER < 0x10100000L || ( defined(LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20700000L )
+ static void
+ DSA_get0_pqg(const DSA *d, const BIGNUM **p, const BIGNUM **q,
+ 	     const BIGNUM **g)

Property changes on: files/patch-lib_dns_openssldsa__link.c
___________________________________________________________________
Added: fbsd:nokeywords
## -0,0 +1 ##
+yes
\ No newline at end of property
Added: svn:eol-style
## -0,0 +1 ##
+native
\ No newline at end of property
Added: svn:mime-type
## -0,0 +1 ##
+text/plain
\ No newline at end of property
Index: files/patch-lib_dns_opensslecdsa__link.c
===================================================================
--- files/patch-lib_dns_opensslecdsa__link.c	(nonexistent)
+++ files/patch-lib_dns_opensslecdsa__link.c	(working copy)
@@ -0,0 +1,11 @@
+--- lib/dns/opensslecdsa_link.c.orig	2018-03-25 00:17:52 UTC
++++ lib/dns/opensslecdsa_link.c
+@@ -42,7 +42,7 @@
+ 
+ #define DST_RET(a) {ret = a; goto err;}
+ 
+-#if OPENSSL_VERSION_NUMBER < 0x10100000L || defined(LIBRESSL_VERSION_NUMBER)
++#if OPENSSL_VERSION_NUMBER < 0x10100000L || ( defined(LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20700000L )
+ /* From OpenSSL 1.1 */
+ static void
+ ECDSA_SIG_get0(const ECDSA_SIG *sig, const BIGNUM **pr, const BIGNUM **ps) {

Property changes on: files/patch-lib_dns_opensslecdsa__link.c
___________________________________________________________________
Added: fbsd:nokeywords
## -0,0 +1 ##
+yes
\ No newline at end of property
Added: svn:eol-style
## -0,0 +1 ##
+native
\ No newline at end of property
Added: svn:mime-type
## -0,0 +1 ##
+text/plain
\ No newline at end of property
Index: files/patch-lib_dns_opensslrsa__link.c
===================================================================
--- files/patch-lib_dns_opensslrsa__link.c	(nonexistent)
+++ files/patch-lib_dns_opensslrsa__link.c	(working copy)
@@ -0,0 +1,11 @@
+--- lib/dns/opensslrsa_link.c.orig	2018-03-25 00:18:28 UTC
++++ lib/dns/opensslrsa_link.c
+@@ -121,7 +121,7 @@
+ #endif
+ #define DST_RET(a) {ret = a; goto err;}
+ 
+-#if OPENSSL_VERSION_NUMBER < 0x10100000L || defined(LIBRESSL_VERSION_NUMBER)
++#if OPENSSL_VERSION_NUMBER < 0x10100000L || ( defined(LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20700000L )
+ /* From OpenSSL 1.1.0 */
+ static int
+ RSA_set0_key(RSA *r, BIGNUM *n, BIGNUM *e, BIGNUM *d) {

Property changes on: files/patch-lib_dns_opensslrsa__link.c
___________________________________________________________________
Added: fbsd:nokeywords
## -0,0 +1 ##
+yes
\ No newline at end of property
Added: svn:eol-style
## -0,0 +1 ##
+native
\ No newline at end of property
Added: svn:mime-type
## -0,0 +1 ##
+text/plain
\ No newline at end of property

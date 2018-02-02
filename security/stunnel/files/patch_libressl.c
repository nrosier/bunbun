Index: files/patch-src_common.h
===================================================================
--- files/patch-src_common.h	(revision 0)
+++ files/patch-src_common.h	(working copy)
@@ -0,0 +1,20 @@
+--- src/common.h.orig	2017-01-02 14:27:26 UTC
++++ src/common.h
+@@ -448,7 +448,7 @@ extern char *sys_errlist[];
+ #define OPENSSL_NO_TLS1_2
+ #endif /* OpenSSL older than 1.0.1 || defined(OPENSSL_NO_TLS1) */
+ 
+-#if OPENSSL_VERSION_NUMBER>=0x10100000L
++#if OPENSSL_VERSION_NUMBER>=0x10100000L && !defined(LIBRESSL_VERSION_NUMBER)
+ #ifndef OPENSSL_NO_SSL2
+ #define OPENSSL_NO_SSL2
+ #endif /* !defined(OPENSSL_NO_SSL2) */
+@@ -474,7 +474,7 @@ extern char *sys_errlist[];
+ #include <openssl/des.h>
+ #ifndef OPENSSL_NO_DH
+ #include <openssl/dh.h>
+-#if OPENSSL_VERSION_NUMBER<0x10100000L
++#if OPENSSL_VERSION_NUMBER<0x10100000L || defined(LIBRESSL_VERSION_NUMBER)
+ int DH_set0_pqg(DH *dh, BIGNUM *p, BIGNUM *q, BIGNUM *g);
+ #endif /* OpenSSL older than 1.1.0 */
+ #endif /* !defined(OPENSSL_NO_DH) */

Property changes on: files/patch-src_common.h
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
Index: files/patch-src_ctx.c
===================================================================
--- files/patch-src_ctx.c	(revision 0)
+++ files/patch-src_ctx.c	(working copy)
@@ -0,0 +1,20 @@
+--- src/ctx.c.orig	2017-08-17 09:18:53 UTC
++++ src/ctx.c
+@@ -295,7 +295,7 @@ NOEXPORT int matches_wildcard(char *serv
+ 
+ #ifndef OPENSSL_NO_DH
+ 
+-#if OPENSSL_VERSION_NUMBER<0x10100000L
++#if OPENSSL_VERSION_NUMBER<0x10100000L || defined(LIBRESSL_VERSION_NUMBER)
+ NOEXPORT STACK_OF(SSL_CIPHER) *SSL_CTX_get_ciphers(const SSL_CTX *ctx) {
+     return ctx->cipher_list;
+ }
+@@ -398,7 +398,7 @@ NOEXPORT int ecdh_init(SERVICE_OPTIONS *
+ /**************************************** initialize OpenSSL CONF */
+ 
+ NOEXPORT int conf_init(SERVICE_OPTIONS *section) {
+-#if OPENSSL_VERSION_NUMBER>=0x10002000L
++#if OPENSSL_VERSION_NUMBER>=0x10002000L && !defined(LIBRESSL_VERSION_NUMBER)
+     SSL_CONF_CTX *cctx;
+     NAME_LIST *curr;
+     char *cmd, *param;

Property changes on: files/patch-src_ctx.c
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
Index: files/patch-src_options.c
===================================================================
--- files/patch-src_options.c	(revision 0)
+++ files/patch-src_options.c	(working copy)
@@ -0,0 +1,11 @@
+--- src/options.c.orig	2017-11-15 07:06:12 UTC
++++ src/options.c
+@@ -3658,7 +3658,7 @@ NOEXPORT char *engine_init(void) {
+     }
+ #endif
+     /* engines can add new algorithms */
+-#if OPENSSL_VERSION_NUMBER>=0x10100000L
++#if OPENSSL_VERSION_NUMBER>=0x10100000L && !defined(LIBRESSL_VERSION_NUMBER)
+     OPENSSL_init_crypto(OPENSSL_INIT_ADD_ALL_CIPHERS|
+         OPENSSL_INIT_ADD_ALL_DIGESTS, NULL);
+ #else

Property changes on: files/patch-src_options.c
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
Index: files/patch-src_ssl.c
===================================================================
--- files/patch-src_ssl.c	(revision 0)
+++ files/patch-src_ssl.c	(working copy)
@@ -0,0 +1,20 @@
+--- src/ssl.c.orig	2017-10-07 14:23:08 UTC
++++ src/ssl.c
+@@ -51,7 +51,7 @@ int index_ssl_cli, index_ssl_ctx_opt;
+ int index_session_authenticated, index_session_connect_address;
+ 
+ int ssl_init(void) { /* init TLS before parsing configuration file */
+-#if OPENSSL_VERSION_NUMBER>=0x10100000L
++#if OPENSSL_VERSION_NUMBER>=0x10100000L && !defined(LIBRESSL_VERSION_NUMBER)
+     OPENSSL_init_ssl(OPENSSL_INIT_LOAD_SSL_STRINGS |
+         OPENSSL_INIT_LOAD_CRYPTO_STRINGS | OPENSSL_INIT_LOAD_CONFIG, NULL);
+ #else
+@@ -87,7 +87,7 @@ int ssl_init(void) { /* init TLS before 
+ }
+ 
+ #ifndef OPENSSL_NO_DH
+-#if OPENSSL_VERSION_NUMBER<0x10100000L
++#if OPENSSL_VERSION_NUMBER<0x10100000L || defined(LIBRESSL_VERSION_NUMBER)
+ /* this is needed for dhparam.c generated with OpenSSL >= 1.1.0
+  * to be linked against the older versions */
+ int DH_set0_pqg(DH *dh, BIGNUM *p, BIGNUM *q, BIGNUM *g) {

Property changes on: files/patch-src_ssl.c
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
Index: files/patch-src_verify.c
===================================================================
--- files/patch-src_verify.c	(revision 0)
+++ files/patch-src_verify.c	(working copy)
@@ -0,0 +1,11 @@
+--- src/verify.c.orig	2017-05-13 09:01:07 UTC
++++ src/verify.c
+@@ -353,7 +353,7 @@ NOEXPORT int cert_check_local(X509_STORE
+     cert=X509_STORE_CTX_get_current_cert(callback_ctx);
+     subject=X509_get_subject_name(cert);
+ 
+-#if OPENSSL_VERSION_NUMBER<0x10100006L
++#if OPENSSL_VERSION_NUMBER<0x10100006L || defined(LIBRESSL_VERSION_NUMBER)
+ #define X509_STORE_CTX_get1_certs X509_STORE_get1_certs
+ #endif
+     /* modern API allows retrieving multiple matching certificates */

Property changes on: files/patch-src_verify.c
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

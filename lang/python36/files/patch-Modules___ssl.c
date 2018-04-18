$OpenBSD: patch-Modules__ssl_c,v 1.2 2018/03/17 22:30:04 sthen Exp $

XXX maybe the second hunk can go away now we have auto-init, I'm not sure
exactly what python's lock protects

Index: Modules/_ssl.c
--- Modules/_ssl.c.orig
+++ Modules/_ssl.c
@@ -99,7 +99,8 @@ struct py_ssl_library_code {
 /* Include generated data (error codes) */
 #include "_ssl_data.h"
 
-#if (OPENSSL_VERSION_NUMBER >= 0x10100000L) && !defined(LIBRESSL_VERSION_NUMBER)
+#if (OPENSSL_VERSION_NUMBER >= 0x10100000L) && \
+    (!defined(LIBRESSL_VERSION_NUMBER) || LIBRESSL_VERSION_NUMBER >= 0x2070000fL)
 #  define OPENSSL_VERSION_1_1 1
 #endif
 
@@ -133,6 +134,9 @@ struct py_ssl_library_code {
 /* OpenSSL 1.1.0+ */
 #ifndef OPENSSL_NO_SSL2
 #define OPENSSL_NO_SSL2
+#endif
+#if defined(LIBRESSL_VERSION_NUMBER) && defined(WITH_THREAD)
+#define HAVE_OPENSSL_CRYPTO_LOCK
 #endif
 #else /* OpenSSL < 1.1.0 */
 #if defined(WITH_THREAD)

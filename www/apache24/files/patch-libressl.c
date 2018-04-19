Index: modules/md/md_crypt.c
===================================================================
--- modules/md/md_crypt.c	(revision 1828089)
+++ modules/md/md_crypt.c	(working copy)
@@ -471,7 +471,7 @@
     }
 }
 
-#if OPENSSL_VERSION_NUMBER < 0x10100000L || defined(LIBRESSL_VERSION_NUMBER)
+#if OPENSSL_VERSION_NUMBER < 0x10100000L || (defined(LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x2070000f)
 
 #ifndef NID_tlsfeature
 #define NID_tlsfeature          1020
Index: modules/ssl/mod_ssl.c
===================================================================
--- modules/ssl/mod_ssl.c	(revision 1828089)
+++ modules/ssl/mod_ssl.c	(working copy)
@@ -407,7 +407,7 @@
     /* We must register the library in full, to ensure our configuration
      * code can successfully test the SSL environment.
      */
-#if MODSSL_USE_OPENSSL_PRE_1_1_API
+#if MODSSL_USE_OPENSSL_PRE_1_1_API || defined(LIBRESSL_VERSION_NUMBER)
     (void)CRYPTO_malloc_init();
 #else
     OPENSSL_malloc_init();
Index: modules/ssl/ssl_engine_init.c
===================================================================
--- modules/ssl/ssl_engine_init.c	(revision 1828089)
+++ modules/ssl/ssl_engine_init.c	(working copy)
@@ -542,7 +542,8 @@
 }
 #endif
 
-#if OPENSSL_VERSION_NUMBER < 0x10100000L
+#if OPENSSL_VERSION_NUMBER < 0x10100000L || \
+	(defined(LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20800000L)
 /*
  * Enable/disable SSLProtocol. If the mod_ssl enables protocol
  * which is disabled by default by OpenSSL, show a warning.
@@ -660,7 +661,8 @@
 
     SSL_CTX_set_options(ctx, SSL_OP_ALL);
 
-#if OPENSSL_VERSION_NUMBER < 0x10100000L
+#if OPENSSL_VERSION_NUMBER < 0x10100000L  || \
+	(defined(LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20800000L)
     /* always disable SSLv2, as per RFC 6176 */
     SSL_CTX_set_options(ctx, SSL_OP_NO_SSLv2);
 
Index: modules/ssl/ssl_private.h
===================================================================
--- modules/ssl/ssl_private.h	(revision 1828089)
+++ modules/ssl/ssl_private.h	(working copy)
@@ -132,13 +132,14 @@
         SSL_CTX_ctrl(ctx, SSL_CTRL_SET_MIN_PROTO_VERSION, version, NULL)
 #define SSL_CTX_set_max_proto_version(ctx, version) \
         SSL_CTX_ctrl(ctx, SSL_CTRL_SET_MAX_PROTO_VERSION, version, NULL)
-#endif
-/* LibreSSL declares OPENSSL_VERSION_NUMBER == 2.0 but does not include most
- * changes from OpenSSL >= 1.1 (new functions, macros, deprecations, ...), so
- * we have to work around this...
+#elif LIBRESSL_VERSION_NUMBER < 0x2070000f
+/* LibreSSL before 2.7 declares OPENSSL_VERSION_NUMBER == 2.0 but does not
+ * include most changes from OpenSSL >= 1.1 (new functions, macros, 
+ * deprecations, ...), so we have to work around this...
  */
 #define MODSSL_USE_OPENSSL_PRE_1_1_API (1)
-#else
+#endif /* LIBRESSL_VERSION_NUMBER < 0x2060000f */
+#else /* defined(LIBRESSL_VERSION_NUMBER) */
 #define MODSSL_USE_OPENSSL_PRE_1_1_API (OPENSSL_VERSION_NUMBER < 0x10100000L)
 #endif
 
@@ -238,7 +239,8 @@
 void free_bio_methods(void);
 #endif
 
-#if OPENSSL_VERSION_NUMBER < 0x10002000L || defined(LIBRESSL_VERSION_NUMBER)
+#if OPENSSL_VERSION_NUMBER < 0x10002000L || \
+	(defined(LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x2070000f)
 #define X509_STORE_CTX_get0_store(x) (x->ctx)
 #endif
 

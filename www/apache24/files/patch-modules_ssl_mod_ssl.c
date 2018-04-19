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

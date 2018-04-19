--- ext/openssl/openssl.c.orig	2018-02-27 16:33:08 UTC
+++ ext/openssl/openssl.c
@@ -564,7 +564,7 @@ ZEND_GET_MODULE(openssl)
 #endif
 
 /* {{{ OpenSSL compatibility functions and macros */
-#if OPENSSL_VERSION_NUMBER < 0x10100000L || defined (LIBRESSL_VERSION_NUMBER)
+#if OPENSSL_VERSION_NUMBER < 0x10100000L || (defined (LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20700000L)
 #define EVP_PKEY_get0_RSA(_pkey) _pkey->pkey.rsa
 #define EVP_PKEY_get0_DH(_pkey) _pkey->pkey.dh
 #define EVP_PKEY_get0_DSA(_pkey) _pkey->pkey.dsa
@@ -681,7 +681,7 @@ static const unsigned char *ASN1_STRING_
 	return M_ASN1_STRING_data(asn1);
 }
 
-#if OPENSSL_VERSION_NUMBER < 0x10002000L || defined (LIBRESSL_VERSION_NUMBER)
+#if OPENSSL_VERSION_NUMBER < 0x10002000L || (defined (LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20700000L)
 
 static int X509_get_signature_nid(const X509 *x)
 {
@@ -1416,7 +1416,7 @@ PHP_MINIT_FUNCTION(openssl)
 	le_x509 = zend_register_list_destructors_ex(php_openssl_x509_free, NULL, "OpenSSL X.509", module_number);
 	le_csr = zend_register_list_destructors_ex(php_openssl_csr_free, NULL, "OpenSSL X.509 CSR", module_number);
 
-#if OPENSSL_VERSION_NUMBER < 0x10100000L || defined (LIBRESSL_VERSION_NUMBER)
+#if OPENSSL_VERSION_NUMBER < 0x10100000L || (defined (LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20700000L)
 	OPENSSL_config(NULL);
 	SSL_library_init();
 	OpenSSL_add_all_ciphers();
@@ -1597,7 +1597,7 @@ PHP_MINFO_FUNCTION(openssl)
  */
 PHP_MSHUTDOWN_FUNCTION(openssl)
 {
-#if OPENSSL_VERSION_NUMBER < 0x10100000L || defined (LIBRESSL_VERSION_NUMBER)
+#if OPENSSL_VERSION_NUMBER < 0x10100000L || (defined (LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20700000L)
 	EVP_cleanup();
 
 	/* prevent accessing locking callback from unloaded extension */
@@ -3642,7 +3642,7 @@ PHP_FUNCTION(openssl_csr_get_public_key)
 		RETURN_FALSE;
 	}
 
-#if OPENSSL_VERSION_NUMBER >= 0x10100000L && !defined(LIBRESSL_VERSION_NUMBER)
+#if OPENSSL_VERSION_NUMBER >= 0x10100000L && !(defined (LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20700000L)
 	/* Due to changes in OpenSSL 1.1 related to locking when decoding CSR,
 	 * the pub key is not changed after assigning. It means if we pass
 	 * a private key, it will be returned including the private part.
@@ -3653,7 +3653,7 @@ PHP_FUNCTION(openssl_csr_get_public_key)
 	/* Retrieve the public key from the CSR */
 	tpubkey = X509_REQ_get_pubkey(csr);
 
-#if OPENSSL_VERSION_NUMBER >= 0x10100000L && !defined(LIBRESSL_VERSION_NUMBER)
+#if OPENSSL_VERSION_NUMBER >= 0x10100000L && !(defined (LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x20700000L)
 	/* We need to free the CSR as it was duplicated */
 	X509_REQ_free(csr);
 #endif

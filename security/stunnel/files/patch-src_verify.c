--- src/verify.c.orig	2017-05-13 09:01:07 UTC
+++ src/verify.c
@@ -353,7 +353,7 @@ NOEXPORT int cert_check_local(X509_STORE
     cert=X509_STORE_CTX_get_current_cert(callback_ctx);
     subject=X509_get_subject_name(cert);
 
-#if OPENSSL_VERSION_NUMBER<0x10100006L
+#if OPENSSL_VERSION_NUMBER<0x10100006L || defined(LIBRESSL_VERSION_NUMBER)
 #define X509_STORE_CTX_get1_certs X509_STORE_get1_certs
 #endif
     /* modern API allows retrieving multiple matching certificates */

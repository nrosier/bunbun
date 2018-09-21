--- src/network/ssl/qsslsocket_openssl_symbols.cpp.orig	2018-09-09 18:05:58 UTC
+++ src/network/ssl/qsslsocket_openssl_symbols.cpp
@@ -1157,6 +1157,7 @@ bool q_resolveOpenSslSymbols()
     RESOLVEFUNC(SSL_CTX_use_PrivateKey_file)
     RESOLVEFUNC(SSL_CTX_get_cert_store);
 #if OPENSSL_VERSION_NUMBER >= 0x10002000L
+#if !defined(LIBRESSL_VERSION_NUMBER) 
     RESOLVEFUNC(SSL_CONF_CTX_new);
     RESOLVEFUNC(SSL_CONF_CTX_free);
     RESOLVEFUNC(SSL_CONF_CTX_set_ssl_ctx);
@@ -1164,6 +1165,7 @@ bool q_resolveOpenSslSymbols()
     RESOLVEFUNC(SSL_CONF_CTX_finish);
     RESOLVEFUNC(SSL_CONF_cmd);
 #endif
+#endif
     RESOLVEFUNC(SSL_accept)
     RESOLVEFUNC(SSL_clear)
     RESOLVEFUNC(SSL_connect)

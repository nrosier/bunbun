$OpenBSD: patch-src__cffi_src_openssl_x509_py,v 1.1 2018/02/18 13:44:41 sthen Exp $

Index: src/_cffi_src/openssl/x509.py
--- src/_cffi_src/openssl/x509.py.orig
+++ src/_cffi_src/openssl/x509.py
@@ -255,8 +255,7 @@ int X509_get_signature_nid(const X509 *);
 
 const X509_ALGOR *X509_get0_tbs_sigalg(const X509 *);
 
-/* in 1.1.0 becomes const ASN1_BIT_STRING, const X509_ALGOR */
-void X509_get0_signature(ASN1_BIT_STRING **, X509_ALGOR **, X509 *);
+void X509_get0_signature(const ASN1_BIT_STRING **, const X509_ALGOR **, const X509 *);
 
 long X509_get_version(X509 *);
 
@@ -339,7 +338,8 @@ void X509_REQ_get0_signature(const X509_REQ *, const A
 CUSTOMIZATIONS = """
 /* Added in 1.0.2 beta but we need it in all versions now due to the great
    opaquing. */
-#if CRYPTOGRAPHY_OPENSSL_LESS_THAN_102
+#if CRYPTOGRAPHY_OPENSSL_LESS_THAN_102 && \
+    (defined(LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x2070000fL)
 /* from x509/x_x509.c version 1.0.2 */
 void X509_get0_signature(ASN1_BIT_STRING **psig, X509_ALGOR **palg,
                          const X509 *x)
@@ -383,9 +383,11 @@ X509_REVOKED *Cryptography_X509_REVOKED_dup(X509_REVOK
    opaquing. */
 #if CRYPTOGRAPHY_OPENSSL_LESS_THAN_110
 
+#if (defined(LIBRESSL_VERSION_NUMBER) && LIBRESSL_VERSION_NUMBER < 0x2070000fL)
 int X509_up_ref(X509 *x) {
    return CRYPTO_add(&x->references, 1, CRYPTO_LOCK_X509);
 }
+#endif
 
 const X509_ALGOR *X509_get0_tbs_sigalg(const X509 *x)
 {

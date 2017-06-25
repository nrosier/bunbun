--- ext/opcache/zend_file_cache.c.old
+++ ext/opcache/zend_file_cache.c
@@ -24,9 +24,10 @@
 
 #include "php.h"
 
+#include "ZendAccelerator.h"
+
 #ifdef HAVE_OPCACHE_FILE_CACHE
 
-#include "ZendAccelerator.h"
 #include "zend_file_cache.h"
 #include "zend_shared_alloc.h"
 #include "zend_accelerator_util_funcs.h"

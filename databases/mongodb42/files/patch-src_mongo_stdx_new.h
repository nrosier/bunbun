# https://github.com/mongodb/mongo/commit/aac59681b3ae4d3806174227ba4c88aaf8e89ea2
--- src/mongo/stdx/new.h.orig	2019-11-18 18:41:44 UTC
+++ src/mongo/stdx/new.h
@@ -37,7 +37,9 @@
 namespace mongo {
 namespace stdx {
 
-#if __cplusplus < 201703L || !defined(__cpp_lib_hardware_interference_size)
+// libc++ 8.0 and later define __cpp_lib_hardware_interference_size but don't actually implement it
+#if __cplusplus < 201703L || \
+    !(defined(__cpp_lib_hardware_interference_size) && !defined(_LIBCPP_VERSION))
 
 #if defined(MONGO_CONFIG_MAX_EXTENDED_ALIGNMENT)
 static_assert(MONGO_CONFIG_MAX_EXTENDED_ALIGNMENT >= sizeof(uint64_t), "Bad extended alignment");

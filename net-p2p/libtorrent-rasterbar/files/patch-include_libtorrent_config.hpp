--- include/libtorrent/config.hpp.orig	2017-07-05 10:36:20.645116000 +0200
+++ include/libtorrent/config.hpp	2017-07-05 10:37:49.876606000 +0200
@@ -195,12 +195,6 @@
 #define TORRENT_USE_EXECINFO 1
 #endif
 
-#else // __APPLE__
-// FreeBSD has a reasonable iconv signature
-// unless we're on glibc
-#ifndef __GLIBC__
-# define TORRENT_ICONV_ARG(x) (x)
-#endif
 #endif // __APPLE__
 
 #define TORRENT_HAVE_MMAP 1

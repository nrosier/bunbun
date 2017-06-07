--- libmariadb/include/mysql.h.orig	2017-06-07 23:15:59.783907000 +0200
+++ libmariadb/include/mysql.h	2017-06-07 23:16:27.764178000 +0200
@@ -342,6 +342,7 @@
     struct st_mysql_options options;
     enum mysql_status status;
     my_bool	free_me;		/* If free in mysql_close */
+    my_bool	reconnect;
     my_bool	unused_1;
     char	        scramble_buff[20+ 1];
     /* madded after 3.23.58 */

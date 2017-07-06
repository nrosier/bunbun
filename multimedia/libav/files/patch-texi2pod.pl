--- doc/texi2pod.pl.old	2017-07-06 11:51:30.277684000 +0200
+++ doc/texi2pod.pl	2017-07-06 11:51:38.173810000 +0200
@@ -379,7 +379,7 @@
     # @* is also impossible in .pod; we discard it and any newline that
     # follows it.  Similarly, our macro @gol must be discarded.
 
-    s/\@anchor{(?:[^\}]*)\}//g;
+    s/\@anchor\{(?:[^\}]*)\}//g;
     s/\(?\@xref\{(?:[^\}]*)\}(?:[^.<]|(?:<[^<>]*>))*\.\)?//g;
     s/\s+\(\@pxref\{(?:[^\}]*)\}\)//g;
     s/;\s+\@pxref\{(?:[^\}]*)\}//g;

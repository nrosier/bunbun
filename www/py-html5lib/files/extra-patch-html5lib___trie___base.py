--- html5lib/_trie/_base.py.orig	2017-12-07 12:25:26 UTC
+++ html5lib/_trie/_base.py
@@ -1,6 +1,6 @@
 from __future__ import absolute_import, division, unicode_literals
 
-from collections import Mapping
+from collections.abc import Mapping
 
 
 class Trie(Mapping):

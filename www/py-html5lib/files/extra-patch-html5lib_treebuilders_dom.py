--- html5lib/treebuilders/dom.py.orig	2017-12-07 12:25:26 UTC
+++ html5lib/treebuilders/dom.py
@@ -1,7 +1,7 @@
 from __future__ import absolute_import, division, unicode_literals
 
 
-from collections import MutableMapping
+from collections.abc import MutableMapping
 from xml.dom import minidom, Node
 import weakref
 

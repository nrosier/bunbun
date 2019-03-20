--- mcs/class/System/System.IO/FileSystemWatcher.cs.orig	2015-09-24 11:36:12.000000000 +0200
+++ mcs/class/System/System.IO/FileSystemWatcher.cs	2016-01-25 23:18:50.755286000 +0100
@@ -457,6 +457,34 @@
 			OnError (args);
 		}
 
+		internal void DispatchEvent(FileSystemEventArgs evnt) {
+			if (waiting) {
+				lastData = new WaitForChangedResult ();
+			}
+
+			lastData.ChangeType = evnt.ChangeType;
+
+			switch (evnt.ChangeType) {
+			case WatcherChangeTypes.Changed:
+				lastData.Name = evnt.Name;
+				OnChanged(evnt);
+				break;
+			case WatcherChangeTypes.Deleted:
+				lastData.Name = evnt.Name;
+				OnDeleted(evnt);
+				break;
+			case WatcherChangeTypes.Created:
+				lastData.Name = evnt.Name;
+				OnCreated(evnt);
+				break;
+			case WatcherChangeTypes.Renamed:
+				lastData.Name = evnt.Name;
+				lastData.OldName = ((RenamedEventArgs)evnt).OldName;
+				OnRenamed((RenamedEventArgs)evnt);
+				break;
+			}
+		}
+
 		internal void DispatchEvents (FileAction act, string filename, ref RenamedEventArgs renamed)
 		{
 			if (waiting) {

--- scan/executil.go.orig	2019-03-20 22:22:55.864055000 +0100
+++ scan/executil.go	2019-03-20 22:23:29.957560000 +0100
@@ -385,6 +385,8 @@
 			if c.User != "root" {
 				cmd = fmt.Sprintf("sudo -S %s", cmd)
 			}
+		case "jail":
+			cmd = fmt.Sprintf(`jexec %s /bin/sh -c "%s"`, c.Container.Name, cmd)
 		}
 	}
 	//  cmd = fmt.Sprintf("set -x; %s", cmd)

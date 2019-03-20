--- base.go.orig	2019-03-20 22:16:40.899863000 +0100
+++ base.go	2019-03-20 22:19:40.833780000 +0100
@@ -117,6 +117,12 @@
 			return containers, err
 		}
 		return l.parseLxcPs(stdout)
+	case "jail":
+		stdout, err := l.jailPs("jid name")
+		if err != nil {
+			return containers, err
+		}
+		return l.parseJailPs(stdout)
 	default:
 		return containers, fmt.Errorf(
 			"Not supported yet: %s", l.ServerInfo.ContainerType)
@@ -143,6 +149,12 @@
 			return containers, err
 		}
 		return l.parseLxcPs(stdout)
+	case "jail":
+		stdout, err := l.jailPs("jid name")
+		if err != nil {
+			return containers, err
+		}
+		return l.parseJailPs(stdout) 
 	default:
 		return containers, fmt.Errorf(
 			"Not supported yet: %s", l.ServerInfo.ContainerType)
@@ -169,6 +181,9 @@
 			return containers, err
 		}
 		return l.parseLxcPs(stdout)
+	case "jail":
+		/* FIXME */
+		return
 	default:
 		return containers, fmt.Errorf(
 			"Not supported yet: %s", l.ServerInfo.ContainerType)
@@ -202,6 +217,15 @@
 	return r.Stdout, nil
 }
 
+func (l *base) jailPs(option string) (string, error) {
+	cmd := fmt.Sprintf("jls %s", option)
+	r := l.exec(cmd, noSudo)
+	if !r.isSuccess() {
+		return "", fmt.Errorf("failed to SSH: %s", r)
+	}
+	return r.Stdout, nil
+}
+
 func (l *base) parseDockerPs(stdout string) (containers []config.Container, err error) {
 	lines := strings.Split(stdout, "\n")
 	for _, line := range lines {
@@ -291,6 +315,25 @@
 		} else {
 			ipv6Addrs = append(ipv6Addrs, ip.String())
 		}
+	}
+	return
+}
+
+func (l *base) parseJailPs(stdout string) (containers []config.Container, err error) {
+	lines := strings.Split(stdout, "\n")
+	for _, line := range lines {
+
+		fields := strings.Fields(line)
+		if len(fields) == 0 {
+			break
+		}
+		if len(fields) != 2 {
+			return containers, fmt.Errorf("Unknown format: %s", line)
+		}
+		containers = append(containers, config.Container{
+			ContainerID: fields[0],
+			Name:        fields[1],
+		})
 	}
 	return
 }

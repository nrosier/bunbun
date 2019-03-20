--- base_test.go.orig	2019-03-20 22:21:04.077223000 +0100
+++ base_test.go	2019-03-20 22:22:23.043912000 +0100
@@ -125,6 +125,39 @@
 	}
 }
 
+func TestParseJailPs(t *testing.T) {
+
+        var test = struct {
+                in       string
+                expected []config.Container
+        }{
+                `1 test1
+2 test2`,
+                []config.Container{
+                        {
+                                ContainerID: "1",
+                                Name:        "test1",
+                        },
+                        {
+                                ContainerID: "2",
+                                Name:        "test2",
+                        },
+                },
+        }
+
+        r := newRedhat(config.ServerInfo{})
+        actual, err := r.parseJailPs(test.in)
+        if err != nil {
+                t.Errorf("Error occurred. in: %s, err: %s", test.in, err)
+                return
+        }
+        for i, e := range test.expected {
+                if !reflect.DeepEqual(e, actual[i]) {
+                        t.Errorf("expected %v, actual %v", e, actual[i])
+                }
+        }
+}
+
 func TestIsAwsInstanceID(t *testing.T) {
 	var tests = []struct {
 		in       string

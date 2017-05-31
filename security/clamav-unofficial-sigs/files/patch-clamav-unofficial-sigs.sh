--- clamav-unofficial-sigs.sh	2017-03-18 12:57:29.000000000 +0100
+++ clamav-unofficial-sigs.sh.new	2017-05-31 20:23:40.390484000 +0200
@@ -867,6 +867,8 @@
 # Remove the clamav-unofficial-sigs script
 function remove_script () {
   echo ""
+  pkg_mgr="FreeBSD ports"
+  pkg_rm="pkg delete"
   if [ -n "$pkg_mgr" ] || [ -n "$pkg_rm" ] ; then
     echo "This script (clamav-unofficial-sigs) was installed on the system via '$pkg_mgr'"
     echo "use '$pkg_rm' to remove the script and all of its associated files and databases from the system."
@@ -1290,7 +1292,7 @@
 minimum_yara_clamav_version="0.99"
 
 # Default config files
-config_dir="/etc/clamav-unofficial-sigs"
+config_dir="%%PREFIX%%/etc/clamav-unofficial-sigs"
 config_files=( "$config_dir/master.conf" "$config_dir/os.conf" "$config_dir/user.conf" )
 
 # Initialise
@@ -2131,20 +2133,11 @@
         xshok_pretty_echo_and_log "Sanesecurity Database & GPG Signature File Updates" "="
         xshok_pretty_echo_and_log "Checking for Sanesecurity updates..."
 
-        sanesecurity_mirror_ips="$(dig +ignore +short "$sanesecurity_url")"
-        # Add fallback to host if dig returns no records
-        if [ ${#sanesecurity_mirror_ips} -lt 1 ] ; then
-          sanesecurity_mirror_ips="$(host -t A "$sanesecurity_url" | sed -n '/has address/{s/.*address \([^ ]*\).*/\1/;p;}')"
-        fi
+        sanesecurity_mirror_ips=$(host "$sanesecurity_url" | sed 's/.*[[:space:]]//')
 
         if [ ${#sanesecurity_mirror_ips} -ge 1 ] ; then
           for sanesecurity_mirror_ip in $sanesecurity_mirror_ips ; do
-            sanesecurity_mirror_name=""
-            sanesecurity_mirror_name="$(dig +short -x "$sanesecurity_mirror_ip" | command sed 's/\.$//')"
-            # Add fallback to host if dig returns no records
-            if [ -z "$sanesecurity_mirror_name" ] ; then
-              sanesecurity_mirror_name="$(host "$sanesecurity_mirror_ip" | sed -n '/name pointer/{s/.*pointer \([^ ]*\).*\.$/\1/;p;}')"
-            fi
+            sanesecurity_mirror_name=$(host "$sanesecurity_mirror_ip" | sed 's/.*[[:space:]]//' | sed 's/\.$//')
             sanesecurity_mirror_site_info="$sanesecurity_mirror_name $sanesecurity_mirror_ip"
             xshok_pretty_echo_and_log "Sanesecurity mirror site used: $sanesecurity_mirror_site_info"
             # shellcheck disable=SC2086

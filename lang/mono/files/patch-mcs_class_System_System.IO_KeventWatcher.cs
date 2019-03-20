--- mcs/class/System/System.IO/KeventWatcher.cs.orig	2016-01-26 00:30:58.287584000 +0100
+++ mcs/class/System/System.IO/KeventWatcher.cs	2016-01-26 08:53:06.181589000 +0100
@@ -1,13 +1,15 @@
 // 
-// System.IO.KeventWatcher.cs: interface with osx kevent
+// System.IO.KeventWatcher.cs: interface with FreeBSD kevent
 //
 // Authors:
 //	Geoff Norton (gnorton@customerdna.com)
 //	Cody Russell (cody@xamarin.com)
 //	Alexis Christoforides (lexas@xamarin.com)
+//  Ivan Radovanovic (radovanovic@gmail.com)
 //
 // (c) 2004 Geoff Norton
 // Copyright 2014 Xamarin Inc
+// Copyright 2016 Ivan Radovanovic
 //
 // Permission is hereby granted, free of charge, to any person obtaining
 // a copy of this software and associated documentation files (the
@@ -29,646 +31,964 @@
 // WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 //
 
+//#define DEBUG_KEVENT_WATCHER
 using System;
-using System.Collections;
 using System.Collections.Generic;
-using System.ComponentModel;
-using System.Runtime.CompilerServices;
-using System.Runtime.InteropServices;
-using System.Text;
+using System.Collections;
+using System.IO;
 using System.Threading;
-using System.Reflection;
+using System.Text;
+using System.Runtime.InteropServices;
 
 namespace System.IO {
 
-        [Flags]
-        enum EventFlags : ushort {
-                Add         = 0x0001,
-                Delete      = 0x0002,
-                Enable      = 0x0004,
-                Disable     = 0x0008,
-                OneShot     = 0x0010,
-                Clear       = 0x0020,
-                Receipt     = 0x0040,
-                Dispatch    = 0x0080,
-
-                Flag0       = 0x1000,
-                Flag1       = 0x2000,
-                SystemFlags = unchecked (0xf000),
-                        
-                // Return values.
-                EOF         = 0x8000,
-                Error       = 0x4000,
-        }
-        
-        enum EventFilter : short {
-                Read = -1,
-                Write = -2,
-                Aio = -3,
-                Vnode = -4,
-                Proc = -5,
-                Signal = -6,
-                Timer = -7,
-                MachPort = -8,
-                FS = -9,
-                User = -10,
-                VM = -11
-        }
+	[Flags]
+	enum EventFlags : ushort {
+		None        = 0,		// XXX
+		Add         = 0x0001,
+		Delete      = 0x0002,
+		Enable      = 0x0004,
+		Disable     = 0x0008,
+		OneShot     = 0x0010,
+		Clear       = 0x0020,
+		Receipt     = 0x0040,
+		Dispatch    = 0x0080,
+
+		Drop        = 0x1000,
+		Flag1       = 0x2000,
+		SystemFlags = 0xf000,
+
+		// Return values.
+		EOF         = 0x8000,
+		Error       = 0x4000,
+	}
+
+	enum EventFilter : short {
+		Read     = -1,
+		Write    = -2,
+		Aio      = -3,
+		Vnode    = -4,
+		Proc     = -5,
+		Signal   = -6,
+		Timer    = -7,
+		NetDev   = -8,
+		FS       = -9,
+		Lio      = -10,
+		User     = -11
+	}
 
 	[Flags]
 	enum FilterFlags : uint {
-                ReadPoll          = EventFlags.Flag0,
-                ReadOutOfBand     = EventFlags.Flag1,
-                ReadLowWaterMark  = 0x00000001,
-
-                WriteLowWaterMark = ReadLowWaterMark,
-
-                NoteTrigger       = 0x01000000,
-                NoteFFNop         = 0x00000000,
-                NoteFFAnd         = 0x40000000,
-                NoteFFOr          = 0x80000000,
-                NoteFFCopy        = 0xc0000000,
-                NoteFFCtrlMask    = 0xc0000000,
-                NoteFFlagsMask    = 0x00ffffff,
-                                  
-                VNodeDelete       = 0x00000001,
-                VNodeWrite        = 0x00000002,
-                VNodeExtend       = 0x00000004,
-                VNodeAttrib       = 0x00000008,
-                VNodeLink         = 0x00000010,
-                VNodeRename       = 0x00000020,
-                VNodeRevoke       = 0x00000040,
-                VNodeNone         = 0x00000080,
-                                  
-                ProcExit          = 0x80000000,
-                ProcFork          = 0x40000000,
-                ProcExec          = 0x20000000,
-                ProcReap          = 0x10000000,
-                ProcSignal        = 0x08000000,
-                ProcExitStatus    = 0x04000000,
-                ProcResourceEnd   = 0x02000000,
-
-                // iOS only
-                ProcAppactive     = 0x00800000,
-                ProcAppBackground = 0x00400000,
-                ProcAppNonUI      = 0x00200000,
-                ProcAppInactive   = 0x00100000,
-                ProcAppAllStates  = 0x00f00000,
-
-                // Masks
-                ProcPDataMask     = 0x000fffff,
-                ProcControlMask   = 0xfff00000,
-
-                VMPressure        = 0x80000000,
-                VMPressureTerminate = 0x40000000,
-                VMPressureSuddenTerminate = 0x20000000,
-                VMError           = 0x10000000,
-                TimerSeconds      =    0x00000001,
-                TimerMicroSeconds =   0x00000002,
-                TimerNanoSeconds  =   0x00000004,
-                TimerAbsolute     =   0x00000008,
-        }
+		None              = 0,
+
+		VNodeDelete       = 0x00000001,
+		VNodeWrite        = 0x00000002,
+		VNodeExtend       = 0x00000004,
+		VNodeAttrib       = 0x00000008,
+		VNodeLink         = 0x00000010,
+		VNodeRename       = 0x00000020,
+		VNodeRevoke       = 0x00000040,
+
+		NoteTrigger       = 0x01000000, 
+		NoteFFAnd         = 0x40000000,
+		NoteFFOr          = 0x80000000,
+		NoteFFCopy        = 0xc0000000,
+		NoteFFCtrlMask    = 0xc0000000,
+		NoteFFlagsMask    = 0x00ffffff,
+	}
 
 	[StructLayout(LayoutKind.Sequential)]
-	struct kevent : IDisposable {
-		public UIntPtr ident;
+	struct kevent {
+		public IntPtr ident;
 		public EventFilter filter;
 		public EventFlags flags;
 		public FilterFlags fflags;
 		public IntPtr data;
 		public IntPtr udata;
 
-		public void Dispose ()
-		{
-			if (udata != IntPtr.Zero)
-				Marshal.FreeHGlobal (udata);
+		public override string ToString() {
+			return string.Format("[ ident = {0}, filter = {1} ({6}), flags = {2} ({7:x8}), fflags = {3} ({8:x8}), data = {4}, udata = {5} ]",
+				ident, filter, flags, fflags, data, udata, (short)filter, (int)flags, (uint)fflags);
 		}
-
-
 	}
 
 	[StructLayout(LayoutKind.Sequential)]
 	struct timespec {
 		public IntPtr tv_sec;
-		public IntPtr tv_usec;
+		public IntPtr tv_nsec;
 	}
 
-	class PathData
-	{
-		public string Path;
-		public bool IsDirectory;
-		public int Fd;
-	}
+	[StructLayout(LayoutKind.Sequential)]
+	struct dirent {
+		const int DT_DIR = 4;
+		public uint d_fileno;       /* file number of entry */
+		ushort d_reclen;            /* length of this record */
+		byte d_type;                /* file type, see below */
+		byte d_namlen;              /* length of string in d_name */
+		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
+		byte[] d_name;              /* name must be no longer than 256 */
 
-	class KqueueMonitor : IDisposable
-	{
-		static bool initialized;
-		
-		public int Connection
-		{
-			get { return conn; }
+		public string Name {
+			get {
+				return System.Text.ASCIIEncoding.ASCII.GetString(d_name);
+			}
 		}
 
-		public KqueueMonitor (FileSystemWatcher fsw)
-		{
-			this.fsw = fsw;
-			this.conn = -1;
-			if (!initialized){
-				int t;
-				initialized = true;
-				var maxenv = Environment.GetEnvironmentVariable ("MONO_DARWIN_WATCHER_MAXFDS");
-				if (maxenv != null && Int32.TryParse (maxenv, out t))
-					maxFds = t;
-			}
+		static public dirent FromPtr(IntPtr ptr) {
+			var retval = (dirent)Marshal.PtrToStructure(ptr, typeof(dirent));
+			var name_bytes = retval.d_name;
+			retval.d_name = new byte[retval.d_namlen];
+			Array.Copy(name_bytes, retval.d_name, retval.d_namlen);
+			return retval;
 		}
 
-		public void Dispose ()
-		{
-			CleanUp ();
+		public bool IsDirectory {
+			get {
+				return (d_type & DT_DIR) != 0;
+			}
 		}
+	};
 
-		public void Start ()
-		{
-			lock (stateLock) {
-				if (started)
-					return;
+	class PathData {
+		// top directory will have full path here instead of just filename!
+		public string Filename;
+		public bool IsDirectory;
+		public int Fd;
+		public uint Inode;
+		public Dictionary<uint, PathData> Files;
+		public PathData ParentDir;
+
+		public string Path {
+			get {
+				if (ParentDir != null)
+					return System.IO.Path.Combine(ParentDir.Path, Filename);
+				else
+					return Filename;
+			}
+		}
 
-				conn = kqueue ();
+		public string RelPath {
+			get {
+				if (ParentDir == null)
+					return ".";
+				else if (ParentDir.ParentDir == null)
+					return Filename;
+				else
+					return System.IO.Path.Combine(ParentDir.RelPath, Filename);
+			}
+		}
 
-				if (conn == -1)
-					throw new IOException (String.Format (
-						"kqueue() error at init, error code = '{0}'", Marshal.GetLastWin32Error ()));
-					
-				thread = new Thread (() => DoMonitor ());
-				thread.IsBackground = true;
-				thread.Start ();
+		public PathData(string filename, bool is_directory, int fd, uint inode, PathData parent_dir) {
+			Filename = filename;
+			IsDirectory = is_directory;
+			Fd = fd;
+			Inode = inode;
+			ParentDir = parent_dir;
+			if (IsDirectory)
+				Files = new Dictionary<uint, PathData>();
+		}
+	}
 
-				startedEvent.WaitOne ();
+	/* 
+	 * BASIC IDEA:
+	 *  * obtain kqueue descriptor to listen for events
+	 *  * collect descriptors of all files we care about in list
+	 *  * we do blocking wait until something happens on one of file descriptors
+	 *    or on EVFILT_USER descriptor
+	 *  * we signal termination using EVFILT_USER mechanism
+	 * (in theory quite simple)
+	 */
+	class KqueueMonitor : IDisposable
+	{
+		const int O_RDONLY = 0;
+		const int F_OK = 0;
+		const int EINTR = 4;
+
+		const int _StopperEvent = 1;
+		// data exclusively used by main thread
+		Thread _KeventThread;
+		// data used by both main and watcher thread
+		AutoResetEvent _StartedRunning = new AutoResetEvent(false);
+		FileSystemWatcher _Watcher;
+		volatile int _kqueue;
+		volatile Exception _ProblemStarting;
+		volatile bool _Started;
+
+		public KqueueMonitor(FileSystemWatcher fsw) {
+			_Started = false;
+			_ProblemStarting = null;
+			_KeventThread = null;
 
-				if (exc != null) {
-					thread.Join ();
-					CleanUp ();
-					throw exc;
-				}
- 
-				started = true;
-			}
+			_Watcher = fsw;
 		}
 
-		public void Stop ()
-		{
-			lock (stateLock) {
-				if (!started)
-					return;
-					
-				requestStop = true;
+		public void Start() {
+			if (_Started)
+				return;
 
-				if (inDispatch)
-					return;
-				// This will break the wait in Monitor ()
-				lock (connLock) {
-					if (conn != -1)
-						close (conn);
-					conn = -1;
-				}
+			_kqueue = kqueue();
+			if (_kqueue == -1)
+				throw new IOException(String.Format("KeventWatcher: kqueue() error at init, error code = '{0}'", Marshal.GetLastWin32Error()));
+			
+			_KeventThread = new Thread(Run);
+			_KeventThread.IsBackground = true;
+			_KeventThread.Start();
 
-				if (!thread.Join (2000))
-					thread.Abort ();
+			_StartedRunning.WaitOne();
 
-				requestStop = false;
-				started = false;
+			if (_ProblemStarting != null)
+				throw _ProblemStarting;
+		}
 
-				if (exc != null)
-					throw exc;
+		public void Stop() {
+			if (!_Started) {
+				close(_kqueue);
+				_kqueue = -1;
+				return;
 			}
-		}
 
-		void CleanUp ()
-		{
-			lock (connLock) {
-				if (conn != -1)
-					close (conn);
-				conn = -1;
+			_Started = false;	// to be able to break dir. scanning
+
+			var stopper = new kevent();
+			EV_SET(ref stopper, _StopperEvent, EventFilter.User, EventFlags.None, FilterFlags.NoteTrigger, IntPtr.Zero, IntPtr.Zero);
+			if (kevent(_kqueue, new kevent[]{stopper}, 1, null, 0, IntPtr.Zero) < 0) {
+				// there was error signaling - quite impossible
+				throw new IOException(string.Format("KeventWatcher: Error stopping monitor: {0}", Marshal.GetLastWin32Error()));
 			}
 
-			foreach (int fd in fdsDict.Keys)
-				close (fd); 
+			_KeventThread.Join();
 
-			fdsDict.Clear ();
-			pathsDict.Clear ();
+			close(_kqueue);
+			_kqueue = -1;
 		}
 
-		void DoMonitor ()
-		{			
-			try {
-				Setup ();
-			} catch (Exception e) {
-				exc = e;
-			} finally {
-				startedEvent.Set ();
-			}
+		public void Dispose() {
+			if (_kqueue == -1)
+				return;
+			else
+				Stop();
+		}
 
-			if (exc != null) {
-				fsw.DispatchErrorEvents (new ErrorEventArgs (exc));
+		void Run() {
+			// first set up our stopper event
+			var stopper = new kevent();
+			EV_SET(ref stopper, _StopperEvent, EventFilter.User, EventFlags.Add, FilterFlags.None, IntPtr.Zero, IntPtr.Zero);
+			if (kevent(_kqueue, new kevent[]{stopper}, 1, null, 0, IntPtr.Zero) < 0) {
+				_ProblemStarting = new IOException(string.Format("KeventWatcher: Error setting up stop event: {0}", Marshal.GetLastWin32Error()));
+				_StartedRunning.Set();
 				return;
 			}
 
-			try {
-				Monitor ();
-			} catch (Exception e) {
-				exc = e;
-			} finally {
-				CleanUp ();
-				if (!requestStop) { // failure
-					started = false;
-					inDispatch = false;
-					fsw.EnableRaisingEvents = false;
-				}
-				if (exc != null)
-					fsw.DispatchErrorEvents (new ErrorEventArgs (exc));
-				requestStop = false;
-			}
-		}
+			// all data we need is allocated on stack here, so we don't worry about sync
+			Dictionary<int, PathData> fds = new Dictionary<int, PathData>();
+			Dictionary<uint, PathData> inodes = new Dictionary<uint, PathData>();
+			bool recursive = _Watcher.IncludeSubdirectories;
+			string base_dir = _Watcher.FullPath;
+			var events_buf = new kevent[32];
+			int received = 0;
+			timespec zero_time;
+			bool watched_tree_destroyed = false;
+			var pattern = _Watcher.Pattern;
+			bool need_file_fd = 
+				(_Watcher.NotifyFilter & NotifyFilters.LastAccess) != 0 ||
+				(_Watcher.NotifyFilter & NotifyFilters.Size) != 0 ||
+				(_Watcher.NotifyFilter & NotifyFilters.Security) != 0 ||
+				(_Watcher.NotifyFilter & NotifyFilters.Attributes) != 0 ||
+				(_Watcher.NotifyFilter & NotifyFilters.LastWrite) != 0;
+			bool report_dirname_change = (_Watcher.NotifyFilter & NotifyFilters.DirectoryName) != 0;
 
-		void Setup ()
-		{	
-			var initialFds = new List<int> ();
-
-			// fsw.FullPath may end in '/', see https://bugzilla.xamarin.com/show_bug.cgi?id=5747
-			if (fsw.FullPath != "/" && fsw.FullPath.EndsWith ("/", StringComparison.Ordinal))
-				fullPathNoLastSlash = fsw.FullPath.Substring (0, fsw.FullPath.Length - 1);
-			else
-				fullPathNoLastSlash = fsw.FullPath;
-				
-			// GetFilenameFromFd() returns the *realpath* which can be different than fsw.FullPath because symlinks.
-			// If so, introduce a fixup step.
-			int fd = open (fullPathNoLastSlash, O_EVTONLY, 0);
-			var resolvedFullPath = GetFilenameFromFd (fd);
-			close (fd);
+			if (_Watcher.FullPath != "/" && _Watcher.FullPath.EndsWith ("/", StringComparison.Ordinal))
+				base_dir = _Watcher.FullPath.Substring (0, _Watcher.FullPath.Length - 1);
 
-			if (resolvedFullPath != fullPathNoLastSlash)
-				fixupPath = resolvedFullPath;
-			else
-				fixupPath = null;
+			zero_time.tv_sec = IntPtr.Zero;
+			zero_time.tv_nsec = IntPtr.Zero;
 
-			Scan (fullPathNoLastSlash, false, ref initialFds);
+			_Started = true;
+			
+			_ProblemStarting = CollectAndMonitorDescriptors(base_dir, pattern, need_file_fd, recursive, this, fds, inodes);
 
-			var immediate_timeout = new timespec { tv_sec = (IntPtr)0, tv_usec = (IntPtr)0 };
-			var eventBuffer = new kevent[0]; // we don't want to take any events from the queue at this point
-			var changes = CreateChangeList (ref initialFds);
+			// make sure to tell others we are running
+			_StartedRunning.Set();
+			
+			if (_ProblemStarting != null)
+				return;
 
-			int numEvents = kevent (conn, changes, changes.Length, eventBuffer, eventBuffer.Length, ref immediate_timeout);
+			/*
+			 * System behavior
+			 * Action              Directory From   Directory To   File            Notes
+			 * -----------------------------------------------------------------------------------
+			 * File move           Write            Write          Rename          Can move out of watched space
+			 * File delete         Write            N/A            Delete          Delete means unlink is called on given inode, not that 
+			 *                                                                     file is really deleted - if more than one file links 
+			 *                                                                     to it all will receive delete when one is deleted
+			 * File create         N/A              Write          -
+			 * File changed        -                -              Write,Extend
+			 * Dir move            Write            Write          Rename          Can move out of watched space, no recursive events on 
+			 *                                                                     files inside
+			 * Dir delete          Write            N/A            Delete          Rec events on files inside
+			 * Dir create          N/A              Write          -
+			 * File move (outside) N/A              Write          -
+			 * Dir move (outside)  N/A              Write          -
+			 * 
+			 * Obviously all operations but actual change on file are reported on directories
+			 * so almost everything can be discovered by scanning dirs.
+			 */
+
+			// do actual monitoring
+			// we are checking that _Started flag is set as well (in case we have
+			// gazillion events pending in queue, stop event could come 
+			// unacceptably late)
+		watcher_loop:
+			while (_Started && (received = kevent(_kqueue, null, 0, events_buf, events_buf.Length, IntPtr.Zero)) > 0) {
+				List<kevent> events = new List<kevent>(received);
+
+				do {
+					for (int i = 0; i < received; i++) {
+						// ASAP check if we were asked to stop 
+						if (events_buf[i].filter == EventFilter.User && events_buf[i].ident.ToInt32() == _StopperEvent)
+							goto exit_watcher;
+						else
+							events.Add(events_buf[i]);
+					}
 
-			if (numEvents == -1) {
-				var errMsg = String.Format ("kevent() error at initial event registration, error code = '{0}'", Marshal.GetLastWin32Error ());
-				throw new IOException (errMsg);
-			}
-		}
+					// in case buffer was filled check if there were more events pending - but don't wait
+					if (received == events_buf.Length)
+						received = kevent(_kqueue, null, 0, events_buf, events_buf.Length, ref zero_time);
+					else
+						break;
+				}
+				while (received > 0);
 
-		kevent[] CreateChangeList (ref List<int> FdList)
-		{
-			if (FdList.Count == 0)
-				return emptyEventList;
+				/*
+				 * we keep two sets of FS objects to check
+				 *  - directory descriptors (we need to rescan them)
+				 *  - inode numbers for changed files
+				 */
+				Hashtable dir_fds = new Hashtable();
+				Hashtable changed_files_inodes = new Hashtable();
+				/*
+				 * Data to record changes - two sets of inodes and
+				 * list of events we detected so far, grouped by inode (in
+				 * order to prevent duplicating of events for single file/dir)
+				 */
+				Hashtable deleted_inodes = new Hashtable();
+				Hashtable created_inodes = new Hashtable();
+				Dictionary<uint, List<FileSystemEventArgs>> detected_events = new Dictionary<uint, List<FileSystemEventArgs>>();
+
+#if DEBUG_KEVENT_WATCHER
+				Console.WriteLine("Got {0} events", events.Count);
+#endif
+				for (int i = 0; i < events.Count; i++) {
+					var evnt = events[i];
+					int fd = evnt.ident.ToInt32();
+
+#if DEBUG_KEVENT_WATCHER
+					Console.WriteLine("Got {0}", evnt.ToString());
+#endif
+					
+					if ((evnt.filter & EventFilter.Vnode) != 0) {
+#if DEBUG_KEVENT_WATCHER						
+						Console.WriteLine("Event {0} on {1} ({2})", evnt.fflags, fds[fd].Path, fds[fd].Inode);
+#endif
+						if (fds[fd].IsDirectory)
+							dir_fds.Add(fd, 0);
+						else
+							if ((evnt.fflags & FilterFlags.VNodeAttrib) != 0 || 
+								(evnt.fflags & FilterFlags.VNodeExtend) != 0 ||
+								(evnt.fflags & FilterFlags.VNodeWrite) != 0)
+								changed_files_inodes.Add(fds[fd].Inode, 0);
+
+						/*
+						 * if entire watched tree is destroyed we still want to
+						 * report all files deleted (we are not just terminating)
+						 */
+						if (fds[fd].Inode == 0 && (evnt.fflags & FilterFlags.VNodeDelete) != 0)
+							watched_tree_destroyed = true;
+					}
+				}
 
-			var changes = new List<kevent> ();
-			foreach (int fd in FdList) {
-				var change = new kevent {
+				// XXX use ref for all parameters where contents can change, 
+				// to make it more obvious when reading code
+				foreach (int dir_fd in dir_fds.Keys)
+					RescanDir(this, base_dir, pattern, need_file_fd, recursive, dir_fd, ref fds, ref inodes, ref deleted_inodes, ref created_inodes, ref detected_events);
+
+				// add changed files to the event list as well
+				foreach (uint file_inode in changed_files_inodes.Keys) {
+					var data = inodes[file_inode];
 
-					ident = (UIntPtr)fd,
-					filter = EventFilter.Vnode,
-					flags = EventFlags.Add | EventFlags.Enable | EventFlags.Clear,
-					fflags = FilterFlags.VNodeDelete | FilterFlags.VNodeExtend |
-						FilterFlags.VNodeRename | FilterFlags.VNodeAttrib |
-						FilterFlags.VNodeLink | FilterFlags.VNodeRevoke |
-						FilterFlags.VNodeWrite,
-					data = IntPtr.Zero,
-					udata = IntPtr.Zero
-				};
+					AddFSEvent(base_dir, data, WatcherChangeTypes.Changed, ref detected_events);
+				}
 
-				changes.Add (change);
+				// order events properly
+				List<uint> event_inodes = new List<uint>(detected_events.Keys);
+				event_inodes.Sort((a, b) => inodes[a].Path.CompareTo(inodes[b].Path));
+
+				List<int> new_descriptors = new List<int>();
+				foreach (uint created_inode in created_inodes.Keys) {
+					var data = inodes[created_inode];
+
+					if (!data.IsDirectory && need_file_fd && pattern.IsMatch(data.Filename) && data.Fd < 0) {
+						// monitor file contents if name matches pattern
+						int fd = open(data.Path, O_RDONLY, 0);
+						if (fd < 0)
+							continue;
+						data.Fd = fd;
+						fds.Add(fd, data);
+						new_descriptors.Add(fd);
+#if DEBUG_KEVENT_WATCHER
+						Console.WriteLine("Now watching {0} at {1}", data.Path, data.Fd);
+#endif
+					}
+				}
+
+				// pass all new descriptors in one bulk
+				if (new_descriptors.Count > 0)
+					MonitorDescriptors(_kqueue, new_descriptors.ToArray());
+
+				/* 
+				 * XXX all moved inodes will be reported either:
+				 *  - 0 times - if destination directory got scanned first 
+				 *  - 2 times - once in deleted and once in created, if source 
+				 *           was scanned first 
+				 * therefore we need to distinguish between them and really deleted
+				 */
+				var really_deleted = ExceptWith<uint>(deleted_inodes, created_inodes);
+				List<int> deleted_descriptors = new List<int>();
+				foreach (var inode in really_deleted) {
+					var data = inodes[inode];
+					if (data.Fd >= 0) {
+						deleted_descriptors.Add(data.Fd);
+						fds.Remove(data.Fd);
+						data.Fd = -1;
+					}
+#if DEBUG_KEVENT_WATCHER
+					Console.WriteLine("Not watching {0} any more", data.Path);
+#endif
+					inodes.Remove(data.Inode);
+				}
+
+				// remove all old descriptors in one bulk
+				UnmonitorDescriptors(_kqueue, deleted_descriptors.ToArray());
+
+				// we are sending events from this thread only - in order to prevent
+				// user handler from experiencing race conditions
+				foreach (var changed_inode in event_inodes)
+					foreach (var evnt in detected_events[changed_inode]) {
+
+						if (evnt is RenamedEventArgs) {
+							// for renamed event we need to check if we should either
+							//  - stop monitoring file (if new name doesn't match)
+							//  - start monitoring file (if new name matches)
+							var data = inodes[changed_inode];
+							if (!data.IsDirectory)
+								if (data.Fd != -1 && !pattern.IsMatch(data.Filename)) {
+									UnmonitorDescriptors(_kqueue, data.Fd);
+									fds.Remove(data.Fd);
+									data.Fd = -1;
+								}
+								else if (need_file_fd && pattern.IsMatch(data.Filename) && data.Fd < 0) {
+									int fd = open(data.Path, O_RDONLY, 0);
+									if (fd < 0)
+										continue;
+									data.Fd = fd;
+									fds.Add(fd, data);
+									// we hope this is not very frequent operation
+									// so we don't create bulk
+									MonitorDescriptors(_kqueue, fd);
+#if DEBUG_KEVENT_WATCHER
+									Console.WriteLine("Now watching {0} at {1}", data.Path, data.Fd);
+#endif
+								}
+						}
+
+						// check if user asked to be notified about this one
+						if (pattern.IsMatch(evnt.Name))
+							_Watcher.DispatchEvent(evnt);
+						else if (evnt is RenamedEventArgs) {
+							var data = inodes[changed_inode];
+							if (report_dirname_change && data.IsDirectory)
+								_Watcher.DispatchEvent(evnt);
+							else {
+								var revnt = (RenamedEventArgs)evnt;
+								if (pattern.IsMatch(revnt.OldName))
+									_Watcher.DispatchEvent(evnt);
+							}
+						}
+					}
+#if DEBUG_KEVENT_WATCHER
+				GetFds(fds);
+#endif
+				if (watched_tree_destroyed)
+					goto exit_watcher;
 			}
-			FdList.Clear ();
 
-			return changes.ToArray ();
+			// in case we were interrupted in our call we want to try again
+			int errno = Marshal.GetLastWin32Error();
+			if (_Started && received == -1 && errno == EINTR) 
+				goto watcher_loop;
+
+		exit_watcher:
+			CloseDescriptors(fds.Keys);
+
+			// mark ourselves as stopped
+			_Started = false;
 		}
 
-		void Monitor ()
-		{
-			var eventBuffer = new kevent[32];
-			var newFds = new List<int> ();
-			List<PathData> removeQueue = new List<PathData> ();
-			List<string> rescanQueue = new List<string> ();
+		static T[] ExceptWith<T>(Hashtable a, Hashtable b) {
+			List<T> retval = new List<T>();
+			foreach (T x in a.Keys)
+				if (!b.ContainsKey(x))
+					retval.Add(x);
 
-			int retries = 0; 
+			return retval.ToArray();
+		}
 
-			while (!requestStop) {
-				var changes = CreateChangeList (ref newFds);
+		static Hashtable HashtableFromCollection(ICollection col) {
+			Hashtable retval = new Hashtable();
+			foreach (var x in col)
+				retval.Add(x, 0);
 
-				int numEvents = kevent_notimeout (conn, changes, changes.Length, eventBuffer, eventBuffer.Length, IntPtr.Zero);
+			return retval;
+		}
 
-				if (numEvents == -1) {
-					// Stop () signals us to stop by closing the connection
-					if (requestStop)
-						break;
-					if (++retries == 3)
-						throw new IOException (String.Format (
-							"persistent kevent() error, error code = '{0}'", Marshal.GetLastWin32Error ()));
+		/// <summary>
+		/// Rescans directory without recursing.
+		/// </summary>
+		/// <param name="dir_fd">Dir fd.</param>
+		/// <param name="fds">Fds.</param>
+		static void RescanDir(KqueueMonitor monitor, string base_dir, SearchPattern2 pattern, bool need_file_fd, bool recursive_watcher, int dir_fd, ref Dictionary<int, PathData> fds, ref Dictionary<uint, PathData> inodes,
+			ref Hashtable deleted_inodes, ref Hashtable created_inodes, ref Dictionary<uint, List<FileSystemEventArgs>> events) {
+			var current_dir = fds[dir_fd];
+			var dir_handle = fdopendir(dup(dir_fd));
+			bool dir_changed = false;
 
-					continue;
-				}
+			// XXX we have to rewind dir since we had its descriptor duplicated
+			rewinddir(dir_handle);
 
-				retries = 0;
+			IntPtr file_handle;
+			var old_contents = HashtableFromCollection(current_dir.Files.Keys);
+			var new_contents = new Hashtable();
+			while (monitor._Started && (file_handle = readdir(dir_handle)) != IntPtr.Zero) {
+				var file_info = dirent.FromPtr(file_handle);
+				var fname = file_info.Name;
 
-				for (var i = 0; i < numEvents; i++) {
-					var kevt = eventBuffer [i];
+				if (fname == "." || fname == "..")
+					continue;
 
-					if (!fdsDict.ContainsKey ((int)kevt.ident))
-						// The event is for a file that was removed
-						continue;
+				new_contents.Add(file_info.d_fileno, 0);
 
-					var pathData = fdsDict [(int)kevt.ident];
+				if (!old_contents.Contains(file_info.d_fileno)) {
+					// created new file or dir
+					dir_changed = true;
+
+#if DEBUG_KEVENT_WATCHER
+					Console.WriteLine("Created: " + fname);
+#endif
+					if (inodes.ContainsKey(file_info.d_fileno)) {
+						// it is actually just moved from somewhere within watched tree
+						var data = inodes[file_info.d_fileno];
+
+						if (data.IsDirectory) {
+							if (data.ParentDir != null) {
+								// destination dir first got chance to handle event
+								ReportSubtreeMoved(base_dir, data, current_dir, ref events);
+							}
+							else {
+								// already disconnected - just attach it to current...
+								current_dir.Files.Add(data.Inode, data);
+								data.ParentDir = current_dir;
+
+								// and pretend it was just created (old parent already reported it deleted)
+								AddFSEvent(base_dir, data, WatcherChangeTypes.Created, ref events);
+								created_inodes.Add(file_info.d_fileno, 0);
+
+								ReportSubtreeCreated(base_dir, data, ref created_inodes, ref events);
+							}
+						}
+						else {
+							if (data.ParentDir != null) {
+								// if it wasn't disconnected from old parent we do that now,
+								// old parent didn't have chance to report it as deleted
+								AddFSEvent(base_dir, data, WatcherChangeTypes.Deleted, ref events);
+								data.ParentDir.Files.Remove(data.Inode);
+							}
+							else {
+								// old parent already reported it deleted, so we have to undo that
+								created_inodes.Add(file_info.d_fileno, 0);
+							}
 
-					if ((kevt.flags & EventFlags.Error) == EventFlags.Error) {
-						var errMsg = String.Format ("kevent() error watching path '{0}', error code = '{1}'", pathData.Path, kevt.data);
-						fsw.DispatchErrorEvents (new ErrorEventArgs (new IOException (errMsg)));
-						continue;
-					}
-						
-					if ((kevt.fflags & FilterFlags.VNodeDelete) == FilterFlags.VNodeDelete || (kevt.fflags & FilterFlags.VNodeRevoke) == FilterFlags.VNodeRevoke) {
-						if (pathData.Path == fullPathNoLastSlash)
-							// The root path is deleted; exit silently
-							return;
-								
-						removeQueue.Add (pathData);
-						continue;
-					}
+							current_dir.Files.Add(data.Inode, data);
+							data.ParentDir = current_dir;
 
-					if ((kevt.fflags & FilterFlags.VNodeRename) == FilterFlags.VNodeRename) {
-							UpdatePath (pathData);
-					} 
-
-					if ((kevt.fflags & FilterFlags.VNodeWrite) == FilterFlags.VNodeWrite) {
-						if (pathData.IsDirectory) //TODO: Check if dirs trigger Changed events on .NET
-							rescanQueue.Add (pathData.Path);
-						else
-							PostEvent (FileAction.Modified, pathData.Path);
+							AddFSEvent(base_dir, data, WatcherChangeTypes.Created, ref events);
+						}
+					}
+					else {
+						// really new (at least for us watching this tree)
+						if (recursive_watcher && file_info.IsDirectory) {
+							Dictionary<uint, PathData> new_inodes = new Dictionary<uint, PathData>();
+
+							CollectAndMonitorDescriptors(System.IO.Path.Combine(current_dir.Path, fname), pattern, need_file_fd, true, monitor, fds, new_inodes);
+
+							// root watched dir gets inode 0
+							var data = new_inodes[0];
+
+							// we fix that now
+							data.Inode = file_info.d_fileno;
+							data.Filename = fname;
+
+							data.ParentDir = current_dir;
+							current_dir.Files.Add(data.Inode, data);
+
+							foreach (var new_node in new_inodes) {
+								AddFSEvent(base_dir, new_node.Value, WatcherChangeTypes.Created, ref events);
+
+								inodes.Add(new_node.Value.Inode, new_node.Value);
+							}
+						}
+						else {
+							var data = new PathData(fname, file_info.IsDirectory, -1, file_info.d_fileno, current_dir);
+
+							AddFSEvent(base_dir, data, WatcherChangeTypes.Created, ref events);
+
+							current_dir.Files.Add(data.Inode, data);
+							inodes.Add(data.Inode, data);
+							created_inodes.Add(data.Inode, 0);
+						}
 					}
-						
-					if ((kevt.fflags & FilterFlags.VNodeAttrib) == FilterFlags.VNodeAttrib || (kevt.fflags & FilterFlags.VNodeExtend) == FilterFlags.VNodeExtend)
-						PostEvent (FileAction.Modified, pathData.Path);
 				}
+				else {
+					var file = current_dir.Files[file_info.d_fileno];
+
+					// existing file - check its name
+					if (file.Filename != fname) {
+						dir_changed = true;
 
-				removeQueue.ForEach (Remove);
-				removeQueue.Clear ();
+						var old_relpath = file.RelPath;
+						file.Filename = fname;
 
-				rescanQueue.ForEach (path => {
-					Scan (path, true, ref newFds);
-				});
-				rescanQueue.Clear ();
+						AddRenamedEvent(file, current_dir.Path, file.RelPath, old_relpath, ref events);
+					}
+					// nothing changed
+				}
 			}
-		}
 
-		PathData Add (string path, bool postEvents, ref List<int> fds)
-		{
-			PathData pathData;
-			pathsDict.TryGetValue (path, out pathData);
+#if DEBUG_KEVENT_WATCHER
+			Console.WriteLine("**** Rescaned {0}, old_count={1}, new_count={2}", current_dir.Filename, old_contents.Count, new_contents.Count);
+#endif
+			// lets mark everything we didn't find as deleted
+			var locally_deleted_inodes = ExceptWith<uint>(old_contents, new_contents);
+			foreach (var inode in locally_deleted_inodes) {
+				dir_changed = true;
+				var deleted = current_dir.Files[inode];
+
+				AddFSEvent(base_dir, deleted, WatcherChangeTypes.Deleted, ref events);
+
+				// disconnect
+				current_dir.Files.Remove(inode);
+
+#if DEBUG_KEVENT_WATCHER
+				Console.WriteLine("Deleted: {0}", deleted.Path);
+#endif
+				if (deleted.IsDirectory)
+					ReportSubtreeDeleted(base_dir, deleted, ref deleted_inodes, ref events);	// recurse
+				else
+					deleted_inodes.Add(deleted.Inode, 0);
+				
+				deleted.ParentDir = null;
+			}
 
-			if (pathData != null)
-				return pathData;
+			if (dir_changed)
+				// XXX for some reason MS doesn't report changes in root of watched tree
+				// so we have to mimic that behavior 
+				if (current_dir.ParentDir != null)
+					AddFSEvent(base_dir, current_dir, WatcherChangeTypes.Changed, ref events);
 
-			if (fdsDict.Count >= maxFds)
-				throw new IOException ("kqueue() FileSystemWatcher has reached the maximum number of files to watch."); 
+			closedir(dir_handle);
+		}
 
-			var fd = open (path, O_EVTONLY, 0);
+		static void AddFSEvent(string base_dir, PathData file, WatcherChangeTypes change, ref Dictionary<uint, List<FileSystemEventArgs>> events) {
+			List<FileSystemEventArgs> changes;
 
-			if (fd == -1) {
-				fsw.DispatchErrorEvents (new ErrorEventArgs (new IOException (String.Format (
-					"open() error while attempting to process path '{0}', error code = '{1}'", path, Marshal.GetLastWin32Error ()))));
-				return null;
+			if (!events.TryGetValue(file.Inode, out changes)) {
+				changes = new List<FileSystemEventArgs>();
+				events.Add(file.Inode, changes);
 			}
 
-			try {
-				fds.Add (fd);
-
-				var attrs = File.GetAttributes (path);
+			foreach (var evt in changes)
+				if (evt.ChangeType == change)
+					return;
 
-				pathData = new PathData {
-					Path = path,
-					Fd = fd,
-					IsDirectory = (attrs & FileAttributes.Directory) == FileAttributes.Directory
-				};
-				
-				pathsDict.Add (path, pathData);
-				fdsDict.Add (fd, pathData);
+			changes.Add(new FileSystemEventArgs(change, base_dir, file.RelPath));
+		}
 
-				if (postEvents)
-					PostEvent (FileAction.Added, path);
+		static void AddRenamedEvent(PathData file, string directory, string rel_path, string old_relpath, ref Dictionary<uint, List<FileSystemEventArgs>> events) {
+			List<FileSystemEventArgs> changes;
 
-				return pathData;
-			} catch (Exception e) {
-				close (fd);
-				fsw.DispatchErrorEvents (new ErrorEventArgs (e));
-				return null;
+			if (!events.TryGetValue(file.Inode, out changes)) {
+				changes = new List<FileSystemEventArgs>();
+				events.Add(file.Inode, changes);
 			}
 
+			changes.Add(new RenamedEventArgs(WatcherChangeTypes.Renamed, directory, rel_path, old_relpath));
 		}
 
-		void Remove (PathData pathData)
-		{
-			fdsDict.Remove (pathData.Fd);
-			pathsDict.Remove (pathData.Path);
-			close (pathData.Fd);
-			PostEvent (FileAction.Removed, pathData.Path);
+		static void ReportSubtreeDeleted(string base_dir, PathData dir, ref Hashtable deleted_inodes, ref Dictionary<uint, List<FileSystemEventArgs>> events) {
+			foreach (var file in dir.Files.Values) {
+				AddFSEvent(base_dir, file, WatcherChangeTypes.Deleted, ref events);
+
+				if (deleted_inodes != null)
+					deleted_inodes.Add(file.Inode, 0);
+
+				if (file.IsDirectory)
+					ReportSubtreeDeleted(base_dir, file, ref deleted_inodes, ref events);
+			}
+
+			if (deleted_inodes != null)
+				deleted_inodes.Add(dir.Inode, 0);
 		}
 
-		void RemoveTree (PathData pathData)
-		{
-			var toRemove = new List<PathData> ();
+		static void ReportSubtreeCreated(string base_dir, PathData dir, ref Hashtable created_inodes, ref Dictionary<uint, List<FileSystemEventArgs>> events) {
+			foreach (var file in dir.Files.Values) {
+				AddFSEvent(base_dir, file, WatcherChangeTypes.Created, ref events);
 
-			toRemove.Add (pathData);
+				if (created_inodes != null)
+					created_inodes.Add(file.Inode, 0);
 
-			if (pathData.IsDirectory) {
-				var prefix = pathData.Path + Path.DirectorySeparatorChar;
-				foreach (var path in pathsDict.Keys)
-					if (path.StartsWith (prefix)) {
-						toRemove.Add (pathsDict [path]);
-					}
+				if (file.IsDirectory)
+					ReportSubtreeCreated(base_dir, file, ref created_inodes, ref events);
 			}
-			toRemove.ForEach (Remove);
 		}
 
-		void UpdatePath (PathData pathData)
-		{
-			var newRoot = GetFilenameFromFd (pathData.Fd);
-			if (!newRoot.StartsWith (fullPathNoLastSlash)) { // moved outside of our watched path (so stop observing it)
-				RemoveTree (pathData);
-				return;
-			}
-				
-			var toRename = new List<PathData> ();
-			var oldRoot = pathData.Path;
+		static void ReportSubtreeMoved(string base_dir, PathData dir, PathData new_parent, ref Dictionary<uint, List<FileSystemEventArgs>> events) {
+			Hashtable dummy = null;
 
-			toRename.Add (pathData);
-															
-			if (pathData.IsDirectory) { // anything under the directory must have their paths updated
-				var prefix = oldRoot + Path.DirectorySeparatorChar;
-				foreach (var path in pathsDict.Keys)
-					if (path.StartsWith (prefix))
-						toRename.Add (pathsDict [path]);
-			}
-		
-			foreach (var renaming in toRename) {
-				var oldPath = renaming.Path;
-				var newPath = newRoot + oldPath.Substring (oldRoot.Length);
-
-				renaming.Path = newPath;
-				pathsDict.Remove (oldPath);
-
-				// destination may exist in our records from a Created event, take care of it
-				if (pathsDict.ContainsKey (newPath)) {
-					var conflict = pathsDict [newPath];
-					if (GetFilenameFromFd (renaming.Fd) == GetFilenameFromFd (conflict.Fd))
-						Remove (conflict);
-					else
-						UpdatePath (conflict);
-				}
-					
-				pathsDict.Add (newPath, renaming);
-			}
-			
-			PostEvent (FileAction.RenamedNewName, oldRoot, newRoot);
+			ReportSubtreeDeleted(base_dir, dir, ref dummy, ref events);
+
+			AddFSEvent(base_dir, dir, WatcherChangeTypes.Deleted, ref events);
+
+			dir.ParentDir.Files.Remove(dir.Inode);
+
+			dir.ParentDir = new_parent;
+
+			new_parent.Files.Add(dir.Inode, dir);
+
+			AddFSEvent(base_dir, dir, WatcherChangeTypes.Created, ref events);
+
+			ReportSubtreeCreated(base_dir, dir, ref dummy, ref events);
 		}
 
-		void Scan (string path, bool postEvents, ref List<int> fds)
-		{
-			if (requestStop)
-				return;
-				
-			var pathData = Add (path, postEvents, ref fds);
+		static Exception CollectAndMonitorDescriptors(string directory, SearchPattern2 pattern, bool need_file_fd, bool recurse, KqueueMonitor monitor, Dictionary<int, PathData> fds, 
+			Dictionary<uint, PathData> inodes) {
+			List<int> descriptors = new List<int>();
+			List<int> subdirs = new List<int>();
 
-			if (pathData == null)
-				return;
-				
-			if (!pathData.IsDirectory)
-				return;
+			int fd = open(directory, O_RDONLY, 0);
+			if (fd < 0)
+				return new IOException(string.Format("KeventWatcher: Error collecting descriptors: {0}", Marshal.GetLastWin32Error()));
 
-			var dirsToProcess = new List<string> ();
-			dirsToProcess.Add (path);
+			var path = new PathData(directory, true, fd, 0, null);	// for root dir we don't care about inode number
 
-			while (dirsToProcess.Count > 0) {
-				var tmp = dirsToProcess [0];
-				dirsToProcess.RemoveAt (0);
-
-				var info = new DirectoryInfo (tmp);
-				FileSystemInfo[] fsInfos = null;
-				try {
-					fsInfos = info.GetFileSystemInfos ();
-						
-				} catch (IOException) {
-					// this can happen if the directory has been deleted already.
-					// that's okay, just keep processing the other dirs.
-					fsInfos = new FileSystemInfo[0];
-				}
+			fds.Add(fd, path);
+			inodes.Add(path.Inode, path);
+			descriptors.Add(fd);
 
-				foreach (var fsi in fsInfos) {
-					if ((fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory && !fsw.IncludeSubdirectories)
-						continue;
+			subdirs.Add(fd);
+
+			// doing BFS to collect files
+			int current_subdir = 0;
+			while (current_subdir < subdirs.Count) {
+				fd = subdirs[current_subdir];
+				current_subdir++;
 
-					if ((fsi.Attributes & FileAttributes.Directory) != FileAttributes.Directory && !fsw.Pattern.IsMatch (fsi.FullName))
+				// descriptor is not leaking because closedir is closing it
+				var dir_handle = fdopendir(dup(fd));
+				var dir_path = fds[fd];
+
+				IntPtr fptr;
+				while (monitor._Started && (fptr = readdir(dir_handle)) != IntPtr.Zero) {
+					var file = dirent.FromPtr(fptr);
+					var fname = file.Name;
+
+					if (fname == "." || fname == "..")
 						continue;
 
-					var currentPathData = Add (fsi.FullName, postEvents, ref fds);
+					string file_path = Path.Combine(dir_path.Path, fname);
+
+					path = new PathData(fname, file.IsDirectory, -1, file.d_fileno, dir_path);
+
+					dir_path.Files.Add(path.Inode, path);
+					inodes.Add(path.Inode, path);
+
+					if (!recurse && file.IsDirectory)
+						continue;
 
-					if (currentPathData != null && currentPathData.IsDirectory)
-						dirsToProcess.Add (fsi.FullName);
+					if ((recurse && file.IsDirectory) || (need_file_fd && pattern.IsMatch(fname))) {
+						fd = open(file_path, O_RDONLY, 0);
+						if (fd < 0)
+							continue;
+						path.Fd = fd;
+						fds.Add(fd, path);
+						descriptors.Add(fd);
+						if (file.IsDirectory)
+							subdirs.Add(fd);
+					}
 				}
+				closedir(dir_handle);
 			}
-		}
-			
-		void PostEvent (FileAction action, string path, string newPath = null)
-		{
-			RenamedEventArgs renamed = null;
 
-			if (requestStop || action == 0)
-				return;
+			if (monitor._Started) {
+				var prob = MonitorDescriptors(monitor._kqueue, descriptors.ToArray());
+				if (prob != null)
+					return prob;
+			}
 
-			// e.Name
-			string name = path.Substring (fullPathNoLastSlash.Length + 1); 
+			return null;
+		}
 
-			// only post events that match filter pattern. check both old and new paths for renames
-			if (!fsw.Pattern.IsMatch (path) && (newPath == null || !fsw.Pattern.IsMatch (newPath)))
-				return;
-				
-			if (action == FileAction.RenamedNewName) {
-				string newName = newPath.Substring (fullPathNoLastSlash.Length + 1);
-				renamed = new RenamedEventArgs (WatcherChangeTypes.Renamed, fsw.Path, newName, name);
+		static Exception MonitorDescriptors(int kqueue_fd, params int[] fds) {
+			List<kevent> kevents = new List<kevent>(fds.Length);
+			var monitor = new kevent();
+
+#if DEBUG_KEVENT_WATCHER
+			Console.Write("Monitoring: ");
+			for (int i = 0; i < fds.Length; i++)
+				Console.Write("{0}, ", fds[i]);
+			Console.WriteLine();
+#endif
+			foreach (var fd in fds) {			
+				EV_SET(ref monitor, fd, EventFilter.Vnode, EventFlags.Add | EventFlags.Clear | EventFlags.Enable, 
+					FilterFlags.VNodeDelete | FilterFlags.VNodeWrite | FilterFlags.VNodeExtend | FilterFlags.VNodeAttrib | 
+					FilterFlags.VNodeLink | FilterFlags.VNodeRename | FilterFlags.VNodeRevoke, 
+					IntPtr.Zero, IntPtr.Zero);
+				kevents.Add(monitor);
 			}
-				
-			fsw.DispatchEvents (action, name, ref renamed);
 
-			if (fsw.Waiting) {
-				lock (fsw) {
-					fsw.Waiting = false;
-					System.Threading.Monitor.PulseAll (fsw);
-				}
+			if (kevent(kqueue_fd, kevents.ToArray(), kevents.Count, null, 0, IntPtr.Zero) < 0) 
+				return new IOException(string.Format("KeventWatcher: Error monitoring descriptor: {0}", Marshal.GetLastWin32Error()));
+
+			return null;
+		}
+
+		static Exception UnmonitorDescriptors(int kqueue_fd, params int[] fds) {
+			List<kevent> kevents = new List<kevent>(fds.Length);
+			var monitor = new kevent();
+
+			foreach (var fd in fds) {			
+				EV_SET(ref monitor, fd, EventFilter.Vnode, EventFlags.Delete, 
+					FilterFlags.VNodeDelete | FilterFlags.VNodeWrite | FilterFlags.VNodeExtend | FilterFlags.VNodeAttrib | 
+					FilterFlags.VNodeLink | FilterFlags.VNodeRename | FilterFlags.VNodeRevoke, 
+					IntPtr.Zero, IntPtr.Zero);
+				kevents.Add(monitor);
 			}
+
+			if (kevent(kqueue_fd, kevents.ToArray(), kevents.Count, null, 0, IntPtr.Zero) < 0) 
+				return new IOException(string.Format("KeventWatcher: Error monitoring descriptor: {0}", Marshal.GetLastWin32Error()));
+
+			for (int i = 0; i < fds.Length; i++)
+				close(fds[i]);
+
+			return null;
 		}
 
-		private string GetFilenameFromFd (int fd)
-		{
-			var sb = new StringBuilder (__DARWIN_MAXPATHLEN);
+		static void CloseDescriptors(IEnumerable<int> fds) {
+			foreach (var fd in fds)
+				close(fd);
+		}
 
-			if (fcntl (fd, F_GETPATH, sb) != -1) {
-				if (fixupPath != null) 
-					sb.Replace (fixupPath, fullPathNoLastSlash, 0, fixupPath.Length); // see Setup()
+		static void EV_SET(ref kevent kev, int ident, EventFilter filter, EventFlags flags, FilterFlags filter_flags, IntPtr data, IntPtr udata) {
+			kev.ident = new IntPtr(ident);
+			kev.filter = filter;
+			kev.flags = flags;
+			kev.fflags = filter_flags;
+			kev.data = data;
+			kev.udata = udata;
+		}
 
-				return sb.ToString ();
-			} else {
-				fsw.DispatchErrorEvents (new ErrorEventArgs (new IOException (String.Format (
-					"fcntl() error while attempting to get path for fd '{0}', error code = '{1}'", fd, Marshal.GetLastWin32Error ()))));
-				return String.Empty;
+#if DEBUG_KEVENT_WATCHER
+		[StructLayout(LayoutKind.Sequential)]
+		struct kinfo_file {
+			int             kf_structsize;          /* Variable size of record. */
+			int             kf_type;                /* Descriptor type. */
+			int             kf_fd;                  /* Array index. */
+			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 356)]
+			byte[]          dummy;
+			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
+			byte[]          kf_path;                /* Path to file, if any. */
+
+			public int Fd {
+				get {
+					return kf_fd;
+				}
+			}
+			public string Name {
+				get {
+					return System.Text.ASCIIEncoding.ASCII.GetString(kf_path).TrimEnd('\0');
+				}
 			}
-		}
 
-		const int O_EVTONLY = 0x8000;
-		const int F_GETPATH = 50;
-		const int __DARWIN_MAXPATHLEN = 1024;
-		static readonly kevent[] emptyEventList = new System.IO.kevent[0];
-		int maxFds = Int32.MaxValue;
-
-		FileSystemWatcher fsw;
-		int conn;
-		Thread thread;
-		volatile bool requestStop = false;
-		AutoResetEvent startedEvent = new AutoResetEvent (false);
-		bool started = false;
-		bool inDispatch = false;
-		Exception exc = null;
-		object stateLock = new object ();
-		object connLock = new object ();
-
-		readonly Dictionary<string, PathData> pathsDict = new Dictionary<string, PathData> ();
-		readonly Dictionary<int, PathData> fdsDict = new Dictionary<int, PathData> ();
-		string fixupPath = null;
-		string fullPathNoLastSlash = null;
+		};
 
-		[DllImport ("libc", EntryPoint="fcntl", CharSet=CharSet.Auto, SetLastError=true)]
-		static extern int fcntl (int file_names_by_descriptor, int cmd, StringBuilder sb);
+		static void GetFds(Dictionary<int, PathData> fds) {
+			Console.WriteLine("Open descriptors:\n----------------------------------------------");
+			int count = 0;
+			var ret = kinfo_getfile(System.Diagnostics.Process.GetCurrentProcess().Id, out count);
+			HashSet<int> fd_hash = new HashSet<int>();
+			if (ret != IntPtr.Zero) {
+				for (int i = 0; i < count; i++) {
+					long ptr = ret.ToInt64() + i * 1392;
+					var kif = (kinfo_file)Marshal.PtrToStructure(new IntPtr(ptr), typeof(kinfo_file));
+					if (!fds.ContainsKey(kif.Fd))
+						Console.Write("NOT FOUND IN HASH ----> ");
+					else
+						Console.Write("{0} --> ", fds[kif.Fd].Path);
+					Console.WriteLine("{0}: {1}", kif.Fd, kif.Name);
+					fd_hash.Add(kif.Fd);
+				}
+				free(ret);
+			}
 
-		[DllImport ("libc")]
-		extern static int open (string path, int flags, int mode_t);
+			foreach (var fd in fds.Keys)
+				if (!fd_hash.Contains(fd))
+					Console.WriteLine("Hanged descriptor in hash: " + fd);
+		}
+#endif
+		
+		[DllImport("libc")]
+		extern static int open(string path, int flags, int mode_t);
 
-		[DllImport ("libc")]
-		extern static int close (int fd);
+		[DllImport("libc")]
+		extern static int close(int fd);
 
-		[DllImport ("libc")]
-		extern static int kqueue ();
+		[DllImport("libc")]
+		extern static int kqueue();
 
-		[DllImport ("libc")]
+		[DllImport("libc", SetLastError = true)]
+		extern static int kevent(int kq, [In]kevent[] ev, int nchanges, [Out]kevent[] evtlist, int nevents, IntPtr ptr);
+
+		[DllImport ("libc", SetLastError = true)]
 		extern static int kevent (int kq, [In]kevent[] ev, int nchanges, [Out]kevent[] evtlist, int nevents, [In] ref timespec time);
 
-		[DllImport ("libc", EntryPoint="kevent")]
-		extern static int kevent_notimeout (int kq, [In]kevent[] ev, int nchanges, [Out]kevent[] evtlist, int nevents, IntPtr ptr);
+		[DllImport("libc")]
+		extern static int dup(int fd);
+
+		[DllImport("libc")]
+		extern static IntPtr fdopendir(int fd);
+
+		[DllImport("libc")]
+		extern static int closedir(IntPtr dir);
+
+		[DllImport("libc")]
+		extern static IntPtr readdir(IntPtr dir);
+
+		[DllImport("libc")]
+		extern static int rewinddir(IntPtr dir);
+
+		[DllImport("libc")]
+		extern static void free(IntPtr m);
+
+		[DllImport("libutil")]
+		extern static IntPtr kinfo_getfile(int pid, out int count);
 	}
 
 	class KeventWatcher : IFileWatcher
@@ -718,7 +1038,7 @@
 				monitor = new KqueueMonitor (fsw);
 				watches.Add (fsw, monitor);
 			}
-				
+
 			monitor.Start ();
 		}
 
@@ -730,12 +1050,13 @@
 
 			monitor.Stop ();
 		}
-			
+
 		[DllImport ("libc")]
 		extern static int close (int fd);
 
 		[DllImport ("libc")]
 		extern static int kqueue ();
 	}
+
 }
 

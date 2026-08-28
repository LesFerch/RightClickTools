using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

namespace RightClickTools
{
    partial class Program
    {
        // Property enumeration structures and methods for property selector
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public struct PROPERTYKEY
        {
            public Guid fmtid;
            public int pid;
        }
#pragma warning restore CS0649

        public enum PROPDESC_ENUMFILTER
        {
            PDEF_ALL = 0,
            PDEF_SYSTEM = 1,
            PDEF_NONSYSTEM = 2,
            PDEF_VIEWABLE = 3,
            PDEF_QUERYABLE = 4,
            PDEF_INFULLTEXTQUERY = 5,
            PDEF_COLUMN = 6,
        }

        [Flags]
        public enum PROPDESC_TYPE_FLAGS
        {
            PDTF_DEFAULT = 0,
            PDTF_MULTIPLEVALUES = 0x1,
            PDTF_ISINNATE = 0x2,
            PDTF_ISGROUP = 0x4,
            PDTF_CANGROUPBY = 0x8,
            PDTF_CANSTACKBY = 0x10,
            PDTF_ISTREEPROPERTY = 0x20,
            PDTF_INCLUDEINFULLTEXTQUERY = 0x40,
            PDTF_ISVIEWABLE = 0x80,
            PDTF_ISQUERYABLE = 0x100,
            PDTF_CANBEPURGED = 0x200,
            PDTF_SEARCHRAWVALUE = 0x400,
            PDTF_DONTCOERCEEMPTYSTRINGS = 0x800,
            PDTF_ALWAYSINSUPPLEMENTALSTORE = 0x1000,
            PDTF_ISSYSTEMPROPERTY = unchecked((int)0x80000000),
            PDTF_MASK_ALL = unchecked((int)0x80001fff),
        }

        [DllImport("propsys")]
        public static extern int PSEnumeratePropertyDescriptions(PROPDESC_ENUMFILTER filterOn, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IPropertyDescriptionList ppv);

        [ComImport, Guid("1F9FC1D0-C39B-4B26-817F-011967D3440E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyDescriptionList
        {
            int GetCount();
            [return: MarshalAs(UnmanagedType.Interface)]
            IPropertyDescription GetAt(int iElem, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
        }

        [ComImport, Guid("6F79D558-3E96-4549-A1D1-7D75D2288814"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyDescription
        {
            PROPERTYKEY GetPropertyKey();
            [PreserveSig] int GetCanonicalName(out IntPtr zPtr);
            int GetPropertyType();
            [PreserveSig] int GetDisplayName(out IntPtr zPtr);
            [PreserveSig] int GetEditInvitation(out IntPtr zPtr);
            PROPDESC_TYPE_FLAGS GetTypeFlags(PROPDESC_TYPE_FLAGS mask);
        }

        public static string SelectFolder(string currentFolder)
        {
            // Save user's current "Expand to current folder" setting
            int userExpandSetting = 0;
            const string expandKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
            const string expandValueName = "NavPaneExpandToCurrentFolder";

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(expandKeyPath))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(expandValueName);
                        if (value != null)
                        {
                            userExpandSetting = (int)value;
                        }
                    }
                }
            }
            catch { }

            // Enable "Expand to current folder"
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(expandKeyPath))
                {
                    key.SetValue(expandValueName, 1, RegistryValueKind.DWord);
                }
            }
            catch { }

            FolderPicker fd = new FolderPicker
            {
                Title = "", // This will display "Select Folder" in the current OS language
                InputPath = !string.IsNullOrEmpty(currentFolder) && System.IO.Directory.Exists(currentFolder) ? currentFolder : "",
                Multiselect = false
            };

            if (fd.ShowDialog(IntPtr.Zero) == true && !string.IsNullOrEmpty(fd.ResultPath))
            {
                currentFolder = fd.ResultPath;
            }

            // Restore user's original "Expand to current folder" setting
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(expandKeyPath))
                {
                    key.SetValue(expandValueName, userExpandSetting, RegistryValueKind.DWord);
                }
            }
            catch { }

            return currentFolder;
        }
        // Courtesy of Simon Mourier https://stackoverflow.com/a/66187224/15764378
        public class FolderPicker
        {
            private readonly List<string> _resultPaths = new List<string>();
            private readonly List<string> _resultNames = new List<string>();

            public IReadOnlyList<string> ResultPaths => _resultPaths;
            public IReadOnlyList<string> ResultNames => _resultNames;
            public string ResultPath => ResultPaths.FirstOrDefault();
            public string ResultName => ResultNames.FirstOrDefault();
            public virtual string InputPath { get; set; }
            public virtual bool ForceFileSystem { get; set; }
            public virtual bool Multiselect { get; set; }
            public virtual string Title { get; set; }
            public virtual string OkButtonLabel { get; set; }
            public virtual string FileNameLabel { get; set; }
            protected virtual int SetOptions(int options)
            {
                if (ForceFileSystem)
                {
                    options |= (int)FOS.FOS_FORCEFILESYSTEM;
                }

                if (Multiselect)
                {
                    options |= (int)FOS.FOS_ALLOWMULTISELECT;
                }
                return options;
            }
            public virtual bool? ShowDialog(IntPtr owner, bool throwOnError = false)
            {
                var dialog = (IFileOpenDialog)new FileOpenDialog();
                if (!string.IsNullOrEmpty(InputPath))
                {
                    if (CheckHr(SHCreateItemFromParsingName(InputPath, null, typeof(IShellItem).GUID, out var item), throwOnError) != 0)
                        return null;
                    dialog.SetFolder(item);
                }
                var options = FOS.FOS_PICKFOLDERS;
                options = (FOS)SetOptions((int)options);
                dialog.SetOptions(options);
                if (Title != null)
                {
                    dialog.SetTitle(Title);
                }
                if (OkButtonLabel != null)
                {
                    dialog.SetOkButtonLabel(OkButtonLabel);
                }
                if (FileNameLabel != null)
                {
                    dialog.SetFileNameLabel(FileNameLabel);
                }
                if (owner == IntPtr.Zero)
                {
                    owner = Process.GetCurrentProcess().MainWindowHandle;
                    if (owner == IntPtr.Zero)
                    {
                        owner = GetDesktopWindow();
                    }
                }
                var hr = dialog.Show(owner);
                if (hr == ERROR_CANCELLED)
                    return null;
                if (CheckHr(hr, throwOnError) != 0)
                    return null;

                if (CheckHr(dialog.GetResults(out var items), throwOnError) != 0)
                    return null;

                items.GetCount(out var count);
                for (var i = 0; i < count; i++)
                {
                    items.GetItemAt(i, out var item);
                    CheckHr(item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out var path), throwOnError);
                    CheckHr(item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEEDITING, out var name), throwOnError);
                    if (path != null || name != null)
                    {
                        _resultPaths.Add(path);
                        _resultNames.Add(name);
                    }
                }
                return true;
            }
            private static int CheckHr(int hr, bool throwOnError)
            {
                if (hr != 0 && throwOnError) Marshal.ThrowExceptionForHR(hr);
                return hr;
            }
            [DllImport("shell32")]
            private static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IShellItem ppv);
            [DllImport("user32")]
            private static extern IntPtr GetDesktopWindow();
            private const int ERROR_CANCELLED = unchecked((int)0x800704C7);
            [ComImport, Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")] // CLSID_FileOpenDialog
            private class FileOpenDialog { }

            [ComImport, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface IFileOpenDialog
            {
                [PreserveSig] int Show(IntPtr parent); // IModalWindow
                [PreserveSig] int SetFileTypes();  // not fully defined
                [PreserveSig] int SetFileTypeIndex(int iFileType);
                [PreserveSig] int GetFileTypeIndex(out int piFileType);
                [PreserveSig] int Advise(); // not fully defined
                [PreserveSig] int Unadvise();
                [PreserveSig] int SetOptions(FOS fos);
                [PreserveSig] int GetOptions(out FOS pfos);
                [PreserveSig] int SetDefaultFolder(IShellItem psi);
                [PreserveSig] int SetFolder(IShellItem psi);
                [PreserveSig] int GetFolder(out IShellItem ppsi);
                [PreserveSig] int GetCurrentSelection(out IShellItem ppsi);
                [PreserveSig] int SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);
                [PreserveSig] int GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
                [PreserveSig] int SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
                [PreserveSig] int SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);
                [PreserveSig] int SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
                [PreserveSig] int GetResult(out IShellItem ppsi);
                [PreserveSig] int AddPlace(IShellItem psi, int alignment);
                [PreserveSig] int SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
                [PreserveSig] int Close(int hr);
                [PreserveSig] int SetClientGuid();  // not fully defined
                [PreserveSig] int ClearClientData();
                [PreserveSig] int SetFilter([MarshalAs(UnmanagedType.IUnknown)] object pFilter);
                [PreserveSig] int GetResults(out IShellItemArray ppenum);
                [PreserveSig] int GetSelectedItems([MarshalAs(UnmanagedType.IUnknown)] out object ppsai);
            }
            [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface IShellItem
            {
                [PreserveSig] int BindToHandler(); // not fully defined
                [PreserveSig] int GetParent(); // not fully defined
                [PreserveSig] int GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
                [PreserveSig] int GetAttributes();  // not fully defined
                [PreserveSig] int Compare();  // not fully defined
            }

            [ComImport, Guid("b63ea76d-1f85-456f-a19c-48159efa858b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface IShellItemArray
            {
                [PreserveSig] int BindToHandler();  // not fully defined
                [PreserveSig] int GetPropertyStore();  // not fully defined
                [PreserveSig] int GetPropertyDescriptionList();  // not fully defined
                [PreserveSig] int GetAttributes();  // not fully defined
                [PreserveSig] int GetCount(out int pdwNumItems);
                [PreserveSig] int GetItemAt(int dwIndex, out IShellItem ppsi);
                [PreserveSig] int EnumItems();  // not fully defined
            }

            private enum SIGDN : uint
            {
                SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
                SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
                SIGDN_FILESYSPATH = 0x80058000,
                SIGDN_NORMALDISPLAY = 0,
                SIGDN_PARENTRELATIVE = 0x80080001,
                SIGDN_PARENTRELATIVEEDITING = 0x80031001,
                SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
                SIGDN_PARENTRELATIVEPARSING = 0x80018001,
                SIGDN_URL = 0x80068000
            }
            [Flags]
            private enum FOS
            {
                FOS_OVERWRITEPROMPT = 0x2,
                FOS_STRICTFILETYPES = 0x4,
                FOS_NOCHANGEDIR = 0x8,
                FOS_PICKFOLDERS = 0x20,
                FOS_FORCEFILESYSTEM = 0x40,
                FOS_ALLNONSTORAGEITEMS = 0x80,
                FOS_NOVALIDATE = 0x100,
                FOS_ALLOWMULTISELECT = 0x200,
                FOS_PATHMUSTEXIST = 0x800,
                FOS_FILEMUSTEXIST = 0x1000,
                FOS_CREATEPROMPT = 0x2000,
                FOS_SHAREAWARE = 0x4000,
                FOS_NOREADONLYRETURN = 0x8000,
                FOS_NOTESTFILECREATE = 0x10000,
                FOS_HIDEMRUPLACES = 0x20000,
                FOS_HIDEPINNEDPLACES = 0x40000,
                FOS_NODEREFERENCELINKS = 0x100000,
                FOS_OKBUTTONNEEDSINTERACTION = 0x200000,
                FOS_DONTADDTORECENT = 0x2000000,
                FOS_FORCESHOWHIDDEN = 0x10000000,
                FOS_DEFAULTNOMINIMODE = 0x20000000,
                FOS_FORCEPREVIEWPANEON = 0x40000000,
                FOS_SUPPORTSTREAMABLEITEMS = unchecked((int)0x80000000)
            }
        }

        //Credit for the following TrustedInstaller code: https://github.com/rara64/GetTrustedInstaller
        class TrustedInstaller
        {
            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFOEX lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern UInt32 WaitForSingleObject(IntPtr handle, UInt32 milliseconds);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool UpdateProcThreadAttribute(IntPtr lpAttributeList, uint dwFlags, IntPtr Attribute, IntPtr lpValue, IntPtr cbSize, IntPtr lpPreviousValue, IntPtr lpReturnSize);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool InitializeProcThreadAttributeList(IntPtr lpAttributeList, int dwAttributeCount, int dwFlags, ref IntPtr lpSize);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool SetHandleInformation(IntPtr hObject, HANDLE_FLAGS dwMask, HANDLE_FLAGS dwFlags);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool CloseHandle(IntPtr hObject);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, ref IntPtr lpTargetHandle, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);

            public static void Run(int parentProcessId, string binaryPath)
            {
                const int PROC_THREAD_ATTRIBUTE_PARENT_PROCESS = 0x00020000;

                const uint EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
                const uint CREATE_NEW_CONSOLE = 0x00000010;

                var pInfo = new PROCESS_INFORMATION();
                var siEx = new STARTUPINFOEX();

                IntPtr lpValueProc = IntPtr.Zero;
                IntPtr hSourceProcessHandle = IntPtr.Zero;
                var lpSize = IntPtr.Zero;

                InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref lpSize);
                siEx.lpAttributeList = Marshal.AllocHGlobal(lpSize);
                InitializeProcThreadAttributeList(siEx.lpAttributeList, 1, 0, ref lpSize);

                IntPtr parentHandle = OpenProcess(ProcessAccessFlags.CreateProcess | ProcessAccessFlags.DuplicateHandle, false, parentProcessId);

                lpValueProc = Marshal.AllocHGlobal(IntPtr.Size);
                Marshal.WriteIntPtr(lpValueProc, parentHandle);

                UpdateProcThreadAttribute(siEx.lpAttributeList, 0, (IntPtr)PROC_THREAD_ATTRIBUTE_PARENT_PROCESS, lpValueProc, (IntPtr)IntPtr.Size, IntPtr.Zero, IntPtr.Zero);

                var ps = new SECURITY_ATTRIBUTES();
                var ts = new SECURITY_ATTRIBUTES();
                ps.nLength = Marshal.SizeOf(ps);
                ts.nLength = Marshal.SizeOf(ts);

                // lpCommandLine was used instead of lpApplicationName to allow for arguments to be passed
                bool ret = CreateProcess(null, binaryPath, ref ps, ref ts, true, EXTENDED_STARTUPINFO_PRESENT | CREATE_NEW_CONSOLE, IntPtr.Zero, null, ref siEx, out pInfo);

                String stringPid = pInfo.dwProcessId.ToString();

            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            struct STARTUPINFOEX
            {
                public STARTUPINFO StartupInfo;
                public IntPtr lpAttributeList;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            struct STARTUPINFO
            {
                public Int32 cb;
                public string lpReserved;
                public string lpDesktop;
                public string lpTitle;
                public Int32 dwX;
                public Int32 dwY;
                public Int32 dwXSize;
                public Int32 dwYSize;
                public Int32 dwXCountChars;
                public Int32 dwYCountChars;
                public Int32 dwFillAttribute;
                public Int32 dwFlags;
                public Int16 wShowWindow;
                public Int16 cbReserved2;
                public IntPtr lpReserved2;
                public IntPtr hStdInput;
                public IntPtr hStdOutput;
                public IntPtr hStdError;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct PROCESS_INFORMATION
            {
                public IntPtr hProcess;
                public IntPtr hThread;
                public int dwProcessId;
                public int dwThreadId;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SECURITY_ATTRIBUTES
            {
                public int nLength;
                public IntPtr lpSecurityDescriptor;
                [MarshalAs(UnmanagedType.Bool)]
                public bool bInheritHandle;
            }

            [Flags]
            public enum ProcessAccessFlags : uint
            {
                All = 0x001F0FFF,
                Terminate = 0x00000001,
                CreateThread = 0x00000002,
                VirtualMemoryOperation = 0x00000008,
                VirtualMemoryRead = 0x00000010,
                VirtualMemoryWrite = 0x00000020,
                DuplicateHandle = 0x00000040,
                CreateProcess = 0x000000080,
                SetQuota = 0x00000100,
                SetInformation = 0x00000200,
                QueryInformation = 0x00000400,
                QueryLimitedInformation = 0x00001000,
                Synchronize = 0x00100000
            }

            [Flags]
            enum HANDLE_FLAGS : uint
            {
                None = 0,
                INHERIT = 1,
                PROTECT_FROM_CLOSE = 2
            }
        }

        // Drive and folder information detection methods
        public static string GetFileSystem(string driveLetter)
        {
            try
            {
                System.IO.DriveInfo driveInfo = new System.IO.DriveInfo(driveLetter);
                return driveInfo.DriveFormat;
            }
            catch
            {
                return "Unknown";
            }
        }

        public static string GetDriveTypeLabel(string driveLetter)
        {
            try
            {
                System.IO.DriveInfo driveInfo = new System.IO.DriveInfo(driveLetter);

                switch (driveInfo.DriveType)
                {
                    case System.IO.DriveType.Fixed:
                        return "Local Disk";
                    case System.IO.DriveType.Removable:
                        return "USB Drive";
                    case System.IO.DriveType.Network:
                        return "Network Drive";
                    case System.IO.DriveType.CDRom:
                        return "CD-ROM Drive";
                    case System.IO.DriveType.Ram:
                        return "RAM Disk";
                    default:
                        return "Unknown";
                }
            }
            catch
            {
                return "Unknown";
            }
        }

        public static bool IsAFTDEnabled()
        {
            try
            {
                string key = @"HKEY_CURRENT_USER\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell";
                string v = (Registry.GetValue(key, "FolderType", "") as string);

                if (string.IsNullOrEmpty(v))
                {
                    return true; // AFTD is enabled (default)
                }

                v = v.ToLower();

                // List of folder types that, when hard set, disable Automatic Folder Type Discovery
                string[] folderTypes = {
                    "notspecified", "accountpictures", "contacts", "contacts.library", "contacts.searchresults",
                    "documents", "documents.library", "documents.searchresults", "downloads", "fileitemapis",
                    "generic", "generic.library", "generic.searchresults", "homefolder", "music",
                    "music.library", "music.searchresults", "opensearch", "otherusers", "otherusers.searchresults",
                    "pictures", "pictures.library", "pictures.searchresults", "publisheditems", "publisheditems.searchresults",
                    "searchconnector", "searches", "storageproviderdocuments", "storageprovidergeneric", "storageprovidermusic",
                    "storageproviderpictures", "storageprovidervideos", "userfiles", "userfiles.searchresults",
                    "userslibraries", "userslibraries.searchresults", "videos", "videos.library", "videos.searchresults"
                };

                return Array.IndexOf(folderTypes, v) == -1; // If NOT in the list, AFTD is enabled (no override set)
            }
            catch
            {
                return true; // Assume enabled if we can't read the registry
            }
        }

        public static void SetAFTDEnabled(bool enable)
        {
            try
            {
                string keyPath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell";
                using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    if (enable)
                    {
                        // Remove the FolderType value to enable AFTD
                        try
                        {
                            key.DeleteValue("FolderType", false);
                        }
                        catch { }
                    }
                    else
                    {
                        // Set to a valid folder type to disable AFTD
                        key.SetValue("FolderType", "Generic", RegistryValueKind.String);
                    }
                }
            }
            catch { }
        }

        public static bool IsAlwaysShowIconsEnabled()
        {
            try
            {
                string key = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                object value = Registry.GetValue(key, "IconsOnly", 0);
                if (value != null && value is int)
                {
                    return (int)value == 1;
                }
                return false; // Default is thumbnails enabled
            }
            catch
            {
                return false;
            }
        }

        public static void SetAlwaysShowIcons(bool enable)
        {
            try
            {
                string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    key.SetValue("IconsOnly", enable ? 1 : 0, RegistryValueKind.DWord);
                }
            }
            catch { }
        }

        public static bool IsFolderThumbnailsDisabled()
        {
            try
            {
                string key = @"HKEY_CURRENT_USER\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell";
                string value = Registry.GetValue(key, "Logo", "") as string;

                if (string.IsNullOrEmpty(value))
                {
                    return false; // Default is enabled (thumbnails shown)
                }

                return value.ToLower() == "none"; // "none" means disabled
            }
            catch
            {
                return false;
            }
        }

        public static void SetFolderThumbnailsDisabled(bool disable)
        {
            try
            {
                string keyPath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell";
                using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    if (disable)
                    {
                        key.SetValue("Logo", "none", RegistryValueKind.String);
                    }
                    else
                    {
                        // Remove the value to enable thumbnails
                        try
                        {
                            key.DeleteValue("Logo", false);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        // INI file helper class for desktop.ini manipulation
        public class IniFileHelper
        {
            private string filePath;

            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

            public IniFileHelper(string path)
            {
                filePath = path;
            }

            public void WriteValue(string section, string key, string value)
            {
                WritePrivateProfileString(section, key, value, filePath);
            }

            public string ReadValue(string section, string key)
            {
                const int size = 255;
                System.Text.StringBuilder builder = new System.Text.StringBuilder(size);
                GetPrivateProfileString(section, key, "", builder, size, filePath);
                return builder.ToString();
            }

            public void UpdateValue(string section, string key, string value)
            {
                WriteValue(section, key, value);
            }
        }

        // Folder type application methods
        public static void ApplyFolderType(string folderType, string directory)
        {
            string desktopIniPath = System.IO.Path.Combine(directory, "desktop.ini");

            if (folderType == "None")
            {
                if (System.IO.File.Exists(desktopIniPath))
                {
                    // Remove the FolderType entry
                    IniFileHelper iniFile = new IniFileHelper(desktopIniPath);
                    iniFile.WriteValue("ViewState", "FolderType", null);

                    // Check if there are other meaningful entries
                    if (!HasOtherEntries(desktopIniPath))
                    {
                        System.IO.File.Delete(desktopIniPath);
                    }
                }
                return;
            }

            if (System.IO.File.Exists(desktopIniPath))
            {
                UpdateIniValue(desktopIniPath, "ViewState", "FolderType", folderType);
            }
            else
            {
                CreateIniFile(desktopIniPath);
                UpdateIniValue(desktopIniPath, "ViewState", "FolderType", folderType);
                System.IO.File.SetAttributes(desktopIniPath, System.IO.File.GetAttributes(desktopIniPath) | System.IO.FileAttributes.System | System.IO.FileAttributes.Hidden);
            }
        }

        public static bool HasOtherEntries(string filePath)
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);
                int entryCount = 0;

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    // Count lines that contain "=" and are not empty or commented
                    if (!trimmedLine.StartsWith(";") && trimmedLine.Contains("=") && !string.IsNullOrWhiteSpace(trimmedLine))
                    {
                        entryCount++;
                    }
                }

                // If there are other entries remaining, keep the file
                return entryCount > 0;
            }
            catch
            {
                return true; // If we can't read the file, assume it has other entries to be safe
            }
        }

        private static void CreateIniFile(string filePath)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
            {
                writer.WriteLine("[ViewState]");
                writer.WriteLine("FolderType=Generic");
            }
        }

        private static void UpdateIniValue(string filePath, string section, string key, string value)
        {
            IniFileHelper iniFile = new IniFileHelper(filePath);
            iniFile.UpdateValue(section, key, value);
        }

        // Get the current folder type from desktop.ini
        public static string GetCurrentFolderType(string directory)
        {
            try
            {
                string desktopIniPath = System.IO.Path.Combine(directory, "desktop.ini");

                if (System.IO.File.Exists(desktopIniPath))
                {
                    IniFileHelper iniFile = new IniFileHelper(desktopIniPath);
                    string folderType = iniFile.ReadValue("ViewState", "FolderType");

                    if (!string.IsNullOrEmpty(folderType))
                    {
                        // Normalize the folder type to match our display names
                        folderType = folderType.Trim();

                        // Map internal folder types to display names
                        switch (folderType.ToLower())
                        {
                            case "generic":
                                return "General Items";
                            case "documents":
                                return "Documents";
                            case "pictures":
                                return "Pictures";
                            case "music":
                                return "Music";
                            case "videos":
                                return "Videos";
                            default:
                                return folderType; // Return as-is if not recognized
                        }
                    }
                }
            }
            catch { }

            return "(Not Set)"; // Default if no folder type is set
        }

        // Get the newest image file in a directory
        public static string GetNewestImageFileInDirectory(string dirPath)
        {
            var supportedExtensions = new[]
            {
                ".bmp", ".dib", ".rle",
                ".jpg", ".jpeg", ".jpe", ".jfif",
                ".tif", ".tiff",
                ".png"
            };

            try
            {
                var imageFiles = System.IO.Directory.GetFiles(dirPath)
                    .Where(f => supportedExtensions.Any(ext =>
                        f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (imageFiles.Count == 0)
                    return null;

                string newestFile = null;
                DateTime newestDate = DateTime.MinValue;

                foreach (var file in imageFiles)
                {
                    var fileInfo = new System.IO.FileInfo(file);
                    var latestDate = fileInfo.CreationTime > fileInfo.LastWriteTime
                        ? fileInfo.CreationTime
                        : fileInfo.LastWriteTime;

                    if (latestDate > newestDate)
                    {
                        newestDate = latestDate;
                        newestFile = file;
                    }
                }

                return newestFile;
            }
            catch
            {
                return null;
            }
        }

        // Get the two newest image files in a directory
        public static List<string> GetTwoNewestImageFilesInDirectory(string dirPath)
        {
            var supportedExtensions = new[]
            {
                ".bmp", ".dib", ".rle",
                ".jpg", ".jpeg", ".jpe", ".jfif",
                ".tif", ".tiff",
                ".png"
            };

            var result = new List<string>();

            try
            {
                var imageFiles = System.IO.Directory.GetFiles(dirPath)
                    .Where(f => supportedExtensions.Any(ext =>
                        f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    .Select(f => new System.IO.FileInfo(f))
                    .Select(fi => new
                    {
                        Path = fi.FullName,
                        Date = fi.CreationTime > fi.LastWriteTime ? fi.CreationTime : fi.LastWriteTime
                    })
                    .OrderByDescending(x => x.Date)
                    .Take(2)
                    .ToList();

                foreach (var imageFile in imageFiles)
                {
                    result.Add(imageFile.Path);
                }
            }
            catch { }

            return result;
        }

        // Get the four newest image files in a directory
        public static List<string> GetFourNewestImageFilesInDirectory(string dirPath)
        {
            var supportedExtensions = new[]
            {
                ".bmp", ".dib", ".rle",
                ".jpg", ".jpeg", ".jpe", ".jfif",
                ".tif", ".tiff",
                ".png"
            };

            var result = new List<string>();

            try
            {
                var imageFiles = System.IO.Directory.GetFiles(dirPath)
                    .Where(f => supportedExtensions.Any(ext =>
                        f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    .Select(f => new System.IO.FileInfo(f))
                    .Select(fi => new
                    {
                        Path = fi.FullName,
                        Date = fi.CreationTime > fi.LastWriteTime ? fi.CreationTime : fi.LastWriteTime
                    })
                    .OrderByDescending(x => x.Date)
                    .Take(4)
                    .ToList();

                foreach (var imageFile in imageFiles)
                {
                    result.Add(imageFile.Path);
                }
            }
            catch { }

            return result;
        }

        // Get the first image file alphabetically in a directory
        public static string GetFirstImageFileInDirectory(string dirPath)
        {
            var supportedExtensions = new[]
            {
                ".bmp", ".dib", ".rle",
                ".jpg", ".jpeg", ".jpe", ".jfif",
                ".tif", ".tiff",
                ".png"
            };

            try
            {
                var imageFiles = System.IO.Directory.GetFiles(dirPath)
                    .Where(f => supportedExtensions.Any(ext =>
                        f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return imageFiles.Count > 0 ? imageFiles[0] : null;
            }
            catch
            {
                return null;
            }
        }

        // Get the first two image files alphabetically in a directory
        public static List<string> GetFirstTwoImageFilesInDirectory(string dirPath)
        {
            var supportedExtensions = new[]
            {
                ".bmp", ".dib", ".rle",
                ".jpg", ".jpeg", ".jpe", ".jfif",
                ".tif", ".tiff",
                ".png"
            };

            var result = new List<string>();

            try
            {
                var imageFiles = System.IO.Directory.GetFiles(dirPath)
                    .Where(f => supportedExtensions.Any(ext =>
                        f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                    .Take(2)
                    .ToList();

                result.AddRange(imageFiles);
            }
            catch { }

            return result;
        }

        // Get the first four image files alphabetically in a directory
        public static List<string> GetFirstFourImageFilesInDirectory(string dirPath)
        {
            var supportedExtensions = new[]
            {
                ".bmp", ".dib", ".rle",
                ".jpg", ".jpeg", ".jpe", ".jfif",
                ".tif", ".tiff",
                ".png"
            };

            var result = new List<string>();

            try
            {
                var imageFiles = System.IO.Directory.GetFiles(dirPath)
                    .Where(f => supportedExtensions.Any(ext =>
                        f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                    .Take(4)
                    .ToList();

                result.AddRange(imageFiles);
            }
            catch { }

            return result;
        }

        // Create icon from images and save to specified path
        public static void CreateIconFromImages(List<string> imagePaths, string outputPath, string mode)
        {
            if (imagePaths.Count == 0) return;

            System.Drawing.Image compositeImage = null;

            try
            {
                if (mode == "Fill (2 landscape images)" || mode == "Fill (2 portrait images)")
                {
                    if (imagePaths.Count >= 2)
                    {
                        var img1 = LoadImageFromPath(imagePaths[0]);
                        var img2 = LoadImageFromPath(imagePaths[1]);
                        if (img1 != null && img2 != null)
                        {
                            bool isLandscape = mode == "Fill (2 landscape images)";
                            compositeImage = CompositeImages(img1, img2, isLandscape);
                        }
                        if (img1 != null) img1.Dispose();
                        if (img2 != null) img2.Dispose();
                    }
                }
                else if (mode == "Fill (4 images)")
                {
                    if (imagePaths.Count >= 4)
                    {
                        var img1 = LoadImageFromPath(imagePaths[0]);
                        var img2 = LoadImageFromPath(imagePaths[1]);
                        var img3 = LoadImageFromPath(imagePaths[2]);
                        var img4 = LoadImageFromPath(imagePaths[3]);
                        if (img1 != null && img2 != null && img3 != null && img4 != null)
                        {
                            compositeImage = CompositeFourImages(img1, img2, img3, img4);
                        }
                        if (img1 != null) img1.Dispose();
                        if (img2 != null) img2.Dispose();
                        if (img3 != null) img3.Dispose();
                        if (img4 != null) img4.Dispose();
                    }
                }
                else
                {
                    // Single image modes
                    compositeImage = LoadImageFromPath(imagePaths[0]);
                }

                if (compositeImage == null) return;

                // Determine if we need overlay (Fit modes)
                bool useFit = mode.StartsWith("Fit");
                System.Drawing.Image overlayImage = null;

                if (useFit)
                {
                    string overlayPath = System.IO.Path.Combine(appParts, @"Icons\FolderBack.png");
                    if (System.IO.File.Exists(overlayPath))
                    {
                        overlayImage = new System.Drawing.Bitmap(overlayPath);
                    }
                }

                // Create icon sizes
                var iconSizes = new[] { 16, 32, 48, 256 };
                var images = new List<System.Drawing.Image>();

                foreach (int size in iconSizes)
                {
                    var resizedImage = ResizeImageForIcon(compositeImage, size, useFit, overlayImage, mode.Contains("transparent"));
                    if (resizedImage != null)
                    {
                        images.Add(resizedImage);
                    }
                }

                // Save as icon
                SaveAsIcon(images, outputPath);

                // Cleanup
                foreach (var img in images)
                {
                    img.Dispose();
                }
                if (overlayImage != null) overlayImage.Dispose();
                compositeImage.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating icon: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static System.Drawing.Image LoadImageFromPath(string path)
        {
            try
            {
                using (var originalImage = new System.Drawing.Bitmap(path))
                {
                    int targetWidth = originalImage.Width;
                    int targetHeight = originalImage.Height;
                    int minDimensionTarget = 256;

                    int minDimension = Math.Min(originalImage.Width, originalImage.Height);
                    if (minDimension > minDimensionTarget)
                    {
                        double scale = (double)minDimensionTarget / minDimension;
                        targetWidth = (int)(originalImage.Width * scale);
                        targetHeight = (int)(originalImage.Height * scale);
                    }

                    var newImage = new System.Drawing.Bitmap(targetWidth, targetHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    using (var graphics = System.Drawing.Graphics.FromImage(newImage))
                    {
                        graphics.Clear(System.Drawing.Color.Transparent);
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        graphics.DrawImage(originalImage, 0, 0, targetWidth, targetHeight);
                    }

                    return newImage;
                }
            }
            catch
            {
                return null;
            }
        }

        private static System.Drawing.Image CompositeImages(System.Drawing.Image image1, System.Drawing.Image image2, bool isLandscape)
        {
            int maxDimension = Math.Max(Math.Max(image1.Width, image1.Height), Math.Max(image2.Width, image2.Height));
            var compositeImage = new System.Drawing.Bitmap(maxDimension, maxDimension, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (var graphics = System.Drawing.Graphics.FromImage(compositeImage))
            {
                graphics.Clear(System.Drawing.Color.Transparent);
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                if (isLandscape)
                {
                    // Top and bottom arrangement
                    int halfHeight = maxDimension / 2;

                    float scale1 = Math.Max((float)maxDimension / image1.Width, (float)halfHeight / image1.Height);
                    int cropWidth1 = (int)(maxDimension / scale1);
                    int cropHeight1 = (int)(halfHeight / scale1);
                    int cropX1 = (image1.Width - cropWidth1) / 2;
                    int cropY1 = (image1.Height - cropHeight1) / 2;

                    graphics.DrawImage(image1,
                        new System.Drawing.Rectangle(0, 0, maxDimension, halfHeight),
                        cropX1, cropY1, cropWidth1, cropHeight1,
                        System.Drawing.GraphicsUnit.Pixel);

                    float scale2 = Math.Max((float)maxDimension / image2.Width, (float)halfHeight / image2.Height);
                    int cropWidth2 = (int)(maxDimension / scale2);
                    int cropHeight2 = (int)(halfHeight / scale2);
                    int cropX2 = (image2.Width - cropWidth2) / 2;
                    int cropY2 = (image2.Height - cropHeight2) / 2;

                    graphics.DrawImage(image2,
                        new System.Drawing.Rectangle(0, halfHeight, maxDimension, halfHeight),
                        cropX2, cropY2, cropWidth2, cropHeight2,
                        System.Drawing.GraphicsUnit.Pixel);
                }
                else
                {
                    // Left and right arrangement
                    int halfWidth = maxDimension / 2;

                    float scale1 = Math.Max((float)halfWidth / image1.Width, (float)maxDimension / image1.Height);
                    int cropWidth1 = (int)(halfWidth / scale1);
                    int cropHeight1 = (int)(maxDimension / scale1);
                    int cropX1 = (image1.Width - cropWidth1) / 2;
                    int cropY1 = (image1.Height - cropHeight1) / 2;

                    graphics.DrawImage(image1,
                        new System.Drawing.Rectangle(0, 0, halfWidth, maxDimension),
                        cropX1, cropY1, cropWidth1, cropHeight1,
                        System.Drawing.GraphicsUnit.Pixel);

                    float scale2 = Math.Max((float)halfWidth / image2.Width, (float)maxDimension / image2.Height);
                    int cropWidth2 = (int)(halfWidth / scale2);
                    int cropHeight2 = (int)(maxDimension / scale2);
                    int cropX2 = (image2.Width - cropWidth2) / 2;
                    int cropY2 = (image2.Height - cropHeight2) / 2;

                    graphics.DrawImage(image2,
                        new System.Drawing.Rectangle(halfWidth, 0, halfWidth, maxDimension),
                        cropX2, cropY2, cropWidth2, cropHeight2,
                        System.Drawing.GraphicsUnit.Pixel);
                }
            }

            return compositeImage;
        }

        private static System.Drawing.Image CompositeFourImages(System.Drawing.Image image1, System.Drawing.Image image2, System.Drawing.Image image3, System.Drawing.Image image4)
        {
            int maxDimension = Math.Max(
                Math.Max(Math.Max(image1.Width, image1.Height), Math.Max(image2.Width, image2.Height)),
                Math.Max(Math.Max(image3.Width, image3.Height), Math.Max(image4.Width, image4.Height))
            );

            var compositeImage = new System.Drawing.Bitmap(maxDimension, maxDimension, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (var graphics = System.Drawing.Graphics.FromImage(compositeImage))
            {
                graphics.Clear(System.Drawing.Color.Transparent);
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                int halfSize = maxDimension / 2;
                var images = new[] { image1, image2, image3, image4 };
                var positions = new[]
                {
                    new { X = 0, Y = 0 },
                    new { X = halfSize, Y = 0 },
                    new { X = 0, Y = halfSize },
                    new { X = halfSize, Y = halfSize }
                };

                for (int i = 0; i < 4; i++)
                {
                    var img = images[i];
                    var pos = positions[i];

                    float scale = Math.Max((float)halfSize / img.Width, (float)halfSize / img.Height);
                    int cropWidth = (int)(halfSize / scale);
                    int cropHeight = (int)(halfSize / scale);
                    int cropX = (img.Width - cropWidth) / 2;
                    int cropY = (img.Height - cropHeight) / 2;

                    graphics.DrawImage(img,
                        new System.Drawing.Rectangle(pos.X, pos.Y, halfSize, halfSize),
                        cropX, cropY, cropWidth, cropHeight,
                        System.Drawing.GraphicsUnit.Pixel);
                }
            }

            return compositeImage;
        }

        private static System.Drawing.Image ResizeImageForIcon(System.Drawing.Image image, int size, bool useFit, System.Drawing.Image overlayImage, bool transparent)
        {
            var destImage = new System.Drawing.Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = System.Drawing.Graphics.FromImage(destImage))
            {
                graphics.Clear(System.Drawing.Color.Transparent);
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                if (useFit)
                {
                    // Draw overlay first (folder.png) only for non-transparent mode
                    if (overlayImage != null && !transparent)
                    {
                        graphics.DrawImage(overlayImage, 0, 0, size, size);
                    }

                    // Scale image to fit
                    float scale = Math.Min((float)size / image.Width, (float)size / image.Height);
                    int newWidth = (int)(image.Width * scale);
                    int newHeight = (int)(image.Height * scale);
                    int x = (size - newWidth) / 2;
                    int y = (size - newHeight) / 2;

                    var sourceRect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
                    var destRect = new System.Drawing.Rectangle(x, y, newWidth, newHeight);

                    using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                    {
                        wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height, System.Drawing.GraphicsUnit.Pixel, wrapMode);
                    }
                }
                else
                {
                    // Fill mode with rounded corners
                    int cornerRadius = size / 8;
                    using (var path = GetRoundedRectPath(new System.Drawing.Rectangle(0, 0, size, size), cornerRadius))
                    {
                        graphics.SetClip(path);
                    }

                    int cropSize = Math.Min(image.Width, image.Height);
                    int cropX = (image.Width - cropSize) / 2;
                    int cropY = (image.Height - cropSize) / 2;

                    var sourceRect = new System.Drawing.Rectangle(cropX, cropY, cropSize, cropSize);
                    var destRect = new System.Drawing.Rectangle(0, 0, size, size);

                    using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                    {
                        wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height, System.Drawing.GraphicsUnit.Pixel, wrapMode);
                    }
                }
            }

            return destImage;
        }

        private static System.Drawing.Drawing2D.GraphicsPath GetRoundedRectPath(System.Drawing.Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int diameter = radius * 2;

            var arc = new System.Drawing.Rectangle(rect.Location, new System.Drawing.Size(diameter, diameter));

            path.AddArc(arc, 180, 90);

            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private static void SaveAsIcon(List<System.Drawing.Image> images, string outputPath)
        {
            System.IO.FileAttributes originalAttributes = System.IO.FileAttributes.Normal;
            bool hadAttributes = false;

            if (System.IO.File.Exists(outputPath))
            {
                try
                {
                    originalAttributes = System.IO.File.GetAttributes(outputPath);
                    hadAttributes = true;

                    if ((originalAttributes & System.IO.FileAttributes.Hidden) != 0 ||
                        (originalAttributes & System.IO.FileAttributes.System) != 0)
                    {
                        System.IO.File.SetAttributes(outputPath, System.IO.FileAttributes.Normal);
                    }
                }
                catch { }
            }

            using (var fs = new System.IO.FileStream(outputPath, System.IO.FileMode.Create))
            using (var bw = new System.IO.BinaryWriter(fs))
            {
                bw.Write((short)0);
                bw.Write((short)1);
                bw.Write((short)images.Count);

                var offset = 6 + (16 * images.Count);

                var imageData = new List<byte[]>();
                foreach (var image in images)
                {
                    var data = GetPngBytes(image);
                    imageData.Add(data);

                    bw.Write((byte)(image.Width >= 256 ? 0 : image.Width));
                    bw.Write((byte)(image.Height >= 256 ? 0 : image.Height));
                    bw.Write((byte)0);
                    bw.Write((byte)0);
                    bw.Write((short)1);
                    bw.Write((short)32);
                    bw.Write(data.Length);
                    bw.Write(offset);

                    offset += data.Length;
                }

                foreach (var data in imageData)
                {
                    bw.Write(data);
                }
            }

            if (hadAttributes)
            {
                try
                {
                    System.IO.File.SetAttributes(outputPath, originalAttributes);
                }
                catch { }
            }
        }

        private static byte[] GetPngBytes(System.Drawing.Image image)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        // File operation wrapper for moving files to recycle bin
        public static class FileOperationAPIWrapper
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            private struct SHFILEOPSTRUCT
            {
                public IntPtr hwnd;
                public uint wFunc;
                public string pFrom;
                public string pTo;
                public ushort fFlags;
                public bool fAnyOperationsAborted;
                public IntPtr hNameMappings;
                public string lpszProgressTitle;
            }

            [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
            private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

            private const uint FO_DELETE = 0x0003;
            private const ushort FOF_ALLOWUNDO = 0x0040;
            private const ushort FOF_NOCONFIRMATION = 0x0010;
            private const ushort FOF_SILENT = 0x0004;

            public static bool MoveToRecycleBin(string filePath)
            {
                try
                {
                    SHFILEOPSTRUCT fileOp = new SHFILEOPSTRUCT
                    {
                        hwnd = IntPtr.Zero,
                        wFunc = FO_DELETE,
                        pFrom = filePath + '\0' + '\0',
                        pTo = null,
                        fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION | FOF_SILENT,
                        fAnyOperationsAborted = false,
                        hNameMappings = IntPtr.Zero,
                        lpszProgressTitle = null
                    };

                    int result = SHFileOperation(ref fileOp);
                    return result == 0;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}

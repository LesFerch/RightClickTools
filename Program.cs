using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Text.RegularExpressions;

namespace RightClickTools
{
    partial class Program
    {
        static string myName = typeof(Program).Namespace;
        static string myPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        static string myExe = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string TempPath = Path.GetTempPath(); //Includes trailing backslash
        static string ElevateCfg = $@"{TempPath}Elevate.cfg";
        static string appParts = $@"{myPath}\AppParts";
        static string myIniFile = $@"{appParts}\{myName}.ini";
        static string MoreToolsIni = $@"{appParts}\MoreTools.ini";
        static string LauncherIni = $@"{appParts}\Launcher.ini";
        static string IconFolder = $@"{myPath}\AppParts\Icons";
        static string myIcon = $@"{IconFolder}\{myName}.ico";
        static string AdvKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        static string perKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        static string ExpKey = @"HKEY_LOCAL_MACHINE\Software\Classes\AppID\{CDCBCFCA-3CDC-436f-A4E2-0E02075250C2}";
        static string bitPath = "64";
        static bool Hidden = false;
        static string NTkey = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion";
        static int buildNumber = int.Parse(Registry.GetValue(NTkey, "CurrentBuild", "").ToString());
        static bool Win11 = buildNumber >= 21996;
        static bool Win11Install = false;
        static bool AnyInstall = false;

        // Determine icon folder based on Windows version
        // Windows 7/8: 7600-9600, Windows 10: 10240-21995, Windows 11: 21996+
        static string GetIconFolderForVersion()
        {
            if (Win11) return "Win11";
            if (buildNumber >= 10240) return "Win10";
            return "Win7"; // Windows 7 and 8/8.1
        }
        static string CCMfolder = FindCustomCommandsFolder(true);
        static string CCMA = @"Software\Classes\CLSID\{86CA1AA0-34AA-4E8B-A509-50C905BAE2A2}";
        static string CCMB = $@"{CCMA}\InprocServer32";
        static bool Win10ContextMenu = false;
        static bool NoTrustedInstaller = false;
        static string EditorExe = "notepad.exe";

        static string[] CmdKeys = { "CmdHere", "PowerShellHere", "PowerShellCoreHere", "FileManagerHere", "SearchHere", "RegEdit", "ClearHistory", "UnblockHere", "TakeOwnHere", "AddDelPathHere", "ShowHide", "RefreshShellHere", "FolderOptionsHere", "RestartExplorerHere", "Settings", "MoreToolsHere" };


        static string Option = "";
        static string StartDirectory = "";
        static string CommandLine = "";

        internal static float ScaleFactor = GetScale();
        internal static bool Dark = isDark();
        static bool isAdmin = false;
        static bool isFullAdmin = false;
        static bool ctrlKey = false;
        static IntPtr explorerHwnd = IntPtr.Zero;
        static bool addTask = true;
        static bool removeTask = true;
        static bool Stop = false;

        static string UserKey = @"HKEY_CURRENT_USER\Environment";
        static string SystemKey = @"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment";
        static string UserPath = (string)Registry.GetValue(UserKey, "Path", "");
        static string SystemPath = (string)Registry.GetValue(SystemKey, "Path", "");
        static bool InUserPath = false;
        static bool InSystemPath = false;
        static int pathLength = UserPath.Length + SystemPath.Length;
        static string UIkey = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI";
        static string userSID = "";

        static string CmdExe = @"C:\Windows\System32\Cmd.exe";
        static string PowerShellExe = @"C:\Windows\System32\WindowsPowerShell\v1.0\PowerShell.exe";
        static string PowerShellCoreExe = @"pwsh.exe";
        static string RegEditExe = @"C:\Windows\RegEdit.exe";
        static string SchTasksExe = @"C:\Windows\System32\SchTasks.exe";

        static string UserName = Environment.GetEnvironmentVariable("UserName");
        static string TaskName = $@"MyTasks\{myName}-{UserName}";
        static string helpPage = "install";
        static int bwidth = 75;

        static CheckBox userPathCheckbox;
        static CheckBox systemPathCheckbox;

        static CheckBox ShellRefreshCheckbox;
        static CheckBox iconCacheCheckbox;
        static CheckBox thumbCacheCheckbox;

        static CheckBox RecentItemsCheckbox;
        static CheckBox AutoSuggestCheckbox;
        static CheckBox TempFilesCheckbox;
        static CheckBox RecycleBinCheckbox;
        static CheckBox DefenderCheckbox;
        static CheckBox SpecifiedFoldersCheckbox;
        static CheckBox checkboxCCM;
        static CheckBox checkboxTask;
        static CheckBox checkboxUnblockAdmin;

        static bool needsIconCacheReset = false;
        static bool useWindowsTerminal = false;
        static string originalPowerShellExe = "";
        static string originalExe = "";

        static string[] mainArgs = null; // Store args for theme change restart
        static bool themeChanged = false; // Flag to indicate theme has changed
        static Point savedDialogPosition = Point.Empty; // Save dialog position for theme change relaunch
        static bool useOriginalPosition = false; // Use saved dialog position on next dialog launch

        [STAThread]
        static void Main(string[] args)
        {
            // Store args for potential theme change restart
            mainArgs = args;

            // Save cursor position for initial dialog launch
            if (savedDialogPosition == Point.Empty)
            {
                savedDialogPosition = Cursor.Position;
            }

            // If the current folder is a long path, the Elevate function will fail, so let's make C:\ the current folder.
            Directory.SetCurrentDirectory(@"C:\");

            explorerHwnd = GetForegroundWindow(); // Capture Explorer's HWND before it loses foreground
            ctrlKey = (GetAsyncKeyState(0x11) & 0x8000) != 0; //Detect if Ctrl key is pressed

            if (!Environment.Is64BitOperatingSystem) bitPath = "32";

            // Process /Lang= parameter before initializing user settings
            string installerLang = null;
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.StartsWith("/Lang=", StringComparison.OrdinalIgnoreCase))
                    {
                        installerLang = arg.Substring(6);
                        break;
                    }
                }
            }

            InitializeUserSettingsFile();

            // Set language from installer if provided
            if (!string.IsNullOrEmpty(installerLang))
            {
                WriteString(myIniFile, "General", "Lang", installerLang);
            }

            LoadLanguageStrings();
            StringsFromCmdLabels();

            // Read NoTrustedInstaller setting from INI file
            NoTrustedInstaller = ReadString(myIniFile, "General", "NoTrustedInstaller", "0") == "1";

            // Read Editor setting from INI file
            string editorPath = ReadString(myIniFile, "General", "Editor", "");
            if (!string.IsNullOrEmpty(editorPath) && File.Exists(editorPath))
            {
                EditorExe = editorPath;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Subscribe to system theme changes
            Microsoft.Win32.SystemEvents.UserPreferenceChanged += OnSystemThemeChanged;

            isAdmin = IsCurrentUserInAdminGroup();
            isFullAdmin = isAdmin && TaskExists();

            try { Hidden = (int)Registry.GetValue(AdvKey, "Hidden", 0) == 1; } catch { }

            try { userSID = (string)Registry.GetValue(UIkey, "LastLoggedOnUserSID", ""); } catch { }

            if (userSID == "") findUserSID();

            // Handle protocol activation (from MSIX context menu to break out of AppContainer)
            if (args.Length > 0 && args[0].StartsWith("rightclicktools:", StringComparison.OrdinalIgnoreCase))
            {
                HandleProtocolActivation(args[0]);
                return;
            }

            // If a directory was passed in addition to an option, save it to registry immediately
            if (args.Length > 1)
            {
                string passedDirectory = args[1].Replace("|", "");
                if (!string.IsNullOrEmpty(passedDirectory) && Directory.Exists(passedDirectory))
                {
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", passedDirectory, RegistryValueKind.String);
                }
            }

            // Load StartDirectory from registry (set by context menu or previous dialog)
            try
            {
                string savedDirectory = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", "");
                if (!string.IsNullOrEmpty(savedDirectory) && Directory.Exists(savedDirectory))
                {
                    StartDirectory = savedDirectory;
                }
                else
                {
                    StartDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
            }
            catch { }

            // Main execution loop - restart if theme changes
            do
            {
                themeChanged = false;

                if (args.Length == 0)
                {
                    Launcher();
                }
                else
                {
                    Option = args[0];

                    // When launched as an elevated helper (/Elevate), the cfg file path is passed as args[1].
                    // It must be captured here (in Main where args is in scope) before ExecuteCommand runs,
                    // because the elevated process may compute a different %TEMP% path than the calling user.
                    if (Option.Equals("/Elevate", StringComparison.OrdinalIgnoreCase) && args.Length > 1)
                    {
                        ElevateCfg = args[1];
                    }

                    ExecuteCommand();
                }
            } while (themeChanged);
        }

        static void ExecuteCommand()
        {
            switch (Option.ToLower())
            {
                case "/dark":
                    SetExplorerOptions(0);
                    break;

                case "/light":
                    SetExplorerOptions(1);
                    break;

                case "/install":
                    Install(false);
                    break;

                case "/installmin":
                    addTask = false;
                    Install(false);
                    break;

                case "/remove":
                    Remove(false);
                    break;

                case "/removemin":
                    removeTask = false;
                    Remove(false);
                    break;

                case "/hkuinstall":
                    HKUInstall();
                    break;

                case "/hkuinstallmin":
                    addTask = false;
                    HKUInstall();
                    break;

                case "/hkutaskonly":
                    TaskInstall(false);
                    break;

                case "/hkuremove":
                    HKURemove();
                    break;

                case "/taskinstall":
                    TaskInstall(true);
                    break;

                case "/taskremove":
                    TaskRemove(true);
                    break;

                case "/taskinstallquiet":
                    TaskInstall(false);
                    break;

                case "/taskremovequiet":
                    TaskRemove(false);
                    break;

                case "/elevate":
                    Elevate();
                    break;

                case "/cmdhere":
                    CmdHere();
                    break;

                case "/cmdadminhere":
                    RunAsAdmin(CmdExe);
                    break;

                case "/cmdtrustedhere":
                    RunAsTrusted(CmdExe);
                    break;

                case "/powershellhere":
                    PowerShellHere();
                    break;

                case "/powershelladminhere":
                    RunAsAdmin(PowerShellExe);
                    break;

                case "/powershelltrustedhere":
                    RunAsTrusted(PowerShellExe);
                    break;

                case "/powershellcorehere":
                    GetPowerShellCorePath();
                    PowerShellCoreHere();
                    break;

                case "/searchadminhere":
                    OpenSearchHelper();
                    break;

                case "/searchtrustedhere":
                    OpenSearchHelper();
                    break;

                case "/searchuserhere":
                    OpenSearchHelper();
                    break;

                case "/regedit":
                    RegEdit();
                    break;

                case "/regeditadmin":
                    CommandLine = "/m";
                    RunAsAdmin(RegEditExe);
                    break;

                case "/regedittrusted":
                    CommandLine = "/m";
                    RunAsTrusted(RegEditExe);
                    break;

                case "/allowelevatedexplorer":
                    object runAsValue = Registry.GetValue(ExpKey, "RunAs", null);
                    if (runAsValue != null && runAsValue.ToString() == "Interactive User")
                    {
                        Registry.SetValue(ExpKey, "RunAs", "", RegistryValueKind.String);
                        Thread.Sleep(5000);
                        Registry.SetValue(ExpKey, "RunAs", "Interactive User", RegistryValueKind.String);
                    }
                    break;

                case "/minifilemanager":
                    OpenFileDialog fd = new OpenFileDialog
                    {
                        Title = sFileManager,
                        Filter = "",
                        InitialDirectory = StartDirectory,
                        Multiselect = true
                    };
                    fd.ShowDialog();
                    break;

                case "/filemanagerhere":
                    FileManagerHere();
                    break;

                case "/unblockhere":
                    UnblockHere();
                    break;

                case "/unblockadmin":
                    UnblockDirectory();
                    break;

                case "/takeownhere":
                    RunTakeOwnHerePS1AsAdmin();
                    break;

                case "/adddelpathhere":
                    AddDelPathHere();
                    break;

                case "/addpathadmin":
                    AddPathAdmin();
                    break;

                case "/delpathadmin":
                    DelPathAdmin();
                    break;

                case "/showhide":
                    ShowHide();
                    break;

                case "/clearhistory":
                    ClearHistory();
                    break;

                case "/setacltempfiles":
                    RunSetACLOnTempFiles();
                    break;

                case "/clearhistoryadmin":
                    ClearDefenderHistoryTask();
                    break;

                case "/refreshshellhere":
                    RefreshShellHere();
                    break;

                case "/reseticoncache":
                    ResetCachesAndRestartExplorer(true, false);
                    break;

                case "/restartexplorerhere":
                    helpPage = "restart-explorer";
                    DialogResult result = CustomMessageBox.Show($"{sRestartExplorer}?", sMain);
                    if (result == DialogResult.Cancel) return;
                    RestartExplorer();
                    break;

                case "/searchhere":
                    SearchHere();
                    break;

                case "/folderoptionshere":
                    FolderOptionsHere();
                    break;

                case "/settings":
                    SettingsDialog.Show();
                    break;

                case "/shortcuttool":
                    ShortcutTool();
                    break;

                case "/datetimetool":
                    DateTimeTool();
                    break;

                case "/moretoolshere":
                    MoreTools();
                    break;

                case "/setup":
                    Setup();
                    break;

                default:
                    return;
            }
        }

        // Handle system theme changes and restart the application
        static void OnSystemThemeChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
        {
            // Only respond to General category changes (which includes theme changes)
            if (e.Category == Microsoft.Win32.UserPreferenceCategory.General)
            {
                bool newDarkMode = isDark();
                if (newDarkMode != Dark)
                {
                    Dark = newDarkMode;
                    themeChanged = true;
                    useOriginalPosition = true; // Use saved position on relaunch

                    // If any form is open, capture its position and close it
                    if (Application.OpenForms.Count > 0)
                    {
                        Form form = Application.OpenForms[0];
                        form.Invoke(new Action(() =>
                        {
                            savedDialogPosition = form.Location; // Save the current dialog position
                            form.Close();
                        }));
                    }
                }
            }
        }

        static void InitializeUserSettingsFile()
        {
            // Determine the user settings folder.
            // If FullyPortable=1 is set in the AppParts INI and the program folder is writable,
            // use a local "AppData" sub-folder instead of %LocalAppData%\RightClickTools.
            string appPartsIniFile = Path.Combine(appParts, $"{myName}.ini");
            bool fullyPortable = IniFileParser.ReadValue("General", "FullyPortable", "0", appPartsIniFile) == "1";
            string portableFolder = Path.Combine(myPath, "AppData");
            if (fullyPortable && !IsDirectoryWritable(myPath))
                fullyPortable = false;

            string userSettingsFolder = fullyPortable
                ? portableFolder
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RightClickTools");

            string userSettingsFile = Path.Combine(userSettingsFolder, $"{myName}.ini");
            string userMoreToolsFile = Path.Combine(userSettingsFolder, "MoreTools.ini");

            try
            {
                // Create directory if it doesn't exist
                if (!Directory.Exists(userSettingsFolder))
                {
                    Directory.CreateDirectory(userSettingsFolder);
                }

                // --- RightClickTools.ini ---
                if (!File.Exists(userSettingsFile) && File.Exists(appPartsIniFile))
                {
                    File.Copy(appPartsIniFile, userSettingsFile, false);
                }

                // Update myIniFile to point to the user's copy
                if (File.Exists(userSettingsFile))
                {
                    myIniFile = userSettingsFile;
                }

                // --- MoreTools.ini ---
                string appPartsMoreTools = Path.Combine(appParts, "MoreTools.ini");
                if (!File.Exists(userMoreToolsFile) && File.Exists(appPartsMoreTools))
                {
                    File.Copy(appPartsMoreTools, userMoreToolsFile, false);
                }
                else if (File.Exists(userMoreToolsFile) && File.Exists(appPartsMoreTools))
                {
                    int srcRev  = IniFileParser.ReadIniRevision(appPartsMoreTools);
                    int destRev = IniFileParser.ReadIniRevision(userMoreToolsFile);
                    if (srcRev > destRev)
                    {
                        IniFileParser.MergeMissingSections(appPartsMoreTools, userMoreToolsFile);
                        IniFileParser.UpdateIniRevision(userMoreToolsFile, srcRev);
                    }
                }

                // Update MoreToolsIni to point to the user's copy
                if (File.Exists(userMoreToolsFile))
                {
                    MoreToolsIni = userMoreToolsFile;
                }

                // --- Launcher.ini ---
                string userLauncherFile = Path.Combine(userSettingsFolder, "Launcher.ini");
                string appPartsLauncher = Path.Combine(appParts, "Launcher.ini");
                if (!File.Exists(userLauncherFile) && File.Exists(appPartsLauncher))
                {
                    File.Copy(appPartsLauncher, userLauncherFile, false);
                }
                else if (File.Exists(userLauncherFile) && File.Exists(appPartsLauncher))
                {
                    int srcRev  = IniFileParser.ReadIniRevision(appPartsLauncher);
                    int destRev = IniFileParser.ReadIniRevision(userLauncherFile);
                    if (srcRev > destRev)
                    {
                        IniFileParser.MergeMissingSections(appPartsLauncher, userLauncherFile);
                        IniFileParser.UpdateIniRevision(userLauncherFile, srcRev);
                    }
                }

                // Update LauncherIni to point to the user's copy
                if (File.Exists(userLauncherFile))
                {
                    LauncherIni = userLauncherFile;
                }
            }
            catch
            {
                // If copy fails, continue using original files
            }
        }

        static void GetPowerShellCorePath()
        {
            string PSPath = ReadString(myIniFile, "PowerShellCoreHere", "Exe", "");
            if (File.Exists(PSPath)) PowerShellCoreExe = PSPath;
        }

        static bool IsDirectoryWritable(string dirPath)
        {
            try
            {
                string testFile = Path.Combine(dirPath, Path.GetRandomFileName());
                using (File.Create(testFile)) { }
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        static void SetExplorerOptions(int light)
        {
            Registry.SetValue(perKey, "AppsUseLightTheme", light, RegistryValueKind.DWord);
            Registry.SetValue(perKey, "SystemUsesLightTheme", light, RegistryValueKind.DWord);
            Registry.SetValue(AdvKey, "Hidden", 1, RegistryValueKind.DWord);
            Registry.SetValue(AdvKey, "ShowSuperHidden", 1, RegistryValueKind.DWord);
            Registry.SetValue(AdvKey, "HideFileExt", 0, RegistryValueKind.DWord);
            Registry.SetValue(AdvKey, "UseCompactMode", 1, RegistryValueKind.DWord);
        }

        static void findUserSID()
        {
            string userName = "";
            try { userName = (string)Registry.GetValue(UIkey, "LastLoggedOnUser", ""); } catch { }
            if (userName == "") return;
            userName = userName.Substring(userName.LastIndexOf('\\') + 1);

            using (RegistryKey hkeyUsers = Registry.Users)
            {
                foreach (string userSid in hkeyUsers.GetSubKeyNames())
                {
                    try
                    {
                        using (RegistryKey volatileEnvKey = hkeyUsers.OpenSubKey($@"{userSid}\Volatile Environment"))
                        {
                            if (volatileEnvKey != null)
                            {
                                object usernameValue = volatileEnvKey.GetValue("USERNAME");
                                if (usernameValue != null && usernameValue.ToString().Equals(userName, StringComparison.OrdinalIgnoreCase))
                                {
                                    userSID = userSid;
                                    return;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            userSID = "";
        }

        static void HandleProtocolActivation(string protocolUri)
        {
            try
            {
                // Parse the protocol URI: rightclicktools:command?path=<encoded_path>
                // Example: rightclicktools:ShowHide?path=C%3A%5CUsers%5CFerch%5CDesktop

                string uri = protocolUri.Substring("rightclicktools:".Length);

                int questionMarkIndex = uri.IndexOf('?');
                if (questionMarkIndex == -1)
                {
                    // No parameters, just command
                    Option = "/" + uri;
                    StartDirectory = "";
                }
                else
                {
                    Option = "/" + uri.Substring(0, questionMarkIndex);

                    // Parse query string
                    string queryString = uri.Substring(questionMarkIndex + 1);
                    string[] parameters = queryString.Split('&');

                    foreach (string param in parameters)
                    {
                        if (param.StartsWith("path=", StringComparison.OrdinalIgnoreCase))
                        {
                            string encodedPath = param.Substring(5);
                            // URL decode the path
                            StartDirectory = Uri.UnescapeDataString(encodedPath);
                            StartDirectory = StartDirectory.Replace("|", "");
                            break;
                        }
                    }
                }

                // Now execute the command using the existing switch statement
                ExecuteCommand();
            }
            catch
            {
                // Silently ignore protocol parsing errors
            }
        }

        static bool IsCurrentUserInAdminGroup()
        {
            var claims = new WindowsPrincipal(WindowsIdentity.GetCurrent()).Claims;
            var adminClaimID = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Value;
            return claims.Any(c => c.Value == adminClaimID);
        }

        static bool TaskExists()
        {
            Process process = new Process();
            process.StartInfo.FileName = "schtasks.exe";
            process.StartInfo.Arguments = $"/query /tn {TaskName}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }

        static void RunUAC(string fileName)
        {
            Process p = new Process();
            p.StartInfo.FileName = fileName;
            p.StartInfo.Arguments = CommandLine;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Verb = "runas";
            p.Start();
            p.WaitForExit();
        }

        // Returns the effective DialogResult based on RunAs setting and Ctrl key:
        //   OK=run as user, Yes=run as admin, No=run as TrustedInstaller, Cancel=cancelled.
        //   RunAs=0 (or not set): run as user without a dialog.
        //   RunAs=1: run as admin without a dialog.
        //   RunAs=2: run as TrustedInstaller without a dialog.
        //   RunAs=3 or Ctrl held: prompt the user.
        static DialogResult GetRunAsResult(string section, string title)
        {
            int runAs = 0;
            int.TryParse(ReadString(myIniFile, section, "RunAs", "0"), out runAs);

            if (runAs == 3 || ctrlKey)
            {
                bwidth = 120;
                return ThreeChoiceBox.Show(title, sMain, sUser, sAdministrator, sTrustedInstaller, !NoTrustedInstaller, topmost: true);
            }

            if (runAs == 1) return DialogResult.Yes;
            if (runAs == 2) return NoTrustedInstaller ? DialogResult.Yes : DialogResult.No;
            return DialogResult.OK;
        }

        // Like GetRunAsResult, but executes the action from inside the ThreeChoiceBox dialog
        // (while the form is still foreground) so that GUI apps launched as User can take the foreground.
        static void RunFromDialog(string section, string title, Action<DialogResult> execute)
        {
            int runAs = 0;
            int.TryParse(ReadString(myIniFile, section, "RunAs", "0"), out runAs);

            if (runAs == 3 || ctrlKey)
            {
                bwidth = 120;
                ThreeChoiceBox.Show(title, sMain, sUser, sAdministrator, sTrustedInstaller, !NoTrustedInstaller, topmost: true, onResult: execute);
                return;
            }

            // Non-dialog cases: execute directly (these already open on top without a dialog).
            if (runAs == 1) execute(DialogResult.Yes);
            else if (runAs == 2) execute(NoTrustedInstaller ? DialogResult.Yes : DialogResult.No);
            else execute(DialogResult.OK);
        }

        static void CmdHere()
        {
            helpPage = "commands";

            // Check for Windows Terminal admin option
            int wtAdmin = 0;
            int.TryParse(ReadString(myIniFile, "CmdHere", "WTadmin", "0"), out wtAdmin);

            DialogResult result = GetRunAsResult("CmdHere", sCmdHere);
            if (result == DialogResult.Cancel) return;

            useWindowsTerminal = false;
            originalExe = "";

            if (result == DialogResult.OK) 
            {
                RunAsUser(CmdExe);
            }
            if (result == DialogResult.Yes) 
            {
                useWindowsTerminal = (wtAdmin == 1 && IsWindowsTerminalInstalled());
                originalExe = CmdExe;
                RunAsAdmin(CmdExe);
            }
            if (result == DialogResult.No) 
            {
                // Don't use Windows Terminal for TrustedInstaller - it's not supported
                RunAsTrusted(CmdExe);
            }

            useWindowsTerminal = false;
            originalExe = "";
        }

        static void PowerShellHere()
        {
            helpPage = "commands";

            // Check for Windows Terminal admin option
            int wtAdmin = 0;
            int.TryParse(ReadString(myIniFile, "PowerShellHere", "WTadmin", "0"), out wtAdmin);

            DialogResult result = GetRunAsResult("PowerShellHere", sPowerShellHere);
            if (result == DialogResult.Cancel) return;

            useWindowsTerminal = false;
            originalPowerShellExe = "";
            originalExe = "";

            if (result == DialogResult.OK) 
            {
                RunAsUser(PowerShellExe);
            }
            if (result == DialogResult.Yes) 
            {
                useWindowsTerminal = (wtAdmin == 1 && IsWindowsTerminalInstalled());
                originalPowerShellExe = PowerShellExe;
                originalExe = PowerShellExe;
                RunAsAdmin(PowerShellExe);
            }
            if (result == DialogResult.No) 
            {
                // Don't use Windows Terminal for TrustedInstaller - it's not supported
                RunAsTrusted(PowerShellExe);
            }

            useWindowsTerminal = false;
            originalPowerShellExe = "";
            originalExe = "";
        }
        static void PowerShellCoreHere()
        {
            helpPage = "commands";

            // Check if PowerShell Core is installed
            if (!IsPowerShellCoreInstalled())
            {
                CustomMessageBox.Show(sPowerShellCoreNotInstalled, sMain);
                return;
            }

            // Check for Windows Terminal admin option
            int wtAdmin = 0;
            int.TryParse(ReadString(myIniFile, "PowerShellCoreHere", "WTadmin", "0"), out wtAdmin);

            DialogResult result = GetRunAsResult("PowerShellCoreHere", sPowerShellCoreHere);
            if (result == DialogResult.Cancel) return;

            useWindowsTerminal = false;
            originalPowerShellExe = "";
            originalExe = "";

            if (result == DialogResult.OK) 
            {
                RunAsUser(PowerShellCoreExe);
            }
            if (result == DialogResult.Yes) 
            {
                useWindowsTerminal = (wtAdmin == 1 && IsWindowsTerminalInstalled());
                originalPowerShellExe = PowerShellCoreExe;
                originalExe = PowerShellCoreExe;
                RunAsAdmin(PowerShellCoreExe);
            }
            if (result == DialogResult.No) 
            {
                // Don't use Windows Terminal for TrustedInstaller - it's not supported
                RunAsTrusted(PowerShellCoreExe);
            }

            useWindowsTerminal = false;
            originalPowerShellExe = "";
            originalExe = "";
        }
        static void RegEdit()
        {
            helpPage = "regedit";

            bool shiftKey = (GetAsyncKeyState(0x10) & 0x8000) != 0; // Detect if Shift key is pressed

            RunFromDialog("RegEdit", sRegEdit, (result) =>
            {
                if (result == DialogResult.OK)
                {
                    Environment.SetEnvironmentVariable("__COMPAT_LAYER", "RUNASINVOKER");
                    if (shiftKey) clearRegEdit();
                    CommandLine = "/m";
                    RunAsUser(RegEditExe);
                }

                if (result == DialogResult.Yes)
                {
                    if (shiftKey) clearRegEdit();
                    CommandLine = "/m";
                    RunAsAdmin(RegEditExe);
                }

                if (result == DialogResult.No)
                {
                    CommandLine = "/m";
                    RunAsTrusted(RegEditExe);
                }
            });
        }
        static void FileManagerHere()
        {
            helpPage = "file-manager-here";

            RunFromDialog("FileManagerHere", sFileManagerHere, (result) =>
            {
                CommandLine = $"\"{StartDirectory}\"";

                string FMExe = ReadString(myIniFile, "FileManagerHere", "Exe", "");

                // Set file manager to Explorer if no valid third-party file manager is set
                if (FMExe == "" || $@"\{FMExe}".ToLower().EndsWith("\\explorer.exe") || !File.Exists(FMExe))
                {
                    if (Win11 && result == DialogResult.No)
                    {
                        CommandLine = $"/MiniFileManager \"{StartDirectory}\"";
                        FMExe = myExe;
                    }
                    else
                    {
                        FMExe = "explorer.exe";

                        if (result == DialogResult.OK)
                        {
                            RunAsUser(FMExe);
                            return;
                        }

                        // Check registry value that prevents Explorer to run elevated
                        object runAsValue = Registry.GetValue(ExpKey, "RunAs", null);
                        if (runAsValue != null && runAsValue.ToString() == "Interactive User")
                        {
                            if (isFullAdmin)
                            {
                                // Temporarily allow Explorer to run elevated
                                CommandLine = "/AllowElevatedExplorer";
                                RunAsTrusted(myExe);
                                // Wait for registry entry to be updated
                                for (int i = 0; i < 100; i++)
                                {
                                    Thread.Sleep(20);
                                    runAsValue = Registry.GetValue(ExpKey, "RunAs", null);
                                    if (runAsValue == null || runAsValue.ToString() != "Interactive User") break;
                                }
                                CommandLine = $"\"{StartDirectory}\"";
                            }
                            else
                            {
                                CommandLine = $"/MiniFileManager \"{StartDirectory}\"";
                                FMExe = myExe;
                            }
                        }
                    }
                }

                if (result == DialogResult.OK) RunAsUser(FMExe);
                if (result == DialogResult.Yes) RunAsAdmin(FMExe);
                if (result == DialogResult.No) RunAsTrusted(FMExe);
            });
        }

        static void AddPathAdmin()
        {
            char[] trimThis = { '\\' };
            string path = StartDirectory.Trim(trimThis);
            InUserPath = IsPathInEnvironmentVariable(path, UserPath);
            InSystemPath = IsPathInEnvironmentVariable(path, SystemPath);
            AddPathToEnvironmentVariable(path, SystemPath, SystemKey, false);
        }

        static void DelPathAdmin()
        {
            char[] trimThis = { '\\' };
            string path = StartDirectory.Trim(trimThis);
            InUserPath = IsPathInEnvironmentVariable(path, UserPath);
            InSystemPath = IsPathInEnvironmentVariable(path, SystemPath);
            RemovePathFromEnvironmentVariable(path, SystemPath, SystemKey, false);
        }

        static void ClearHistory()
        {
            DialogResult result = ClearHistoryDialog.Show(sClearHistory, sMain);

            if (result == DialogResult.Cancel) return;

            if (RecentItemsCheckbox.Checked)
            {
                string Recent = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
                try
                {
                    Directory.GetFiles(Recent, "*", SearchOption.TopDirectoryOnly).ToList().ForEach(File.Delete);
                    Directory.GetFiles($@"{Recent}\AutomaticDestinations", "*", SearchOption.TopDirectoryOnly).ToList().ForEach(File.Delete);
                    Directory.GetFiles($@"{Recent}\CustomDestinations", "*", SearchOption.TopDirectoryOnly).ToList().ForEach(File.Delete);
                }
                catch
                {
                }
            }

            if (AutoSuggestCheckbox.Checked)
            {
                string parentKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer";
                ClearRegValues($@"{parentKey}\RunMRU");
                ClearRegValues($@"{parentKey}\TypedPaths");

                Process p = new Process();
                p.StartInfo.FileName = "Rundll32.exe";
                p.StartInfo.Arguments = "InetCpl.cpl,ClearMyTracksByProcess 1";
                p.StartInfo.WorkingDirectory = @"C:\";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
            }

            if (TempFilesCheckbox.Checked)
            {
                DeleteTempFiles();

                if (isFullAdmin)
                {
                    string cfg = $"[Process]\r\nEXEFilename={myExe}\r\nCommandLine=/setacltempfiles\r\nRunAs=Administrator";
                    File.WriteAllText(ElevateCfg, cfg);

                    Process task = new Process();
                    task.StartInfo.FileName = SchTasksExe;
                    task.StartInfo.Arguments = $"/run /tn \"{TaskName}\"";
                    task.StartInfo.UseShellExecute = false;
                    task.StartInfo.CreateNoWindow = true;
                    task.Start();
                    task.WaitForExit();
                }
            }

            if (RecycleBinCheckbox.Checked)
            {
                // SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND
                SHEmptyRecycleBin(IntPtr.Zero, null, 0x00000001 | 0x00000002 | 0x00000004);
            }

            if (DefenderCheckbox.Checked)
            {
                ClearDefenderHistory();
            }

            if (SpecifiedFoldersCheckbox.Checked)
            {
                ClearSpecifiedFolders();
            }
        }

        static void DeleteTempFiles()
        {
            var filesAndFolders = Directory.GetFileSystemEntries(TempPath, "*", SearchOption.TopDirectoryOnly);
            foreach (var entry in filesAndFolders)
            {
                try
                {
                    if (File.Exists(entry))
                        File.Delete(entry);
                    else if (Directory.Exists(entry))
                        Directory.Delete(entry, true);
                }
                catch
                {
                }
            }
        }

        static void ClearSpecifiedFolders()
        {
            string cleanupFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RightClickTools", "Cleanup.txt");

            if (!File.Exists(cleanupFile)) return;

            string[] lines = File.ReadAllLines(cleanupFile);
            foreach (string line in lines)
            {
                string path = line.TrimEnd();
                if (string.IsNullOrEmpty(path)) continue;

                bool removeFolder = path.EndsWith("\\");
                string folderPath = removeFolder ? path.TrimEnd('\\') : path;

                if (!Directory.Exists(folderPath)) continue;

                if (removeFolder)
                {
                    RunRD(folderPath);
                }
                else
                {
                    // Hold the folder open as our CWD so rd cannot delete it,
                    // but will still delete everything inside it.
                    string savedDirectory = Directory.GetCurrentDirectory();
                    try
                    {
                        Directory.SetCurrentDirectory(folderPath);
                        RunRD(folderPath);
                    }
                    finally
                    {
                        Directory.SetCurrentDirectory(savedDirectory);
                    }
                }
            }
        }

        static void RunRD(string folderPath)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = CmdExe;
                p.StartInfo.Arguments = $"/c rd /s /q \"{folderPath}\"";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit();
            }
            catch { }
        }

        static void RunSetACLOnTempFiles()
        {
            string setACL = $@"{appParts}\{bitPath}\SetACL.exe";
            string userName = WindowsIdentity.GetCurrent().Name;
            string tempPathArg = TempPath.TrimEnd('\\');

            Process setACLOwner = new Process();
            setACLOwner.StartInfo.FileName = setACL;
            setACLOwner.StartInfo.Arguments = $"-on \"{tempPathArg}\" -ot file -actn setowner -ownr \"n:{userName}\" -rec cont_obj";
            setACLOwner.StartInfo.UseShellExecute = false;
            setACLOwner.StartInfo.CreateNoWindow = true;
            setACLOwner.Start();
            setACLOwner.WaitForExit();

            Process setACLPerm = new Process();
            setACLPerm.StartInfo.FileName = setACL;
            setACLPerm.StartInfo.Arguments = $"-on \"{tempPathArg}\" -ot file -actn setprot -op \"dacl:np;sacl:np\" -rec cont_obj";
            setACLPerm.StartInfo.UseShellExecute = false;
            setACLPerm.StartInfo.CreateNoWindow = true;
            setACLPerm.Start();
            setACLPerm.WaitForExit();

            // Delete temp files now that ownership/permissions are fixed
            var filesAndFolders = Directory.GetFileSystemEntries(tempPathArg, "*", SearchOption.TopDirectoryOnly);
            foreach (var entry in filesAndFolders)
            {
                try
                {
                    if (File.Exists(entry))
                        File.Delete(entry);
                    else if (Directory.Exists(entry))
                        Directory.Delete(entry, true);
                }
                catch
                {
                }
            }
        }

        static void ClearRegValues(string keyPath)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
                {
                    if (key != null)
                    {
                        foreach (string valueName in key.GetValueNames())
                        {
                            key.DeleteValue(valueName);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        static void AddDelPathHere()
        {
            string path = StartDirectory;

            InUserPath = IsPathInEnvironmentVariable(path, UserPath);
            InSystemPath = IsPathInEnvironmentVariable(path, SystemPath);

            if (path.EndsWith(":")) path += "\\";

            DialogResult result = AddDelPathDialog.Show(path, sMain);

            if (result == DialogResult.Cancel) return;

            if (userPathCheckbox.Checked != InUserPath)
            {
                if (userPathCheckbox.Checked)
                    AddPathToEnvironmentVariable(path, UserPath, UserKey, true);
                else
                    RemovePathFromEnvironmentVariable(path, UserPath, UserKey, true);
            }

            if (systemPathCheckbox.Checked != InSystemPath)
            {

                if (systemPathCheckbox.Checked)
                {
                    CommandLine = "/AddPathAdmin";
                }
                else
                {
                    CommandLine = "/DelPathAdmin";
                }
                RunElevated(myExe, "Administrator");
            }
        }

        static bool IsPathInEnvironmentVariable(string pathToCheck, string environmentVariable)
        {
            string[] paths = environmentVariable.Split(';');
            char[] trimThis = { '\\' };
            foreach (string p in paths)
            {
                if (string.Equals(p.Trim(trimThis), pathToCheck, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        static void AddPathToEnvironmentVariable(string pathToAdd, string environmentVariable, string Key, bool User)
        {
            if (User && InUserPath) return;
            if (!User && InSystemPath) return;

            if ((pathLength + pathToAdd.Length) > 4095) return;

            string newPath = $"{environmentVariable};{pathToAdd}";
            newPath = newPath.Replace(";;", ";");
            Registry.SetValue(Key, "Path", newPath, RegistryValueKind.ExpandString);
        }

        static void RemovePathFromEnvironmentVariable(string pathToRemove, string environmentVariable, string Key, bool User)
        {
            if (User && !InUserPath) return;
            if (!User && !InSystemPath) return;

            string[] paths = environmentVariable.Split(';');
            char[] trimThis = { '\\' };
            pathToRemove = pathToRemove.Trim(trimThis);
            string newPath = "";
            foreach (string p in paths)
            {
                if (!string.Equals(p.Trim(trimThis), pathToRemove, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        newPath += ";";
                    }
                    newPath += p;
                }
            }
            newPath = newPath.Replace(";;", ";");
            Registry.SetValue(Key, "Path", newPath, RegistryValueKind.ExpandString);
        }

        static void ShowHide()
        {
            Hidden = !Hidden;
            ToggleHiddenFiles(Hidden);
        }

        static void RefreshShell()
        {
            SHChangeNotify(0x08000000, 0x1000, IntPtr.Zero, IntPtr.Zero);

            // SHChangeNotify alone doesn't reliably refresh Explorer on Win10 RS1+;
            // toggling the hidden-files state forces Explorer to redraw folder views.
            if (buildNumber >= 14393)
            {
                ToggleHiddenFiles(!Hidden);
                ToggleHiddenFiles(Hidden);
            }
        }

        static void RefreshShellHere()
        {
            DialogResult result = ShellRefreshDialog.Show("", sMain);

            if (result == DialogResult.Cancel) return;

            RefreshShell();

            if (iconCacheCheckbox.Checked || thumbCacheCheckbox.Checked)
            {
                ResetCachesAndRestartExplorer(iconCacheCheckbox.Checked, thumbCacheCheckbox.Checked);
            }
        }

        static void ResetCachesAndRestartExplorer(bool resetIconCache, bool resetThumbnailCache)
        {
            if (StartDirectory.ToLower().EndsWith("\\desktop"))
            {
                if (!DesktopWindowFound()) StartDirectory = "";
            }

            using (Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo
                {
                    FileName = "taskkill.exe",
                    Arguments = "/f /im explorer.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                };
                p.Start();
                p.WaitForExit();
            }

            Thread.Sleep(2000);

            if (resetIconCache) DeleteCacheFiles("iconcache_*.db");

            if (resetThumbnailCache) DeleteCacheFiles("thumbcache_*.db");

            Process.Start("explorer.exe");
            if (StartDirectory != "") Process.Start("explorer.exe", StartDirectory);
        }

        static void DeleteCacheFiles(string searchPattern)
        {
            string targetDirectory = $@"{Environment.GetEnvironmentVariable("LocalAppData")}\Microsoft\Windows\Explorer";

            try
            {
                string[] files = Directory.GetFiles(targetDirectory, searchPattern, SearchOption.TopDirectoryOnly);

                foreach (string file in files)
                {
                    try { File.Delete(file); }
                    catch { }
                }
            }
            catch { }
        }

        static void ToggleHiddenFiles(bool bShow)
        {
            if (buildNumber >= 14393)
            {
                Structures.SHELLSTATE state = new Structures.SHELLSTATE();
                state.FShowAllObjects = (uint)(bShow ? 1 : 2);
                state.FShowSuperHidden = (uint)(bShow ? 1 : 0);
                SHGetSetSettings(ref state, Structures.SSF.SSF_SHOWALLOBJECTS | Structures.SSF.SSF_SHOWSUPERHIDDEN, true);
            }
            else
            {
                // For Windows 7/8 and early Windows 10: use registry
                // Hidden: 1 = show hidden files, 2 = do not show
                // ShowSuperHidden: 1 = show protected OS files, 0 = do not show
                int h1 = bShow ? 1 : 2;
                int h2 = bShow ? 1 : 0;
                Registry.SetValue(AdvKey, "Hidden", h1, RegistryValueKind.DWord);
                Registry.SetValue(AdvKey, "ShowSuperHidden", h2, RegistryValueKind.DWord);
                SHChangeNotify(0x08000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
                Thread.Sleep(100);
                SendKeys.SendWait("{F5}");
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHGetSetFolderCustomSettings(ref SHFOLDERCUSTOMSETTINGS pfcs, string pszPath, uint dwReadWrite);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFOLDERCUSTOMSETTINGS
        {
            public uint dwSize;
            public uint dwMask;
            public IntPtr pvid;
            public string pszWebViewTemplate;
            public uint cchWebViewTemplate;
            public string pszWebViewTemplateVersion;
            public string pszInfoTip;
            public uint cchInfoTip;
            public IntPtr pclsid;
            public uint dwFlags;
            public string pszIconFile;
            public uint cchIconFile;
            public int iIconIndex;
            public string pszLogo;
            public uint cchLogo;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static void SHGetSetSettings(ref Structures.SHELLSTATE lpss, Structures.SSF dwMask, bool bSet);

        internal static class Structures
        {
            [Flags]
            public enum SSF : int
            {
                SSF_SHOWALLOBJECTS = 0x00000001,
                SSF_SHOWSUPERHIDDEN = 0x00040000,
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct SHELLSTATE
            {
                public uint bitvector;

                public uint FShowAllObjects
                {
                    get => this.bitvector & 1;
                    set => this.bitvector = value | this.bitvector;
                }

                public uint FShowSuperHidden
                {
                    get => (this.bitvector & 0x8000) / 0x8000;
                    set => this.bitvector = (value * 0x8000) | this.bitvector;
                }
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool AllowSetForegroundWindow(uint dwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(System.Drawing.Point pt, uint dwFlags);

        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr hMonitor, int dpiType, out uint dpiX, out uint dpiY);

        [DllImport("user32.dll")]
        private static extern IntPtr SetThreadDpiAwarenessContext(IntPtr dpiContext);

        static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);

        // Grant any process the right to bring itself to the foreground.
        // Must be called while our process still holds the foreground lock (e.g. from FormClosing).
        static void GrantForegroundRights()
        {
            const uint ASFW_ANY = 0xFFFFFFFF;
            AllowSetForegroundWindow(ASFW_ANY);
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        static bool DesktopWindowFound()
        {
            bool desktopFound = false;
            IntPtr hwnd = IntPtr.Zero;
            do
            {
                hwnd = FindWindowEx(IntPtr.Zero, hwnd, "CabinetWClass", null);

                if (hwnd != IntPtr.Zero)
                {
                    StringBuilder windowTitle = new StringBuilder(256);
                    GetWindowText(hwnd, windowTitle, windowTitle.Capacity);
                    string t = windowTitle.ToString().ToLower();
                    if (t == "desktop" || t == "desktop - file explorer")
                    {
                        desktopFound = true;
                        break;
                    }
                }
            }
            while (hwnd != IntPtr.Zero);
            return desktopFound;
        }

        static void RestartExplorer()
        {
            if (StartDirectory.ToLower().EndsWith("\\desktop"))
            {
                if (!DesktopWindowFound()) StartDirectory = "";
            }

            RefreshShell();

            var processes = Process.GetProcessesByName("explorer");
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                }
                catch { }
            }
            if (StartDirectory != "") Process.Start("explorer.exe", StartDirectory);
        }

        static void CreateChangeDirectoryFile(string EXEFilename)
        {
            if (EXEFilename == CmdExe)
            {
                string cdFile = $@"{TempPath}ChangeDirectory.cmd";
                StartDirectory = StartDirectory.Replace("%", "%%"); //Escape percent signs
                string Data = $"@echo off\r\nchcp 65001>nul\r\ncd /d \"{StartDirectory}\"";
                Data += "\r\nstart /b \"\" cmd /c del \"%~f0\"";
                File.WriteAllText(cdFile, Data);
                CommandLine = $"/k \"{cdFile}\"";
            }

            if (EXEFilename == PowerShellExe || EXEFilename == PowerShellCoreExe)
            {
                string cdFile = $@"{TempPath}ChangeDirectory.ps1";
                StartDirectory = StartDirectory.Replace("'", "''"); //Escape single quotes
                string Data = $@"Set-Location -LiteralPath '{StartDirectory}'";
                if (StartDirectory.Contains("~")) Data += "\r\nfunction Prompt {$shortPath = (New-Object -ComObject Scripting.FileSystemObject).GetFolder($pwd).ShortPath; return \"PS $($shortPath)> \"}";
                Data += "\r\nStart-Sleep -Milliseconds 100; Remove-Item $MyInvocation.MyCommand.Path -Force\r\n"; //Delete itself when done
                File.WriteAllText(cdFile, Data, Encoding.UTF8); //UTF-8 with BOM
                CommandLine = $"-NoLogo -NoExit -NoProfile -ExecutionPolicy Bypass -file \"{cdFile}\"";
            }
        }

        static void CreateTakeOwnHerePS1()
        {
            StartDirectory = StartDirectory.Replace("'", "''"); //Escape single quotes
            string PS1Data = $"$SetACL = '{appParts.Replace("'", "''")}\\{bitPath}\\SetACL.exe'\r\n";
            PS1Data += "$UserName = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name\r\n";
            PS1Data += $"& $SetACL -on '{StartDirectory}' -ot file -actn setowner -ownr \"n:$UserName\" -rec cont_obj\r\n";
            PS1Data += $"& $SetACL -on '{StartDirectory}' -ot file -actn setprot -op \"dacl:np;sacl:np\" -rec cont_obj\r\n";
            PS1Data += "Start-Sleep -Milliseconds 100; Remove-Item $MyInvocation.MyCommand.Path -Force\r\n"; //Delete itself when done
            string PS1File = $@"{TempPath}TakeOwn.ps1";
            File.WriteAllText(PS1File, PS1Data, Encoding.UTF8);
            ctrlKey = (GetAsyncKeyState(0x11) & 0x8000) != 0;
            string NoExit = ""; if (ctrlKey) NoExit = "-NoExit";
            CommandLine = $"{NoExit} -NoLogo -NoProfile -ExecutionPolicy Bypass -file \"{PS1File}\"";
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteFile(string name);
        public static void UnblockPath(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string file in files)
            {
                UnblockFile(file);
            }
            foreach (string dir in dirs)
            {
                UnblockPath(dir);
            }
        }
        public static bool UnblockFile(string fileName)
        {
            return DeleteFile(fileName + ":Zone.Identifier");
        }

        static void RunAsUser(string EXEFilename)
        {
            CreateChangeDirectoryFile(EXEFilename);

            Process p = new Process();
            p.StartInfo.FileName = EXEFilename;
            p.StartInfo.Arguments = CommandLine;
            p.StartInfo.WorkingDirectory = @"C:\";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.Start();
        }

        static void RunAsAdmin(string EXEFilename)
        {
            RunElevated(EXEFilename, "Administrator");
        }

        static void RunAsTrusted(string EXEFilename)
        {
            RunElevated(EXEFilename, "TrustedInstaller");
        }

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetShortPathName(string path, StringBuilder shortPath, int shortPathLength);

        static string GetShortPath(string longPath)
        {
            StringBuilder shortPath = new StringBuilder(260);
            int result = GetShortPathName(longPath, shortPath, shortPath.Capacity);
            if (result == 0)
            {
                return longPath; // Return original path if conversion fails
            }
            return shortPath.ToString();
        }

        static void clearRegEdit()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit", true))
                {
                    key?.DeleteValue("LastKey", false);
                }
            }
            catch
            {
            }
        }

        static void Elevate()
        {
            string iniFile = ElevateCfg;

            string EXEFilename = ReadString(iniFile, "Process", "EXEFilename", "");
            string CommandLine = ReadString(iniFile, "Process", "CommandLine", "");
            string RunAs = ReadString(iniFile, "Process", "RunAs", "");
            string Dark = ReadString(iniFile, "Process", "Dark", "false");
            bool dark = Dark == "True";

            File.Delete(ElevateCfg);

            if (RunAs == "TrustedInstaller")
            {
                ServiceController sc = new ServiceController
                {
                    ServiceName = "TrustedInstaller",
                };

                if (sc.Status != ServiceControllerStatus.Running) sc.Start();

                Process[] proc = Process.GetProcessesByName("TrustedInstaller");

                if (dark) TrustedInstaller.Run(proc[0].Id, $"{myExe} /Dark");
                if (!dark) TrustedInstaller.Run(proc[0].Id, $"{myExe} /Light");

                Thread.Sleep(100);
                proc = Process.GetProcessesByName("TrustedInstaller" +
                    "");
                TrustedInstaller.Run(proc[0].Id, $"{EXEFilename} {CommandLine}");
            }
            else
            {
                Process p = new Process();
                p.StartInfo.FileName = EXEFilename;
                p.StartInfo.Arguments = CommandLine;
                p.StartInfo.WorkingDirectory = @"C:\";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = false;
                p.Start();
            }
        }

        static void RunElevated(string EXEFilename, string mode)
        {
            CreateChangeDirectoryFile(EXEFilename);

            string actualExe = EXEFilename;
            string actualCmd = CommandLine;

            // Check if we should use Windows Terminal
            if (useWindowsTerminal && !string.IsNullOrEmpty(originalExe))
            {
                string exeName;

                // Determine the executable name for Windows Terminal
                if (originalExe == PowerShellExe)
                {
                    exeName = "powershell.exe";
                }
                else if (originalExe == PowerShellCoreExe || originalPowerShellExe == PowerShellCoreExe)
                {
                    exeName = "pwsh.exe";
                }
                else if (originalExe == CmdExe)
                {
                    exeName = "cmd.exe";
                }
                else
                {
                    // Fallback for unknown executables
                    exeName = Path.GetFileName(originalExe);
                }

                // Wrap command for Windows Terminal
                actualExe = "wt.exe";
                actualCmd = $"-w 0 {exeName} {CommandLine}";
            }

            string cfg = $"[Process]\r\nEXEFilename={actualExe}\r\nCommandLine={actualCmd}\r\nRunAs={mode}\r\nDark={Dark}";

            File.WriteAllText(ElevateCfg, cfg);

            if (isFullAdmin)
            {
                Process p = new Process();
                p.StartInfo.FileName = SchTasksExe;
                p.StartInfo.Arguments = $"/run /tn \"{TaskName}\"";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
            }
            else
            {
                if (mode == "Administrator")
                {
                    // For UAC elevation with Windows Terminal
                    if (useWindowsTerminal && !string.IsNullOrEmpty(originalExe))
                    {
                        string exeName;

                        // Determine the executable name for Windows Terminal
                        if (originalExe == PowerShellExe)
                        {
                            exeName = "powershell.exe";
                        }
                        else if (originalExe == PowerShellCoreExe || originalPowerShellExe == PowerShellCoreExe)
                        {
                            exeName = "pwsh.exe";
                        }
                        else if (originalExe == CmdExe)
                        {
                            exeName = "cmd.exe";
                        }
                        else
                        {
                            // Fallback for unknown executables
                            exeName = Path.GetFileName(originalExe);
                        }

                        Process p = new Process();
                        p.StartInfo.FileName = "wt.exe";
                        p.StartInfo.Arguments = $"-w 0 {exeName} {CommandLine}";
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.Verb = "runas";
                        p.Start();
                        p.WaitForExit();
                    }
                    else
                    {
                        RunUAC(EXEFilename);
                    }
                }
                else
                {
                    Process p = new Process();
                    p.StartInfo.FileName = myExe;
                    p.StartInfo.Arguments = $"/Elevate \"{ElevateCfg}\"";
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.Verb = "runas";
                    p.Start();
                }
            }
        }

        static void UnblockHere()
        {
            helpPage = "unblock-files-here";

            DialogResult result = UnblockHereDialog.Show($"{sUnblockHere}?\n\n{StartDirectory}\n\n", sMain);

            if (result == DialogResult.Cancel || Stop) return;

            if (checkboxUnblockAdmin.Checked)
            {
                CommandLine = "/UnblockAdmin";

                string cfg = $"[Process]\r\nEXEFilename={myExe}\r\nCommandLine={CommandLine}\r\nRunAs=Administrator";

                File.WriteAllText(ElevateCfg, cfg);

                if (isFullAdmin)
                {
                    Process p = new Process();
                    p.StartInfo.FileName = SchTasksExe;
                    p.StartInfo.Arguments = $"/run /tn \"{TaskName}\"";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                }
                else
                {
                    RunUAC(myExe);
                }
            }
            else
            {
                UnblockDirectory();
            }
        }

        static void UnblockDirectory()
        {
            try
            {
                UnblockPath(StartDirectory);
                CustomMessageBox.Show(sDone, sMain);
            }
            catch (UnauthorizedAccessException)
            {
                CustomMessageBox.Show(sAccessDenied, sMain);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"{sError} {ex.Message}", sMain);
            }
        }

        static void SearchHere()
        {
            helpPage = "search-here";

            RunFromDialog("SearchHere", sSearchHelper, (runAsResult) =>
            {
                // Check for custom search program in INI file
                string searchExe = ReadString(myIniFile, "SearchHere", "Exe", "");

                if (searchExe != "" && File.Exists(searchExe))
                {
                    // Custom search program specified - launch it with RunAs support
                    string cmdLine = ReadString(myIniFile, "SearchHere", "CmdLine", "");

                    // Replace %V with StartDirectory path
                    if (cmdLine.Contains("%V"))
                    {
                        cmdLine = cmdLine.Replace("%V", StartDirectory);
                    }

                    CommandLine = cmdLine;
                    if (runAsResult == DialogResult.OK) RunAsUser(searchExe);
                    if (runAsResult == DialogResult.Yes) RunAsAdmin(searchExe);
                    if (runAsResult == DialogResult.No) RunAsTrusted(searchExe);
                }
                else
                {
                    // No custom search program - launch Search Helper dialog with RunAs support
                    if (runAsResult == DialogResult.OK)
                    {
                        CommandLine = "/searchuserhere";
                        RunAsUser(myExe);
                        return;
                    }
                    if (runAsResult == DialogResult.Yes)
                    {
                        CommandLine = "/searchadminhere";
                        RunAsAdmin(myExe);
                        return;
                    }
                    if (runAsResult == DialogResult.No)
                    {
                        CommandLine = "/searchtrustedhere";
                        RunAsTrusted(myExe);
                        return;
                    }
                }
            });
        }

        static void OpenSearchHelper()
        {
            DialogResult result = SearchHelperDialog.Show(sSearchHelper, sMain);

            if (result == DialogResult.Cancel) return;
        }

        static void FolderOptionsHere()
        {
            helpPage = "folder-options-here";

            needsIconCacheReset = false;

            DialogResult result = FolderOptionsDialog.Show(sFolderOptions, sMain);

            if (result == DialogResult.Cancel) return;

            RefreshShell();

            if (needsIconCacheReset)
            {
                Thread.Sleep(500);
                ResetCachesAndRestartExplorer(true, false);
                needsIconCacheReset = false;
            }
        }

        static void ShortcutTool()
        {
            helpPage = "shortcut-tool";

            ShortcutToolDialog dialog = ShortcutToolDialog.Show(sShortcutTool, sMain);

            if (dialog.DialogResult == DialogResult.Cancel)
            {
                dialog.Dispose();
                return;
            }

            if (dialog.ConvertUrlToLnk)
            {
                ConvertUrlToLnkInDirectory(StartDirectory, dialog.ApplyToSubfolders, dialog.MoveUrlToRecycleBin);
                CustomMessageBox.Show(sDone, sMain);
            }

            if (!string.IsNullOrEmpty(dialog.SearchText) && (dialog.SearchTarget || dialog.SearchStartIn || dialog.SearchIcon))
            {
                SearchAndReplaceInLnkFiles(StartDirectory, dialog.SearchText, dialog.ReplaceText, 
                    dialog.SearchTarget, dialog.SearchStartIn, dialog.SearchIcon, dialog.ApplyToSubfolders);
                CustomMessageBox.Show(sDone, sMain);
            }

            dialog.Dispose();
        }

        static void DateTimeTool()
        {
            helpPage = "date-time-tool";

            DateTimeToolDialog dialog = DateTimeToolDialog.Show(sDateTimeTool, sMain);

            if (dialog.DialogResult == DialogResult.Cancel)
            {
                dialog.Dispose();
                return;
            }

            SearchOption searchOption = dialog.ApplyToSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            switch (dialog.SelectedAction)
            {
                case DateTimeToolDialog.DateTimeAction.SetDateModified:
                {
                    DateTime dt = dialog.SelectedDateTime;
                    foreach (string file in Directory.GetFiles(StartDirectory, "*", searchOption))
                    {
                        try { File.SetLastWriteTime(file, dt); } catch { }
                    }
                    break;
                }
                case DateTimeToolDialog.DateTimeAction.SetDateCreated:
                {
                    DateTime dt = dialog.SelectedDateTime;
                    foreach (string file in Directory.GetFiles(StartDirectory, "*", searchOption))
                    {
                        try { File.SetCreationTime(file, dt); } catch { }
                    }
                    break;
                }
                case DateTimeToolDialog.DateTimeAction.CopyModifiedToCreated:
                {
                    bool onlyIfOlder = dialog.OnlyIfDateModifiedIsOlder;
                    foreach (string file in Directory.GetFiles(StartDirectory, "*", searchOption))
                    {
                        try
                        {
                            DateTime modified = File.GetLastWriteTime(file);
                            if (!onlyIfOlder || modified < File.GetCreationTime(file))
                                File.SetCreationTime(file, modified);
                        }
                        catch { }
                    }
                    break;
                }
                case DateTimeToolDialog.DateTimeAction.CopyTakenToCreated:
                {
                    string dt2dcExe = Path.Combine(appParts, "DT2DC.exe");
                    string args = $"\"{StartDirectory}\\*\"";
                    if (dialog.ApplyToSubfolders) args += " /s";

                    Process p = new Process();
                    p.StartInfo.FileName = dt2dcExe;
                    p.StartInfo.Arguments = args;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.WaitForExit();
                    break;
                }
                case DateTimeToolDialog.DateTimeAction.CopyTakenToCreatedAndModified:
                {
                    string dt2dcExe = Path.Combine(appParts, "DT2DC.exe");
                    string args = $"\"{StartDirectory}\\*\" /m";
                    if (dialog.ApplyToSubfolders) args += " /s";

                    Process p = new Process();
                    p.StartInfo.FileName = dt2dcExe;
                    p.StartInfo.Arguments = args;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.WaitForExit();
                    break;
                }
                case DateTimeToolDialog.DateTimeAction.CopyCreatedToModified:
                {
                    foreach (string file in Directory.GetFiles(StartDirectory, "*", searchOption))
                    {
                        try { File.SetLastWriteTime(file, File.GetCreationTime(file)); } catch { }
                    }
                    break;
                }
            }

            RefreshShell();
            dialog.Dispose();
        }

        static void Launcher()
        {
            helpPage = "launcher";

            if (!File.Exists(LauncherIni))
            {
                return;
            }

            // Read AutoClose setting from [Launcher] section in RightClickTools.ini
            bool autoClose = ReadString(myIniFile, "Launcher", "AutoClose", "0") == "1";

            using (var dialog = new Form())
            {
                dialog.Text = sMain;

                if (Dark)
                {
                    dialog.BackColor = Color.FromArgb(43, 43, 43);
                    dialog.ForeColor = Color.White;
                }
                else
                {
                    dialog.BackColor = Color.FromArgb(238, 238, 238);
                    dialog.ForeColor = Color.Black;
                }

                int fontSize = 9;
                int itemHeightBase = 24;
                int launcherStyle = 0;
                int.TryParse(ReadString(myIniFile, "Launcher", "Style", "0"), out launcherStyle);
                bool useWin11Style = launcherStyle == 0 ? Win11 : launcherStyle == 2;
                if (useWin11Style)
                {
                    fontSize = 10;
                    itemHeightBase = 30;
                }

                dialog.Font = new Font("Segoe UI", fontSize);
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.ShowInTaskbar = true;
                dialog.Icon = Icon.ExtractAssociatedIcon(myExe);
                dialog.StartPosition = FormStartPosition.Manual;
                dialog.AutoSize = true;
                dialog.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                dialog.AutoScroll = true;
                dialog.Padding = new Padding(0);

                // Apply dark title bar if using dark theme
                dialog.HandleCreated += (sender, e) =>
                {
                    if (Dark) DarkTitleBar(dialog.Handle);
                };

                var toolEntries = ParseLauncherIni(LauncherIni);

                int yOffset = 0;
                int maxWidth = 0;
                int minWidth = (int)(120 * ScaleFactor);
                int iconSize = (int)(16 * ScaleFactor);
                int iconPadding = (int)(6 * ScaleFactor);
                int itemHeight = (int)(itemHeightBase * ScaleFactor);

                foreach (var entry in toolEntries)
                {
                    string displayText = entry.Title ?? entry.Name;
                    int w = TextRenderer.MeasureText(displayText, dialog.Font).Width;
                    maxWidth = Math.Max(maxWidth, w);
                }

                if (maxWidth < minWidth) maxWidth = minWidth;

                // Create menu items from Launcher.ini entries
                foreach (var entry in toolEntries)
                {
                    string iconPath = myIcon;

                    // Determine icon path priority: explicit Icon > Exe > default
                    if (!string.IsNullOrEmpty(entry.Icon))
                    {
                        iconPath = entry.Icon;
                    }
                    else if (!string.IsNullOrEmpty(entry.Exe) && entry.Exe != myExe)
                    {
                        iconPath = entry.Exe;
                    }

                    Icon icon = ExtractIconFromFile(iconPath, iconSize);

                    var pictureBox = new PictureBox
                    {
                        Width = iconSize,
                        Height = iconSize,
                        Location = new Point((int)(4 * ScaleFactor), (itemHeight - iconSize) / 2),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Image = icon?.ToBitmap(),
                        BackColor = dialog.BackColor
                    };

                    var label = new Label
                    {
                        Text = entry.Title ?? entry.Name,
                        Cursor = Cursors.Default,
                        Width = maxWidth,
                        Height = itemHeight,
                        Location = new Point((int)(4 * ScaleFactor) + iconSize + iconPadding, 0),
                        AutoSize = false,
                        BackColor = dialog.BackColor,
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    var panel = new Panel
                    {
                        Width = (int)(4 * ScaleFactor) + iconSize + iconPadding + maxWidth + (int)(8 * ScaleFactor),
                        Height = itemHeight,
                        Location = new Point(0, yOffset),
                        Cursor = Cursors.Default,
                        BackColor = dialog.BackColor
                    };

                    panel.Controls.Add(pictureBox);
                    panel.Controls.Add(label);

                    var currentPanel = panel;
                    var currentLabel = label;
                    var currentPictureBox = pictureBox;
                    var currentEntry = entry;

                    currentPanel.MouseEnter += (sender, e) =>
                    {
                        if (Dark)
                        {
                            currentPanel.BackColor = Color.FromArgb(65, 65, 65);
                            currentLabel.BackColor = Color.FromArgb(65, 65, 65);
                            currentPictureBox.BackColor = Color.FromArgb(65, 65, 65);
                        }
                        else
                        {
                            currentPanel.BackColor = Color.White;
                            currentLabel.BackColor = Color.White;
                            currentPictureBox.BackColor = Color.White;
                        }
                    };

                    currentPanel.MouseLeave += (sender, e) =>
                    {
                        currentPanel.BackColor = dialog.BackColor;
                        currentLabel.BackColor = dialog.BackColor;
                        currentPictureBox.BackColor = dialog.BackColor;
                    };

                    currentLabel.MouseEnter += (sender, e) =>
                    {
                        if (Dark)
                        {
                            currentPanel.BackColor = Color.FromArgb(65, 65, 65);
                            currentLabel.BackColor = Color.FromArgb(65, 65, 65);
                            currentPictureBox.BackColor = Color.FromArgb(65, 65, 65);
                        }
                        else
                        {
                            currentPanel.BackColor = Color.White;
                            currentLabel.BackColor = Color.White;
                            currentPictureBox.BackColor = Color.White;
                        }
                    };

                    currentLabel.MouseLeave += (sender, e) =>
                    {
                        currentPanel.BackColor = dialog.BackColor;
                        currentLabel.BackColor = dialog.BackColor;
                        currentPictureBox.BackColor = dialog.BackColor;
                    };

                    currentPanel.Click += (sender, e) =>
                    {
                        LaunchTool(currentEntry);
                        if (autoClose) dialog.Close();
                    };

                    currentLabel.Click += (sender, e) =>
                    {
                        LaunchTool(currentEntry);
                        if (autoClose) dialog.Close();
                    };

                    currentPictureBox.Click += (sender, e) =>
                    {
                        LaunchTool(currentEntry);
                        if (autoClose) dialog.Close();
                    };

                    dialog.Controls.Add(panel);

                    yOffset += panel.Height;
                }

                Point cursorPosition = Cursor.Position;
                Screen screen = Screen.FromPoint(cursorPosition);

                int screenWidth = screen.WorkingArea.Width;
                int screenHeight = screen.WorkingArea.Height;

                if (dialog.Height > screenHeight)
                {
                    dialog.Height = screenHeight;
                    dialog.Width += (int)(16 * ScaleFactor);
                    dialog.AutoSize = false;
                }

                dialog.Location = GetDialogPosition(dialog);

                dialog.ShowDialog();
            }
        }

        static void MoreTools()
        {
            helpPage = "more-tools";

            if (!File.Exists(MoreToolsIni))
            {
                return;
            }

            using (var dialog = new CustomFormNoTitle())
            {
                if (Dark)
                {
                    dialog.BackColor = Color.FromArgb(43, 43, 43);
                    dialog.ForeColor = Color.White;
                }
                else
                {
                    dialog.BackColor = Color.FromArgb(238, 238, 238);
                    dialog.ForeColor = Color.Black;
                }

                int fontSize = 9;
                int itemHeightBase = 24;
                int moreToolsStyle = 0;
                int.TryParse(ReadString(myIniFile, "MoreTools", "Style", "0"), out moreToolsStyle);
                bool useWin11Style = moreToolsStyle == 0 ? Win11 : moreToolsStyle == 2;
                if (useWin11Style)
                {
                    fontSize = 10;
                    itemHeightBase = 30;
                }

                dialog.Font = new Font("Segoe UI", fontSize);
                dialog.FormBorderStyle = FormBorderStyle.FixedSingle;
                dialog.ControlBox = false;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.ShowInTaskbar = false;
                dialog.Icon = Icon.ExtractAssociatedIcon(myExe);
                dialog.StartPosition = FormStartPosition.Manual;
                dialog.AutoSize = true;
                dialog.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                dialog.AutoScroll = true;
                dialog.Padding = new Padding(0);

                dialog.Deactivate += (sender, e) =>
                {
                    dialog.Close();
                };

                if (Win11)
                {
                    dialog.HandleCreated += (sender, e) =>
                    {
                        int preference = DWMWCP_ROUND;
                        DwmSetWindowAttribute(dialog.Handle, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, sizeof(uint));
                    };
                }

                var toolEntries = ParseMoreToolsIni(MoreToolsIni);

                // Filter out entries with invalid executable paths
                toolEntries = toolEntries.Where(entry => File.Exists(entry.Exe)).ToList();

                int yOffset = 0;
                int maxWidth = 0;
                int minWidth = (int)(120 * ScaleFactor);
                int iconSize = (int)(16 * ScaleFactor);
                int iconPadding = (int)(6 * ScaleFactor);
                int itemHeight = (int)(itemHeightBase * ScaleFactor);

                foreach (var entry in toolEntries)
                {
                    string displayText = entry.Title ?? entry.Name;
                    int w = TextRenderer.MeasureText(displayText, dialog.Font).Width;
                    maxWidth = Math.Max(maxWidth, w);
                }

                if (maxWidth < minWidth) maxWidth = minWidth;

                // Create menu items from MoreTools.ini entries
                foreach (var entry in toolEntries)
                {
                    string iconPath = !string.IsNullOrEmpty(entry.Icon) ? entry.Icon : entry.Exe;
                    Icon icon = ExtractIconFromFile(iconPath, iconSize);

                    var pictureBox = new PictureBox
                    {
                        Width = iconSize,
                        Height = iconSize,
                        Location = new Point((int)(4 * ScaleFactor), (itemHeight - iconSize) / 2),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Image = icon?.ToBitmap(),
                        BackColor = dialog.BackColor
                    };

                    var label = new Label
                    {
                        Text = entry.Title ?? entry.Name,
                        Cursor = Cursors.Default,
                        Width = maxWidth,
                        Height = itemHeight,
                        Location = new Point((int)(4 * ScaleFactor) + iconSize + iconPadding, 0),
                        AutoSize = false,
                        BackColor = dialog.BackColor,
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    var panel = new Panel
                    {
                        Width = (int)(4 * ScaleFactor) + iconSize + iconPadding + maxWidth + (int)(8 * ScaleFactor),
                        Height = itemHeight,
                        Location = new Point(0, yOffset),
                        Cursor = Cursors.Default,
                        BackColor = dialog.BackColor
                    };

                    panel.Controls.Add(pictureBox);
                    panel.Controls.Add(label);

                    var currentPanel = panel;
                    var currentLabel = label;
                    var currentPictureBox = pictureBox;
                    var currentEntry = entry;

                    currentPanel.MouseEnter += (sender, e) =>
                    {
                        if (Dark)
                        {
                            currentPanel.BackColor = Color.FromArgb(65, 65, 65);
                            currentLabel.BackColor = Color.FromArgb(65, 65, 65);
                            currentPictureBox.BackColor = Color.FromArgb(65, 65, 65);
                        }
                        else
                        {
                            currentPanel.BackColor = Color.White;
                            currentLabel.BackColor = Color.White;
                            currentPictureBox.BackColor = Color.White;
                        }
                    };

                    currentPanel.MouseLeave += (sender, e) =>
                    {
                        currentPanel.BackColor = dialog.BackColor;
                        currentLabel.BackColor = dialog.BackColor;
                        currentPictureBox.BackColor = dialog.BackColor;
                    };

                    currentLabel.MouseEnter += (sender, e) =>
                    {
                        if (Dark)
                        {
                            currentPanel.BackColor = Color.FromArgb(65, 65, 65);
                            currentLabel.BackColor = Color.FromArgb(65, 65, 65);
                            currentPictureBox.BackColor = Color.FromArgb(65, 65, 65);
                        }
                        else
                        {
                            currentPanel.BackColor = Color.White;
                            currentLabel.BackColor = Color.White;
                            currentPictureBox.BackColor = Color.White;
                        }
                    };

                    currentLabel.MouseLeave += (sender, e) =>
                    {
                        currentPanel.BackColor = dialog.BackColor;
                        currentLabel.BackColor = dialog.BackColor;
                        currentPictureBox.BackColor = dialog.BackColor;
                    };

                    currentPanel.Click += (sender, e) =>
                    {
                        dialog.Hide();
                        LaunchMoreTool(currentEntry);
                        dialog.Close();
                    };

                    currentLabel.Click += (sender, e) =>
                    {
                        dialog.Hide();
                        LaunchMoreTool(currentEntry);
                        dialog.Close();
                    };

                    currentPictureBox.Click += (sender, e) =>
                    {
                        dialog.Hide();
                        LaunchMoreTool(currentEntry);
                        dialog.Close();
                    };

                    dialog.Controls.Add(panel);

                    yOffset += panel.Height;
                }

                Point cursorPosition = Cursor.Position;
                Screen screen = Screen.FromPoint(cursorPosition);

                int screenWidth = screen.WorkingArea.Width;
                int screenHeight = screen.WorkingArea.Height;

                if (dialog.Height > screenHeight)
                {
                    dialog.Height = screenHeight;
                    dialog.Width += (int)(16 * ScaleFactor);
                    dialog.AutoSize = false;
                }

                dialog.Location = GetDialogPosition(dialog);

                dialog.ShowDialog();
            }
        }

        static System.Collections.Generic.List<LauncherEntry> ParseLauncherIni(string iniFilePath)
        {
            var entries = new System.Collections.Generic.List<LauncherEntry>();

            try
            {
                var lines = File.ReadAllLines(iniFilePath, Encoding.UTF8);
                string currentSection = null;
                string currentTitle = null;
                string currentCommand = null;
                string currentExe = null;
                string currentCmdLine = null;
                string currentIcon = null;
                int currentRunAs = 0;

                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                    {
                        continue;
                    }

                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        // Save previous entry
                        if (currentSection != null && (currentCommand != null || currentExe != null))
                        {
                            entries.Add(new LauncherEntry
                            {
                                Name = currentSection,
                                Title = currentTitle,
                                Command = currentCommand,
                                Exe = currentExe,
                                CmdLine = currentCmdLine,
                                Icon = currentIcon,
                                RunAs = currentRunAs
                            });
                        }

                        currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                        currentTitle = null;
                        currentCommand = null;
                        currentExe = null;
                        currentCmdLine = null;
                        currentIcon = null;
                        currentRunAs = 0;
                    }
                    else
                    {
                        var parts = trimmedLine.Split(new char[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();

                            if (key.Equals("Command", StringComparison.OrdinalIgnoreCase))
                            {
                                currentCommand = value;
                            }
                            else if (key.Equals("Exe", StringComparison.OrdinalIgnoreCase))
                            {
                                if (value.Equals("RightClickTools.exe", StringComparison.OrdinalIgnoreCase))
                                {
                                    currentExe = myExe;
                                }
                                else if (string.IsNullOrEmpty(Path.GetDirectoryName(value)))
                                {
                                    // No path specified - look in AppParts folder first
                                    string appPartsExe = Path.Combine(appParts, value);
                                    currentExe = File.Exists(appPartsExe) ? appPartsExe : value;
                                }
                                else if (!Path.IsPathRooted(value))
                                {
                                    // Relative path - resolve from one level up from the program's folder
                                    string parentFolder = Path.GetDirectoryName(myPath);
                                    currentExe = parentFolder != null ? Path.Combine(parentFolder, value) : value;
                                }
                                else
                                {
                                    currentExe = value;
                                }
                            }
                            else if (key.Equals("CmdLine", StringComparison.OrdinalIgnoreCase))
                            {
                                currentCmdLine = value;
                            }
                            else if (key.Equals("Icon", StringComparison.OrdinalIgnoreCase))
                            {
                                // If Icon is just a filename (not a full path), use IconFolder
                                if (!string.IsNullOrEmpty(value) && !Path.IsPathRooted(value))
                                {
                                    currentIcon = Path.Combine(IconFolder, value);
                                }
                                else
                                {
                                    currentIcon = value;
                                }
                            }
                            else if (key.Equals("Title", StringComparison.OrdinalIgnoreCase))
                            {
                                currentTitle = value;
                            }
                            else if (key.Equals("RunAs", StringComparison.OrdinalIgnoreCase))
                            {
                                int.TryParse(value, out currentRunAs);
                            }
                        }
                    }
                }

                // Save last entry
                if (currentSection != null && (currentCommand != null || currentExe != null))
                {
                    entries.Add(new LauncherEntry
                    {
                        Name = currentSection,
                        Title = currentTitle,
                        Command = currentCommand,
                        Exe = currentExe,
                        CmdLine = currentCmdLine,
                        Icon = currentIcon,
                        RunAs = currentRunAs
                    });
                }
            }
            catch
            {
            }

            // Process entries for localization and command extraction
            foreach (var entry in entries)
            {
                // If using Exe/CmdLine format, try to extract command from CmdLine
                if (string.IsNullOrEmpty(entry.Command) && !string.IsNullOrEmpty(entry.CmdLine))
                {
                    // Extract command from CmdLine (e.g., "/CmdHere" from "/CmdHere \"%V\"")
                    string cmdLine = entry.CmdLine.Trim();
                    if (cmdLine.StartsWith("/"))
                    {
                        int spaceIndex = cmdLine.IndexOf(' ');
                        if (spaceIndex > 0)
                        {
                            entry.Command = cmdLine.Substring(1, spaceIndex - 1);
                        }
                        else
                        {
                            entry.Command = cmdLine.Substring(1);
                        }
                    }
                }

                // Localize titles by matching Command with CmdKeys
                if (!string.IsNullOrEmpty(entry.Command))
                {
                    for (int i = 0; i < CmdKeys.Length; i++)
                    {
                        if (entry.Command.Equals(CmdKeys[i], StringComparison.OrdinalIgnoreCase))
                        {
                            // Replace title with localized version from CmdLabels
                            if (CmdLabels != null && i < CmdLabels.Length)
                            {
                                entry.Title = CmdLabels[i];
                            }
                            break;
                        }
                    }

                    // Localize ShortcutTool and DateTimeTool titles
                    if (entry.Command.Equals("ShortcutTool", StringComparison.OrdinalIgnoreCase))
                        entry.Title = sShortcutTool;
                    else if (entry.Command.Equals("DateTimeTool", StringComparison.OrdinalIgnoreCase))
                        entry.Title = sDateTimeTool;
                }

                // For external tools with a known name and no Title in the INI, apply localized title
                if (string.IsNullOrEmpty(entry.Title) &&
                    (entry.Name ?? "").Equals("SnipWithBorder", StringComparison.OrdinalIgnoreCase))
                    entry.Title = sSnipWithBorder;
            }

            // Filter out entries with invalid exe paths (only for external tools)
            entries = entries.Where(entry => 
                string.IsNullOrEmpty(entry.Exe) || 
                entry.Exe == myExe || 
                File.Exists(entry.Exe)).ToList();

            return entries;
        }

        static System.Collections.Generic.List<MoreToolEntry> ParseMoreToolsIni(string iniFilePath)
        {
            var entries = new System.Collections.Generic.List<MoreToolEntry>();

            try
            {
                var lines = File.ReadAllLines(iniFilePath, Encoding.UTF8);
                string currentSection = null;
                string currentTitle = null;
                string currentExe = null;
                string currentCmdLine = null;
                string currentIcon = null;
                int currentRunAs = 0;

                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                    {
                        continue;
                    }

                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        if (currentSection != null && currentExe != null)
                        {
                            entries.Add(new MoreToolEntry
                            {
                                Name = currentSection,
                                Title = currentTitle,
                                Exe = currentExe,
                                CmdLine = currentCmdLine ?? "",
                                Icon = currentIcon,
                                RunAs = currentRunAs
                            });
                        }

                        currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                        currentTitle = null;
                        currentExe = null;
                        currentCmdLine = null;
                        currentIcon = null;
                        currentRunAs = 0;
                    }
                    else
                    {
                        var parts = trimmedLine.Split(new char[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();

                            if (key.Equals("Exe", StringComparison.OrdinalIgnoreCase))
                            {
                                if (value.Equals("RightClickTools.exe", StringComparison.OrdinalIgnoreCase))
                                {
                                    currentExe = myExe;
                                }
                                else if (string.IsNullOrEmpty(Path.GetDirectoryName(value)))
                                {
                                    // No path specified - look in AppParts folder first
                                    string appPartsExe = Path.Combine(appParts, value);
                                    currentExe = File.Exists(appPartsExe) ? appPartsExe : value;
                                }
                                else if (!Path.IsPathRooted(value))
                                {
                                    // Relative path - resolve from one level up from the program's folder
                                    string parentFolder = Path.GetDirectoryName(myPath);
                                    currentExe = parentFolder != null ? Path.Combine(parentFolder, value) : value;
                                }
                                else
                                {
                                    currentExe = value;
                                }
                            }
                            else if (key.Equals("CmdLine", StringComparison.OrdinalIgnoreCase))
                            {
                                currentCmdLine = value;
                            }
                            else if (key.Equals("Icon", StringComparison.OrdinalIgnoreCase))
                            {
                                // If Icon is just a filename (not a full path), use IconFolder
                                if (!string.IsNullOrEmpty(value) && !Path.IsPathRooted(value))
                                {
                                    currentIcon = Path.Combine(IconFolder, value);
                                }
                                else
                                {
                                    currentIcon = value;
                                }
                            }
                            else if (key.Equals("Title", StringComparison.OrdinalIgnoreCase))
                            {
                                currentTitle = value;
                            }
                            else if (key.Equals("RunAs", StringComparison.OrdinalIgnoreCase))
                            {
                                int.TryParse(value, out currentRunAs);
                            }
                        }
                    }
                }

                if (currentSection != null && currentExe != null)
                {
                    entries.Add(new MoreToolEntry
                    {
                        Name = currentSection,
                        Title = currentTitle,
                        Exe = currentExe,
                        CmdLine = currentCmdLine ?? "",
                        Icon = currentIcon,
                        RunAs = currentRunAs
                    });
                }
            }
            catch
            {
            }

            // Localize titles for ShortcutTool and DateTimeTool (internal tools)
            foreach (var entry in entries)
            {
                if (entry.Exe == myExe)
                {
                    string cmdLower = (entry.CmdLine ?? "").Trim().ToLower();
                    if (cmdLower == "/shortcuttool" || cmdLower.StartsWith("/shortcuttool "))
                    {
                        entry.Title = sShortcutTool;
                    }
                    else if (cmdLower == "/datetimetool" || cmdLower.StartsWith("/datetimetool "))
                    {
                        entry.Title = sDateTimeTool;
                    }
                }

                // For external tools with a known name and no Title in the INI, apply localized title
                if (string.IsNullOrEmpty(entry.Title) &&
                    (entry.Name ?? "").Equals("SnipWithBorder", StringComparison.OrdinalIgnoreCase))
                    entry.Title = sSnipWithBorder;
            }

            return entries;
        }

        static void LaunchTool(LauncherEntry entry)
        {
            try
            {
                // Check if this is an internal command
                // Some commands are treated as external tools so that their options and behavior are consistent with MoreTools.
                bool isTreatLikeExternal = !string.IsNullOrEmpty(entry.Command) &&
                    (entry.Command.Equals("ShortcutTool", StringComparison.OrdinalIgnoreCase) ||
                     entry.Command.Equals("FileManagerHere", StringComparison.OrdinalIgnoreCase) ||
                     entry.Command.Equals("SearchHere", StringComparison.OrdinalIgnoreCase) ||
                     entry.Command.Equals("RegEdit", StringComparison.OrdinalIgnoreCase) ||
                     entry.Command.Equals("DateTimeTool", StringComparison.OrdinalIgnoreCase));

                if (!isTreatLikeExternal && !string.IsNullOrEmpty(entry.Command) && (string.IsNullOrEmpty(entry.Exe) || entry.Exe == myExe))
                {
                    // Internal command - launch new process (like context menu does)
                    Process p = new Process();
                    p.StartInfo.FileName = myExe;
                    p.StartInfo.Arguments = $"/{entry.Command}";
                    p.StartInfo.WorkingDirectory = @"C:\";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = false;
                    p.Start();
                }
                else if (!string.IsNullOrEmpty(entry.Exe))
                {
                    // External tool - execute like MoreTools
                    string cmdLine = entry.CmdLine ?? "";

                    if (cmdLine.Contains("%V"))
                    {
                        // Read StartDirectory from registry before replacing %V
                        // This ensures external commands get the current directory
                        string currentStartDirectory = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", "");
                        if (!string.IsNullOrEmpty(currentStartDirectory) && Directory.Exists(currentStartDirectory))
                        {
                            cmdLine = cmdLine.Replace("%V", currentStartDirectory);
                        }
                        else
                        {
                            cmdLine = cmdLine.Replace("%V", StartDirectory);
                        }
                    }

                    CommandLine = cmdLine;

                    if (entry.RunAs == 1 && (Control.ModifierKeys & Keys.Control) == 0)
                    {
                        RunAsAdmin(entry.Exe);
                    }
                    else if (entry.RunAs == 2 && (Control.ModifierKeys & Keys.Control) == 0)
                    {
                        // If NoTrustedInstaller is enabled, treat RunAs=2 as RunAs=1
                        if (NoTrustedInstaller)
                        {
                            RunAsAdmin(entry.Exe);
                        }
                        else
                        {
                            RunAsTrusted(entry.Exe);
                        }
                    }
                    else if (entry.RunAs == 3 || (Control.ModifierKeys & Keys.Control) != 0)
                    {
                        // Prompt user for privilege level
                        bwidth = 120;
                        string toolName = entry.Title ?? entry.Name ?? Path.GetFileNameWithoutExtension(entry.Exe);
                        DialogResult result = ThreeChoiceBox.Show(toolName, sMain, sUser, sAdministrator, sTrustedInstaller, !NoTrustedInstaller);
                        if (result == DialogResult.Cancel) return;
                        if (result == DialogResult.OK) RunAsUser(entry.Exe);
                        if (result == DialogResult.Yes) RunAsAdmin(entry.Exe);
                        if (result == DialogResult.No) RunAsTrusted(entry.Exe);
                    }
                    else
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = entry.Exe;
                        p.StartInfo.Arguments = cmdLine;
                        p.StartInfo.WorkingDirectory = @"C:\";
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = false;
                        p.Start();
                    }
                }
            }
            catch
            {
            }
        }

        static void LaunchMoreTool(MoreToolEntry entry)
        {
            try
            {
                string cmdLine = entry.CmdLine;

                if (cmdLine.Contains("%V"))
                {
                    // Read StartDirectory from registry before replacing %V
                    // This ensures external commands get the current directory
                    string currentStartDirectory = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", "");
                    if (!string.IsNullOrEmpty(currentStartDirectory) && Directory.Exists(currentStartDirectory))
                    {
                        cmdLine = cmdLine.Replace("%V", currentStartDirectory);
                    }
                    else
                    {
                        cmdLine = cmdLine.Replace("%V", StartDirectory);
                    }
                }

                CommandLine = cmdLine;

                if (entry.RunAs == 1 && (Control.ModifierKeys & Keys.Control) == 0)
                {
                    RunAsAdmin(entry.Exe);
                }
                else if (entry.RunAs == 2 && (Control.ModifierKeys & Keys.Control) == 0)
                {
                    // If NoTrustedInstaller is enabled, treat RunAs=2 as RunAs=1
                    if (NoTrustedInstaller)
                    {
                        RunAsAdmin(entry.Exe);
                    }
                    else
                    {
                        RunAsTrusted(entry.Exe);
                    }
                }
                else if (entry.RunAs == 3 || (Control.ModifierKeys & Keys.Control) != 0)
                {
                    // Prompt user for privilege level
                    bwidth = 120;
                    string toolName = entry.Title ?? entry.Name ?? Path.GetFileNameWithoutExtension(entry.Exe);
                    DialogResult result = ThreeChoiceBox.Show(toolName, sMain, sUser, sAdministrator, sTrustedInstaller, !NoTrustedInstaller);
                    if (result == DialogResult.Cancel) return;
                    if (result == DialogResult.OK) RunAsUser(entry.Exe);
                    if (result == DialogResult.Yes) RunAsAdmin(entry.Exe);
                    if (result == DialogResult.No) RunAsTrusted(entry.Exe);
                }
                else
                {
                    Process p = new Process();
                    p.StartInfo.FileName = entry.Exe;
                    p.StartInfo.Arguments = cmdLine;
                    p.StartInfo.WorkingDirectory = @"C:\";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = false;
                    p.Start();
                }
            }
            catch
            {
            }
        }

        static Icon ExtractIconFromFile(string filePath, int size)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return SystemIcons.Application;
                }

                IntPtr large = IntPtr.Zero;
                IntPtr small = IntPtr.Zero;
                ExtractIconEx(filePath, 0, ref large, ref small, 1);

                if (large != IntPtr.Zero)
                {
                    Icon icon = (Icon)Icon.FromHandle(large).Clone();
                    DestroyIcon(large);
                    return icon;
                }
                if (small != IntPtr.Zero)
                {
                    Icon icon = (Icon)Icon.FromHandle(small).Clone();
                    DestroyIcon(small);
                    return icon;
                }

                return Icon.ExtractAssociatedIcon(filePath);
            }
            catch
            {
                return SystemIcons.Application;
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int ExtractIconEx(string lpszFile, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyIcon(IntPtr hIcon);

        class LauncherEntry
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string Command { get; set; }
            public string Exe { get; set; }
            public string CmdLine { get; set; }
            public string Icon { get; set; }
            public int RunAs { get; set; }
        }

        class MoreToolEntry
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string Exe { get; set; }
            public string CmdLine { get; set; }
            public string Icon { get; set; }
            public int RunAs { get; set; }
        }

        class CustomFormNoTitle : Form
        {
            private const int WS_HSCROLL = 0x00100000;
            private const int WM_NCCALCSIZE = 0x0083;
            private const int CS_DROPSHADOW = 0x00020000;

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ClassStyle |= CS_DROPSHADOW;
                    return cp;
                }
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_NCCALCSIZE)
                {
                    int style = GetWindowLong(Handle, GWL_STYLE);
                    if ((style & WS_HSCROLL) == WS_HSCROLL)
                    {
                        style &= ~WS_HSCROLL;
                        SetWindowLong(Handle, GWL_STYLE, style);
                    }
                }
                base.WndProc(ref m);
            }

            [DllImport("user32.dll", SetLastError = true)]
            private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

            private const int GWL_STYLE = -16;
        }

        static void ConvertUrlToLnkInDirectory(string directory, bool recurse, bool moveToRecycleBin)
        {
            try
            {
                // Get .url files in current directory
                string[] urlFiles = Directory.GetFiles(directory, "*.url", SearchOption.TopDirectoryOnly);

                // Process each .url file
                foreach (string urlFile in urlFiles)
                {
                    ConvertUrlFileToLnk(urlFile, moveToRecycleBin);
                }

                // Process subdirectories if requested
                if (recurse)
                {
                    string[] subdirectories = Directory.GetDirectories(directory);
                    foreach (string subdirectory in subdirectories)
                    {
                        ConvertUrlToLnkInDirectory(subdirectory, recurse, moveToRecycleBin);
                    }
                }
            }
            catch
            {
                // Silently ignore errors (access denied, etc.)
            }
        }

        static void ConvertUrlFileToLnk(string urlFilePath, bool moveToRecycleBin)
        {
            try
            {
                // Read the .url file
                string[] lines = File.ReadAllLines(urlFilePath);

                string url = null;
                string iconFile = null;
                int iconIndex = 0;

                // Parse the .url file
                foreach (string line in lines)
                {
                    if (line.StartsWith("URL=", StringComparison.OrdinalIgnoreCase))
                    {
                        url = line.Substring(4);
                    }
                    else if (line.StartsWith("IconFile=", StringComparison.OrdinalIgnoreCase))
                    {
                        iconFile = line.Substring(9);
                    }
                    else if (line.StartsWith("IconIndex=", StringComparison.OrdinalIgnoreCase))
                    {
                        int.TryParse(line.Substring(10), out iconIndex);
                    }
                }

                // Skip if no URL found
                if (string.IsNullOrEmpty(url)) return;

                // Create .lnk file path
                string lnkPath = Path.ChangeExtension(urlFilePath, ".lnk");

                // Create WScript.Shell COM object
                Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(shellType);

                // Create shortcut
                dynamic shortcut = shell.CreateShortcut(lnkPath);

                // Set properties
                string explorerPath = Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe";
                shortcut.TargetPath = explorerPath;
                shortcut.Arguments = "\"" + url + "\"";
                shortcut.WorkingDirectory = Path.GetDirectoryName(explorerPath);

                // Set icon if available
                if (!string.IsNullOrEmpty(iconFile))
                {
                    shortcut.IconLocation = iconFile + "," + iconIndex;
                }

                // Save the shortcut
                shortcut.Save();

                // Release COM objects
                System.Runtime.InteropServices.Marshal.ReleaseComObject(shortcut);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(shell);

                // Move to recycle bin if requested
                if (moveToRecycleBin)
                {
                    FileOperationAPIWrapper.MoveToRecycleBin(urlFilePath);
                }
            }
            catch
            {
                // Silently ignore errors for individual files
            }
        }

        static void SearchAndReplaceInLnkFiles(string directory, string searchText, string replaceText, 
            bool searchTarget, bool searchStartIn, bool searchIcon, bool recurse)
        {
            try
            {
                // Get .lnk files in current directory
                string[] lnkFiles = Directory.GetFiles(directory, "*.lnk", SearchOption.TopDirectoryOnly);

                // Process each .lnk file
                foreach (string lnkFile in lnkFiles)
                {
                    SearchAndReplaceInLnkFile(lnkFile, searchText, replaceText, searchTarget, searchStartIn, searchIcon);
                }

                // Process subdirectories if requested
                if (recurse)
                {
                    string[] subdirectories = Directory.GetDirectories(directory);
                    foreach (string subdirectory in subdirectories)
                    {
                        SearchAndReplaceInLnkFiles(subdirectory, searchText, replaceText, searchTarget, searchStartIn, searchIcon, recurse);
                    }
                }
            }
            catch
            {
                // Silently ignore errors (access denied, etc.)
            }
        }

        static void SearchAndReplaceInLnkFile(string lnkFilePath, string searchText, string replaceText,
            bool searchTarget, bool searchStartIn, bool searchIcon)
        {
            try
            {
                // Create WScript.Shell COM object
                Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(shellType);

                // Load the shortcut
                dynamic shortcut = shell.CreateShortcut(lnkFilePath);

                bool modified = false;

                // Search and replace in Target
                if (searchTarget && !string.IsNullOrEmpty(shortcut.TargetPath))
                {
                    string originalTarget = shortcut.TargetPath;
                    string newTarget = Regex.Replace(originalTarget, Regex.Escape(searchText), replaceText, RegexOptions.IgnoreCase);
                    if (newTarget != originalTarget)
                    {
                        shortcut.TargetPath = newTarget;
                        modified = true;
                    }
                }

                // Search and replace in Working Directory (Start in)
                if (searchStartIn && !string.IsNullOrEmpty(shortcut.WorkingDirectory))
                {
                    string originalWorkDir = shortcut.WorkingDirectory;
                    string newWorkDir = Regex.Replace(originalWorkDir, Regex.Escape(searchText), replaceText, RegexOptions.IgnoreCase);
                    if (newWorkDir != originalWorkDir)
                    {
                        shortcut.WorkingDirectory = newWorkDir;
                        modified = true;
                    }
                }

                // Search and replace in Icon Location
                if (searchIcon && !string.IsNullOrEmpty(shortcut.IconLocation))
                {
                    string originalIcon = shortcut.IconLocation;
                    string newIcon = Regex.Replace(originalIcon, Regex.Escape(searchText), replaceText, RegexOptions.IgnoreCase);
                    if (newIcon != originalIcon)
                    {
                        shortcut.IconLocation = newIcon;
                        modified = true;
                    }
                }

                // Save if modified
                if (modified)
                {
                    shortcut.Save();
                }

                // Release COM objects
                System.Runtime.InteropServices.Marshal.ReleaseComObject(shortcut);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(shell);
            }
            catch
            {
                // Silently ignore errors for individual files
            }
        }

        static string validateTakeOwnPath()
        {
            string sStopAll = ReadString(myIniFile, "TakeOwnHere", "StopAll", "");
            string[] StopAll = sStopAll.Split(new char[] { '|' });

            string sStopRoot = ReadString(myIniFile, "TakeOwnHere", "StopRoot", "");
            string[] StopRoot = sStopRoot.Split(new char[] { '|' });

            for (int i = 0; i < StopAll.Length; i++)
            {
                if (StartsWith(StopAll[i], StartDirectory)) { Stop = true; break; }
            }

            for (int i = 0; i < StopRoot.Length; i++)
            {
                if (StrCmp(StopRoot[i], StartDirectory)) { Stop = true; break; }
            }

            string sMsg = "";

            if (Stop)
            {
                sMsg = sFolderNotAllowed;
            }
            else
            {
                string UserProfile = Environment.GetEnvironmentVariable("UserProfile");
                sMsg = $"{sTakeOwnHere}?";
                if (StartsWith(StartDirectory, "C:\\Users\\") && !StartsWith(StartDirectory, UserProfile)) sMsg = $"{sWarningTakeOwn}\n\n{sMsg}";
            }

            return sMsg;
        }
        static void RunTakeOwnHerePS1AsAdmin()
        {
            helpPage = "take-ownership-and-get-access";

            string sMsg = validateTakeOwnPath();

            DialogResult result = TakeOwnDialog.Show($"{sMsg}\n\n{StartDirectory}\n\n", sMain);

            if (result == DialogResult.Cancel || Stop) return;

            CreateTakeOwnHerePS1();

            string cfg = $"[Process]\r\nEXEFilename={PowerShellExe}\r\nCommandLine={CommandLine}\r\nRunAs=Administrator";

            File.WriteAllText(ElevateCfg, cfg);

            if (isFullAdmin)
            {
                Process p = new Process();
                p.StartInfo.FileName = SchTasksExe;
                p.StartInfo.Arguments = $"/run /tn \"{TaskName}\"";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
            }
            else
            {
                RunUAC(PowerShellExe);
            }
        }

        static void ClearDefenderHistory()
        {
            CommandLine = "/ClearHistoryAdmin";

            string cfg = $"[Process]\r\nEXEFilename={myExe}\r\nCommandLine={CommandLine}\r\nRunAs=Administrator";

            File.WriteAllText(ElevateCfg, cfg);

            if (isFullAdmin)
            {
                Process p = new Process();
                p.StartInfo.FileName = SchTasksExe;
                p.StartInfo.Arguments = $"/run /tn \"{TaskName}\"";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
            }
            else
            {
                RunUAC(myExe);
            }

        }

        static void ClearDefenderHistoryTask()
        {
            Process p = new Process();
            p.StartInfo.FileName = SchTasksExe;
            p.StartInfo.Arguments = $"/create /f /tn MyTasks\\DWDH /xml \"{appParts}\\DWDH.cfg\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();

            helpPage = "clear-history";

            DialogResult result = CustomMessageBox.Show(sRestartPC, sMain);

            if (result == DialogResult.Cancel) return;

            Process.Start("shutdown", "/r /t 0");
        }

        static bool StrCmp(string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        // Calculate dialog position - use saved dialog position for theme relaunch, current cursor otherwise
        static Point GetDialogPosition(Form dialog, int yOffset = 0)
        {
            int dialogX, dialogY;
            Point positionForScreen;

            if (useOriginalPosition)
            {
                // savedDialogPosition is already the top-left corner, use it directly
                dialogX = savedDialogPosition.X;
                dialogY = savedDialogPosition.Y;
                positionForScreen = savedDialogPosition;
                useOriginalPosition = false; // Reset after use
            }
            else
            {
                // Cursor position is a point - center the dialog around it
                Point cursorPos = Cursor.Position;
                dialogX = cursorPos.X - dialog.Width / 2;
                dialogY = cursorPos.Y - dialog.Height / 2 + yOffset;
                positionForScreen = cursorPos;
            }

            // Ensure dialog stays within screen bounds
            Screen screen = Screen.FromPoint(positionForScreen);

            int screenWidth = screen.WorkingArea.Width;
            int screenHeight = screen.WorkingArea.Height;

            int baseX = screen.Bounds.X;
            int baseY = screen.Bounds.Y;

            dialogX = Math.Max(baseX, Math.Min(baseX + screenWidth - dialog.Width, dialogX));
            dialogY = Math.Max(baseY, Math.Min(baseY + screenHeight - dialog.Height, dialogY));

            return new Point(dialogX, dialogY);
        }

        static bool StartsWith(string str1, string str2)
        {
            int length = Math.Min(str1.Length, str2.Length);
            return string.Equals(str1.Substring(0, length), str2.Substring(0, length), StringComparison.OrdinalIgnoreCase);
        }

        // Get current screen scaling factor
        static float GetScale()
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiX = graphics.DpiX;
                return dpiX / 96;
            }
        }

        // Determine if dark colors (theme) are being used
        public static bool isDark()
        {
            const string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string valueName = "AppsUseLightTheme";

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
            {
                if (key != null)
                {
                    object value = key.GetValue(valueName);
                    if (value is int intValue)
                    {
                        return intValue == 0;
                    }
                }
            }
            return false; // Return false if the key or value is missing
        }

        // Check if Windows Terminal is installed
        static bool IsWindowsTerminalInstalled()
        {
            try
            {
                // Check if wt.exe exists in PATH
                string pathEnv = Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrEmpty(pathEnv))
                {
                    string[] paths = pathEnv.Split(';');
                    foreach (string path in paths)
                    {
                        try
                        {
                            string wtPath = Path.Combine(path.Trim(), "wt.exe");
                            if (File.Exists(wtPath))
                            {
                                return true;
                            }
                        }
                        catch { }
                    }
                }

                // Check common installation locations
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string wtPathLocal = Path.Combine(localAppData, @"Microsoft\WindowsApps\wt.exe");
                if (File.Exists(wtPathLocal))
                {
                    return true;
                }

                // Check Program Files
                string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string wtPathProgramFiles = Path.Combine(programFiles, @"WindowsApps\Microsoft.WindowsTerminal_*\wt.exe");
                if (Directory.Exists(Path.Combine(programFiles, "WindowsApps")))
                {
                    string[] wtFiles = Directory.GetFiles(Path.Combine(programFiles, "WindowsApps"), "wt.exe", SearchOption.AllDirectories);
                    if (wtFiles.Length > 0)
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        // Check if PowerShell Core is installed
        static bool IsPowerShellCoreInstalled()
        {
            try
            {
                // Check if pwsh.exe exists in PATH
                string pathEnv = Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrEmpty(pathEnv))
                {
                    string[] paths = pathEnv.Split(';');
                    foreach (string path in paths)
                    {
                        try
                        {
                            string pwshPath = Path.Combine(path.Trim(), "pwsh.exe");
                            if (File.Exists(pwshPath))
                            {
                                return true;
                            }
                        }
                        catch { }
                    }
                }

                // Check Program Files
                string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string pwshPathProgramFiles = Path.Combine(programFiles, @"PowerShell\7\pwsh.exe");
                if (File.Exists(pwshPathProgramFiles))
                {
                    return true;
                }

                // Also check if custom path is configured in INI and file exists
                string customPath = ReadString(myIniFile, "PowerShellCoreHere", "Exe", "");
                if (!string.IsNullOrEmpty(customPath) && File.Exists(customPath))
                {
                    return true;
                }
            }
            catch { }

            return false;
        }

        // Make dialog title bar black
        public enum DWMWINDOWATTRIBUTE : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
        }

        private const int DWMWCP_ROUND = 2;

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref int pvAttribute, uint cbAttribute);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, UIntPtr wParam,
            string lParam, uint fuFlags, uint uTimeout, out UIntPtr lpdwResult);

        static void BroadcastThemeChange()
        {
            UIntPtr result;
            // Notify Explorer and all top-level windows that the color scheme has changed.
            // This is the same message Windows sends when you change Light/Dark in Personalization Settings.
            SendMessageTimeout((IntPtr)0xFFFF /*HWND_BROADCAST*/, 0x001A /*WM_SETTINGCHANGE*/,
                UIntPtr.Zero, "ImmersiveColorSet", 0x0002 /*SMTO_ABORTIFHUNG*/, 1000, out result);
        }

        static void DarkTitleBar(IntPtr hWnd)
        {
            // Dark mode title bar is only supported on Windows 10 build 17763 (October 2018 Update) and later
            if (buildNumber >= 17763)
            {
                var preference = Convert.ToInt32(true);
                DwmSetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref preference, sizeof(uint));
            }
        }

        static string ReadString(string iniFile, string section, string key, string defaultValue)
        {
            try
            {
                if (File.Exists(iniFile))
                {
                    return IniFileParser.ReadValue(section, key, defaultValue, iniFile);
                }
            }
            catch { }

            return defaultValue;
        }

        static void WriteString(string iniFile, string section, string key, string value)
        {
            try
            {
                IniFileParser.WriteValue(section, key, value, iniFile);
            }
            catch { }
        }

        // INI file parser
        public static class IniFileParser
        {
            public static string ReadValue(string section, string key, string defaultValue, string filePath)
            {
                try
                {
                    var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                    string currentSection = null;

                    foreach (var line in lines)
                    {
                        string trimmedLine = line.Trim();

                        if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                        {
                            currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                        }
                        else if (currentSection != null && currentSection.Equals(section, StringComparison.OrdinalIgnoreCase))
                        {
                            var parts = trimmedLine.Split(new char[] { '=' }, 2);
                            if (parts.Length == 2 && parts[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                            {
                                return parts[1].Trim();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                return defaultValue;
            }

            public static void WriteValue(string section, string key, string value, string filePath)
            {
                try
                {
                    var lines = File.Exists(filePath) ? File.ReadAllLines(filePath, Encoding.UTF8).ToList() : new List<string>();
                    string currentSection = null;
                    int sectionIndex = -1;
                    int keyIndex = -1;

                    for (int i = 0; i < lines.Count; i++)
                    {
                        string trimmedLine = lines[i].Trim();

                        if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                        {
                            currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                            if (currentSection.Equals(section, StringComparison.OrdinalIgnoreCase))
                            {
                                sectionIndex = i;
                            }
                        }
                        else if (currentSection != null && currentSection.Equals(section, StringComparison.OrdinalIgnoreCase))
                        {
                            var parts = trimmedLine.Split(new char[] { '=' }, 2);
                            if (parts.Length >= 1 && parts[0].Trim().TrimStart(';').Equals(key, StringComparison.OrdinalIgnoreCase))
                            {
                                keyIndex = i;
                                break;
                            }
                        }
                    }

                    if (keyIndex >= 0)
                    {
                        lines[keyIndex] = $"{key}={value}";
                    }
                    else if (sectionIndex >= 0)
                    {
                        lines.Insert(sectionIndex + 1, $"{key}={value}");
                    }
                    else
                    {
                        lines.Add($"[{section}]");
                        lines.Add($"{key}={value}");
                    }

                    File.WriteAllLines(filePath, lines, Encoding.UTF8);
                }
                catch (Exception)
                {
                }
            }

            // Copies any sections present in sourceFile but absent in destFile into destFile.
            // Existing sections in destFile are never modified.
            // Reads the revision integer from the first non-empty line of an INI file.
            // Expects the line to be in the form  ;Rev=<n>  (comment style).
            // Returns 0 if no revision line is found or it cannot be parsed.
            public static int ReadIniRevision(string filePath)
            {
                try
                {
                    foreach (var line in File.ReadLines(filePath, Encoding.UTF8))
                    {
                        string t = line.Trim();
                        if (t.Length == 0) continue;

                        // Accept ";Rev=n" (comment style)
                        if (t.StartsWith(";", StringComparison.Ordinal))
                            t = t.Substring(1).Trim();

                        if (t.StartsWith("Rev=", StringComparison.OrdinalIgnoreCase))
                        {
                            int rev;
                            if (int.TryParse(t.Substring(4).Trim(), out rev))
                                return rev;
                        }

                        // Stop after the first non-empty line
                        break;
                    }
                }
                catch { }
                return 0;
            }

            // Writes (or replaces) the revision comment on the first line of an INI file.
            public static void UpdateIniRevision(string filePath, int revision)
            {
                try
                {
                    var lines = File.Exists(filePath)
                        ? new System.Collections.Generic.List<string>(File.ReadAllLines(filePath, Encoding.UTF8))
                        : new System.Collections.Generic.List<string>();

                    string revLine = $";Rev={revision}";

                    if (lines.Count > 0)
                    {
                        string first = lines[0].Trim();
                        // Replace an existing revision line
                        bool isRevLine = false;
                        if (first.StartsWith(";", StringComparison.Ordinal))
                        {
                            string inner = first.Substring(1).Trim();
                            isRevLine = inner.StartsWith("Rev=", StringComparison.OrdinalIgnoreCase);
                        }
                        if (isRevLine)
                            lines[0] = revLine;
                        else
                            lines.Insert(0, revLine);
                    }
                    else
                    {
                        lines.Add(revLine);
                    }

                    File.WriteAllLines(filePath, lines, Encoding.UTF8);
                }
                catch { }
            }

            // Copies any sections present in sourceFile but absent in destFile into destFile.
            // Existing sections in destFile are never modified.
            public static void MergeMissingSections(string sourceFile, string destFile)
            {
                try
                {
                    var sourceLines = File.ReadAllLines(sourceFile, Encoding.UTF8);
                    var destLines   = File.ReadAllLines(destFile,   Encoding.UTF8);

                    // Collect section names already present in the destination file
                    var destSections = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var line in destLines)
                    {
                        string t = line.Trim();
                        if (t.StartsWith("[") && t.EndsWith("]"))
                            destSections.Add(t.Substring(1, t.Length - 2));
                    }

                    // Parse source into ordered list of (sectionName, lines-including-header)
                    var sourceSections = new System.Collections.Generic.List<KeyValuePair<string, System.Collections.Generic.List<string>>>();
                    string currentSection = null;
                    System.Collections.Generic.List<string> currentBlock = null;

                    foreach (var line in sourceLines)
                    {
                        string t = line.Trim();
                        if (t.StartsWith("[") && t.EndsWith("]"))
                        {
                            if (currentSection != null)
                                sourceSections.Add(new KeyValuePair<string, System.Collections.Generic.List<string>>(currentSection, currentBlock));
                            currentSection = t.Substring(1, t.Length - 2);
                            currentBlock = new System.Collections.Generic.List<string> { line };
                        }
                        else if (currentBlock != null)
                        {
                            currentBlock.Add(line);
                        }
                    }
                    if (currentSection != null)
                        sourceSections.Add(new KeyValuePair<string, System.Collections.Generic.List<string>>(currentSection, currentBlock));

                    // Build lines to append: only sections missing from dest
                    var toAppend = new System.Collections.Generic.List<string>();
                    foreach (var kvp in sourceSections)
                    {
                        if (!destSections.Contains(kvp.Key))
                        {
                            // Collect non-comment lines for this block
                            var block = new System.Collections.Generic.List<string>();
                            foreach (var line in kvp.Value)
                            {
                                string t = line.Trim();
                                if (!t.StartsWith(";") && !t.StartsWith("#"))
                                    block.Add(line);
                            }

                            // Drop trailing blank lines from the block
                            while (block.Count > 0 && block[block.Count - 1].Trim().Length == 0)
                                block.RemoveAt(block.Count - 1);

                            if (block.Count > 0)
                            {
                                toAppend.Add(""); // blank separator line
                                toAppend.AddRange(block);
                            }
                        }
                    }

                    if (toAppend.Count > 0)
                    {
                        var result = new System.Collections.Generic.List<string>(destLines);

                        // Strip trailing blank lines from the destination so the separator
                        // we add below always produces exactly one blank line between sections.
                        while (result.Count > 0 && result[result.Count - 1].Trim().Length == 0)
                            result.RemoveAt(result.Count - 1);

                        result.AddRange(toAppend);
                        File.WriteAllLines(destFile, result, Encoding.UTF8);
                    }
                }
                catch
                {
                }
            }
        }


        // Get language from INI file or system
        static string GetLang()
        {
            string lang = ReadString(myIniFile, "General", "Lang", "");
            if (lang != "") return lang;

            lang = "en";

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\International");
                if (key != null)
                {
                    lang = key.GetValue("LocaleName") as string;
                    key.Close();
                }
            }
            catch { }

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop");
                if (key != null)
                {
                    string[] preferredLanguages = key.GetValue("PreferredUILanguages") as string[];
                    if (preferredLanguages != null && preferredLanguages.Length > 0)
                    {
                        lang = preferredLanguages[0];
                    }
                    key.Close();
                }
            }
            catch { }

            return lang.Substring(0, 2).ToLower();
        }

        static void Setup()
        {
            // Determine if Custom Context Menu (third-party app) is available
            CCMfolder = FindCustomCommandsFolder(true);
            bool showCCM = CCMfolder != null;

            // Check current Win10ContextMenu status if on Win11
            bool showWin11Toggle = Win11;
            if (Win11)
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(CCMB))
                {
                    Win10ContextMenu = key != null;
                }
            }

            // Show the setup dialog
            SetupDialog dialog = SetupDialog.Show(sSetup, sMain, showCCM, showWin11Toggle);

            if (dialog.DialogResult == DialogResult.Cancel)
            {
                dialog.Dispose();
                return;
            }

            // Store checkbox states before disposal
            bool installClassicContextMenu = dialog.InstallClassicContextMenu;
            bool installTask = dialog.InstallTask;
            bool installCustomContextMenu = dialog.InstallCustomContextMenu;
            bool enableWin11ClassicMenu = dialog.EnableWin11ClassicMenu;

            dialog.Dispose();

            // Process classic context menu (registry entries)
            bool currentlyInstalledClassic = IsClassicContextMenuInstalled();
            if (installClassicContextMenu && !currentlyInstalledClassic)
            {
                // Install classic context menu entries
                InstallContextMenuEntries(@"Drive", true);
                InstallContextMenuEntries(@"Directory", true);
                InstallContextMenuEntries(@"Directory\Background", true);
            }
            else if (!installClassicContextMenu && currentlyInstalledClassic)
            {
                // Remove classic context menu entries
                RemoveContextMenuEntries(true);
            }

            // Process task installation/removal (admin only)
            if (isAdmin)
            {
                bool taskCurrentlyExists = TaskExists();
                if (installTask && !taskCurrentlyExists)
                {
                    // Install task
                    CommandLine = "/TaskInstallQuiet";
                    RunUAC(myExe);
                }
                else if (!installTask && taskCurrentlyExists)
                {
                    // Remove task
                    CommandLine = "/TaskRemoveQuiet";
                    RunUAC(myExe);
                }
            }

            // Process Custom Context Menu (third-party app JSON files)
            if (showCCM)
            {
                bool currentlyInstalledCCM = IsCustomContextMenuInstalled();
                if (installCustomContextMenu && !currentlyInstalledCCM)
                {
                    // Install RightClickTools into Custom Context Menu app
                    InstallContextMenuEntriesCCM(true);
                }
                else if (!installCustomContextMenu && currentlyInstalledCCM)
                {
                    // Remove RightClickTools from Custom Context Menu app
                    RemoveContextMenuEntriesCCM(true);
                }
            }

            // Process Windows 11 classic/modern context menu toggle
            if (showWin11Toggle)
            {
                if (enableWin11ClassicMenu && !Win10ContextMenu)
                {
                    // Enable Windows 11 classic context menu
                    try
                    {
                        Registry.SetValue($@"HKEY_CURRENT_USER\{CCMB}", "", "", RegistryValueKind.String);
                        StartDirectory = myPath;

                        // Ask user for confirmation before restarting Explorer
                        DialogResult result = TwoChoiceBox.Show(sRestartExplorerPrompt, sMain, sYes, sNo);
                        if (result == DialogResult.Yes)
                        {
                            RestartExplorer();
                        }
                    }
                    catch { }
                }
                else if (!enableWin11ClassicMenu && Win10ContextMenu)
                {
                    // Disable Windows 11 classic context menu (use modern menu)
                    try
                    {
                        Registry.CurrentUser.DeleteSubKeyTree(CCMA, false);
                        StartDirectory = myPath;

                        // Ask user for confirmation before restarting Explorer
                        DialogResult result = TwoChoiceBox.Show(sRestartExplorerPrompt, sMain, sYes, sNo);
                        if (result == DialogResult.Yes)
                        {
                            RestartExplorer();
                        }
                    }
                    catch { }
                }
            }

            CustomMessageBox.Show(sDone, sMain);
        }

        static bool IsClassicContextMenuInstalled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Drive\shell\" + myName))
                {
                    if (key != null) return true;
                }
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Directory\shell\" + myName))
                {
                    if (key != null) return true;
                }
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Directory\Background\shell\" + myName))
                {
                    if (key != null) return true;
                }
            }
            catch { }
            return false;
        }

        static bool IsCustomContextMenuInstalled()
        {
            if (CCMfolder == null) return false;

            try
            {
                DirectoryInfo directory = new DirectoryInfo(CCMfolder);
                foreach (FileInfo file in directory.GetFiles("*.JSON"))
                {
                    if (File.ReadAllText(file.FullName).Contains(myName))
                    {
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        static void ContextMenuInstall(bool HKCU)
        {
            RemoveContextMenuEntries(HKCU);
            RemoveContextMenuEntriesCCM(HKCU);
            InstallContextMenuEntries(@"Drive", HKCU);
            InstallContextMenuEntries(@"Directory", HKCU);
            InstallContextMenuEntries(@"Directory\Background", HKCU);
            InstallContextMenuEntriesCCM(HKCU);
            if (HKCU) SetWin11ContextMenu();
        }
        static void ContextMenuRemove(bool HKCU)
        {
            RemoveContextMenuEntries(HKCU);
            RemoveContextMenuEntriesCCM(HKCU);
            if (HKCU) SetWin11ContextMenu();
        }

        static void Install(bool interactive)
        {
            ContextMenuInstall(true);

            if (!isAdmin)
            {
                if (interactive) CustomMessageBox.Show(sDone, sMain);
                return;
            }

            if (interactive) addTask = checkboxTask.Checked;

            if (addTask)
            {
                CommandLine = "/TaskInstallQuiet";
                if (interactive) CommandLine = "/TaskInstall";
                RunUAC(myExe);
            }
            else
            {
                if (interactive) CustomMessageBox.Show(sDone, sMain);
            }
        }

        static void Remove(bool interactive)
        {
            ContextMenuRemove(true);

            if (!isAdmin)
            {
                if (interactive) CustomMessageBox.Show(sDone, sMain);
                return;
            }

            if (removeTask && TaskExists())
            {
                CommandLine = "/TaskRemoveQuiet";
                if (interactive) CommandLine = "/TaskRemove";
                RunUAC(myExe);
            }
            else
            {
                if (interactive) CustomMessageBox.Show(sDone, sMain);
            }

        }

        static void HKUInstall()
        {
            CCMfolder = FindCustomCommandsFolder(false);
            ContextMenuInstall(false);
            if (addTask) TaskInstall(false);
        }

        static void HKURemove()
        {
            CCMfolder = FindCustomCommandsFolder(false);
            ContextMenuRemove(false);
            TaskRemove(false);
        }

        static void SetWin11ContextMenu()
        {
            if (!Win11) return;

            string H = @"HKEY_CURRENT_USER\";

            if (checkboxCCM.Checked && !Win10ContextMenu)
            {
                try
                {
                    Registry.SetValue($@"{H}{CCMB}", "", "", RegistryValueKind.String);
                    StartDirectory = myPath;
                    RestartExplorer();
                }
                catch
                {
                }
            }

            if (!checkboxCCM.Checked && Win10ContextMenu)
            {
                try
                {
                    Registry.CurrentUser.DeleteSubKeyTree(CCMA, false);
                }
                catch
                {
                }
            }
        }

        static void TaskInstall(bool interactive)
        {
            Process p = new Process();
            p.StartInfo.FileName = SchTasksExe;
            p.StartInfo.Arguments = $"/delete /f /tn \"{TaskName}\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();

            string XMLFile = $@"{TempPath}Task.xml";
            string XMLData = File.ReadAllText($@"{appParts}\Task.cfg");
            XMLData = XMLData.Replace("{myPath}", myPath);
            XMLData = XMLData.Replace("{bitPath}", bitPath);
            File.WriteAllText(XMLFile, XMLData, Encoding.Unicode);

            p = new Process();
            p.StartInfo.FileName = SchTasksExe;
            p.StartInfo.Arguments = $"/create /f /xml \"{XMLFile}\" /tn \"{TaskName}\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();

            File.Delete(XMLFile);

            if (interactive) CustomMessageBox.Show(sDone, sMain);
        }

        static void TaskRemove(bool interactive)
        {
            Process p = new Process();
            p.StartInfo.FileName = SchTasksExe;
            p.StartInfo.Arguments = $"/delete /f /tn \"{TaskName}\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();

            if (interactive) CustomMessageBox.Show(sDone, sMain);
        }

        static void InstallContextMenuEntries(string thiskey, bool HKCU)
        {
            RegistryKey baseKey = Registry.CurrentUser;

            if (!HKCU)
            {
                if (userSID == "") return;
                baseKey = Registry.Users.OpenSubKey(userSID, true);
            }

            string MyKey = $@"Software\Classes\{thiskey}\shell\{myName}";

            using (RegistryKey key = baseKey.CreateSubKey(MyKey))
            {
                key.SetValue("SubCommands", "");
                key.SetValue("", "");
                key.SetValue("MUIVerb", sMain);
                key.SetValue("Icon", myExe);
            }

            for (int i = 0; i < CmdKeys.Length; i++)
            {
                using (RegistryKey key = baseKey.CreateSubKey($@"{MyKey}\shell\{i:D2}-{CmdKeys[i]}"))
                {
                    key.SetValue("", CmdLabels[i]);
                    key.SetValue("Icon", $@"{appParts}\Icons\{CmdKeys[i]}.ico");

                    using (RegistryKey commandKey = key.CreateSubKey("command"))
                    {
                        string CmdLine = $"\"{myExe}\" /{CmdKeys[i]}";
                        if (CmdKeys[i].Substring(CmdKeys[i].Length - 4) == "Here") CmdLine += " \"%v|\"";
                        commandKey.SetValue("", CmdLine);
                    }
                }
            }
        }

        static void InstallContextMenuEntriesCCM(bool HKCU)
        {
            if (CCMfolder == null) return;
            string exe = myExe.Replace(@"\", @"\\");

            for (int i = 0; i < CmdKeys.Length; i++)
            {
                string icon = $@"{appParts}\Icons\{CmdKeys[i]}.ico";
                icon = icon.Replace(@"\", @"\\");
                string JSONFile = $@"{CCMfolder}\{CmdLabels[i]}.JSON";
                string JSONData = File.ReadAllText($@"{appParts}\JSON.cfg");
                JSONData = JSONData.Replace("{i}", $"{i + 100}");
                JSONData = JSONData.Replace("{label}", CmdLabels[i]);
                JSONData = JSONData.Replace("{exe}", exe);
                JSONData = JSONData.Replace("{cmdline}", $@"/{CmdKeys[i]}");
                JSONData = JSONData.Replace("{icon}", icon);
                File.WriteAllText(JSONFile, JSONData);
            }
        }

        static void RemoveContextMenuEntriesCCM(bool HKCU)
        {
            if (CCMfolder == null) return;

            DirectoryInfo directory = new DirectoryInfo(CCMfolder);

            foreach (FileInfo file in directory.GetFiles("*.JSON"))
            {
                if (File.ReadAllText(file.FullName).Contains(myName))
                {
                    file.Delete();
                }
            }
        }

        static void RemoveContextMenuEntries(bool HKCU)
        {
            RegistryKey baseKey = Registry.CurrentUser;
            if (!HKCU) baseKey = Registry.Users.OpenSubKey(userSID, true);

            using (RegistryKey key = baseKey.OpenSubKey(@"Software\Classes\Drive\shell", true))
            {
                try { key.DeleteSubKeyTree(myName, false); }
                catch { }
            }
            using (RegistryKey key = baseKey.OpenSubKey(@"Software\Classes\Directory\shell", true))
            {
                try { key.DeleteSubKeyTree(myName, false); }
                catch { }
            }
            using (RegistryKey key = baseKey.OpenSubKey(@"Software\Classes\Directory\Background\shell", true))
            {
                try { key.DeleteSubKeyTree(myName, false); }
                catch { }
            }

        }

        static string FindCustomCommandsFolder(bool HKCU)
        {
            string packagesFolderPath = $@"{Environment.GetEnvironmentVariable("LocalAppData")}\Packages";

            if (!HKCU)
            {
                string keyPath = $@"HKEY_USERS\{userSID}\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders";
                packagesFolderPath = $@"{(string)Registry.GetValue(keyPath, @"Local AppData", "")}\Packages";
            }

            if (!Directory.Exists(packagesFolderPath)) return null;

            string[] packageFolders = Directory.GetDirectories(packagesFolderPath);

            foreach (string packageFolder in packageFolders)
            {
                string customCommandsPath = Path.Combine(packageFolder, @"LocalState\custom_commands");
                if (Directory.Exists(customCommandsPath)) return customCommandsPath;
            }
            return null;
        }
    }
}

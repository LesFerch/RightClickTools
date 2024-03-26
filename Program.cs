using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace RightClickTools
{
    class Program
    {
        static string myName = typeof(Program).Namespace;
        static string myPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        static string myExe = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string TempPath = Path.GetTempPath(); //Includes trailing backslash
        static string ElevateCfg = $@"{TempPath}Elevate.cfg";
        static string appParts = $@"{myPath}\AppParts";
        static string myIcon = $@"{myPath}\AppParts\Icons\{myName}.ico";
        static string AdvKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        static string bitPath = "64";
        static bool Hidden = (int)Registry.GetValue(AdvKey, "Hidden", 0) == 1;
        static string NTkey = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion";
        static int buildNumber = int.Parse(Registry.GetValue(NTkey, "CurrentBuild", "").ToString());
        static bool Win11 = buildNumber >= 21996;
        static bool Win11Install = false;
        static string CCMfolder = FindCustomCommandsFolder();
        static string CCMA = @"Software\Classes\CLSID\{86CA1AA0-34AA-4E8B-A509-50C905BAE2A2}";
        static string CCMB = $@"{CCMA}\InprocServer32";
        static bool Win10ContextMenu = false;

        static string sMain = "Right-Click Tools";
        static string sSetup = "Install or Remove this tool";
        static string sOK = "OK";
        static string sYes = "Yes";
        static string sNo = "No";
        static string sInstall = "Install";
        static string sRemove = "Remove";
        static string sDone = "Done";
        static string[] CmdKeys = { "CmdHere", "CmdAdminHere", "CmdTrustedHere", "PowerShellHere", "PowerShellAdminHere", "PowerShellTrustedHere", "RegEdit", "RegEditAdmin", "RegEditTrusted", "ClearHistory", "TakeOwnHere", "AddDelPathHere", "ShowHide", "RefreshShell", "RestartExplorerHere" };
        static string[] CmdLabels = { "Cmd here", "Cmd here as Administrator", "Cmd here as TrustedInstaller", "PowerShell here", "PowerShell here as Administrator", "PowerShell here as TrustedInstaller", "RegEdit as User", "RegEdit as Administrator", "RegEdit as TrustedInstaller", "Clear History", "Take ownership and get access", "Add or Remove folder in Path variable", "Toggle display of hidden and system files", "Refresh shell", "Restart Explorer" };
        static string sClearHistory = CmdLabels[9];
        static string sTakeOwnHere = CmdLabels[10];
        static string sRestartExplorer = CmdLabels[14];
        static string sFolderNotAllowed = "Not allowed for this folder";
        static string sWarningTakeOwn = "WARNING: Other users may lose access";
        static string sUserPath = "User Path";
        static string sSystemPath = "System Path";
        static string sRecent = "Recent items";
        static string sAutoSuggest = "Auto-suggest items";
        static string sTemp = "Temporary files";
        static string sDefender = "Defender history";
        static string sCCM = "Classic context menu";

        static string Option = "";
        static string StartDirectory = "";
        static string CommandLine = "";

        static float ScaleFactor = GetScale();
        static bool Dark = isDark();
        static bool isAdmin = false;
        static bool ctrlKey = false;

        static string UserKey = @"HKEY_CURRENT_USER\Environment";
        static string SystemKey = @"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment";
        static string UserPath = (string)Registry.GetValue(UserKey, "Path", "");
        static string SystemPath = (string)Registry.GetValue(SystemKey, "Path", "");
        static bool InUserPath = false;
        static bool InSystemPath = false;
        static int pathLength = UserPath.Length + SystemPath.Length;

        static string CmdExe = @"C:\Windows\System32\Cmd.exe";
        static string PowerShellExe = @"C:\Windows\System32\WindowsPowerShell\v1.0\PowerShell.exe";
        static string RegEditExe = @"C:\Windows\RegEdit.exe";
        static string SchTasksExe = @"C:\Windows\System32\SchTasks.exe";

        static string UserName = Environment.GetEnvironmentVariable("UserName");
        static string TaskName = $@"MyTasks\{myName}-{UserName}";
        static string helpPage = "install-and-remove";

        static CheckBox userPathCheckbox;
        static CheckBox systemPathCheckbox;

        static CheckBox RecentItemsCheckbox;
        static CheckBox AutoSuggestCheckbox;
        static CheckBox TempFilesCheckbox;
        static CheckBox DefenderCheckbox;
        static CheckBox checkBoxCCM;


        static void Main(string[] args)
        {
            // If the current folder is a long path, the Elevate function will fail, so let's make C:\ the current folder.
            Directory.SetCurrentDirectory(@"C:\");

            ctrlKey = (GetAsyncKeyState(0x11) & 0x8000) != 0; //Detect if Ctrl key is pressed

            if (!Environment.Is64BitOperatingSystem) bitPath = "32";

            LoadLanguageStrings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            isAdmin = IsCurrentUserInAdminGroup();

            if (args.Length == 0) { InstallRemove(); return; }

            Option = args[0];

            if (args.Length > 1)
            {
                StartDirectory = args[1].Replace("|","");
            }

            switch (Option)
            {
                case "/Install":
                    Install();
                    break;

                case "/Remove":
                    Remove();
                    break;

                case "/UACInstall":
                    UACInstall();
                    break;

                case "/Elevate":
                    if (args.Length > 1) { ElevateCfg = args[1]; }
                    Elevate();
                    break;

                case "/UACRemove":
                    UACRemove();
                    break;

                case "/CmdHere":
                    RunAsUser(CmdExe);
                    break;

                case "/CmdAdminHere":
                    RunAsAdmin(CmdExe);
                    break;

                case "/CmdTrustedHere":
                    RunAsTrusted(CmdExe);
                    break;

                case "/PowerShellHere":
                    RunAsUser(PowerShellExe);
                    break;

                case "/PowerShellAdminHere":
                    RunAsAdmin(PowerShellExe);
                    break;

                case "/PowerShellTrustedHere":
                    RunAsTrusted(PowerShellExe);
                    break;

                case "/RegEdit":
                    Environment.SetEnvironmentVariable("__COMPAT_LAYER", "RUNASINVOKER");
                    if (ctrlKey) clearRegEdit();
                    CommandLine = "/m";
                    RunAsUser(RegEditExe);
                    break;

                case "/RegEditAdmin":
                    CommandLine = "/m";
                    RunAsAdmin(RegEditExe);
                    break;

                case "/RegEditTrusted":
                    CommandLine = "/m";
                    RunAsTrusted(RegEditExe);
                    break;

                case "/TakeOwnHere":
                    RunTakeOwnHerePS1AsAdmin();
                    break;

                case "/AddDelPathHere":
                    AddDelPathHere();
                    break;

                case "/AddPathAdmin":
                    AddPathAdmin();
                    break;

                case "/DelPathAdmin":
                    DelPathAdmin();
                    break;

                case "/ShowHide":
                    ShowHide();
                    break;

                case "/ClearHistory":
                    ClearHistory();
                    break;

                case "/RefreshShell":
                    RefreshShell();
                    break;

                case "/RestartExplorerHere":
                    DialogResult result = TwoChoiceBox.Show(sRestartExplorer, sMain, sYes, sNo);
                    if (result != DialogResult.Yes) return;
                    RefreshShell();
                    RestartExplorer();
                    break;

                default:
                    return;
            }
        }

        static bool IsCurrentUserInAdminGroup()
        {
            var claims = new WindowsPrincipal(WindowsIdentity.GetCurrent()).Claims;
            var adminClaimID = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Value;
            return claims.Any(c => c.Value == adminClaimID);
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

            if (result == DialogResult.OK)
            {
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
                    var filesAndFolders = Directory.GetFileSystemEntries(TempPath, "*", SearchOption.TopDirectoryOnly);

                    foreach (var entry in filesAndFolders)
                    {
                        try
                        {
                            if (File.Exists(entry))
                            {
                                File.Delete(entry);
                            }
                            else if (Directory.Exists(entry))
                            {
                                Directory.Delete(entry, true);
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                if (DefenderCheckbox.Checked)
                {
                    RunClearDefenderPS1AsTrusted();
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

            if (result == DialogResult.OK)
            {
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
                        CommandLine = $"/AddPathAdmin \"{StartDirectory}\"";
                    }
                    else
                    {
                        CommandLine = $"/DelPathAdmin \"{StartDirectory}\"";
                    }
                    RunElevated(myExe, "Administrator");
                }
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
            newPath = newPath.Replace(";;",";");
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
            ToggleHiddenFiles(!Hidden);
        }

        static void RefreshShell()
        {
            SHChangeNotify(0x08000000, 0x1000, IntPtr.Zero, IntPtr.Zero);

            if (buildNumber >= 14393)
            {
                ToggleHiddenFiles(!Hidden);
                ToggleHiddenFiles(Hidden);
            }
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
                int h1 = 1; int h2 = 1;
                if (Hidden) { h1 = 2; h2 = 0; }
                Registry.SetValue(AdvKey, "Hidden", h1, RegistryValueKind.DWord);
                Registry.SetValue(AdvKey, "ShowSuperHidden", h2, RegistryValueKind.DWord);
                Thread.Sleep(100);
                SendKeys.SendWait("{F5}");
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

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

        static void RestartExplorer()
        {
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
            Process.Start("explorer.exe", StartDirectory);
        }

        static void CreateChangeDirectoryFile(string EXEFilename)
        {
            if (EXEFilename == CmdExe)
            {
                string cdFile = $@"{TempPath}ChangeDirectory.cmd";
                StartDirectory = StartDirectory.Replace("%", "%%"); //Escape percent signs
                File.WriteAllText(cdFile, $"@chcp 65001>nul\r\n@cd /d \"{StartDirectory}\"");
                CommandLine = $"/k \"{cdFile}\"";
            }

            if (EXEFilename == PowerShellExe)
            {
                string cdFile = $@"{TempPath}ChangeDirectory.ps1";
                StartDirectory = StartDirectory.Replace("'", "''"); //Escape single quotes
                string Data = $@"Set-Location -LiteralPath '{StartDirectory}'";
                if (StartDirectory.Contains("~")) Data += "\r\nfunction Prompt {$shortPath = (New-Object -ComObject Scripting.FileSystemObject).GetFolder($pwd).ShortPath; return \"PS $($shortPath)> \"}";
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
            string PS1File = $@"{TempPath}TakeOwn.ps1";
            File.WriteAllText(PS1File, PS1Data, Encoding.UTF8);
            CommandLine = $"-NoLogo -NoProfile -ExecutionPolicy Bypass -file \"{PS1File}\"";
        }

        static void CreateClearDefenderPS1()
        {
            string PS1Data = $"$MAPS_Status = (Get-MpPreference).MAPSReporting\r\n";
            PS1Data += $"Set-MpPreference -DisableRealtimeMonitoring 1\r\n";
            PS1Data += $"Set-MpPreference -MAPSReporting Disabled\r\n";
            PS1Data += $"Get-ChildItem -File 'C:\\ProgramData\\Microsoft\\Windows Defender\\Scans\\History\\Service' -Recurse | Remove-Item -Force\r\n";
            PS1Data += $"Set-MpPreference -DisableRealtimeMonitoring 0\r\n";
            PS1Data += $"Set-MpPreference -MAPSReporting $MAPS_Status\r\n";
            string PS1File = $@"{TempPath}ClearDefender.ps1";
            File.WriteAllText(PS1File, PS1Data, Encoding.UTF8);
            CommandLine = $"-NoLogo -NoProfile -WindowStyle Hidden -ExecutionPolicy Bypass -file \"{PS1File}\"";
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

        static void clearRegEdit()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKey(@"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit", false);
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
            string Hidden = ReadString(iniFile, "Process", "Hidden", "0");
            bool hidden = Convert.ToBoolean(Convert.ToInt32(Hidden));

            if (RunAs == "TrustedInstaller")
            {
                ServiceController sc = new ServiceController
                {
                    ServiceName = "TrustedInstaller",
                };
                if (sc.Status != ServiceControllerStatus.Running) sc.Start();
                Process[] proc = Process.GetProcessesByName("TrustedInstaller");
                TrustedInstaller.Run(proc[0].Id, $"{EXEFilename} {CommandLine}");
            }
            else
            {
                if (ctrlKey && (EXEFilename == RegEditExe)) clearRegEdit();

                Process p = new Process();
                p.StartInfo.FileName = EXEFilename;
                p.StartInfo.Arguments = CommandLine;
                p.StartInfo.WorkingDirectory = @"C:\";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = hidden;
                p.Start();
            }

        }

        static void RunElevated(string EXEFilename, string mode) 
        {
            CreateChangeDirectoryFile(EXEFilename);

            string cfg = $"[Process]\r\nEXEFilename={EXEFilename}\r\nCommandLine={CommandLine}\r\nRunAs={mode}";

            File.WriteAllText(ElevateCfg, cfg);

            if (isAdmin)
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
                    RunUAC(EXEFilename);
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

        static void RunTakeOwnHerePS1AsAdmin()
        {
            string iniFile = $@"{appParts}\{myName}.ini";

            string sStopAll = ReadString(iniFile, "TakeOwnHere", "StopAll", "");
            string[] StopAll = sStopAll.Split(new char[] { '|' });

            string sStopRoot = ReadString(iniFile, "TakeOwnHere", "StopRoot", "");
            string[] StopRoot = sStopRoot.Split(new char[] { '|' });

            bool Stop = false;

            for (int i = 0; i < StopAll.Length; i++)
            {
                if (StartsWith(StopAll[i], StartDirectory)) { Stop = true; break; }
            }

            for (int i = 0; i < StopRoot.Length; i++)
            {
                if (StrCmp(StopRoot[i], StartDirectory)) { Stop = true; break; }
            }

            helpPage = "take-ownership-and-get-access";

            if (Stop) { CustomMessageBox.Show($"{sFolderNotAllowed}:\n\n{StartDirectory}\n\n", sMain); return; }

            string UserProfile = Environment.GetEnvironmentVariable("UserProfile");
            string sMsg = sTakeOwnHere;
            if (StartsWith(StartDirectory, "C:\\Users\\") && !StartsWith(StartDirectory, UserProfile)) sMsg = $"{sWarningTakeOwn}\r\n\r\n{sTakeOwnHere}";

            DialogResult result = TwoChoiceBox.Show($"{sMsg}?\n\n{StartDirectory}\n\n", sMain, sYes, sNo);
            if (result != DialogResult.Yes) return;

            CreateTakeOwnHerePS1();

            string cfg = $"[Process]\r\nEXEFilename={PowerShellExe}\r\nCommandLine={CommandLine}\r\nRunAs=Administrator";

            File.WriteAllText(ElevateCfg, cfg);

            if (isAdmin)
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

        static void RunClearDefenderPS1AsTrusted()
        {
            CreateClearDefenderPS1();

            string cfg = $"[Process]\r\nEXEFilename={PowerShellExe}\r\nCommandLine={CommandLine}\r\nRunAs=TrustedInstaller";

            File.WriteAllText(ElevateCfg, cfg);

            if (isAdmin)
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
                CommandLine = $"/Elevate \"{ElevateCfg}\"";
                RunUAC(myExe);
            }

        }

        static bool StrCmp(string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
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

        // Make dialog title bar black
        public enum DWMWINDOWATTRIBUTE : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref int pvAttribute, uint cbAttribute);

        static void DarkTitleBar(IntPtr hWnd)
        {
            var preference = Convert.ToInt32(true);
            DwmSetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref preference, sizeof(uint));

        }

        // Load language strings from INI file
        static void LoadLanguageStrings()
        {
            string iniFile = $@"{appParts}\language.ini";

            if (!File.Exists(iniFile)) return;

            string lang = GetLang();

            sMain = ReadString(iniFile, lang, "sMain", sMain);
            sSetup = ReadString(iniFile, lang, "sSetup", sSetup);
            sOK = ReadString(iniFile, lang, "sOK", sOK);
            sYes = ReadString(iniFile, lang, "sYes", sYes);
            sNo = ReadString(iniFile, lang, "sNo", sNo);
            sInstall = ReadString(iniFile, lang, "sInstall", sInstall);
            sRemove = ReadString(iniFile, lang, "sRemove", sRemove);
            sDone = ReadString(iniFile, lang, "sDone", sDone);
            sFolderNotAllowed = ReadString(iniFile, lang, "sFolderNotAllowed", sFolderNotAllowed);
            sWarningTakeOwn = ReadString(iniFile, lang, "sWarningTakeOwn", sWarningTakeOwn);
            sUserPath = ReadString(iniFile, lang, "sUserPath", sUserPath);
            sSystemPath = ReadString(iniFile, lang, "sSystemPath", sSystemPath);
            sRecent = ReadString(iniFile, lang, "sRecent", sRecent);
            sAutoSuggest = ReadString(iniFile, lang, "sAutoSuggest", sAutoSuggest);
            sTemp = ReadString(iniFile, lang, "sTemp", sTemp);
            sDefender = ReadString(iniFile, lang, "sDefender", sDefender);
            sCCM = ReadString(iniFile, lang, "sCCM", sCCM);

            string sCmdLabels = ReadString(iniFile, lang, "CmdLabels", "");
            string[] LangLabels = sCmdLabels.Split(new char[] { '|' });
            for (int i = 0; i < Math.Min(CmdLabels.Length, LangLabels.Length); i++)
            {
                CmdLabels[i] = LangLabels[i];
            }
            sClearHistory = CmdLabels[9];
            sTakeOwnHere = CmdLabels[10];
            sRestartExplorer = CmdLabels[14];
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
                        else if (currentSection == section)
                        {
                            var parts = trimmedLine.Split(new char[] { '=' }, 2);
                            if (parts.Length == 2 && parts[0].Trim() == key)
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
        }


        // Get the current system language
        static string GetLang()
        {
            string lang = "en";
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

        static void InstallRemove()
        {
            if (Win11)
            {
                Win11Install = true;
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(CCMB))
                {
                    Win10ContextMenu = key != null;
                }
            }

            DialogResult result = TwoChoiceBox.Show(sSetup, sMain, sInstall, sRemove);

            if (result == DialogResult.Yes)
            {
                Install();
            }
            if (result == DialogResult.No)
            {
                Remove();
            }
        }

        static void Install()
        {
            RemoveContextMenuEntries();
            RemoveContextMenuEntriesCCM();
            InstallContextMenuEntries(@"Drive");
            InstallContextMenuEntries(@"Directory");
            InstallContextMenuEntries(@"Directory\Background");
            InstallContextMenuEntriesCCM();
            SetWin11ContextMenu();

            if (!isAdmin) 
            {
                CustomMessageBox.Show(sDone, sMain);
                return;
            }

            CommandLine = "/UACInstall";
            RunUAC(myExe);
        }

        static void Remove()
        {
            RemoveContextMenuEntries();
            RemoveContextMenuEntriesCCM();
            SetWin11ContextMenu();

            if (!isAdmin)
            {
                CustomMessageBox.Show(sDone, sMain);
                return;
            }

            CommandLine = "/UACRemove";
            RunUAC(myExe);
        }

        static void SetWin11ContextMenu()
        {
            if (!Win11) return;

            string H = @"HKEY_CURRENT_USER\";

            if (checkBoxCCM.Checked && !Win10ContextMenu)
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

            if (!checkBoxCCM.Checked && Win10ContextMenu)
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

        static void UACInstall()
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

            CustomMessageBox.Show(sDone, sMain);
        }

        static void UACRemove()
        {
            Process p = new Process();
            p.StartInfo.FileName = SchTasksExe;
            p.StartInfo.Arguments = $"/delete /f /tn \"{TaskName}\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();

            CustomMessageBox.Show(sDone, sMain);
        }

        static void InstallContextMenuEntries(string thiskey)
        {
            string MyKey = $@"Software\Classes\{thiskey}\shell\{myName}";

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(MyKey))
            {
                key.SetValue("SubCommands", "");
                key.SetValue("", "");
                key.SetValue("MUIVerb", sMain);
                key.SetValue("Icon", myExe);
            }

            for (int i = 0; i < CmdKeys.Length; i++)
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey($@"{MyKey}\shell\{i:D2}-{CmdKeys[i]}"))
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

        static void InstallContextMenuEntriesCCM()
        {
            if (CCMfolder == null) return;
            //string zmyExe = "\\\"" + myExe.Replace(@"\", @"\\") + "\\\"";
            string exe = myExe.Replace(@"\", @"\\");

            for (int i = 0; i < CmdKeys.Length; i++)
            {
                string icon = $@"{appParts}\Icons\{CmdKeys[i]}.ico";
                //icon = "\\\"" + icon.Replace(@"\", @"\\") + "\\\"";
                icon = icon.Replace(@"\", @"\\");
                string JSONFile = $@"{CCMfolder}\{CmdLabels[i]}.JSON";
                string JSONData = File.ReadAllText($@"{appParts}\JSON.cfg");
                JSONData = JSONData.Replace("{i}", $"{i+100}");
                JSONData = JSONData.Replace("{label}", CmdLabels[i]);
                JSONData = JSONData.Replace("{exe}", exe);
                JSONData = JSONData.Replace("{cmdline}", $@"/{CmdKeys[i]}");
                JSONData = JSONData.Replace("{icon}", icon);
                File.WriteAllText(JSONFile, JSONData);
            }
        }

        static void RemoveContextMenuEntriesCCM()
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

        static void RemoveContextMenuEntries()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Drive\shell", true))
            {
                try { key.DeleteSubKeyTree(myName, false); }
                catch { }
            }
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Directory\shell", true))
            {
                try { key.DeleteSubKeyTree(myName, false); }
                catch { }
            }
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Directory\Background\shell", true))
            {
                try { key.DeleteSubKeyTree(myName, false); }
                catch { }
            }

        }

        static string FindCustomCommandsFolder()
        {
            string packagesFolderPath = $@"{Environment.GetEnvironmentVariable("LocalAppData")}\Packages";

            if (!Directory.Exists(packagesFolderPath)) return null;

            string[] packageFolders = Directory.GetDirectories(packagesFolderPath);

            foreach (string packageFolder in packageFolders)
            {
                string customCommandsPath = Path.Combine(packageFolder, @"LocalState\custom_commands");
                if (Directory.Exists(customCommandsPath)) return customCommandsPath;
            }
            return null;
        }


        // Dialog for simple OK messages
        public class CustomMessageBox : Form
        {
            private Label messageLabel;
            private Button buttonOK;

            public CustomMessageBox(string message, string caption)
            {
                message = $"\n{message}";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(300 * ScaleFactor);
                Height = (int)(150 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    Height = Math.Max(Height, (int)(size.Height + (int)(100 * ScaleFactor)));
                }


                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.DialogResult = DialogResult.OK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(32, 32, 32);
                    ForeColor = Color.White;
                }
                Controls.Add(buttonOK);
                Controls.Add(messageLabel);

                Point cursorPosition = Cursor.Position;
                int dialogX = Cursor.Position.X - Width / 2;
                int dialogY = Cursor.Position.Y - Height / 2 - (int)(50 * ScaleFactor);
                Screen screen = Screen.FromPoint(cursorPosition);
                int screenWidth = screen.WorkingArea.Width;
                int screenHeight = screen.WorkingArea.Height;
                int baseX = screen.Bounds.X;
                int baseY = screen.Bounds.Y;
                dialogX = Math.Max(baseX, Math.Min(baseX + screenWidth - Width, dialogX));
                dialogY = Math.Max(baseY, Math.Min(baseY + screenHeight - Height, dialogY));
                Location = new Point(dialogX, dialogY);
            }
            public static DialogResult Show(string message, string caption)
            {
                using (var customMessageBox = new CustomMessageBox(message, caption))
                {
                    return customMessageBox.ShowDialog();
                }
            }

        }

        // Dialog for install/Remove and Yes/No
        public class TwoChoiceBox : Form
        {
            private Label messageLabel;
            private Button buttonHelp;
            private Button buttonYes;
            private Button buttonNo;

            public TwoChoiceBox(string message, string caption, string button1, string button2)
            {
                int b2Width = (int)(75 * ScaleFactor);
                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(button2, new Font("Segoe UI", 9));
                    b2Width = Math.Max((int)size.Width, b2Width);
                }
                message = $"\n{message}";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                Text = caption;
                Width = (int)(300 * ScaleFactor);
                int h = 150; if (Win11Install) h = 164;
                Height = (int)(h * ScaleFactor);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    Height = Math.Max(Height, (int)(size.Height + (int)(100 * ScaleFactor)));
                }

                buttonHelp = new Button();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                buttonHelp.BackgroundImage = scaledImage;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.FlatAppearance.BorderSize = 0;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.DialogResult = DialogResult.None;
                buttonHelp.Click += ButtonHelp_Click;

                messageLabel.Padding = new Padding(0, 0, (int)(26 * ScaleFactor), 0);

                checkBoxCCM = new CheckBox();
                checkBoxCCM.Font = new Font("Segoe UI", 10);
                checkBoxCCM.Text = sCCM;
                checkBoxCCM.Checked = Win10ContextMenu;
                checkBoxCCM.AutoSize = true;
                checkBoxCCM.Location = new Point((int)(12 * ScaleFactor), (int)(58 * ScaleFactor));

                buttonYes = new Button();
                buttonYes.Text = button1;
                buttonYes.Font = new Font("Segoe UI", 9);
                buttonYes.MinimumSize = new Size((int)(75 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonYes.Left = (int)(10 * ScaleFactor);
                buttonYes.Top = ClientSize.Height - buttonYes.Height - (int)(12 * ScaleFactor);
                buttonYes.DialogResult = DialogResult.Yes;

                buttonNo = new Button();
                buttonNo.Text = button2;
                buttonNo.Font = new Font("Segoe UI", 9);
                buttonNo.MinimumSize = new Size((int)(75 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonNo.Left = ClientSize.Width - b2Width - (int)(16 * ScaleFactor);
                buttonNo.Top = ClientSize.Height - buttonNo.Height - (int)(12 * ScaleFactor);
                buttonNo.DialogResult = DialogResult.No;

                if (Dark)
                {
                    buttonYes.FlatStyle = FlatStyle.Flat;
                    buttonYes.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonYes.FlatAppearance.BorderSize = 1;
                    buttonNo.FlatStyle = FlatStyle.Flat;
                    buttonNo.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonNo.FlatAppearance.BorderSize = 1;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(32, 32, 32);
                    ForeColor = Color.White;
                }

                if (Win11Install) Controls.Add(checkBoxCCM);
                Controls.Add(buttonHelp);
                Controls.Add(buttonYes);
                Controls.Add(buttonNo);
                Controls.Add(messageLabel);

                Point cursorPosition = Cursor.Position;
                int dialogX = Cursor.Position.X - Width / 2;
                int x = 50; if (Win11Install) x = 40;
                int dialogY = Cursor.Position.Y - Height / 2 - (int)(x * ScaleFactor);
                Screen screen = Screen.FromPoint(cursorPosition);
                int screenWidth = screen.WorkingArea.Width;
                int screenHeight = screen.WorkingArea.Height;
                int baseX = screen.Bounds.X;
                int baseY = screen.Bounds.Y;
                dialogX = Math.Max(baseX, Math.Min(baseX + screenWidth - Width, dialogX));
                dialogY = Math.Max(baseY, Math.Min(baseY + screenHeight - Height, dialogY));
                Location = new Point(dialogX, dialogY);
            }

            public static DialogResult Show(string message, string caption, string button1, string button2)
            {
                using (var TwoChoiceBox = new TwoChoiceBox(message, caption, button1, button2))
                {
                    return TwoChoiceBox.ShowDialog();
                }
            }

        }

        static void ButtonHelp_Click(object sender, EventArgs e)
        {
            Process.Start("https://lesferch.github.io/RightClickTools#" + helpPage);
        }

        // Dialog for Add-Del Path
        public class AddDelPathDialog : Form
        {
            private Label messageLabel;
            private Button buttonHelp;
            private Button buttonOK;

            public AddDelPathDialog(string message, string caption)
            {
                message = $"\n\n\n\n{message}";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(300 * ScaleFactor);
                Height = (int)(150 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    Height = Math.Max(Height, (int)(size.Height + (int)(100 * ScaleFactor)));
                }

                buttonHelp = new Button();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                buttonHelp.BackgroundImage = scaledImage;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.FlatAppearance.BorderSize = 0;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.DialogResult = DialogResult.None;
                buttonHelp.Click += ButtonHelp_Click;
                helpPage = "add-or-remove-folder-in-path-variable";

                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.DialogResult = DialogResult.OK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);

                userPathCheckbox = new CheckBox();
                userPathCheckbox.Font = new Font("Segoe UI", 10);
                userPathCheckbox.Text = sUserPath;
                userPathCheckbox.Checked = InUserPath;
                userPathCheckbox.AutoSize = true;
                userPathCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(16 * ScaleFactor));

                systemPathCheckbox = new CheckBox();
                systemPathCheckbox.Font = new Font("Segoe UI", 10);
                systemPathCheckbox.Text = sSystemPath;
                systemPathCheckbox.Checked = InSystemPath;
                systemPathCheckbox.AutoSize = true;
                systemPathCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(40 * ScaleFactor));

                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;

                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(32, 32, 32);
                    ForeColor = Color.White;
                }

                Controls.Add(buttonHelp);
                Controls.Add(userPathCheckbox);
                Controls.Add(systemPathCheckbox);
                Controls.Add(buttonOK);
                Controls.Add(messageLabel);

                Point cursorPosition = Cursor.Position;
                int dialogX = Cursor.Position.X - Width / 2;
                int dialogY = Cursor.Position.Y - Height / 2 - (int)(50 * ScaleFactor);
                Screen screen = Screen.FromPoint(cursorPosition);
                int screenWidth = screen.WorkingArea.Width;
                int screenHeight = screen.WorkingArea.Height;
                int baseX = screen.Bounds.X;
                int baseY = screen.Bounds.Y;
                dialogX = Math.Max(baseX, Math.Min(baseX + screenWidth - Width, dialogX));
                dialogY = Math.Max(baseY, Math.Min(baseY + screenHeight - Height, dialogY));
                Location = new Point(dialogX, dialogY);
            }

            public static DialogResult Show(string message, string caption)
            {
                using (var AddDelPathDialog = new AddDelPathDialog(message, caption))
                {
                    return AddDelPathDialog.ShowDialog();
                }
            }
        }

        // Dialog for Clear History
        public class ClearHistoryDialog : Form
        {
            private Label messageLabel;
            private Button buttonOK;
            private Button buttonHelp;

            public ClearHistoryDialog(string message, string caption)
            {
                message = $"\n\n\n\n\n\n\n{message}?";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(300 * ScaleFactor);
                Height = (int)(150 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    Height = Math.Max(Height, (int)(size.Height + (int)(100 * ScaleFactor)));
                }

                buttonHelp = new Button();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                buttonHelp.BackgroundImage = scaledImage;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.FlatAppearance.BorderSize = 0;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.DialogResult = DialogResult.None;
                buttonHelp.Click += ButtonHelp_Click;
                helpPage = "clear-history";
                
                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.DialogResult = DialogResult.OK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(32, 32, 32);
                    ForeColor = Color.White;
                }

                RecentItemsCheckbox = new CheckBox();
                RecentItemsCheckbox.Font = new Font("Segoe UI", 10);
                RecentItemsCheckbox.Text = sRecent;
                RecentItemsCheckbox.Checked = false;
                RecentItemsCheckbox.AutoSize = true;
                RecentItemsCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(16 * ScaleFactor));

                AutoSuggestCheckbox = new CheckBox();
                AutoSuggestCheckbox.Font = new Font("Segoe UI", 10);
                AutoSuggestCheckbox.Text = sAutoSuggest;
                AutoSuggestCheckbox.Checked = false;
                AutoSuggestCheckbox.AutoSize = true;
                AutoSuggestCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(40 * ScaleFactor));

                TempFilesCheckbox = new CheckBox();
                TempFilesCheckbox.Font = new Font("Segoe UI", 10);
                TempFilesCheckbox.Text = sTemp;
                TempFilesCheckbox.Checked = false;
                TempFilesCheckbox.AutoSize = true;
                TempFilesCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(64 * ScaleFactor));

                DefenderCheckbox = new CheckBox();
                DefenderCheckbox.Font = new Font("Segoe UI", 10);
                DefenderCheckbox.Text = sDefender;
                DefenderCheckbox.Checked = false;
                DefenderCheckbox.AutoSize = true;
                DefenderCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(88 * ScaleFactor));

                Controls.Add(buttonHelp);
                Controls.Add(RecentItemsCheckbox);
                Controls.Add(AutoSuggestCheckbox);
                Controls.Add(TempFilesCheckbox);
                Controls.Add(DefenderCheckbox);
                Controls.Add(buttonOK);
                Controls.Add(messageLabel);

                Point cursorPosition = Cursor.Position;
                int dialogX = Cursor.Position.X - Width / 2;
                int dialogY = Cursor.Position.Y - Height / 2 - (int)(50 * ScaleFactor);
                Screen screen = Screen.FromPoint(cursorPosition);
                int screenWidth = screen.WorkingArea.Width;
                int screenHeight = screen.WorkingArea.Height;
                int baseX = screen.Bounds.X;
                int baseY = screen.Bounds.Y;
                dialogX = Math.Max(baseX, Math.Min(baseX + screenWidth - Width, dialogX));
                dialogY = Math.Max(baseY, Math.Min(baseY + screenHeight - Height, dialogY));
                Location = new Point(dialogX, dialogY);
            }

            public static DialogResult Show(string message, string caption)
            {
                using (var ClearHistoryDialog = new ClearHistoryDialog(message, caption))
                {
                    return ClearHistoryDialog.ShowDialog();
                }
            }
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

}

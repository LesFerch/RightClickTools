using System;
using System.IO;

namespace RightClickTools
{
    partial class Program
    {
        // Context menu
        static string[] CmdLabels = { "Cmd here", "PowerShell here", "PowerShell Core here", "File Manager here", "Search here", "RegEdit", "Clear History", "Unblock files here", "Take ownership and get access", "Add or Remove folder in Path variable", "Toggle display of hidden and system files", "Refresh shell", "Folder Options here", "Restart Explorer", "Settings", "More Tools…" };

        static string sCmdHere;
        static string sPowerShellHere;
        static string sPowerShellCoreHere;
        static string sFileManagerHere;
        static string sRegEdit;
        static string sClearHistory;
        static string sUnblockHere;
        static string sTakeOwnHere;
        static string sRestartExplorer;

        // Misc
        static string sMain = "Right-Click Tools";
        static string sSetup = "Setup";
        static string sOK = "OK";
        static string sYes = "Yes";
        static string sNo = "No";
        static string sInstall = "Install";
        static string sRemove = "Remove";
        static string sDone = "Done";
        static string sCCM = "Classic context menu";
        static string sUser = "User";
        static string sAdministrator = "Administrator";
        static string sTrustedInstaller = "TrustedInstaller";
        static string sFileManager = "File Manager";
        static string sInstallTask = "Add the privilege elevation task";
        static string sClassicContextMenu = "Add to the classic context menu";
        static string sCustomContextMenu = "Add to Custom Context Menu";
        static string sWin11ClassicMenu = "Show only the classic context menu";
        static string sRestartExplorerPrompt = "Explorer restart needed. Restart Explorer now?";
        static string sAccessDenied = "Access denied";
        static string sError = "Error:";
        static string sPowerShellCoreNotInstalled = "PowerShell Core is not installed.";

        // Take ownership and get access
        static string sWarningTakeOwn = "WARNING: Other users may lose access";

        // Add or Remove folder in Path variable
        static string sUserPath = "User Path";
        static string sSystemPath = "System Path";

        // Refresh shell
        static string sShellRefresh = "Shell refresh only";
        static string sResetIcons = "Reset icon cache";
        static string sResetThumbs = "Reset thumbnail cache";

        // Clear History
        static string sRecent = "Recent items";
        static string sAutoSuggest = "Auto-suggest items";
        static string sTemp = "Temporary files";
        static string sRecycleBin = "Recycle Bin";
        static string sDefender = "Defender history";
        static string sSpecifiedFolders = "Specified folders";
        static string sRestartPC = "A restart is required to clear the Protection history. Restart now?";

        // Search Helper
        static string sSearchHelper = "Search Helper";
        static string sSearchHelperHint = "Queries may include OR (all caps),  - for NOT,  and space for AND.";
        static string sKindPresets = "Kind presets";
        static string sSizePresets = "Size presets";
        static string sDatePresets = "Date presets";
        static string sPickADate = "Pick a date";
        static string sDateRange = "Date range";
        static string sPickStartDate = "Pick Start Date";
        static string sPickEndDate = "Pick End Date";
        static string sCustom = "Custom:";
        static string sCopy = "Copy";
        static string sMore = "More…";
        static string sEdit = "Edit";

        // Search Helper main options
        static string sKindLabel = "Kind:";
        static string sExtLabel = "Ext:";
        static string sSizeLabel = "Size:";
        static string sWidthLabel = "Width:";
        static string sHeightLabel = "Height:";
        static string sDimensionsLabel = "Dimensions:";
        static string sModifiedLabel = "Modified:";
        static string sCreatedLabel = "Created:";
        static string sDateLabel = "Date:";
        static string sDateTakenLabel = "DateTaken:";
        static string sContentsLabel = "Contents:";
        static string sTagsLabel = "Tags:";
        static string sNameLabel = "Name:";
        static string sTitleLabel = "Title:";

        // Search Helper Kind presets
        static string sKindText = "Text";
        static string sKindDocument = "Document";
        static string sKindPicture = "Picture";
        static string sKindMusic = "Music";
        static string sKindVideo = "Video";
        static string sKindFolder = "Folder";

        // Search Helper Size presets
        static string sSizeEmpty = "Empty (0 KB)";
        static string sSizeTiny = "Tiny (0 - 16 KB)";
        static string sSizeSmall = "Small (16 KB - 1 MB)";
        static string sSizeMedium = "Medium (1 - 128 MB)";
        static string sSizeLarge = "Large (128 MB - 1 GB)";
        static string sSizeHuge = "Huge (1 - 4 GB)";
        static string sSizeGigantic = "Gigantic (>4 GB)";

        // Search Helper Date presets
        static string sDateToday = "Today";
        static string sDateYesterday = "Yesterday";
        static string sDateThisWeek = "This week";
        static string sDateLastWeek = "Last week";
        static string sDateThisMonth = "This month";
        static string sDateLastMonth = "Last month";
        static string sDateThisYear = "This year";
        static string sDateLastYear = "Last year";

        // Folder Options
        static string sFolderOptions = "Folder Options";
        static string sFolderNotAllowed = "Not allowed for this folder";
        static string sSettings = "Settings";
        static string sGlobalSettings = "Global settings";
        static string sFileSystemPrefix = "File System: ";
        static string sTypePrefix = "Type: ";
        static string sNA = "N/A";
        static string sAFTD = "Automatic Folder Type Discovery";
        static string sAFTDSubtitle = "(Uncheck to make folders default to type General Items)";
        static string sAlwaysShowIcons = "Always show icons, never thumbnails";
        static string sDisableFolderThumbnails = "Disable folder thumbnails";
        static string sForceFolderType = "Force folder type via desktop.ini to:";
        static string sRestoreDefaults = "Restore defaults";
        static string sRequiresAFTD = "(Requires Automatic Folder Type Discovery)";
        static string sRequiresNTFS = "(File system: NTFS and Type: Local disk required)";
        static string sSetFolderIcon = "Set Folder icon via desktop.ini from:";
        static string sResetIconCacheRestart = "Reset icon cache and restart Explorer";
        static string sDeleteDesktopIni = "Delete desktop.ini and desktop.ico files";
        static string sAlsoApplyToSubfolders = "Also apply to subfolders";
        static string sSelectIconFile = "Select Icon File";
        static string sPickAColor = "Pick a Color";
        static string sSelectProperties = "Select Properties";

        // Folder Options menus
        static string sNoChange = "No change";
        static string sGeneralItems = "General Items";
        static string sDocuments = "Documents";
        static string sPictures = "Pictures";
        static string sMusic = "Music";
        static string sVideos = "Videos";
        static string sSelectedColor = "Selected color";
        static string sSelectedIcon = "Selected icon";
        static string sSelectedImage = "Selected image";
        static string sMostRecentImages = "Most recent image(s) in folder";
        static string sFirstAlphabeticalImages = "First image(s) alphabetically in folder";
        static string sFitTransparent = "Fit (transparent background)";
        static string sFitSolid = "Fit (solid background)";
        static string sFillSingle = "Fill (single image)";
        static string sFill2Landscape = "Fill (2 landscape images)";
        static string sFill2Portrait = "Fill (2 portrait images)";
        static string sFill4Images = "Fill (4 images)";

        // Shortcut Tool
        static string sShortcutTool = "Shortcut Tool";
        static string sConvert = "Convert";
        static string sConvertUrlToLnk = "Convert Internet shortcuts (.url) to Windows shortcuts (.lnk)";
        static string sMoveUrlToRecycleBin = "Move the .url files to the recycle bin";
        static string sSearchAndReplace = "Search and Replace";
        static string sSearchFor = "Search for:";
        static string sReplaceWith = "Replace with:";
        static string sSearchIn = "Search in:";
        static string sTarget = "Target";
        static string sStartIn = "Start in";
        static string sIcon = "Icon";

        // Date Time Tool
        static string sDateTimeTool = "Date Time Tool";
        static string sSetDateModified = "Set Date modified to a specific date and time";
        static string sSetDateCreated = "Set Date created to a specific date and time";

        static string sCopyDateModifiedToDateCreated = "Copy Date modified to Date created";
        static string sOnlyIfDateModifiedIsOlder = "Only if Date modified is older";
        static string sCopyDateCreatedToDateModified = "Copy Date created to Date modified";
        static string sCopyDateTakenToDateCreated = "Copy Date taken to Date created";
        static string sCopyDateTakenToDateCreatedAndModified = "Copy Date taken to Date created and Date modified";
        static string sWarnChangeDates = "Are you sure you want to change dates for all files in the folder?";
        static string sWarnChangeDatesSubfolders = "Are you sure you want to change dates for all files in the folder and all subfolders?";

        // Snip with Border
        static string sSnipWithBorder = "Snip with border";

        // Settings
        static string sRCTSettings = "Right-Click Tools Settings";
        static string sWinSettings = "Windows Settings";
        static string sControlPanel = "Control Panel";
        static string sPerfOptions = "Performance Options";
        static string sSysProps = "System Properties";
        static string sEnvVars = "Environment Variables";
        static string sAppsFeatures = "Installed apps";
        static string sProgramsFeatures = "Programs and Features";
        static string sOptFeatures = "Optional Features";
        static string sClassicSettings = "Classic settings flat list";
        static string sScale = "Scale";

        // set some strings from CmdLabels array
        static void StringsFromCmdLabels()
        {
            sCmdHere = CmdLabels[0];
            sPowerShellHere = CmdLabels[1];
            sPowerShellCoreHere = CmdLabels[2];
            sFileManagerHere = CmdLabels[3];
            sRegEdit = CmdLabels[5];
            sClearHistory = CmdLabels[6];
            sUnblockHere = CmdLabels[7];
            sTakeOwnHere = CmdLabels[8];
            sRestartExplorer = CmdLabels[13];
        }

        // Load language strings from INI file
        static void LoadLanguageStrings()
        {
            string iniFile = $@"{appParts}\language.ini";

            if (!File.Exists(iniFile)) return;

            string lang = GetLang();

            // Misc
            sMain = ReadString(iniFile, lang, "sMain", sMain);
            sSetup = ReadString(iniFile, lang, "sSetup", sSetup);
            sOK = ReadString(iniFile, lang, "sOK", sOK);
            sYes = ReadString(iniFile, lang, "sYes", sYes);
            sNo = ReadString(iniFile, lang, "sNo", sNo);
            sInstall = ReadString(iniFile, lang, "sInstall", sInstall);
            sRemove = ReadString(iniFile, lang, "sRemove", sRemove);
            sDone = ReadString(iniFile, lang, "sDone", sDone);
            sCCM = ReadString(iniFile, lang, "sCCM", sCCM);
            sUser = ReadString(iniFile, lang, "sUser", sUser);
            sAdministrator = ReadString(iniFile, lang, "sAdministrator", sAdministrator);
            sTrustedInstaller = ReadString(iniFile, lang, "sTrustedInstaller", sTrustedInstaller);
            sFileManager = ReadString(iniFile, lang, "sFileManager", sFileManager);
            sInstallTask = ReadString(iniFile, lang, "sInstallTask", sInstallTask);
            sClassicContextMenu = ReadString(iniFile, lang, "sClassicContextMenu", sClassicContextMenu);
            sCustomContextMenu = ReadString(iniFile, lang, "sCustomContextMenu", sCustomContextMenu);
            sWin11ClassicMenu = ReadString(iniFile, lang, "sWin11ClassicMenu", sWin11ClassicMenu);
            sRestartExplorerPrompt = ReadString(iniFile, lang, "sRestartExplorerPrompt", sRestartExplorerPrompt);
            sAccessDenied = ReadString(iniFile, lang, "sAccessDenied", sAccessDenied);
            sError = ReadString(iniFile, lang, "sError", sError);
            sPowerShellCoreNotInstalled = ReadString(iniFile, lang, "sPowerShellCoreNotInstalled", sPowerShellCoreNotInstalled);

            // Take ownership and get access
            sWarningTakeOwn = ReadString(iniFile, lang, "sWarningTakeOwn", sWarningTakeOwn);

            // Add or Remove folder in Path variable
            sUserPath = ReadString(iniFile, lang, "sUserPath", sUserPath);
            sSystemPath = ReadString(iniFile, lang, "sSystemPath", sSystemPath);

            // Refresh shell
            sShellRefresh = ReadString(iniFile, lang, "sShellRefresh", sShellRefresh);
            sResetIcons = ReadString(iniFile, lang, "sResetIcons", sResetIcons);
            sResetThumbs = ReadString(iniFile, lang, "sResetThumbs", sResetThumbs);

            // Clear History
            sRecent = ReadString(iniFile, lang, "sRecent", sRecent);
            sAutoSuggest = ReadString(iniFile, lang, "sAutoSuggest", sAutoSuggest);
            sTemp = ReadString(iniFile, lang, "sTemp", sTemp);
            sRecycleBin = ReadString(iniFile, lang, "sRecycleBin", sRecycleBin);
            sDefender = ReadString(iniFile, lang, "sDefender", sDefender);
            sSpecifiedFolders = ReadString(iniFile, lang, "sSpecifiedFolders", sSpecifiedFolders);
            sRestartPC = ReadString(iniFile, lang, "sRestartPC", sRestartPC);

            // Search Helper
            sSearchHelper = ReadString(iniFile, lang, "sSearchHelper", sSearchHelper);
            sSearchHelperHint = ReadString(iniFile, lang, "sSearchHelperHint", sSearchHelperHint);
            sKindPresets = ReadString(iniFile, lang, "sKindPresets", sKindPresets);
            sSizePresets = ReadString(iniFile, lang, "sSizePresets", sSizePresets);
            sDatePresets = ReadString(iniFile, lang, "sDatePresets", sDatePresets);
            sPickADate = ReadString(iniFile, lang, "sPickADate", sPickADate);
            sDateRange = ReadString(iniFile, lang, "sDateRange", sDateRange);
            sPickStartDate = ReadString(iniFile, lang, "sPickStartDate", sPickStartDate);
            sPickEndDate = ReadString(iniFile, lang, "sPickEndDate", sPickEndDate);
            sCustom = ReadString(iniFile, lang, "sCustom", sCustom);
            sCopy = ReadString(iniFile, lang, "sCopy", sCopy);
            sMore = ReadString(iniFile, lang, "sMore", sMore);
            sEdit = ReadString(iniFile, lang, "sEdit", sEdit);

            // Search Helper main options
            sKindLabel = ReadString(iniFile, lang, "sKindLabel", sKindLabel);
            sExtLabel = ReadString(iniFile, lang, "sExtLabel", sExtLabel);
            sSizeLabel = ReadString(iniFile, lang, "sSizeLabel", sSizeLabel);
            sWidthLabel = ReadString(iniFile, lang, "sWidthLabel", sWidthLabel);
            sHeightLabel = ReadString(iniFile, lang, "sHeightLabel", sHeightLabel);
            sDimensionsLabel = ReadString(iniFile, lang, "sDimensionsLabel", sDimensionsLabel);
            sModifiedLabel = ReadString(iniFile, lang, "sModifiedLabel", sModifiedLabel);
            sCreatedLabel = ReadString(iniFile, lang, "sCreatedLabel", sCreatedLabel);
            sDateLabel = ReadString(iniFile, lang, "sDateLabel", sDateLabel);
            sDateTakenLabel = ReadString(iniFile, lang, "sDateTakenLabel", sDateTakenLabel);
            sContentsLabel = ReadString(iniFile, lang, "sContentsLabel", sContentsLabel);
            sTagsLabel = ReadString(iniFile, lang, "sTagsLabel", sTagsLabel);
            sNameLabel = ReadString(iniFile, lang, "sNameLabel", sNameLabel);
            sTitleLabel = ReadString(iniFile, lang, "sTitleLabel", sTitleLabel);

            // Search Helper Kind presets
            sKindText = ReadString(iniFile, lang, "sKindText", sKindText);
            sKindDocument = ReadString(iniFile, lang, "sKindDocument", sKindDocument);
            sKindPicture = ReadString(iniFile, lang, "sKindPicture", sKindPicture);
            sKindMusic = ReadString(iniFile, lang, "sKindMusic", sKindMusic);
            sKindVideo = ReadString(iniFile, lang, "sKindVideo", sKindVideo);
            sKindFolder = ReadString(iniFile, lang, "sKindFolder", sKindFolder);

            // Search Helper Size presets
            sSizeEmpty = ReadString(iniFile, lang, "sSizeEmpty", sSizeEmpty);
            sSizeTiny = ReadString(iniFile, lang, "sSizeTiny", sSizeTiny);
            sSizeSmall = ReadString(iniFile, lang, "sSizeSmall", sSizeSmall);
            sSizeMedium = ReadString(iniFile, lang, "sSizeMedium", sSizeMedium);
            sSizeLarge = ReadString(iniFile, lang, "sSizeLarge", sSizeLarge);
            sSizeHuge = ReadString(iniFile, lang, "sSizeHuge", sSizeHuge);
            sSizeGigantic = ReadString(iniFile, lang, "sSizeGigantic", sSizeGigantic);

            // Search Helper Date presets
            sDateToday = ReadString(iniFile, lang, "sDateToday", sDateToday);
            sDateYesterday = ReadString(iniFile, lang, "sDateYesterday", sDateYesterday);
            sDateThisWeek = ReadString(iniFile, lang, "sDateThisWeek", sDateThisWeek);
            sDateLastWeek = ReadString(iniFile, lang, "sDateLastWeek", sDateLastWeek);
            sDateThisMonth = ReadString(iniFile, lang, "sDateThisMonth", sDateThisMonth);
            sDateLastMonth = ReadString(iniFile, lang, "sDateLastMonth", sDateLastMonth);
            sDateThisYear = ReadString(iniFile, lang, "sDateThisYear", sDateThisYear);
            sDateLastYear = ReadString(iniFile, lang, "sDateLastYear", sDateLastYear);

            // Folder Options
            sFolderOptions = ReadString(iniFile, lang, "sFolderOptions", sFolderOptions);
            sFolderNotAllowed = ReadString(iniFile, lang, "sFolderNotAllowed", sFolderNotAllowed);
            sSettings = ReadString(iniFile, lang, "sSettings", sSettings);
            sGlobalSettings = ReadString(iniFile, lang, "sGlobalSettings", sGlobalSettings);
            sFileSystemPrefix = ReadString(iniFile, lang, "sFileSystemPrefix", sFileSystemPrefix);
            sTypePrefix = ReadString(iniFile, lang, "sTypePrefix", sTypePrefix);
            sNA = ReadString(iniFile, lang, "sNA", sNA);
            sAFTD = ReadString(iniFile, lang, "sAFTD", sAFTD);
            sAFTDSubtitle = ReadString(iniFile, lang, "sAFTDSubtitle", sAFTDSubtitle);
            sAlwaysShowIcons = ReadString(iniFile, lang, "sAlwaysShowIcons", sAlwaysShowIcons);
            sDisableFolderThumbnails = ReadString(iniFile, lang, "sDisableFolderThumbnails", sDisableFolderThumbnails);
            sForceFolderType = ReadString(iniFile, lang, "sForceFolderType", sForceFolderType);
            sRestoreDefaults = ReadString(iniFile, lang, "sRestoreDefaults", sRestoreDefaults);
            sRequiresAFTD = ReadString(iniFile, lang, "sRequiresAFTD", sRequiresAFTD);
            sRequiresNTFS = ReadString(iniFile, lang, "sRequiresNTFS", sRequiresNTFS);
            sSetFolderIcon = ReadString(iniFile, lang, "sSetFolderIcon", sSetFolderIcon);
            sResetIconCacheRestart = ReadString(iniFile, lang, "sResetIconCacheRestart", sResetIconCacheRestart);
            sDeleteDesktopIni = ReadString(iniFile, lang, "sDeleteDesktopIni", sDeleteDesktopIni);
            sAlsoApplyToSubfolders = ReadString(iniFile, lang, "sAlsoApplyToSubfolders", sAlsoApplyToSubfolders);
            sSelectIconFile = ReadString(iniFile, lang, "sSelectIconFile", sSelectIconFile);
            sPickAColor = ReadString(iniFile, lang, "sPickAColor", sPickAColor);
            sSelectProperties = ReadString(iniFile, lang, "sSelectProperties", sSelectProperties);

            // Folder Options menus
            sNoChange = ReadString(iniFile, lang, "sNoChange", sNoChange);
            sGeneralItems = ReadString(iniFile, lang, "sGeneralItems", sGeneralItems);
            sDocuments = ReadString(iniFile, lang, "sDocuments", sDocuments);
            sPictures = ReadString(iniFile, lang, "sPictures", sPictures);
            sMusic = ReadString(iniFile, lang, "sMusic", sMusic);
            sVideos = ReadString(iniFile, lang, "sVideos", sVideos);
            sSelectedColor = ReadString(iniFile, lang, "sSelectedColor", sSelectedColor);
            sSelectedIcon = ReadString(iniFile, lang, "sSelectedIcon", sSelectedIcon);
            sSelectedImage = ReadString(iniFile, lang, "sSelectedImage", sSelectedImage);
            sMostRecentImages = ReadString(iniFile, lang, "sMostRecentImages", sMostRecentImages);
            sFirstAlphabeticalImages = ReadString(iniFile, lang, "sFirstAlphabeticalImages", sFirstAlphabeticalImages);
            sFitTransparent = ReadString(iniFile, lang, "sFitTransparent", sFitTransparent);
            sFitSolid = ReadString(iniFile, lang, "sFitSolid", sFitSolid);
            sFillSingle = ReadString(iniFile, lang, "sFillSingle", sFillSingle);
            sFill2Landscape = ReadString(iniFile, lang, "sFill2Landscape", sFill2Landscape);
            sFill2Portrait = ReadString(iniFile, lang, "sFill2Portrait", sFill2Portrait);
            sFill4Images = ReadString(iniFile, lang, "sFill4Images", sFill4Images);

            // Shortcut Tool
            sShortcutTool = ReadString(iniFile, lang, "sShortcutTool", sShortcutTool);
            sConvert = ReadString(iniFile, lang, "sConvert", sConvert);
            sConvertUrlToLnk = ReadString(iniFile, lang, "sConvertUrlToLnk", sConvertUrlToLnk);
            sMoveUrlToRecycleBin = ReadString(iniFile, lang, "sMoveUrlToRecycleBin", sMoveUrlToRecycleBin);
            sSearchAndReplace = ReadString(iniFile, lang, "sSearchAndReplace", sSearchAndReplace);
            sSearchFor = ReadString(iniFile, lang, "sSearchFor", sSearchFor);
            sReplaceWith = ReadString(iniFile, lang, "sReplaceWith", sReplaceWith);
            sSearchIn = ReadString(iniFile, lang, "sSearchIn", sSearchIn);
            sTarget = ReadString(iniFile, lang, "sTarget", sTarget);
            sStartIn = ReadString(iniFile, lang, "sStartIn", sStartIn);
            sIcon = ReadString(iniFile, lang, "sIcon", sIcon);

            // Date Time Tool
            sDateTimeTool = ReadString(iniFile, lang, "sDateTimeTool", sDateTimeTool);
            sSetDateModified = ReadString(iniFile, lang, "sSetDateModified", sSetDateModified);
            sSetDateCreated = ReadString(iniFile, lang, "sSetDateCreated", sSetDateCreated);
            sCopyDateModifiedToDateCreated = ReadString(iniFile, lang, "sCopyDateModifiedToDateCreated", sCopyDateModifiedToDateCreated);
            sOnlyIfDateModifiedIsOlder = ReadString(iniFile, lang, "sOnlyIfDateModifiedIsOlder", sOnlyIfDateModifiedIsOlder);
            sCopyDateCreatedToDateModified = ReadString(iniFile, lang, "sCopyDateCreatedToDateModified", sCopyDateCreatedToDateModified);
            sCopyDateTakenToDateCreated = ReadString(iniFile, lang, "sCopyDateTakenToDateCreated", sCopyDateTakenToDateCreated);
            sCopyDateTakenToDateCreatedAndModified = ReadString(iniFile, lang, "sCopyDateTakenToDateCreatedAndModified", sCopyDateTakenToDateCreatedAndModified);
            sWarnChangeDates = ReadString(iniFile, lang, "sWarnChangeDates", sWarnChangeDates);
            sWarnChangeDatesSubfolders = ReadString(iniFile, lang, "sWarnChangeDatesSubfolders", sWarnChangeDatesSubfolders);

            // Snip with Border
            sSnipWithBorder = ReadString(iniFile, lang, "sSnipWithBorder", sSnipWithBorder);

            // Settings
            sRCTSettings = ReadString(iniFile, lang, "sRCTSettings", sRCTSettings);
            sWinSettings = ReadString(iniFile, lang, "sWinSettings", sWinSettings);
            sControlPanel = ReadString(iniFile, lang, "sControlPanel", sControlPanel);
            sPerfOptions = ReadString(iniFile, lang, "sPerfOptions", sPerfOptions);
            sSysProps = ReadString(iniFile, lang, "sSysProps", sSysProps);
            sEnvVars = ReadString(iniFile, lang, "sEnvVars", sEnvVars);
            sAppsFeatures = ReadString(iniFile, lang, "sAppsFeatures", sAppsFeatures);
            sProgramsFeatures = ReadString(iniFile, lang, "sProgramsFeatures", sProgramsFeatures);
            sOptFeatures = ReadString(iniFile, lang, "sOptFeatures", sOptFeatures);
            sClassicSettings = ReadString(iniFile, lang, "sClassicSettings", sClassicSettings);
            sScale = ReadString(iniFile, lang, "sScale", sScale);

            // Context menu labels array
            string sCmdLabels = ReadString(iniFile, lang, "CmdLabels", "");
            string[] LangLabels = sCmdLabels.Split(new char[] { '|' });

            for (int i = 0; i < Math.Min(CmdLabels.Length, LangLabels.Length); i++)
            {
                CmdLabels[i] = LangLabels[i];
            }
            StringsFromCmdLabels();
        }

    }
}
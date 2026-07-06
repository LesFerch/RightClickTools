# RightClickTools

### Version 2.0.0

<img width="570" height="672" alt="image" src="https://github.com/user-attachments/assets/06f2d16b-e6d6-47d7-957e-6e71d1cd38e2#gh-dark-mode-only" />
<img width="568" height="676" alt="image" src="https://github.com/user-attachments/assets/067f7b88-89ba-472c-a374-268dd0dcf535#gh-light-mode-only" />


## Summary

This program provides a set of powerful shortcuts and tools that are described in detail below. Where appropriate, the tools can be run as the current user, Administrator, or **TrustedInstaller**. The tool set is user-extendable and may be configured for individual requirements. For example, the default elevation can be set for each tool and TrustedInstaller capability can be disabled if desired.

The tools may be run directly via the built-in launcher or added to the Explorer right-click context menu. Both the classic and modern context menu are fully supported. See the installation section below for details.

The tools are compatible with Windows 7 and above, 32 bit and 64 bit, standard and administrator users, multiple users on the same computer, and long paths.

For administrator users, that choose to install the optional privilege elevation task, there is only a single UAC prompt to install the tools. After that, all the tools run without any UAC prompts.

The included **Language.ini** file includes 30 languages for the interface and can be edited to add other languages and/or change any of the labels.

## How to Download and Install 

[![image](https://github.com/user-attachments/assets/75e62417-c8ee-43b1-a8a8-a217ce130c91)Download the installer](https://github.com/LesFerch/RightClickTools/releases/download/2.0.0/RightClickTools-Setup.exe)

[![image](https://github.com/LesFerch/WinSetView/assets/79026235/0188480f-ca53-45d5-b9ff-daafff32869e)Download the zip file](https://github.com/LesFerch/RightClickTools/releases/download/2.0.0/RightClickTools.zip)

**Note**: Some antivirus software may falsely detect the download as a virus. This can happen any time you download a new executable and may require extra steps to whitelist the file.

**Note**: Scanning Right-Click Tools with VirusTotal will show that many AV products detect it as a trojan. Those are false positives. This is expected because the program optionally creates a scheduled task in order to provide the convenience of Administrator and TrustedInstaller access without UAC prompts. Right-Click Tools is provided on GitHub as open source, the executables are signed, and my identity is not hidden. I submit my apps to the Windows Defender team to ensure that Windows Defender is okay with them, but I don't have the resources to do that for all AV products.

### Install Using Setup Program

Use this option if you want to:

- add Right-Click Tools to the modern (Windows 11) context menu.
- install Right-Click Tools to the Program Files folder (using either context menu option).

1. Download the installer using the link above.
2. Double-click **RightClickTools-Setup.exe** to start the installation.
3. Click **Yes** when the UAC prompt appears.
4. On Windows 11 and higher, select the desired context menu option (see below for details).
5. For the **Enable privilege elevation task** option, see the **Setup** section below for more details.
6. Click **OK** to continue with the installation.

**Note**: The option **Context menu via registry (classic only)** is checked by default. This option uses registry keys to add Right-Click Tools to Explorer's classic context menu. This is a zero-overhead option, but you must use the classic context menu either by holding the **Shift** key when right-clicking or by making the classic menu the default (See *Setup** below). The option **Context menu handler (modern + classic)** installs a signed context menu handler that adds Right-Click Tools to both the modern and classic context menu.

**Note**: The installer will automatically run in your Windows current lanaguage. If you wish to force the installer to run in a different language, you can specify that language on the command line using its two letter code. For example:

`RightClickTools-Setup /lang=en`

**Note**: The right-click menu items will be created for the user that is currently logged on interactively. If you wish to add the right-click menu items to *other* users, log on as each user and run **RightClickTools-Setup.exe** again.

### Portable Install

Use this option if you want to:

- run Right-Click Tools from a drive or folder of your choice, including removable media.
- run Right-Click Tools without adding it to the context menu.
- add Right-Click Tools to the classic context menu, but not install it to Program Files.
- Add Right-Click Tools to the program [Custom Context Menu](https://apps.microsoft.com/detail/9pc7bzz28g0x).

1. Download the zip file using the link above.
2. Extract the contents. You should see **RightClickTools.exe**, **Setup.exe**, and an **AppParts** folder.
3. Move the contents to a permanent location of your choice. For example **C:\Tools\RightClickTools**.
3. Right-click **Setup.exe**, select Properties, check **Unblock**, and click **OK**.
5. Double-click **Setup.exe** to open the Setup dialog.
6. If you skipped step 4, then, in the SmartScreen window, click **More info** and then **Run anyway**.
7. Enable your desired options and then click **OK**
8. Click **OK** when the **Done** message box appears.

**Note**: When Right-click Tools is installed as a portable app, you will NOT see the app listed under **Apps** or **Programs and Files**.

**Note**: The right-click menu items will be created for the user that is currently logged on interactively. If you wish to add the right-click menu items to *other* users, log on as each user and run **Setup.exe** again.

## Setup

**NOTE**: You do NOT need to use **Setup.exe** if you installed Right-Click Tools using **RightClickTools-Setup.exe**.

<img width="373" height="291" alt="image" src="https://github.com/user-attachments/assets/103dd503-8002-4c0c-b51d-aa1879055e01#gh-dark-mode-only" />
<img width="374" height="293" alt="image" src="https://github.com/user-attachments/assets/8d41c90c-cddd-4af3-953d-574c4ddbe646#gh-light-mode-only" />

The Setup dialog will show options that are applicable to your Windows installation. Each option is described below.

The current state of each option is shown by its slider control. Changing a slider's position has no effect until **OK** is clicked.

### Add to the classic context menu

When enabled, Right-Click Tools is added to the classic context menu. This option adds the context menu via registry entries only (i.e. no separate menu handler code is added).

By default, the context menu entries will be created in the current system language if that language is found in the **Language.ini** file. Otherwise, it will default to English. To force the context menu items to be created in a specific language, edit the **RightClickTools.ini** file and uncomment (remove the semicolon) and change the **Lang=en** entry to the two letter code of the desired language found in the **Language.ini** file. Then, just run **Setup.exe** again to update the context menu entries to the new language.

**Note**: If you move **RightClickTools.exe** after using **Setup**, the context menu entries will do nothing because the exe path will be incorrect. To fix that issue, just run **Setup** again.

**Note**: If you installed Right-Click Tools using **RightClickTools-Setup.exe** with the **Context menu handler** option and then ran **Setup.exe** and enabled **Add to the classic context menu**, you will see Right-Click Tools in the classic context menu twice. In that case, one menu is provided by the context menu handler and the other is provided by registry entries. Just run **Setup.exe** again and uncheck **Add to the classic context menu** to fix that.

### Add the privilege elevation task

When enabled, a privilege elevation task is added to Windows Task Scheduler for the current user that allows Right-Click Tools to run its commands as Administrator or TrustedInstaller without a UAC prompt popping up.

**IMPORTANT**: Even though the **Privilege elevation task** is set up to only be used by Right-Click Tools, anyone, with some programming skill, could leverage the task to run any code without a UAC prompt. That's highly unlikely to happen because a niche utility, such as Right-Click Tools, is never going to be on enough computers to be of interest as an attack vector for bad actors. Nevertheless, it's up to you to decide to accept the risk and install the task. If you're on a work computer this task will likely raise a red flag with your IT department and therefore should not be installed.

**Note**: If you have already disabled UAC, the privilege elevation task does not add any additional risk, but it is then mostly unnecessary. There is a small difference for the **File manager here** option in that it will only launch a full Explorer window if the task is installed or the registry setting, that prevents Explorer elevation, is disabled (see details about that later in this document).

### Add to Custom Context Menu

This option only appears if you've installed the program [Custom Context Menu](https://apps.microsoft.com/detail/9pc7bzz28g0x). When enabled, Right-Click Tools is added to Custom Context Menu. Be sure to turn Custom Context Menu off and back on (via its interface) in order to see the changes. Since Right-Click Tools 2.x directly supports the modern context menu, this option is essentially redundant. It may appeal to users of Custom Context Menu that wish to minimize the number of context menu handlers that are added to Windows or wish to pick and choose which tools appear in the menu.

### Show only the classic context menu

This option appears on Windows 11 or higher. The slider will be in the on position if you've already changed Windows 11 to use the classic context menu. When enabled, the Explorer context menu will be the classic version. When disabled, the Explorer context menu will be the modern version (i.e. Windows 11 default), with the classic version available by holding the **Shift** key when right-clicking.

## How to Use

Right-click a folder, the background of an open folder, or a drive to get to the **Right-click Tools** context menu, as shown at the beginning of this document. Right-Click Tools will NOT appear when right-clicking a file. It is a folder level tool set.

Select the action you wish to perform. If nothing happens, then the Exe was likely moved after running Setup. In that case, run **Setup.exe** again.

You can also double-click Right-Click Tools to run it direcly without installation. See the **Launcher Mode** section for configuration options.

**Notes**:
- Clicking the **`X`** in any dialog means _do nothing_. Therefore, there's no need for "No" or "Cancel" buttons.
- Since Right-Click Tools is a .Net application, there can be some delay on first run of some menu items. Also Windows Defender (or other AV software) can add delays to intial launches as it scans the components.
- If the current user is an "Administrator", running a tool as Administrator elevates the current user and therefore runs within the current user's profile.
- If the current user is a "Standard" user, running a tool as Administrator will prompt for a login and therefore run in the profile of the newly logged in administrator user.
- TrustedInstaller is useful for accessing and making changes in protected folders, such as **WindowsApps**. Use with care.
- TrustedInstaller is not a user. It's a service that runs via the SYSTEM account. Therefore a **whoami** command will display **nt authority\system**.
- When opening a folder with a long path (> 260 characters) a Cmd or PowerShell prompt will be in the short (8.3) format. This lets you know that you're in a long path and prevents the prompt from taking too much screen space.
- When PowerShell is opened in a long path, running an executable, such as **whoami** will result in an error (or no output with PowerShell Core). This is a PowerShell problem. It has nothing to do with Right-click Tools and there is no known fix or workaround.
- See the **Configuration** section later in this document for configuration details.

## Launcher Mode

<img width="303" height="552" alt="image" src="https://github.com/user-attachments/assets/f8e9e12b-c87d-47cf-a136-f941a1ba39aa#gh-dark-mode-only" />
<img width="305" height="553" alt="image" src="https://github.com/user-attachments/assets/21216b7f-ad8a-4db5-bedd-bac31b21d2f5#gh-light-mode-only" />

When `RightClickTools.exe` is double-clicked, it opens its own built-in tool launcher. The launcher menu can be configured via the file **Launcher.ini**. See the commented out examples to see how you can add third party tools, such as grepWin and Bulk Rename Utility, to the menu. See the **Configuration** section for entry details.

## Commands

By default, where elevation is applicable to a tool, a dialog will pop up allowing you to run as **User**, **Administrator**, or **TrustedInstaller**. 

<img width="345" height="222" alt="image" src="https://github.com/user-attachments/assets/7c4fb3de-1c68-4444-b133-f241987f6562#gh-dark-mode-only" />
<img width="346" height="223" alt="image" src="https://github.com/user-attachments/assets/0f58f579-15ac-41ff-aaf8-675ec377fdd6#gh-light-mode-only" />

You can also configure Right-Click Tools to open any of the applicable tools at the Run-As level of your choice and only show the pop-up when the **Ctrl** key is held down. See the **Configuration** section later in this document for details.


### Cmd Here, PowerShell Here, and PowerShell Core Here

This opens a console host or terminal window, with the selected shell, in the currently selected folder.

When run as the current user, the selected item will open using your default console program (**Console Host** or **Windows Terminal**). When running as Administrator, it will default to Windows Terminal (if available) but you can configure it to use the classic ConHost if prefered.

### File manager here

This starts the file manager of your choice as the current user, Administrator, or TrustedInstaller, in the currently selected folder.

Edit the file **RightClickTools.ini** and, in the **FileManagerHere** section, uncomment (remove the semicolon) and change the **Exe=** entry to the path of your preferred file manager. The path for the 7-Zip file manager is provided as an example (and is a recommended configuration).

By default, **File manager here** will run Explorer which is fine in most cases where you just need to manage files in a system folder with Administrator or TrustedInstaller access. If you need to manage files within a long path, then a [different file manager](https://gist.github.com/LesFerch/2facb07079394cf2324b6db459bd25d1) is needed because Explorer can only *navigate* long paths.

Please note the following expected behaviors when using this feature with the default (Explorer) configuration:

- On Windows 11, Explorer as **TrustedInstaller** will open a file dialog (aka mini Explorer) instead of a full Explorer window.
- As an Administrator user on Windows 11, Explorer as **Adminstrator** will open the new Windows 11 Explorer, even if you have set the old Explorer as the default using a tool such as [SwitchExplorer](https://lesferch.github.io/SwitchExplorer/).
- As a Standard user on Windows 11, Explorer as **Adminstrator** will open a file dialog (aka mini Explorer) unless the registry setting, that prohibits Explorer elevation, is not set (see below for details).
- When you open the file manager as **TrustedInstaller**, the window that opens will be running in the context of the SYSTEM account, so you will get an error if you click on the shortcuts for Documents, Downloads, etc. but you can navigate to your data folders via `C:\Users`.
- When an ***Administrator user*** opens file manager as **Adminstrator**, the window that opens will be running in the same context as the current user, but with privileges fully elevated to Administrator. All folders and links will be the same as a normal file manager window.
- When a ***Standard user*** opens file manager as **Adminstrator**, the window that opens will be running in the context of the account used at the UAC prompt. Which personal folders and links are shown, and whether the window opens in light or dark mode, will depend on the account used. 

**Note**: Normally Explorer does not allow itself to be "Run as Administrator", but that behavior can be disabled via a [registry setting](https://gist.github.com/LesFerch/a7e43762bb84f18c8ef6ccdfe606eff8) that requires TrustedInstaller privileges to change. Right-Click Tools temporarily changes that registry setting in order to run Explorer elevated (for Administrator users). Some details about why the restriction exists can be found in [this article and its comments](https://devblogs.microsoft.com/oldnewthing/20220524-00/?p=106682)

### Search here

<img width="543" height="592" alt="image" src="https://github.com/user-attachments/assets/042df66e-3893-47bf-9dd1-dfd7d4cdece5#gh-dark-mode-only" />
<img width="545" height="595" alt="image" src="https://github.com/user-attachments/assets/54ebb6e3-de1c-4dbc-90d0-a8f0f4128832#gh-light-mode-only" />

By default, this opens a dialog that helps you build a search term using Advanced Query Syntax (AQS) which is sent to Explorer using the search-ms: protocol. If the query is valid, Explorer should open a window with the search results for that query. The Search Helper does not evaluate your query. It only passes it to Explorer.

If you prefer to use a third-party search tool such as [Everything](https://www.voidtools.com/) or [FileLocator Pro](https://www.mythicsoft.com/filelocatorpro/), you can configure this option to open the search tool of your choice at the current folder. See the **Configuration** section later in this document for details.

**Note**: Windows 7 does not support the search-ms: protocol, so, for Windows 7, the Search Helper puts the AQS query on the clipboard and then opens an Explorer window where it can be pasted into the search box. 

Please note that the results you get with an AQS query is up to Explorer. If you don't get the expected results, your query needs adjustment. It's very easy to construct a query that looks correct, but returns no results (or doesn't even open Explorer). For example, `*.*` for `Name:` will not work, but `*` will work. Also note that the design of the Search Helper dialog is meant to put the most common queries types as quick selections, but those options should not be interpreted as being exclusive. For example, the first pull down menu lets you select either `Kind:` or `Ext:` but in reality, AQS will allow a query to include both of those properties.

If you wish to construct a query that extends beyond the provided quick picks, enable the `Custom` slider. This will add a field where you can manually edit the query. The query generated from your quick picks will initially be copied to that field. You can update the quick picks and re-copy the query by clicking the `Copy` button. The `More` button will let you add any property to your query.

Clicking **OK** sends your query to Explorer.

Your query history is shown in the box just above the OK button. You can execute a query from your history with a single click when the `Custom` slider is in the off position. When the `Custom` slider is in the on position, clicking a query in your history copies it to the Custom field. Then clicking **OK** executes the query. You can remove individual queries from the history with a **Ctrl-click** or you can click the **Edit** button to open and edit the `Searches.txt` file.

### RegEdit

This starts RegEdit as the current user, Administrator, or TrustedInstaller.

**User**: This can be handy to verify what a standard user can or cannot change in the registry without having to analyze the permissions. As a standard user, this option opens RegEdit as it would normally.

**Administrator**: For an "Administrator" user, this is the same as the normal method of opening RegEdit except that it eliminates the nuisance of the UAC prompt (if the privilege elevation task is installed). For a standard user, this option pops up a UAC prompt and then regedit will be running under the profile of the account used at the UAC prompt.

**TustedInstaller**: This opens **RegEdit** via the SYSTEM account with **TrustedInstaller** privileges. This is handy for changing protected settings. Use with care.

**Hidden feature**: Hold down the **Shift** key when selecting this item to open RegEdit collapsed (not supported for TrustedInstaller or Standard user launching as Administrator).


### Clear History

This clears the data for the selected items.

<img width="344" height="299" alt="image" src="https://github.com/user-attachments/assets/0ce7a240-f497-4437-8c76-14cb18d30dbe#gh-light-mode-only" />
<img width="342" height="295" alt="image" src="https://github.com/user-attachments/assets/e3f42685-f01d-4001-9b4a-c8cfb074030b#gh-dark-mode-only" />


- `Recent items` are the recent files and folders shown in Explorer's Home or Quick Access section.

- `Auto-suggest items` are the items that show in drop-down lists, such as the Run box.

- `Temporary files` are all files that are not currently in use within the `%Temp%` location.

- The `Recycle Bin` will be emptied when this option is selected.

- `Defender history` is the "Protection history" list in Windows Defender. Right-click Tools creates a task to clear Windows Defender log files on next restart. The task removes itself after it runs. A UAC prompt will appear if you are logged in as a standard user or the privilege elevation task is not installed.

- The `Specified folders` option will clear any folder paths listed in the `Cleanup.txt` file (one folder path per line without quotes). Folder paths specified without a trailing backslash will be emptied (if possible).  Folder paths specified with a trailing backslash will be removed completely (if possible).

### Unblock files here

<img width="392" height="234" alt="image" src="https://github.com/user-attachments/assets/4bc6e8bb-3900-45f4-8beb-7c902211969b#gh-light-mode-only" />
<img width="392" height="234" alt="image" src="https://github.com/user-attachments/assets/540d40bf-8fd6-49d6-9de1-1643be0b11e4#gh-dark-mode-only" />

This clears the Mark of the Web (MOTW) from files in the current folder (and optionally all subfolders). This is applicable to NTFS volumes only.

### Take ownership and get access

This gives you ownership and access to the selected folder. Right-click Tools uses the **SetACL** program to do the work. This allows it to set ownership and access on folders with Unicode characters in the name and ones that are in long paths.

<img width="394" height="201" alt="image" src="https://github.com/user-attachments/assets/2cc15377-8627-4bf1-afa9-1babe8f3544b#gh-light-mode-only" />
<img width="393" height="202" alt="image" src="https://github.com/user-attachments/assets/3eb5df36-19cc-45ae-a40f-60c49487ee27#gh-dark-mode-only" />

This option will not allow changing permissions on system folders and will display a message when that's attempted. The restrictions can be edited in the file **RightClickTools.ini**, but that should be avoided. If you need to make changes in a restricted folder, such as **WindowsApps**, you should access the folder using the Cmd or PowerShell Here as **TrustedInstaller** option or the **Privileged file manager here** option as **TrustedInstaller**.

**Hidden feature**: Hold down the **Ctrl** key when clicking **OK** to keep the console window open. This can be useful to review the **SetACL** output.

### Add or Remove folder in Path variable

This will show whether the selected folder is currently part of the user or system path and will allow you to change that by checking or unchecking the appropriate box.

<img width="394" height="220" alt="image" src="https://github.com/user-attachments/assets/56cfb924-5482-4372-bf24-099fefd4ef1b#gh-light-mode-only" />
<img width="392" height="217" alt="image" src="https://github.com/user-attachments/assets/e89b4a8f-a183-4c57-a4e9-9dbd1f84760f#gh-dark-mode-only" />

### Toggle display of hidden and system files

This toggles between showing hidden and system files and hiding them. It immediately updates the Explorer view without restarting Explorer.

### Refresh shell

This item provides options to do a quick shell refresh or a complete reset of the icon and/or thumbnail caches.

<img width="343" height="182" alt="image" src="https://github.com/user-attachments/assets/bb1299a6-32ba-4025-98db-20fa4c7fa47b#gh-light-mode-only" />
<img width="344" height="181" alt="image" src="https://github.com/user-attachments/assets/de101cd4-3fca-4a02-8a0e-a468ccc71613#gh-dark-mode-only" />

The default option, "Shell refresh only", quickly refreshes the shell, which is most useful when you've changed an icon, but the old icon or a blank icon is displayed. It tells Explorer to refresh its icon cache and window views. It also tells Explorer to update its current settings (where possible) from the settings in the registry.

If the "Shell refresh only" option does not correct the display of icons and thumbnails then you can select either or both of the "Reset" options which will kill Explorer, wait 2 seconds, clear the icon and/or thumbnail cache, and then restart Explorer.

### Folder Options here

<img width="484" height="670" alt="image" src="https://github.com/user-attachments/assets/f49cefb3-cfa6-4ab0-9485-25ff5411587e#gh-light-mode-only" />
<img width="482" height="665" alt="image" src="https://github.com/user-attachments/assets/2eb1ae85-efad-4236-ad26-3fba4871db20#gh-dark-mode-only" />

This dialog provides various options for configuring your folder views including `Global settings`, `Folder type` settings, and `Folder icon` settings.

#### Global Settings

**Automatic Folder Type Discovery**: When this is enabled (Windows default) Explorer sets each folder's type (General items, Documents, Music, Pictures, Videos) based on the folder's contents. When it's disabled, all folders default to type `General items`. This is the same setting as `Make all folders Generic` in [WinSetView](https://lesferch.github.io/WinSetView). Note that automatic folder type discovery must be enabled to use `Force Folder type via desktop.ini`.

**Always show icons, never thumbnails**: This is exactly the same option you can find in Explorer's settings.

**Disable folder thumbnails**: This setting disables thumbnails for folders without disabling thumbnails for files. This same setting is also in [WinSetView](https://lesferch.github.io/WinSetView).

#### Force Folder type via desktop.ini

This allows you to set the folder type, overriding Explorer's automatic folder type discovery, for a single folder or a whole folder tree (if `Also apply to subfolders` is checked). It does this by setting the folder type via an entry in the hidden `desktop.ini` file within the folder.

**Note**:

- Explorer updates the view in the background as it notices the addition (or change) of the desktop.ini files. How long it will take is variable. It depends on the speed of the computer, what other processes are running, how many folders are affected, and so forth. But it can often take 30 seconds or so for all the folder views to update. So, before you jump onto GitHub and post an issue, relax, do something else for a minute and then go back and check your folders. You should see that Explorer did its thing and updated the view.

- Explorer will not update the folder's view until the folder is closed. However, having an open folder only stops the view update for that particular folder level. The subfolder's views will update even if you have the parent folder open.

#### Set Folder icon via desktop.ini

This allows you to set the folder's icon for a single folder or a whole folder tree (if `Also apply to subfolders` is checked). It does this by setting the folder icon via an entry in the hidden `desktop.ini` file within the folder. The following options are provided:

**Selected color**: This lets you pick a color (consistent with OneDrive folder coloring) that sets the folder's icon to a colored icon. Please note that this is simply an icon change, so you cannot get a thumbnail plus a colored folder.

**Selected icon**: This opens a dialog where you can browse for and pick any valid icon you may have on hand.

**Selected image**: This lets you pick an image file from which an icon will be automatically created and assigned to the folder.

**Most recent image** and **First image alphabetically**: If the folder contains one or more images, the most recent (or first by name) will be used to generate an icon that is assigned to the folder. This is an alternative to Explorer's thumbnail feature that provides various options such as Fit, Fill, 2 up, and 4 up. A preview is shown for those options as long as the selected folder contains one or more images. If the `Also apply to subfolders` option is selected, a preview is shown using sample images.

### Restart Explorer

This restarts Explorer and opens a window to the currently selected folder.

<img width="344" height="154" alt="image" src="https://github.com/user-attachments/assets/43895481-7cae-4d8d-b914-f98a8e4e4226#gh-light-mode-only" />
<img width="343" height="151" alt="image" src="https://github.com/user-attachments/assets/d904d69a-c4ba-49f6-b7a0-2af9027757e5#gh-dark-mode-only" />

### Settings

This provides quick access to the Right-Click Tools configuration files and provides shortcuts to most Windows settings.

**Note**: The options `Windows Settings` and `Installed apps` do nothing on Windows 7 and 8.

### More Tools

This submenu provides two more tools (`Shortcut Tool` and `Date Time Tool`) and can be configured to add any other executables or scripts which may be run as User, Administrator, or TrustedInstaller. In Right-Click Tools, open **Settings** > **Right-Click Tool Settings** and edit the file **MoreTools.ini** to modify this submenu. See the commented-out example entries for details.

### Shortcut Tool

<img width="484" height="451" alt="image" src="https://github.com/user-attachments/assets/175ef0f6-5df5-47e7-acf3-23721b11da99#gh-light-mode-only" />
<img width="484" height="451" alt="image" src="https://github.com/user-attachments/assets/ac890155-0e78-48b0-80fb-d7a71ab6a6df#gh-dark-mode-only" />

The **Convert** section is used to convert URL type shortcuts to LNK type shortcuts. This is useful because URL shortcuts do not reliably show icons larger than size medium. That's an old bug that dates back to at least Windows 7 and continues with Windows 11.

The **Search and Replace** saection allows dping a search and replace of text within fields in Windows LNK type shortcuts. Note that you must enter something to search for before the `Target`, `Start in`, and `Icon` options can be checked.

### Date Time Tool

<img width="485" height="468" alt="image" src="https://github.com/user-attachments/assets/662ddb93-6cd5-454d-9e59-c1c40fd8ad7a#gh-light-mode-only" />
<img width="484" height="464" alt="image" src="https://github.com/user-attachments/assets/7f0eb037-d0bf-4b95-aa9b-1bc020465c1a#gh-dark-mode-only" />

This is used to make changes to the dates/times for all file in a folder (or all subfolders if that option is checked). The options are self-explanatory.

**Note**: If you need to change the date/time for only one file, or only selected files, you'll need a different tool such as [SKTimeStamp](https://tools.stefankueng.com/SKTimeStamp.html).

\
\
[![image](https://github.com/LesFerch/WinSetView/assets/79026235/63b7acbc-36ef-4578-b96a-d0b7ea0cba3a)](https://github.com/LesFerch/RightClickTools)




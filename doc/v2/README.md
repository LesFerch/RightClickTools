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

**Note**: The installer will automatically run in your Windows current lanaguage. If you wish to force the installer to run in a different language, you can specific that language on the command line using its two letter code. For example:

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
- TrustedInstaller is not a user. It's a service that runs via the SYSTEM account. Therefore a **WhoAmI** command will display **nt authority\system**.
- When opening a folder with a long path (> 260 characters) a Cmd or PowerShell prompt will be in the short (8.3) format. This lets you know that you're in a long path and prevents the prompt from taking too much screen space.
- When PowerShell is opened in a long path, running an executable, such as **whoami** will result in an error (or no output with PowerShell Core). This is a PowerShell problem. It has nothing to do with Right-click Tools and there is no known fix or workaround.
- See the **Configuration** section later in this document for configuration details.

## Commands

By default, where elevation is applicable to a tool, a dialog will pop up allowing you to run as **User**, **Administrator**, or **TrustedInstaller**. 

You can also configure Right-Click Tools to open any of these tools at the Run-As level of your choice and only show the pop-up when the **Ctrl** key is held down.

<img width="345" height="222" alt="image" src="https://github.com/user-attachments/assets/7c4fb3de-1c68-4444-b133-f241987f6562#gh-dark-mode-only" />
<img width="346" height="223" alt="image" src="https://github.com/user-attachments/assets/0f58f579-15ac-41ff-aaf8-675ec377fdd6#gh-light-mode-only" />


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

Your query history is shown in the box just above the OK button. You can execute a query from your history with a single click when the `Custom` slider is in the off position. When the `Custom` slider is in the on position, clicking a query in your history copies it to the Custom field. Then clicking **OK** executes the query. You can remove individual queries from the histroy with a **Ctrl-click** or you can click the **Edit** button to open and edit the `Searches.txt` file.

### RegEdit

This starts RegEdit as the current user, Administrator, or TrustedInstaller.

**User**: This can be handy to verify what a standard user can or cannot change in the registry without having to analyze the permissions. As a standard user, this option opens RegEdit as it would normally.

**Administrator**: For an "Administrator" user, this is the same as the normal method of opening RegEdit except that it eliminates the nuisance of the UAC prompt (if the privilege elevation task is installed). For a standard user, this option pops up a UAC prompt and then regedit will be running under the profile of the account used at the UAC prompt.

**TustedInstaller**: This opens **RegEdit** via the SYSTEM account with **TrustedInstaller** privileges. This is handy for changing protected settings. Use with care.

**Hidden feature**: Hold down the **Shift** key when selecting this item to open RegEdit collapsed (not supported for TrustedInstaller or Standard user launching as Administrator). **Warning**: This option will clear your Regedit favorites.


### Clear History

This clears the data for the selected items.

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/9724da1a-b634-4f3f-9903-8d2b808a88fa)

Recent items are the recent files and folders shown in Explorer's Home or Quick Access section.

Auto-suggest items are the items that show in drop-down lists, such as the Run box.

Temporary files are all files that are not currently in use within the %Temp% location.

Defender history is the "Protection history" list in Windows Defender. Right-click Tools creates a task to clear Windows Defender log files on next restart. The task removes itself after it runs. A UAC prompt will appear if you are logged in as a standard user or the privilege elevation task is not installed.

### Take ownership and get access

This gives you ownership and access to the selected folder. Right-click Tools uses the **SetACL** program to do the work. This allows it to set ownership and access on folders with Unicode characters in the name and ones that are in long paths.

![image](https://github.com/user-attachments/assets/9e1f9975-b859-432e-9b8a-9c8d0a8ab45a)

This option will not allow changing permissions on system folders and will display a message when that's attempted. The restrictions can be edited in the file **RightClickTools.ini**, but that should be avoided. If you need to make changes in a restricted folder, such as **WindowsApps**, you should access the folder using the Cmd or PowerShell Here as **TrustedInstaller** option or the **Privileged file manager here** option as **TrustedInstaller**.

**Hidden feature**: Hold down the **Ctrl** key when clicking **OK** to keep the console window open. This can be useful to review the **SetACL** output.

### Add or Remove folder in Path variable

This will show whether the selected folder is currently part of the user or system path and will allow you to change that by checking or unchecking the appropriate box.

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/c0751d3c-a4fe-4e95-911a-54d872bf9e27)

### Toggle display of hidden and system files

This toggles between showing hidden and system files and hiding them. It immediately updates the Explorer view without restarting Explorer.

### Refresh shell

This item provides options to do a quick shell refresh or a complete reset of the icon and/or thumbnail caches.

![image](https://github.com/user-attachments/assets/9f25caee-9d38-49c5-8a3a-e989efdd8771)

The default option, "Shell refresh only", quickly refreshes the shell, which is most useful when you've changed an icon, but the old icon or a blank icon is displayed. It tells Explorer to refresh its icon cache and window views. It also tells Explorer to update its current settings (where possible) from the settings in the registry.

If the "Shell refresh only" option does not correct the display of icons and thumbnails then you can select either or both of the "Reset" options which will kill Explorer, wait 2 seconds, clear the icon and/or thumbnail cache, and then restart Explorer.


### Restart Explorer

This restarts Explorer and opens a window to the currently selected folder.

![image](https://github.com/user-attachments/assets/fda6dffc-061c-4dbb-b359-52efe48da39f)




\
\
[![image](https://github.com/LesFerch/WinSetView/assets/79026235/63b7acbc-36ef-4578-b96a-d0b7ea0cba3a)](https://github.com/LesFerch/RightClickTools)




# RightClickTools

### Version 2.0.0

The documentation for version 1.2.1 can be found [here](./Version121Readme.md).

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/2ea10bbc-7d1f-4a05-bfb2-6115be03bf9f">
  <img alt="image" src="https://github.com/user-attachments/assets/5d8929b4-b384-44cb-a7eb-513fd9ad9ea7" style="max-width: 100%; height: auto;" />
</picture>

## Summary

This program provides a set of powerful tools and shortcuts that are described in detail below. Where appropriate, the tools can be run as the current user, Administrator, or **TrustedInstaller**. The tool set is user-extendable and may be configured for individual requirements. For example, the default elevation can be set for each tool and TrustedInstaller capability can be disabled if desired.

The tools may be run directly via the built-in launcher or added to the Explorer right-click context menu. Both the classic and modern context menu are fully supported. See the [Install](#install) section below for details.

The tools are compatible with Windows 7 and above, 32 bit and 64 bit, standard and administrator users, multiple users on the same computer, and long paths.

For administrator users, that choose to install the optional privilege elevation task, there is only a single UAC prompt to install the tools. After that, all the tools run without any UAC prompts.

The included **Language.ini** file includes 30 languages for the interface and can be edited to add other languages and/or change any of the labels.

## Download

[![image](https://github.com/user-attachments/assets/75e62417-c8ee-43b1-a8a8-a217ce130c91)Download the installer](https://github.com/LesFerch/RightClickTools/releases/download/2.0.0/RightClickTools-Setup.exe)

[![image](https://github.com/LesFerch/WinSetView/assets/79026235/0188480f-ca53-45d5-b9ff-daafff32869e)Download the zip file](https://github.com/LesFerch/RightClickTools/releases/download/2.0.0/RightClickTools.zip)

**Note**: Some antivirus software may falsely detect the download as a virus. This can happen any time you download a new executable and may require extra steps to whitelist the file.

**Note**: Scanning Right-Click Tools with VirusTotal will show that many AV products detect it as a trojan. Those are false positives. This is expected because the program optionally creates a scheduled task in order to provide the convenience of Administrator and TrustedInstaller access without UAC prompts. Right-Click Tools is provided on GitHub as open source, the executables are signed, and my identity is not hidden. I submit my apps to the Windows Defender team to ensure that Windows Defender is okay with them, but I don't have the resources to do that for all AV products.

## Install

### Install Using Setup Program

This option supports both the classic and modern context menu. Administrator rights are required for the initial install to the machine.

Use this option if you want to:

- add Right-Click Tools to the modern (Windows 11) context menu.
- install Right-Click Tools to the Program Files folder (using either context menu option).

1. Download the installer using the link above.
2. Double-click **RightClickTools-Setup.exe** to start the installation.
3. Click **Yes** when the UAC prompt appears.
4. On Windows 11 and higher, select the desired context menu option (see below for details).
5. For the **Enable privilege elevation task** option, see the [Setup](#setup) section below for more details.
6. Click **OK** to continue with the installation.

**IMPORTANT**: To add the tools to the Windows 11 modern context menu for a Standard (i.e. non-admin) user, or any subsequent user after the initial install, login as that user, open `C:\Program Files\RightClickTools` and double-click `RightClickTools.msix` to add Right-Click Tools to the current user's context menu. To remove the menu items added by the MSIX package, open `Settings > Installed Apps` and uninstall `Right-Click Tools Context Menu Handler`. For the classic context menu, use **Setup.exe** (described below) to add or remove the menu items to/from any user.

**Note**: The option **Context menu handler (modern + classic)** (which only appears for Windows 11) installs a signed context menu handler that adds Right-Click Tools to both the modern and classic context menu. The option **Context menu via registry (classic only)** uses registry keys to add Right-Click Tools to Explorer's classic context menu. This is a zero-overhead option, but, if used on Windows 11, you must use the classic context menu either by holding the **Shift** key when right-clicking or by making the classic menu the default (See **Setup** below).

**Note**: The installer will automatically run in your Windows current language. If you wish to force the installer to run in a different language, you can specify that language on the command line using its two letter code. For example:

`RightClickTools-Setup /lang=en`

**Note**: The right-click menu items will be created for the user that is currently logged on interactively. If you wish to add the right-click menu items to *other* users, log on as each user and run **RightClickTools-Setup.exe** again.

### Portable Install

This option only supports the classic context menu.

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

**Note**: If you wish to have the program settings saved with the executable, see the `FullyPortable` option in the **[Configuration](#configuration)** section.

## Setup

**NOTE**: You do NOT need to use **Setup.exe** if you installed Right-Click Tools using **RightClickTools-Setup.exe**.

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/d130aa49-3e63-43b8-8bc0-fd0f7e1a5efb">
  <img alt="image" src="https://github.com/user-attachments/assets/6124e5b6-f1ea-491a-bc88-e703b43a457e" style="max-width: 100%; height: auto;" />
</picture>

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

You can also double-click Right-Click Tools to run it directly without installation. See the [Launcher](#launcher) section for configuration options.

**Notes**:
- Clicking the **`X`** in any dialog means _do nothing_. Therefore, there's no need for "No" or "Cancel" buttons.
- Since Right-Click Tools is a .Net application, there can be some delay on first run of some menu items. Also Windows Defender (or other AV software) can add delays to initial launches as it scans the components.
- If the current user is an "Administrator", running a tool as Administrator elevates the current user and therefore runs within the current user's profile.
- If the current user is a "Standard" user, running a tool as Administrator will prompt for a login and therefore run in the profile of the newly logged in administrator user.
- TrustedInstaller is useful for accessing and making changes in protected folders, such as **WindowsApps**. Use with care.
- TrustedInstaller is not a user. It's a service that runs via the SYSTEM account. Therefore a **whoami** command will display **nt authority\system**.
- When opening a folder with a long path (> 260 characters) a Cmd or PowerShell prompt will be in the short (8.3) format. This lets you know that you're in a long path and prevents the prompt from taking too much screen space.
- When PowerShell is opened in a long path, running an executable, such as **whoami** will result in an error (or no output with PowerShell Core). This is a PowerShell problem. It has nothing to do with Right-click Tools and there is no known fix or workaround.
- See the **[Configuration](#configuration)** section for configuration details.

## Launcher

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/8b85ae4f-9587-480f-a3b1-7d69dbad5708">
  <img alt="image" src="https://github.com/user-attachments/assets/64a5f986-106c-4c83-bbf5-74bebff91d11" style="max-width: 100%; height: auto;" />
</picture>

When `RightClickTools.exe` is double-clicked, it opens its own built-in tool launcher. The launcher menu can be configured via the file **Launcher.ini**. In Right-Click Tools, open **Settings** > **Right-Click Tool Settings** and edit the file **Launcher.ini** to modify this submenu. See the commented out examples to see how you can add third party tools to the menu. See the **[Configuration](#configuration)** section for entry details.

## Commands

By default, where elevation is applicable to a tool, a dialog will pop up allowing you to run as **User**, **Administrator**, or **TrustedInstaller**.

You can also configure Right-Click Tools to open any of the applicable tools at the Run-As level of your choice and only show the pop-up when the **Ctrl** key is held down. See the **[Configuration](#configuration)** section for details.

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/084ead87-2887-4e98-93aa-d56c5ed76f98">
  <img alt="image" src="https://github.com/user-attachments/assets/857e0a06-7c72-462e-b8d5-993bcb70edd3" style="max-width: 100%; height: auto;" />
</picture>


### Cmd Here, PowerShell Here, and PowerShell Core Here

This opens a console host or terminal window, with the selected shell, in the currently selected folder.

When run as the current user, the selected item will open using your default console program (**Console Host** or **Windows Terminal**). When running as Administrator, it will default to Windows Terminal (if available) but you can configure it to use the classic ConHost if preferred.

### File manager here

This starts the file manager of your choice as the current user, Administrator, or TrustedInstaller, in the currently selected folder.

Edit the file **RightClickTools.ini** and, in the **FileManagerHere** section, uncomment (remove the semicolon) and change the **Exe=** entry to the path of your preferred file manager. The path for the 7-Zip file manager is provided as an example (and is a recommended configuration).

By default, **File manager here** will run Explorer which is fine in most cases where you just need to manage files in a system folder with Administrator or TrustedInstaller access. If you need to manage files within a long path, then a [different file manager](https://gist.github.com/LesFerch/2facb07079394cf2324b6db459bd25d1) is needed because Explorer can only *navigate* long paths.

Please note the following expected behaviors when using this feature with the default (Explorer) configuration:

- On Windows 11, Explorer as **TrustedInstaller** will open a file dialog (aka mini Explorer) instead of a full Explorer window.
- As an Administrator user on Windows 11, Explorer as **Administrator** will open the new Windows 11 Explorer, even if you have set the old Explorer as the default using a tool such as [SwitchExplorer](https://lesferch.github.io/SwitchExplorer/).
- As a Standard user on Windows 11, Explorer as **Administrator** will open a file dialog (aka mini Explorer) unless the registry setting, that prohibits Explorer elevation, is not set (see below for details).
- When you open the file manager as **TrustedInstaller**, the window that opens will be running in the context of the SYSTEM account, so you will get an error if you click on the shortcuts for Documents, Downloads, etc. but you can navigate to your data folders via `C:\Users`.
- When an ***Administrator user*** opens file manager as **Administrator**, the window that opens will be running in the same context as the current user, but with privileges fully elevated to Administrator. All folders and links will be the same as a normal file manager window.
- When a ***Standard user*** opens file manager as **Administrator**, the window that opens will be running in the context of the account used at the UAC prompt. Which personal folders and links are shown, and whether the window opens in light or dark mode, will depend on the account used. 

**Note**: Normally Explorer does not allow itself to be "Run as Administrator", but that behavior can be disabled via a [registry setting](https://gist.github.com/LesFerch/a7e43762bb84f18c8ef6ccdfe606eff8) that requires TrustedInstaller privileges to change. Right-Click Tools temporarily changes that registry setting in order to run Explorer elevated (for Administrator users). Some details about why the restriction exists can be found in [this article and its comments](https://devblogs.microsoft.com/oldnewthing/20220524-00/?p=106682)

### Search here

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/63689d8e-cea8-4c04-9fa7-f3ba084595b4">
  <img alt="image" src="https://github.com/user-attachments/assets/311e4a0d-387a-4c2c-b551-21e7b4d0d968" style="max-width: 100%; height: auto;" />
</picture>

By default, this opens a dialog that helps you build a search term using Advanced Query Syntax (AQS) which is sent to Explorer using the search-ms: protocol. If the query is valid, Explorer should open a window with the search results for that query. The Search Helper does not evaluate your query. It only passes it to Explorer.

If you prefer to use a third-party search tool such as [Everything](https://www.voidtools.com/) or [FileLocator Pro](https://www.mythicsoft.com/filelocatorpro/), you can configure this option to open the search tool of your choice at the current folder. See the **[Configuration](#configuration)** section for details.

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

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/06b1314d-8eb0-44d4-953c-2522cdafa81d">
  <img alt="image" src="https://github.com/user-attachments/assets/fe3ff926-c7bf-4435-ac9a-0bdf6867feeb" style="max-width: 100%; height: auto;" />
</picture>


- `Recent items` are the recent files and folders shown in Explorer's Home or Quick Access section.

- `Auto-suggest items` are the items that show in drop-down lists, such as the Run box.

- `Temporary files` are all files that are not currently in use within the `%Temp%` location.

- The `Recycle Bin` will be emptied when this option is selected.

- `Defender history` is the "Protection history" list in Windows Defender. Right-click Tools creates a task to clear Windows Defender log files on next restart. The task removes itself after it runs. A UAC prompt will appear if you are logged in as a standard user or the privilege elevation task is not installed.

- The `Specified folders` option will clear any folder paths listed in the `Cleanup.txt` file (one folder path per line without quotes). Folder paths specified without a trailing backslash will be emptied (if possible).  Folder paths specified with a trailing backslash will be removed completely (if possible).

### Unblock files here

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/aa4ccd0b-1c7b-40e9-aa7d-8554c8f09dd1">
  <img alt="image" src="https://github.com/user-attachments/assets/a6a45fef-4dd9-4674-b4d2-c241c8279fdc" style="max-width: 100%; height: auto;" />
</picture>

This clears the Mark of the Web (MOTW) from files in the current folder (and optionally all subfolders). This is applicable to NTFS volumes only.

### Take ownership and get access

This gives you ownership and access to the selected folder. Right-click Tools uses the **SetACL** program to do the work. This allows it to set ownership and access on folders with Unicode characters in the name and ones that are in long paths.

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/b55fce8e-87a3-40a7-aed1-1547cf6cfcd7">
  <img alt="image" src="https://github.com/user-attachments/assets/eb0bed42-c4c2-4426-a1a9-8dc397963ce0" style="max-width: 100%; height: auto;" />
</picture>

This option will not allow changing permissions on system folders and will display a message when that's attempted. The restrictions can be edited in the file **RightClickTools.ini**, but that should be avoided. If you need to make changes in a restricted folder, such as **WindowsApps**, you should access the folder using the Cmd or PowerShell Here as **TrustedInstaller** option or the **File manager here** option as **TrustedInstaller**.

**Hidden feature**: Hold down the **Ctrl** key when clicking **OK** to keep the console window open. This can be useful to review the **SetACL** output.

### Add or Remove folder in Path variable

This will show whether the selected folder is currently part of the user or system path and will allow you to change that by checking or unchecking the appropriate box.

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/30b48a2d-b81a-42aa-be4b-8f24f974464b">
  <img alt="image" src="https://github.com/user-attachments/assets/225e403b-17e1-4b22-b655-f8049fc2d6d7" style="max-width: 100%; height: auto;" />
</picture>

### Toggle display of hidden and system files

This toggles between showing hidden and system files and hiding them. It immediately updates the Explorer view without restarting Explorer.

### Refresh shell

This item provides options to do a quick shell refresh or a complete reset of the icon and/or thumbnail caches.

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/03b65c5a-536f-4cc1-8ef2-c0bdfb1e52bb">
  <img alt="image" src="https://github.com/user-attachments/assets/97223057-bbc9-432f-aae8-2e39aebe61aa" style="max-width: 100%; height: auto;" />
</picture>

The default option, "Shell refresh only", quickly refreshes the shell, which is most useful when you've changed an icon, but the old icon or a blank icon is displayed. It tells Explorer to refresh its icon cache and window views. It also tells Explorer to update its current settings (where possible) from the settings in the registry.

If the "Shell refresh only" option does not correct the display of icons and thumbnails then you can select either or both of the "Reset" options which will kill Explorer, wait 2 seconds, clear the icon and/or thumbnail cache, and then restart Explorer.

### Folder Options here

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/36fad1a2-450c-4fc1-9309-a6b9d8d1f32e">
  <img alt="image" src="https://github.com/user-attachments/assets/1c2cc97b-8001-42ff-bcd1-f58e5431c7e6" style="max-width: 100%; height: auto;" />
</picture>

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

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/3c4e5caf-37b1-40ff-8ce1-77be87932d40">
  <img alt="image" src="https://github.com/user-attachments/assets/8d272b7f-c7a3-401a-8cd3-249c42c4ba5e" style="max-width: 100%; height: auto;" />
</picture>

### Settings

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/30bb8549-f5a1-4c3a-93f7-e29a5ae3cd58">
  <img alt="image" src="https://github.com/user-attachments/assets/89bd4cac-d7a6-47dd-a25e-75ab221c737e" style="max-width: 100%; height: auto;" />
</picture>

This provides quick access to the Right-Click Tools configuration files and provides shortcuts to most Windows settings.

The bottom row provides drop-down menus to set the display scale and light or dark colors. This is the same as going to Windows Settings > Display Settings and changing the Scale or going to Settings > Personalize > Colors and selecting Light or Dark.

The scale is set for the monitor where the mouse pointer is currently located. When setting the scale of the primary monitor, the Scale option in Right-Click Tools additionally updates `HKCU\Control Panel\Desktop\WindowMetrics\AppliedDPI`. Normally Windows does not update that registry value until the next startup of Explorer.

Scale values are shown up to 300 by default. The `MaxScale` setting in `RightClickTools.ini` can be used to set a custom maximum from 125 to 500. Although scale values may be shown in the menu all the way up to 500, the scale will not change to anything higher than the maximum that can be set in Windows Settings > Display Settings.

**Note**: The options `Windows Settings` and `Installed apps` do nothing on Windows 7 and 8.

### More Tools

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/8628b081-8086-4874-8487-2cef9cc3b0f9">
  <img alt="image" src="https://github.com/user-attachments/assets/b194ee4c-6997-4e98-a8d1-d6b0b9ffffef" style="max-width: 100%; height: auto;" />
</picture>

This submenu provides two more tools (`Shortcut Tool` and `Date Time Tool`) and can be configured to add any other executables or scripts which may be run as User, Administrator, or TrustedInstaller. In Right-Click Tools, open **Settings** > **Right-Click Tool Settings** and edit the file **MoreTools.ini** to modify this submenu. See the commented-out example entries for details. See the **[Configuration](#configuration)** section for entry details.

### Shortcut Tool

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/110db76c-c49a-4ac7-8a4c-54c7fe4a4926">
  <img alt="image" src="https://github.com/user-attachments/assets/7b17e806-f0e9-426b-bc6d-d387c30ce6b3" style="max-width: 100%; height: auto;" />
</picture>

The **Convert** section is used to convert URL type shortcuts to LNK type shortcuts. This is useful because URL shortcuts do not reliably show icons larger than size medium. That's an old bug that dates back to at least Windows 7 and continues with Windows 11.

The **Search and Replace** section allows doing a search and replace of text within fields in Windows LNK type shortcuts. Note that you must enter something to search for before the `Target`, `Start in`, and `Icon` options can be checked.

### Date Time Tool

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/519af13e-3f9a-4336-b3c2-bf9139c5b35e">
  <img alt="image" src="https://github.com/user-attachments/assets/b7727858-2c92-4aaa-8653-9c45ab6be7cc" style="max-width: 100%; height: auto;" />
</picture>

This is used to make changes to the dates/times for all file in a folder (or all subfolders if that option is checked). The options are self-explanatory.

**Note**: If you need to change the date/time for only one file, or only selected files, you'll need a different tool such as [SKTimeStamp](https://tools.stefankueng.com/SKTimeStamp.html).

### Snip with border

This is a screen capture tool specifically for capturing a window + border. All of the dialog screenshots in this document have a small border of background around them for contrast. They were all captured with the Snip with border tool. This is much faster, and much more accurate, than using the Snipping Tool and trying to select a consistent amount of border around a window.

Once the tool is activated, it will appear in the taskbar. Clicking on the taskbar icon simply shows a small window that tells you the current shortcut key to do a capture. It's **Alt-Z** by default. Press **Alt-Z** to start a capture. The mouse cursor will change to a finger pointer. Click on the window you want to capture. It will be placed on the clipboard.

You can change the shortcut key and/or the border width via the **RightClickTools.ini** file. See the **[Configuration](#configuration)** section for entry details. 

## Configuration

- To change the current configuration, edit the INI files within your `%localappdata%\RightClickTools`folder.
- To change the default (i.e. initial) settings for portable use (e.g. when run from a flash drive), edit the INI files within the `AppParts` folder. This would also be the place to change the current configuration if `FullyPortable=1` and the `AppParts` folder is writeable.

| INI File | Section | Setting | Value |
| :--- | :--- | :--- | :--- |
| RightClickTools | General | Lang | Use any two letter language code found in the `Language.ini` file |
| RightClickTools | General | Editor | Set the path to your preferred text editor for RightClickTools edit functions |
| RightClickTools | General | NoTrustedInstaller | 0 = Enable TrustedInstaller options <br> 1 = Disable TrustedInstaller options |
| RightClickTools | General | FullyPortable | This value must be set in the `AppParts` INI file <br> 0 = Settings are stored in %localappdata% <br> 1 = Settings are stored in the `AppParts` INI file (if writeable) |
| RightClickTools | Launcher <br> MoreTools <br> SearchHere <br> Settings | AutoClose | 0 = Keep dialog open <br> 1 = Auto-close dialog after making a selection |
| RightClickTools | Launcher <br> MoreTools <br> MoreTools | Style | 0 = Use style 2 on Win 11+, otherwise use style 1 <br> 1 = 9 point font and tight spacing <br> 2 = 10 point font and loose spacing |
| RightClickTools | CmdHere <br> PowerShellHere <br> PowerShellCoreHere <br> FileManagerHere <br> SearchHere <br> RegEdit | RunAs | 0 = Run as current user <br> 1 = Run as Administrator <br> 2 = Run as TrustedInstaller <br> 3 = Pop-up menu to select elevation <br><br> Optionally hold **Ctrl** key to get pop-up menu|
| RightClickTools | CmdHere <br> PowerShellHere <br> PowerShellCoreHere | WTadmin | 0 = Use ConHost for Run as Administrator <br> 1 = Use Windows Terminal (if installed) for Run as Administrator |
| RightClickTools | PowerShellCoreHere <br> FileManagerHere <br> SearchHere | Exe | Optional path to your preferred executable |
| RightClickTools | TakeOwnHere | StopAll | Owner and permission changes will be refused for these paths and all their subfolders |
| RightClickTools | TakeOwnHere | StopRoot | Owner and permission changes will be refused for these paths at the root only |
| RightClickTools | Settings | MaxScale | Maximum scale value to show in the `Scale` dropdown menu within the Settings dialog (default=300) |
| RightClickTools | SnipWithBorder | Key | Alt key combination (e.g. Alt-Z) |
| RightClickTools | SnipWithBorder | BorderWidth | Border width in pixels at 100% scaling. The border width will be scaled proportionally with your screen scaling setting. |
| Launcher <br> MoreTools | All | Exe | Path to the executable <br><br> For internal commands it's always `RightClickTools.exe` <br><br> For external commmands it's the full path to the executable (without quotes) <br><br> For scripts this must be the full path of the script interpreter (Cmd PowerShell WScript Python etc.) |
| Launcher <br> MoreTools | All | CmdLine | Command line to pass to the executable <br><br> Include `"%V"` (with the quotes) to specify the selected path <br><br> For internal commands the value will always be a single argument that specifies which tool to run <br><br> For scripts this would typically be the full path to the script and possibly additional arguments |
| Launcher <br> MoreTools | All | Icon | Full path to an ICO file to show in the launcher <br><br> If omitted, the executable's icon will be used |
| Launcher <br> MoreTools | All | RunAs | 0 = Run as current user <br> 1 = Run as Administrator <br> 2 = Run as TrustedInstaller <br> 3 = Pop-up menu to select elevation <br><br> Optionally hold **Ctrl** key to get pop-up menu |

\
\
[![image](https://github.com/LesFerch/WinSetView/assets/79026235/63b7acbc-36ef-4578-b96a-d0b7ea0cba3a)](https://github.com/LesFerch/RightClickTools)

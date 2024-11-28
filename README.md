# RightClickTools

**Classic context menu**:

![image](https://github.com/user-attachments/assets/bc1529c1-2ee7-4185-bba7-f4a8298129fb)


**Windows 11 context menu** (requires [Custom Context Menu](https://apps.microsoft.com/detail/9pc7bzz28g0x?hl=en-US&gl=US)):

![image](https://github.com/user-attachments/assets/645850ab-d06d-4eec-8c0e-93263243f227)


## Summary

**Version 1.2.0**\
lesferch@gmail.com

This program adds a right-click context menu to Windows Explorer that provides a number of tools that are described in detail below. The tools are compatible with Windows 7 and above, 32 bit and 64 bit, standard and administrator users, multiple users on the same computer, and long paths.

The tools are added to the new Windows 11 context menu if you have the app [Custom Context Menu](https://apps.microsoft.com/detail/9pc7bzz28g0x?hl=en-US&gl=US) installed. Please note that its web page shows a price of $0.99, but if you click the link, you should see that the app is available for an unlimited free trial.

For administrator users, that choose to install the optional privilege elevation task, there is only a single UAC prompt to install the tools. After that, all the tools run without any UAC prompts.

All languages are supported. The included **Language.ini** file includes many languages and can be edited to add other languages and/or change any of the labels. The included translations are machine generated, so they may need some editing.

## How to Download and Install 

[![image](https://github.com/user-attachments/assets/75e62417-c8ee-43b1-a8a8-a217ce130c91)Download the installer](https://github.com/LesFerch/RightClickTools/releases/download/1.2.0/RightClickTools-Setup.exe)

[![image](https://github.com/LesFerch/WinSetView/assets/79026235/0188480f-ca53-45d5-b9ff-daafff32869e)Download the zip file](https://github.com/LesFerch/RightClickTools/releases/download/1.2.0/RightClickTools.zip)

**Note**: Some antivirus software may falsely detect the download as a virus. This can happen any time you download a new executable and may require extra steps to whitelist the file.

**Note**: Scanning Right-Click Tools with VirusTotal will show that many AV products out there detect it as a trojan (9 of 73 last check). Those are false positives. This is to be expected with a tool that creates a scheduled task in order to provide the convenience of Administrator and Trusted Installer access without UAC prompts. Right-Click Tools is provided on GitHub as open source, the Exe is signed, and my identity is not hidden. I submit my apps to the Windows Defender team to ensure that Windows Defender is okay with them, but I don't have the resources to do that for all AV products.

### Install Using Setup Program

1. Download the installer using the link above.
2. Double-click **RightClickTools-Setup.exe** to start the installation.
4. In the SmartScreen window, click **More info** and then **Run anyway**.

**Note**: The installer is only provided in English, but the right-click menu items will be created using your current Windows language, if that language is included in the **Language.ini** file.

The right-click menu items will be created for the user that is currently logged on interactively (i.e. desktop is displayed). If you wish to add the right-click menu items to *other* users, log on as each user and either run **RightClickTools-Setup.exe** again or navigate to the **RightClickTools** folder and double-click **RightClickTools.exe** (see **Install and Remove** below for details).

If you don't have other users to set up, skip down to the [**How to Use**](#how-to-use) section.

### Portable Install

1. Download the zip file using the link above.
2. Extract the contents. You should see **RightClickTools.exe** and an **AppParts** folder.
3. Move the contents to a permanent location of your choice. For example **C:\Tools\RightClickTools**.
3. Right-click **RightClickTools.exe**, select Properties, check **Unblock**, and click **OK**.
5. Double-click **RightClickTools.exe** to open the Install/Remove dialog and click **Install** to add the tools to the Explorer right-click menu.
6. If you skipped step 4, then, in the SmartScreen window, click **More info** and then **Run anyway**.
7. Click **OK** when the **Done** message box appears.

When Right-click Tools is installed as a portable app, you will NOT see the app listed under **Apps** or **Programs and Files**. 

## Install and Remove

The app's install/remove procedure adds, or removes, the commands to/from the context menu. Those commands all use **RightClickTools.exe**, so the files must remain in place after doing the **Install**.

The **Remove** option removes the context menu entries and the scheduled task item (if installed). It does not delete the app files.

On Windows 7 through Windows 10, you should see an Install/Remove prompt with a checkbox to select whether the privilege elevation task is to be installed.

![image](https://github.com/user-attachments/assets/1fc53cc7-5f24-45f4-9c67-d1c20a052b3a)


On Windows 11, there's an additional checkbox to allow selecting the context menu type. The box will be checked if you've already changed Windows 11 to use the classic context menu. Check or uncheck the box to select your preferred context menu type. The change, if any, will occur after clicking **Install** or **Remove**.

![image](https://github.com/user-attachments/assets/97edd04c-a4b2-4a4e-a5c8-746d484ac5ab)


If you're an Administrator user, and the privilege elevation task option is checked, you will then see the following UAC prompt after clicking **Install** or **Remove**. This is required so that a scheduled task may be added or removed. The task allows right-click commands, such as **Cmd Here as Administrator** to open without popping up a UAC prompt.

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/4d1cb77e-3d13-4a2a-bf93-2db5e60bb4da)

If you're a Standard user, or the privilege elevation task option is unchecked, there will be no UAC prompt with a portable install. Only the context menu will be created. In that case, a UAC prompt will pop up when any tools are used that require elevated privileges.

**IMPORTANT**: Please note that, even though the **Privilege elevation task** is set up to only be used by Right-Click Tools, anyone, with some programming skill, could leverage the task to run any code without a UAC prompt. That's highly unlikely to happen because a niche utility, such as Right-Click Tools, is never going to be on enough computers to be of interest as an attack vector for bad actors. Nevertheless, it's up to you to decide to accept the risk and install the task. If you're on a work computer, I would advise against installing the task.

**Note**: If you have already disabled UAC, the privilege elevation task does not add any additional risk, but it is then mostly unnecessary. There is a small difference for the **Privileged file manager here** option in that it will only launch a full Explorer window if the task is installed or the registry setting, that prevents Explorer elevation, is disabled (see details about that later in this document).

Upon completion the following dialog pops up. It may be hidden under another window, but can always be found on the taskbar.

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/0e063de7-d0bc-4dcd-9cce-fc07fcbb6ade)

**Note**: If you move **RightClickTools.exe** after installing, the context menu entries will do nothing because the exe path will be incorrect. To fix that issue, just run the install again.

### About the context menu

The context menu item is created with registry entries only and simply provides submenus entries for each command. When one of those commands are selected, **RightClickTools.exe** is run with the appropriate arguments to open the selected option.

This program does NOT create a context menu handler. That is, there is no code that runs when you right-click a folder. Code only runs when you actually select an action. Right-click Tools adds no overhead to your context menu, other than the insignificant impact of one more context menu item.

If you're running Windows 11 and have [Custom Context Menu](https://apps.microsoft.com/detail/9pc7bzz28g0x?hl=en-US&gl=US) installed, Right-click Tools adds its entries to the menu, by creating the necessary JSON files in Custom Context Menu's data folder. The main menu entry is labelled **Open with** by default. You can change that label to whatever you like in the app settings for **Custom Context Menu**.

## Language Selection

By default, **Install** will create the context menu items in the current system language if that language is found in the **Language.ini** file. Otherwise, it will default to English. To force the context menu items to be created in a specific language, edit the **RightClickTools.ini** file and uncomment (remove the semicolon) and change the **Lang=en** entry to the two letter code of the desired language found in the **Language.ini** file. Then, just double-click **RightClickTools.exe** and click **Install** to update the context menu entries to the new language.

## How to Use

Right-click a folder, the background of an open folder, or a drive to get to the **Right-click Tools** context menu, as shown at the beginning of this document.

**Note**: If you are using Windows 11, and do not have [Custom Context Menu](https://apps.microsoft.com/detail/9pc7bzz28g0x?hl=en-US&gl=US) installed or the classic context menu enabled, you will have to hold down the **Shift** key, when right-clicking, to access Right-Click Tools (or select "Show more options"). Also, if the classic context menu is not enabled, you will have to hold down the **Shift** key when right-clicking a *drive* (or select "Show more options").

Select the action you wish to perform. If nothing happens, then the Exe was likely moved after installing. In that case, just double-click the Exe to re-install.

**Note**: Clicking the **`X`** in any dialog means _do nothing_. Therefore, there's no need for "No" or "Cancel" buttons.

**Note**: Since Right-Click Tools is a .Net application, there can be some delay on first run of some menu items. Also Windows Defender (or other AV software) can add delays to intial launches as it scans the components.

## Commands

### Cmd Here and PowerShell Here

This opens **Cmd** or **PowerShell**, with your normal privileges, in the selected location, using your default console program (**Console Host** or **Windows Terminal**).

This is identical to the built-in Windows commands with the following improvements: 1) For Cmd, folders with environment variables in their names, such as **%OS%** will not cause an error. 2) For PowerShell, folders with an apostrophe in the name, such as **Bob's Files** will not cause an error. 3) For PowerShell, opening a folder with a long path (i.e. > 260 characters) will create a prompt in the short (8.3) format. This lets you know that you're in a long path and prevents the prompt from taking too much screen space.

**Note**: If you open a PowerShell prompt within a long path, running an executable, such as **whoami** will result in an error (or no output at all with PowerShell Core). This is a PowerShell problem. It has nothing to do with Right-click Tools and there is no known fix or workaround.

### Cmd Here and PowerShell Here as Administrator

This opens **Cmd** or **PowerShell**, with Administrator privileges, in the selected location, using Console Host.

Administrator users, with the privilege elevation task installed, will not see a UAC prompt. Otherwise a UAC prompt will appear for both Standard and Administrator users (unless UAC is disabled).

Administrator users will be working within their own user profile. Standard users will be working within the profile of the account used at the UAC prompt.

The enhancements and issues noted above for the regular **PowerShell Here** window also apply when running as Administrator.

### Cmd Here and PowerShell Here as TrustedInstaller

This opens **Cmd** or **PowerShell** via the SYSTEM account with **TrustedInstaller** privileges. This is useful for accessing and making changes in protected folders, such as **WindowsApps**. Use with care.

**Note**: Trusted Installer is not a user. It's a service that runs via the SYSTEM account. Therefore a **WhoAmI** command will display **nt authority\system**.

#### PowerShell Core configuration

If you want the "PowerShell Here" commands to open PowerShell Core instead of PowerShell 5.1, edit the file **RightClickTools.ini** and, in the **PowerShellHere** section, uncomment (remove the semicolon) and change the **Exe=** entry to the path for PowerShell Core.

### RegEdit as User

This opens RegEdit as a standard user. As an administrator, this can be handy to verify what a standard user can or cannot change in the registry without having to analyze the permissions. As a standard user, this option opens RegEdit as it would normally.

**Hidden feature**: Hold down the **Ctrl** key when selecting this item to open RegEdit collapsed.

### RegEdit as Administrator

This opens RegEdit as an administrator. As an administrator, this is the same as the normal method of opening RegEdit except that it eliminates the nuisance of the UAC prompt (if the privilege elevation task is installed). For a standard user, this option pops up a UAC prompt and then regedit will be running under the profile of the account used at the UAC prompt.

**Hidden feature**: For administrator users, hold down the **Ctrl** key when selecting this item to open RegEdit collapsed.

### RegEdit as TrustedInstaller

This opens **RegEdit** via the SYSTEM account with **TrustedInstaller** privileges. This is handy for changing a protected setting. Use with care.

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

This option will not allow changing permissions on system folders and will display a message when that's attempted. The restrictions can be edited in the file **RightClickTools.ini**, but that should be avoided. If you need to make changes in a restricted folder, such as **WindowsApps**, you should access the folder using the Cmd or PowerShell Here as **TrustedInstaller** option or the **Privileged file manager here** option as **Trusted Installer**.

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


### Privileged file manager here

This starts the file manager of your choice as **Administrator** or **Trusted Installer**.

![image](https://github.com/user-attachments/assets/50887fd7-ada1-412b-a827-8a878796acbf)

Edit the file **RightClickTools.ini** and, in the **FileManagerHere** section, uncomment (remove the semicolon) and change the **Exe=** entry to the path of your preferred file manager. The path for 7-Zip is provided as an example.

**Note**: If **Privileged file manager here** fails to open a window (may happen on first run due to .Net initialization), try one of the other "Trusted Installer" options, such as **Cmd here as Trusted Installler** and then try again.

**Note**: Explorer can only *navigate* long paths. If you need to make changes to long paths, consider using a [different file manager](https://gist.github.com/LesFerch/2facb07079394cf2324b6db459bd25d1) that fully supports long paths, such as 7-Zip. 

Please note the following expected behaviors when using this feature:

- On Windows 11, Explorer as **Trusted Installer** will open a file dialog (aka mini Explorer) instead of a full Explorer window.
- As an Administrator user on Windows 11, Explorer as **Adminstrator** will open the new Windows 11 Explorer, even if you have set the old Explorer as the default using a tool such as [SwitchExplorer](https://lesferch.github.io/SwitchExplorer/).
- As a Standard user on Windows 11, Explorer as **Adminstrator** will open a file dialog (aka mini Explorer) unless the registry setting, that prohibits Explorer elevation, is not set (see below for details).
- When you open the file manager as **Trusted Installer**, the window that opens will be running in the context of the SYSTEM account, so you will get an error if you click on the shortcuts for Documents, Downloads, etc. but you can navigate to your data folders via `C:\Users`.
- When an ***Administrator user*** opens file manager as **Adminstrator**, the window that opens will be running in the same context as the current user, but with privileges fully elevated to Administrator. All folders and links will be the same as a normal file manager window.
- When a ***Standard user*** opens file manager as **Adminstrator**, the window that opens will be running in the context of the account used at the UAC prompt. Which personal folders and links are shown, and whether the window opens in light or dark mode, will depend on the account used. 

**Note**: Normally Explorer does not allow itself to be "Run as Administrator", but that behavior can be disabled via a [registry setting](https://gist.github.com/LesFerch/a7e43762bb84f18c8ef6ccdfe606eff8) that requires TrustedInstaller privileges to change. Right-Click Tools temporarily changes that registry setting in order to run Explorer elevated (for Administrator users). Some details about why the restriction exists can be found in [this article and its comments](https://devblogs.microsoft.com/oldnewthing/20220524-00/?p=106682)

## It's Multilingual

The Right-click Tools **Install** will detect your Windows language and use it, as long as it has your language in its **Language.ini** file. Edit the **Language.ini** file to set the labels to your preferences.

Here's a screenshot of Right-click Tools in German:

![image](https://github.com/user-attachments/assets/4ff11617-1eac-442a-a4ce-a9e6c8d47de9)


## Dark Theme Compatible

Right-click Tools automatically detects and switches to a dark theme. For example:

![image](https://github.com/user-attachments/assets/0a0f18ea-4c68-470f-93fa-620e84a6c640)


## Customizations

### Checkbox Style

The checkboxes in the Right-Click Tools dialogs can be configured to use one of two different styles. Edit the file **RightClickTools.ini** and change **AlternateCheckbox=0** to  **AlternateCheckbox=1** to change the style. Below is an example of the difference in Windows 10 dark mode:

![image](https://github.com/user-attachments/assets/8c18bcef-3e77-411b-9bf6-e659afd4f84c)

**Tip**: When changing options in Right-Click Tools, it's easier to click the label beside the checkbox.


### Adding and Removing Context Menu Items

Removing an item you don't use, or adding your own custom items, can be done with a context menu editor.

If you are using Windows 11 with [Custom Context Menu](https://apps.microsoft.com/detail/9pc7bzz28g0x?hl=en-US&gl=US), use that tool to modify the entries in the Windows 11 context menu.

You can use [ContextMenuManager](https://github.com/BluePointLilac/ContextMenuManager/blob/master/README-en.md) to modify the classic context menu. The classic context menu can also be directly edited with RegEdit if you know what you're doing.

\
\
[![image](https://github.com/LesFerch/WinSetView/assets/79026235/63b7acbc-36ef-4578-b96a-d0b7ea0cba3a)](https://github.com/LesFerch/RightClickTools)

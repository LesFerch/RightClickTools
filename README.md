# RightClickTools

## Coming soon!

Classic context menu:

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/f5cbcbb7-5a9d-42e3-84a6-53d9259cc69b)

Windows 11 context menu:

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/6dbf7308-7cc0-49a0-92b2-6d616b8c91ee)

## Summary

This program adds a right-click context menu to Windows Explorer that provides a number of tools that are described in detail below. The tools are compatible with Windows 7 and above, 32 bit and 64 bit, standard and administrator users, multiple users on the same computer, and long paths.

The tools are added to the new Windows 11 context menu if you have the app [Custom Context Menu](https://apps.microsoft.com/detail/9pc7bzz28g0x?hl=en-US&gl=US) installed. Please note that its web page shows a price of $0.99, but if you click the link, you should see that the app is available for an unlimited free trial.

For administrator users, there is only a single UAC prompt to install the tools. After that, all the tools run without any UAC prompts.

All languages are supported. The included **language.ini** file includes many languages and can be edited to add other languages and/or change any of the labels. The included translations are machine generated, so they may need some editing.

## How to Download and Install 

[![image](https://user-images.githubusercontent.com/79026235/152910441-59ba653c-5607-4f59-90c0-bc2851bf2688.png)Download the zip file](https://github.com/LesFerch/RightClickTools/releases/download/1.0.0/RightClickTools.zip)

**Note**: Some antivirus software may falsely detect the download as a virus. This can happen any time you download a new executable and may require extra steps to whitelist the file.

1. Download the zip file using the link above.
2. Extract the contents. You should see **RightClickTools.exe** and an **AppParts** folder.
3. Move the folder to a permanent location of your choice. For example **C:\Tools\RightClickTools**.
3. Right-click **RightClickTools.exe**, select Properties, check **Unblock**, and click **OK**.
5. Double-click **RightClickTools.exe** to open the Install/Remove dialog and click **Install** to add the tools to the Explorer right-click menu.
6. If you skipped step 3, then, in the SmartScreen window, click **More info** and then **Run anyway**.
7. Click **OK** when the **Done** message box appears.

## Install and Remove

Right-click Tools is a portable app. That is, you can place the files wherever you like and you will NOT see the app listed under **Apps** or **Programs and Files**. However, there is an install/remove procedure that adds, or removes, the commands to/from the context menu. Those commands all use **RightClickTools.exe**, so the files must remain in place after doing the **Install**.

The **Remove** option removes the context menu entries and the scheduled task item (for administrators). It does not delete the app files.

On Windows 7 through Windows 10, you should see a simple Install/Remove prompt.

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/7af3e31c-088a-4afe-acd1-a449c3a4115c)

On Windows 11, there is an additional checkbox to allow selecting the context menu type. The box will be checked if you've already changed Windows 11 to use the classic context menu. Check or uncheck the box to select your preferred context menu type. The change, if any, will occur after clicking **Install** or **Remove**.

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/c46e1d96-33be-445d-8630-38ed92094a27)

If you're an administrator user, you will then see the following UAC prompt after clicking **Install** or **Remove**. This is required so that a scheduled task may be added or removed. The task allows right-click commands, such as **Cmd Here as Administrator** to open without popping up a UAC prompt.

For Standard users, the task cannot be used, so it's not installed and therefore no UAC prompt pops up during installation. Instead, Standard users will get a UAC prompt every time they use an option that requires administrator privileges, such as **Cmd Here as Administrator**. 

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/38815f01-b30c-4de1-b7c5-d855a6cb402a)

Upon completion the following dialog pops up. It may be hidden under another window, but can always be found on the taskbar.

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/2f4ba1b1-1d97-4626-afcb-f0510790359e)

**Note**: If you move **RightClickTools.exe** after installing, the context menu entries will do nothing because the exe path will be incorrect. To fix that issue, just run the install again.

### About the context menu

The context menu item is created with registry entries only and simply provides submenus entries for each command. When one of those commands are selected, **RightClickTools.exe** is run with the appropriate arguments to open the selected option.

This program does NOT create a context menu handler. That is, there is no code that runs when you right-click a folder. Code only runs when you actually select an action. Right-click Tools adds no overhead to your context menu, other than the insignificant impact of one more context menu item.

If you're running Windows 11 and have [Custom Context Menu](https://apps.microsoft.com/detail/9pc7bzz28g0x?hl=en-US&gl=US) installed, Right-click Tools adds its entries to the menu, by creating the necessary JSON files in Custom Context Menu's data folder. The main menu entry is labelled **Open with** by default. You can change that label to whatever you like in the app settings for **Custom Context Menu**.

## How to Use

Right-click a folder, the background of an open folder, or a drive to get to the **Right-click Tools** context menu, as shown at the beginning of this document.

**Note**: On Windows 11, to access Right-click Tools for a *drive*, you will have to hold down the **Shift** key when right-clicking, unless the classic context menu has been set as the default.

Select the action you wish to perform. If nothing happens, then the Exe was likely moved after installing. In that case, just double-click the Exe to re-install.

## Commands

### Cmd Here and PowerShell Here

This opens **Cmd** or **PowerShell**, with your normal privileges, in the selected location, using your default console program (**Console Host** or **Windows Terminal**).

This is identical to the built-in Windows commands with two improvements for PowerShell: 1) Folders with an apostrophe in the name, such as **Bob's Files** will not cause an error. 2) Opening a folder with a long path (i.e. > 260 characters) will cause the prompt to be shown in the short (8.3) format. This lets you know that you're in a long path and prevents the prompt from taking too much screen space.

**Note**: If you open a PowerShell prompt within a long path, running an executable, such as **whoami** will result in an error. This is a PowerShell problem. It has nothing to do with Right-click Tools and there is no known fix or workaround.

### Cmd Here and PowerShell Here as Administrator

This opens **Cmd** or **PowerShell**, with Administrator privileges, in the selected location, using Console Host.

For administrator users, the window will open with no UAC prompt and you will still be working within your user profile.

For standard users, there will be a UAC prompt and you will be working within the profile of the account used at the UAC prompt.

The enhancements and issues noted above for the regular **PowerShell Here** window also apply when running as Administrator.

### Cmd Here and PowerShell Here as TrustedInstaller

This opens **Cmd** or **PowerShell** via the SYSTEM account with **TrustedInstaller** privileges. This is useful for accessing and making changes in proteced folders, such as **WindowsApps**. Use with care.

**Tip**: Create a folder, such as C:\Tools\OnPath, to contain scripts and executables that you want easy access to via Cmd or PowerShell. Add that folder to your search path using the **Add or Remove folder in Path variable** tool. Put a copy of a portable file manager in that folder, such as [Explorer++](https://explorerplusplus.com/) and give it a short name, such as **ep.exe**. That will give you quick access to a GUI file manager with TrustedInstaller access.

### RegEdit Here as User

This opens RegEdit as a standard user. As an administrator, this can be handy to verify what a standard user can or cannot change in the registry without having to analyze the permissions. As a standard user, this option opens RegEdit as it would normally.

### RegEdit Here as Administrator

This opens RegEdit as an administrator. As an administrator, this is the same as the normal method of opening RegEdit except that it eliminates the nuisance of the UAC prompt. For a standard user, this option pops up a UAC prompt and then regedit will by running under the profile of the account used at the UAC prompt.

### RegEdit Here as TrustedInstaller

This opens **RegEdit** via the SYSTEM account with **TrustedInstaller** privileges. This is handy for changing a protected setting. Use with care.

### Clear History

This clears the data for the selected items.

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/a5a322d6-7674-4fcc-a36c-808c6a6bab47)

Recent items are the recent files and folders shown in Explorer's Home or Quick Access section.

Auto-suggest items are the items that show in drop-down lists, such as the Run box.

Temporary files are all files that are not currently in use within the %Temp% location.

Defender history is the "Protection history" list in Windows Defender. Right-click tools elevates to TrustedInstaller access in order to clear this history. A UAC prompt will appear if you are logged in as a standard user.

### Take owernship and get access

This gives you ownership and access to the selected folder. Right-click Tools uses the **SetACL** program to do the work. This allows it to set ownership and access on folders with Unicode characters in the name and ones that are in long paths.

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/b0d1807a-7b34-400d-8d01-83adf8d71fc9)

This option will not allow changing permissions on system folders and will display a message when that's attempted. The restrictions can be edited in the file **RightClickTools.ini**, but that should be avoided. If you need to make changes in a restricted folder, such as **WindowsApps**, you should access the folder using the Cmd or PowerShell Here as **TrustedInstaller** option.

### Add or Remove folder in Path variable

This will show whether the selected folder is currently part of the user or system path and will allow you to change that by checking or unchecking the appropriate box.

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/5f19b328-082a-4489-bdb6-ef84be49a7c1)

### Toggle display of hidden and system files

This toggles between showing hidden and system files and hiding them. It immediately updates the Explorer view without restarting Explorer.

### Refresh shell

This refreshes the shell, which is most useful when you've changed an icon, but the old icon or a blank icon is displayed. It tells Explorer to refresh its icon cache and window views. It also tells Explorer to update its current settings (where possible) from the settings in the registry.

### Restart Explorer

This restarts Explorer and opens a window to the currently selected folder.

## It's Multilingual

The Right-click Tools **Install** will detect your Windows language and use it, as long as it has your language in its **Language.ini** file. Edit the **Language.ini** file to set the labels to your preferences.

Here's a screenshot of Right-click Tools in German:

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/1d994985-8743-46f6-b7bb-e890e8284de7)

## Dark Theme Compatible

Right-click Tools automatically detects and switches to a dark theme. For example:

![image](https://github.com/LesFerch/RightClickTools/assets/79026235/a566d75c-577d-4fac-81ff-fbdde96b5bfa)

## Customizations

Customizations, such as moving an item to the root of the context menu or removing an item you don't use, can be done with a context menu editor.

If you are using Windows 11 with [Custom Context Menu](https://apps.microsoft.com/detail/9pc7bzz28g0x?hl=en-US&gl=US), use that tool to modify the entries in the Windows 11 context menu.

You can use [ContextMenuManager](https://github.com/BluePointLilac/ContextMenuManager/blob/master/README-en.md) to modify the classic context menu.

\
\
[![image](https://user-images.githubusercontent.com/79026235/153264696-8ec747dd-37ec-4fc1-89a1-3d6ea3259a95.png)](https://github.com/LesFerch/RightClickTools)

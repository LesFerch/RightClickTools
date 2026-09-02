## 2.0.0

- Now directly supports the Windows 11 modern context menu.
- Cmd, PowerShell, PowerShell Core (new), and RegEdit menu items are now single entries with a pop-up to run-as User, Administrator, or TrustedInstaller.
- An alternative configuration can be set to only show the run-as pop-up when the **Ctrl** key is held down.
- File Manager Here now can be run as User, in addition to Administrator and TrustedInstaller, to allow launching a third-party file manager as the current user at the current location.
- New **Search here** option can be configured to run your preferred search tool such as Everything or File Locator Pro or use the default which adds an Explorer AQS search helper.
- Improved **Clear History** has additional options to empty the recycle bin and clear a user-defined set of folders.
- New **Unblock files here** unblocks all files for the selected folder.
- New **Folder options here** provides options to set the folder type for a whole folder or tree, set folder colors and 1, 2 and 4 up thumbnails using icons that are generated automatically from image files.
- New **Settings** menu provides shortcuts to all Windows settings, the app's configuration files, and Scale and Light/Dark selectors.
- New **Shortcut Tool** allows search and replace on LNK files and an option to convert URL shortcuts to LNK shortcuts.
- New **Date Time Tool** provides many options for copying one date format to another for a whole folder or whole tree.
- New **Snip with border** utility provides an easy way to capture any window with a consistent amount of background border for contrast.
- New **More Tools** menu allows for additional tools and user-specified tools to be added. For example, you could add a tool, such as grepWin, with the option to run it as User, Administrator, or TrustedInstaller.
- New Launcher mode (double-click the main exe) allows all tools, including user-specified tools, to be run without any installation.
- Many configuration options added (see README for details).
- Improved and modernized interface with better dark mode support.
- Improved installer and setup program.

## 1.2.1

- Clear Defender History now also clears the Quarantine folder.
- The Japanese translation has been corrected (Thanks to GitHub user reindex-ot).
- Other minor Language file corrections.

## 1.2.0

- The privilege elevation task (UAC suppression) is now an optional install.
- The language of the context menu items can be forced via a setting in RightClickTools.ini.
- The Take ownership console window can be kept open by holding Ctrl when clicking OK.

## 1.1.4

- Added option in RightClickTools.ini to set PowerShell Core as the default for the PowerShell Here commands

## 1.1.3

- Enabled Windows 11 Explorer compact mode for all instances of Privileged file manager here as Trusted Installer

## 1.1.2

- Changed Privileged file manager here as Trusted Installer to use a file dialog (mini Explorer) on Windows 11 because allowing the full Explorer to load would result in a legacy Explorer window where the navigation buttons don't work.
- Enabled showing of Hidden and System files for all instances of Privileged file manager here as Trusted Installer

## 1.1.1

- Fixed bug where first context menu item was not created correctly if the current system language did not exist in the Language.ini file.

## 1.1.0

- Added Privileged file manager here feature
- Added options to reset icon and thumbnail caches
- Replaced Yes/No prompts with OK (since X already serves as a No)
- Added option to choose an alternate checkbox style
- Improved dark mode button highlighting
- Now recognizes difference between desktop and a desktop window on Explorer restart
- Added an installer (RightClickTools-Setup.exe)
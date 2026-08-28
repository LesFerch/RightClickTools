#include "pch.h"

using namespace Microsoft::WRL;

HMODULE g_hModule = nullptr;

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD ul_reason_for_call,
    LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        g_hModule = hModule;
        DisableThreadLibraryCalls(hModule);
        break;
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

// Helper function to get RightClickTools.exe path from App Paths registry
std::wstring GetRightClickToolsExePath()
{
    WCHAR exePath[MAX_PATH] = { 0 };
    DWORD bufferSize = sizeof(exePath);

    // Try to read from HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\RightClickTools.exe
    HKEY hKey = nullptr;
    LONG result = RegOpenKeyExW(
        HKEY_LOCAL_MACHINE,
        L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\RightClickTools.exe",
        0,
        KEY_READ,
        &hKey
    );

    if (result == ERROR_SUCCESS)
    {
        result = RegQueryValueExW(
            hKey,
            nullptr,  // Query the (Default) value
            nullptr,
            nullptr,
            reinterpret_cast<LPBYTE>(exePath),
            &bufferSize
        );

        RegCloseKey(hKey);

        if (result == ERROR_SUCCESS && exePath[0] != L'\0')
        {
            return std::wstring(exePath);
        }
    }

    // Fallback to hard-coded path if registry key doesn't exist
    return L"C:\\Program Files\\RightClickTools\\RightClickTools.exe";
}

// UTF-8 aware INI file reader
std::wstring ReadIniValueUTF8(const std::wstring& iniPath, const std::wstring& section, const std::wstring& key, const std::wstring& defaultValue)
{
    // Read the entire file as UTF-8
    std::ifstream file(iniPath, std::ios::binary);
    if (!file.is_open())
        return defaultValue;

    // Read file into string
    std::string content((std::istreambuf_iterator<char>(file)), std::istreambuf_iterator<char>());
    file.close();

    // Skip UTF-8 BOM if present
    if (content.size() >= 3 && 
        (unsigned char)content[0] == 0xEF && 
        (unsigned char)content[1] == 0xBB && 
        (unsigned char)content[2] == 0xBF)
    {
        content = content.substr(3);
    }

    // Convert search strings to UTF-8
    int sectionLen = WideCharToMultiByte(CP_UTF8, 0, section.c_str(), -1, NULL, 0, NULL, NULL);
    int keyLen = WideCharToMultiByte(CP_UTF8, 0, key.c_str(), -1, NULL, 0, NULL, NULL);

    if (sectionLen <= 0 || keyLen <= 0)
        return defaultValue;

    std::vector<char> sectionUtf8(sectionLen);
    std::vector<char> keyUtf8(keyLen);

    WideCharToMultiByte(CP_UTF8, 0, section.c_str(), -1, sectionUtf8.data(), sectionLen, NULL, NULL);
    WideCharToMultiByte(CP_UTF8, 0, key.c_str(), -1, keyUtf8.data(), keyLen, NULL, NULL);

    std::string sectionStr = "[" + std::string(sectionUtf8.data()) + "]";
    std::string keyStr = std::string(keyUtf8.data());

    // Find the section
    size_t sectionPos = content.find(sectionStr);
    if (sectionPos == std::string::npos)
        return defaultValue;

    // Find the next section or end of file
    size_t nextSectionPos = content.find("\n[", sectionPos + 1);
    if (nextSectionPos == std::string::npos)
        nextSectionPos = content.length();

    // Search for the key within this section
    size_t searchStart = sectionPos + sectionStr.length();
    size_t keyPos = content.find(keyStr + "=", searchStart);

    // Make sure the key is in this section
    if (keyPos == std::string::npos || keyPos >= nextSectionPos)
        return defaultValue;

    // Check if this is at the start of a line
    if (keyPos > 0 && content[keyPos - 1] != '\n' && content[keyPos - 1] != '\r')
        return defaultValue;

    // Extract the value
    size_t valueStart = keyPos + keyStr.length() + 1; // +1 for '='
    size_t valueEnd = content.find_first_of("\r\n", valueStart);

    if (valueEnd == std::string::npos)
        valueEnd = content.length();

    std::string valueUtf8 = content.substr(valueStart, valueEnd - valueStart);

    // Trim whitespace
    size_t firstNonSpace = valueUtf8.find_first_not_of(" \t");
    size_t lastNonSpace = valueUtf8.find_last_not_of(" \t");

    if (firstNonSpace != std::string::npos && lastNonSpace != std::string::npos)
        valueUtf8 = valueUtf8.substr(firstNonSpace, lastNonSpace - firstNonSpace + 1);
    else if (firstNonSpace == std::string::npos)
        return defaultValue;

    // Convert UTF-8 to wide string
    int wideLen = MultiByteToWideChar(CP_UTF8, 0, valueUtf8.c_str(), -1, NULL, 0);
    if (wideLen <= 0)
        return defaultValue;

    std::vector<WCHAR> wideBuffer(wideLen);
    MultiByteToWideChar(CP_UTF8, 0, valueUtf8.c_str(), -1, wideBuffer.data(), wideLen);

    return std::wstring(wideBuffer.data());
}

// Helper function to load localized strings from Language.ini
std::wstring LoadLocalizedString(const std::wstring& key, const std::wstring& defaultValue)
{
    WCHAR modulePath[MAX_PATH];
    if (!GetModuleFileNameW(g_hModule, modulePath, ARRAYSIZE(modulePath)))
        return defaultValue;

    PathCchRemoveFileSpec(modulePath, ARRAYSIZE(modulePath));

    std::wstring iniPath = modulePath;
    iniPath += L"\\AppParts\\Language.ini";

    // First, try to get user's language preference from their INI file
    WCHAR langBuffer[10] = L"en";
    WCHAR userIniPath[MAX_PATH] = { 0 };

    // Expand %LOCALAPPDATA%\RightClickTools\RightClickTools.ini
    DWORD envResult = ExpandEnvironmentStringsW(L"%LOCALAPPDATA%\\RightClickTools\\RightClickTools.ini", userIniPath, ARRAYSIZE(userIniPath));

    if (envResult > 0 && PathFileExistsW(userIniPath))
    {
        // User has a configuration file, try to read Lang setting
        WCHAR userLang[10] = { 0 };
        GetPrivateProfileStringW(L"General", L"Lang", L"", userLang, ARRAYSIZE(userLang), userIniPath);

        // If user specified a language, use it
        if (wcslen(userLang) > 0)
        {
            wcsncpy_s(langBuffer, ARRAYSIZE(langBuffer), userLang, _TRUNCATE);
            CharLowerW(langBuffer);
        }
    }

    // If no user preference, fall back to system locale
    if (wcscmp(langBuffer, L"en") == 0)
    {
        WCHAR localeName[LOCALE_NAME_MAX_LENGTH] = { 0 };
        if (GetUserDefaultLocaleName(localeName, LOCALE_NAME_MAX_LENGTH) > 0)
        {
            // Extract first 2 characters (e.g., "fr-FR" -> "fr")
            if (wcslen(localeName) >= 2)
            {
                wcsncpy_s(langBuffer, ARRAYSIZE(langBuffer), localeName, 2);
                CharLowerW(langBuffer);
            }
        }
    }

    // Check if the language section exists
    std::wstring testResult = ReadIniValueUTF8(iniPath, langBuffer, L"sMain", L"");

    // If the section doesn't exist, fall back to English
    if (testResult.empty())
    {
        wcscpy_s(langBuffer, ARRAYSIZE(langBuffer), L"en");
    }

    return ReadIniValueUTF8(iniPath, langBuffer, key, defaultValue);
}

// Helper function to load menu labels from CmdLabels
std::vector<std::wstring> LoadMenuLabels()
{
    std::vector<std::wstring> labels;

    WCHAR modulePath[MAX_PATH];
    if (!GetModuleFileNameW(g_hModule, modulePath, ARRAYSIZE(modulePath)))
        return labels;

    PathCchRemoveFileSpec(modulePath, ARRAYSIZE(modulePath));

    std::wstring iniPath = modulePath;
    iniPath += L"\\AppParts\\Language.ini";

    // First, try to get user's language preference from their INI file
    WCHAR langBuffer[10] = L"en";
    WCHAR userIniPath[MAX_PATH] = { 0 };

    // Expand %LOCALAPPDATA%\RightClickTools\RightClickTools.ini
    DWORD envResult = ExpandEnvironmentStringsW(L"%LOCALAPPDATA%\\RightClickTools\\RightClickTools.ini", userIniPath, ARRAYSIZE(userIniPath));

    if (envResult > 0 && PathFileExistsW(userIniPath))
    {
        // User has a configuration file, try to read Lang setting
        WCHAR userLang[10] = { 0 };
        GetPrivateProfileStringW(L"General", L"Lang", L"", userLang, ARRAYSIZE(userLang), userIniPath);

        // If user specified a language, use it
        if (wcslen(userLang) > 0)
        {
            wcsncpy_s(langBuffer, ARRAYSIZE(langBuffer), userLang, _TRUNCATE);
            CharLowerW(langBuffer);
        }
    }

    // If no user preference, fall back to system locale
    if (wcscmp(langBuffer, L"en") == 0)
    {
        WCHAR localeName[LOCALE_NAME_MAX_LENGTH] = { 0 };
        if (GetUserDefaultLocaleName(localeName, LOCALE_NAME_MAX_LENGTH) > 0)
        {
            // Extract first 2 characters (e.g., "fr-FR" -> "fr")
            if (wcslen(localeName) >= 2)
            {
                wcsncpy_s(langBuffer, ARRAYSIZE(langBuffer), localeName, 2);
                CharLowerW(langBuffer);
            }
        }
    }

    // Check if the language section exists
    std::wstring testResult = ReadIniValueUTF8(iniPath, langBuffer, L"CmdLabels", L"");

    // If the section doesn't exist, fall back to English
    if (testResult.empty())
    {
        wcscpy_s(langBuffer, ARRAYSIZE(langBuffer), L"en");
    }

    std::wstring result = ReadIniValueUTF8(iniPath, langBuffer, L"CmdLabels",
        L"Cmd here|PowerShell here|PowerShell Core here|File Manager here|Search here|RegEdit|Clear History|Unblock files here|Take ownership and get access|Add or Remove folder in Path variable|Toggle display of hidden and system files|Refresh shell|Folder Options here|Restart Explorer|Settings|More Tools…");

    // Parse pipe-separated values
    std::wstring str(result);
    size_t start = 0;
    size_t end = str.find(L'|');

    while (end != std::wstring::npos)
    {
        labels.push_back(str.substr(start, end - start));
        start = end + 1;
        end = str.find(L'|', start);
    }
    labels.push_back(str.substr(start));

    return labels;
}

class SubCommand : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IExplorerCommand>
{
public:
    SubCommand(const std::wstring& title, const std::wstring& iconName, const std::wstring& command, ComPtr<IUnknown> site) :
        m_title(title), m_iconName(iconName), m_command(command), m_site(site) {
    }

    // IExplorerCommand methods
    IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
    {
        *name = nullptr;
        auto title = wil::make_cotaskmem_string_nothrow(m_title.c_str());
        RETURN_IF_NULL_ALLOC(title);
        *name = title.release();
        return S_OK;
    }

    IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* iconPath)
    {
        *iconPath = nullptr;
        WCHAR modulePath[MAX_PATH];
        if (GetModuleFileNameW(g_hModule, modulePath, ARRAYSIZE(modulePath)))
        {
            PathCchRemoveFileSpec(modulePath, ARRAYSIZE(modulePath));
            StringCchCatW(modulePath, ARRAYSIZE(modulePath), L"\\AppParts\\Icons\\");
            StringCchCatW(modulePath, ARRAYSIZE(modulePath), m_iconName.c_str());

            auto iconPathStr = wil::make_cotaskmem_string_nothrow(modulePath);
            if (iconPathStr)
            {
                *iconPath = iconPathStr.release();
            }
        }
        return *iconPath ? S_OK : E_FAIL;
    }

    IFACEMETHODIMP GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip)
    {
        *infoTip = nullptr;
        return E_NOTIMPL;
    }

    IFACEMETHODIMP GetCanonicalName(_Out_ GUID* guidCommandName)
    {
        *guidCommandName = GUID_NULL;
        return S_OK;
    }

    IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState)
    {
        *cmdState = ECS_ENABLED;
        return S_OK;
    }

    IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept
    {
        try
        {
            std::wstring selectedPath;
            bool hasValidSelection = false;

            if (selection)
            {
                DWORD count = 0;
                HRESULT hr = selection->GetCount(&count);

                if (SUCCEEDED(hr) && count > 0)
                {
                    ComPtr<IShellItem> item;
                    if (SUCCEEDED(selection->GetItemAt(0, &item)))
                    {
                        PWSTR filePath = nullptr;
                        if (SUCCEEDED(item->GetDisplayName(SIGDN_FILESYSPATH, &filePath)) && filePath)
                        {
                            selectedPath = filePath;
                            CoTaskMemFree(filePath);
                            hasValidSelection = true;
                        }
                    }
                }
            }

            if (!hasValidSelection)
            {
                // Workaround for right-clicking on directory background
                ComPtr<IServiceProvider> sp;
                if (SUCCEEDED(m_site->QueryInterface(sp.GetAddressOf())))
                {
                    ComPtr<IShellBrowser> browser;
                    if (SUCCEEDED(sp->QueryService(SID_STopLevelBrowser, browser.GetAddressOf())))
                    {
                        ComPtr<IShellView> view;
                        if (SUCCEEDED(browser->QueryActiveShellView(&view)))
                        {
                            ComPtr<IFolderView> fview;
                            if (SUCCEEDED(view->QueryInterface(fview.GetAddressOf())))
                            {
                                ComPtr<IShellItem> folder;
                                if (SUCCEEDED(fview->GetFolder(IID_PPV_ARGS(folder.GetAddressOf()))))
                                {
                                    PWSTR path = nullptr;
                                    if (SUCCEEDED(folder->GetDisplayName(SIGDN_FILESYSPATH, &path)) && path)
                                    {
                                        selectedPath = path;
                                        CoTaskMemFree(path);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Use Documents folder as fallback for virtual folders
            if (selectedPath.empty())
            {
                PWSTR documentsPath = nullptr;
                if (SUCCEEDED(SHGetKnownFolderPath(FOLDERID_Documents, 0, NULL, &documentsPath)) && documentsPath)
                {
                    selectedPath = documentsPath;
                    CoTaskMemFree(documentsPath);
                }
            }

            // Get RightClickTools.exe path from registry
            std::wstring exePath = GetRightClickToolsExePath();

            // Check if exe exists
            if (!PathFileExistsW(exePath.c_str()))
            {
                // Load localized error message
                std::wstring errorMsg = LoadLocalizedString(L"sExeMissing", 
                    L"RightClickTools.exe missing. Please reinstall.");
                std::wstring title = LoadLocalizedString(L"sMain", L"Right-Click Tools");

                // Show simple error message
                MessageBoxW(NULL, errorMsg.c_str(), title.c_str(), MB_OK | MB_ICONERROR | MB_SYSTEMMODAL);

                return S_OK;
            }

            // Launch directly without protocol - ensures we use the correct exe
            // Append | before closing quote to prevent trailing backslash escaping (e.g., "C:\" becomes "C:\|")
            // For virtual folders, selectedPath may be Documents folder as fallback
            std::wstring cmdLine = L"\"" + exePath + L"\" /" + m_command + L" \"" + selectedPath + L"|\"";

            STARTUPINFOW si = { sizeof(si) };
            PROCESS_INFORMATION pi = { 0 };

            // CreateProcessW requires a writable buffer for the command line
            std::vector<wchar_t> cmdLineBuffer(cmdLine.begin(), cmdLine.end());
            cmdLineBuffer.push_back(L'\0');

            if (CreateProcessW(NULL, cmdLineBuffer.data(), NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi))
            {
                CloseHandle(pi.hThread);
                CloseHandle(pi.hProcess);
            }

            return S_OK;
        }
        catch (...)
        {
            return E_FAIL;
        }
    }

    IFACEMETHODIMP GetFlags(_Out_ EXPCMDFLAGS* flags)
    {
        *flags = ECF_DEFAULT;
        return S_OK;
    }

    IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands)
    {
        *enumCommands = nullptr;
        return E_NOTIMPL;
    }

private:
    std::wstring m_title;
    std::wstring m_iconName;
    std::wstring m_command;
    ComPtr<IUnknown> m_site;
};

class SubCommandEnum : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IEnumExplorerCommand>
{
public:
    SubCommandEnum(ComPtr<IUnknown> site) : m_site(site)
    {
        // Load localized labels from Language.ini
        std::vector<std::wstring> labels = LoadMenuLabels();

        // Menu item configuration: {CmdKey, IconFileName}
        struct MenuItem {
            const wchar_t* cmdKey;
            const wchar_t* iconFile;
        };

        MenuItem menuItems[] = {
            { L"CmdHere", L"CmdHere.ico" },
            { L"PowerShellHere", L"PowerShellHere.ico" },
            { L"PowerShellCoreHere", L"PowerShellCoreHere.ico" },
            { L"FileManagerHere", L"FileManagerHere.ico" },
            { L"SearchHere", L"SearchHere.ico" },
            { L"RegEdit", L"RegEdit.ico" },
            { L"ClearHistory", L"ClearHistory.ico" },
            { L"UnblockHere", L"UnblockHere.ico" },
            { L"TakeOwnHere", L"TakeOwnHere.ico" },
            { L"AddDelPathHere", L"AddDelPathHere.ico" },
            { L"ShowHide", L"ShowHide.ico" },
            { L"RefreshShellHere", L"RefreshShellHere.ico" },
            { L"FolderOptionsHere", L"FolderOptionsHere.ico" },
            { L"RestartExplorerHere", L"RestartExplorerHere.ico" },
            { L"Settings", L"Settings.ico" },
            { L"MoreToolsHere", L"MoreToolsHere.ico" }
        };

        // Create menu items from loaded labels
        for (size_t i = 0; i < ARRAYSIZE(menuItems); i++)
        {
            if (i < labels.size())
            {
                m_commands.push_back(Make<SubCommand>(
                    labels[i],
                    menuItems[i].iconFile,
                    menuItems[i].cmdKey,
                    m_site
                ));
            }
        }
    }

    // IEnumExplorerCommand methods
    IFACEMETHODIMP Next(ULONG celt, IExplorerCommand** pUICommand, ULONG* pceltFetched)
    {
        ULONG fetched = 0;
        while (fetched < celt && m_index < m_commands.size())
        {
            pUICommand[fetched] = m_commands[m_index].Get();
            pUICommand[fetched]->AddRef();
            ++fetched;
            ++m_index;
        }

        if (pceltFetched)
        {
            *pceltFetched = fetched;
        }

        return (fetched == celt) ? S_OK : S_FALSE;
    }

    IFACEMETHODIMP Skip(ULONG celt)
    {
        m_index += celt;
        return (m_index < m_commands.size()) ? S_OK : S_FALSE;
    }

    IFACEMETHODIMP Reset()
    {
        m_index = 0;
        return S_OK;
    }

    IFACEMETHODIMP Clone(IEnumExplorerCommand** ppenum)
    {
        auto clone = Make<SubCommandEnum>(m_site);
        clone->m_index = m_index;
        *ppenum = clone.Detach();
        return S_OK;
    }

private:
    std::vector<ComPtr<IExplorerCommand>> m_commands;
    size_t m_index = 0;
    ComPtr<IUnknown> m_site;
};

class RightClickToolsMenu : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IExplorerCommand, IObjectWithSite>
{
public:
    // IExplorerCommand methods
    IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
    {
        *name = nullptr;

        // Hard-coded title to avoid file I/O
        std::wstring title = L"Right-Click Tools";

        auto titleStr = wil::make_cotaskmem_string_nothrow(title.c_str());
        RETURN_IF_NULL_ALLOC(titleStr);
        *name = titleStr.release();
        return S_OK;
    }

    IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* iconPath)
    {
        *iconPath = nullptr;
        WCHAR modulePath[MAX_PATH];
        if (GetModuleFileNameW(g_hModule, modulePath, ARRAYSIZE(modulePath)))
        {
            PathCchRemoveFileSpec(modulePath, ARRAYSIZE(modulePath));
            StringCchCatW(modulePath, ARRAYSIZE(modulePath), L"\\AppParts\\Icons\\RightClickTools.ico");

            auto iconPathStr = wil::make_cotaskmem_string_nothrow(modulePath);
            if (iconPathStr)
            {
                *iconPath = iconPathStr.release();
            }
        }
        return *iconPath ? S_OK : E_FAIL;
    }

    IFACEMETHODIMP GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip)
    {
        *infoTip = nullptr;
        return E_NOTIMPL;
    }

    IFACEMETHODIMP GetCanonicalName(_Out_ GUID* guidCommandName)
    {
        *guidCommandName = GUID_NULL;
        return S_OK;
    }

    IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState)
    {
        *cmdState = ECS_ENABLED;
        return S_OK;
    }

    IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept
    {
        // Do nothing, as the main menu item should only display the submenu
        return S_OK;
    }

    IFACEMETHODIMP GetFlags(_Out_ EXPCMDFLAGS* flags)
    {
        *flags = ECF_HASSUBCOMMANDS;
        return S_OK;
    }

    IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands)
    {
        *enumCommands = Make<SubCommandEnum>(m_site).Detach();
        return S_OK;
    }

    // IObjectWithSite methods
    IFACEMETHODIMP SetSite(_In_ IUnknown* site) noexcept
    {
        m_site = site;
        return S_OK;
    }

    IFACEMETHODIMP GetSite(_In_ REFIID riid, _COM_Outptr_ void** site) noexcept
    {
        return m_site.CopyTo(riid, site);
    }

protected:
    ComPtr<IUnknown> m_site;
};

// Use a GUID that you generate yourself - this is just an example
// Generate your own with guidgen.exe or online GUID generator
class __declspec(uuid("AB57CE13-DACB-4129-952B-8D5209135772")) RightClickToolsMenuHandler final : public RightClickToolsMenu
{
};

CoCreatableClass(RightClickToolsMenuHandler)

STDAPI DllGetActivationFactory(_In_ HSTRING activatableClassId, _COM_Outptr_ IActivationFactory** factory)
{
    return Module<ModuleType::InProc>::GetModule().GetActivationFactory(activatableClassId, factory);
}

_Use_decl_annotations_
STDAPI DllCanUnloadNow()
{
    return Module<InProc>::GetModule().GetObjectCount() == 0 ? S_OK : S_FALSE;
}

_Use_decl_annotations_
STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, void** instance)
{
    return Module<InProc>::GetModule().GetClassObject(rclsid, riid, instance);
}

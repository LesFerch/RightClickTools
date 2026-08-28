#define MyAppName "RightClickTools"
#define MyAppVersion "2.0.0"
#define MyAppPublisher "LesFerch"
#define MyAppURL "https://lesferch.github.io/RightClickTools/"
#define MyAppExeName "RightClickTools.exe"
#define MyEmail "lesferch@gmail.com"

[Setup]

SignedUninstaller=yes
SignTool=Certum

AppId={{74705F78-A938-4880-903A-03EBC449DFE7}

AppName={#MyAppName}
AppVerName={#MyAppName}
VersionInfoVersion={#MyAppVersion}
AppVersion={#MyAppVersion}

AppPublisher={#MyAppPublisher}
AppCopyright={#MyEmail}
AppSupportURL={#MyAppURL}

DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename={#MyAppName}-Setup

Compression=lzma
SolidCompression=yes
WizardStyle=modern dynamic

ArchitecturesInstallIn64BitMode=x64compatible

SetupIconFile={#MyAppName}\AppParts\Icons\{#MyAppName}.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

LanguageDetectionMethod=uilanguage
ShowLanguageDialog=auto

; ---------------------------------------------------
; LANGUAGES
; ---------------------------------------------------
[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "cs"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "da"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"
Name: "el"; MessagesFile: "C:\InnoSetupProjects\ExtraInnoLanguages\Greek.isl"
Name: "es"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "fi"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "fr"; MessagesFile: "compiler:Languages\French.isl"
Name: "hr"; MessagesFile: "C:\InnoSetupProjects\ExtraInnoLanguages\Croatian.isl"
Name: "hu"; MessagesFile: "compiler:Languages\Hungarian.isl"
Name: "it"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "ja"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "ko"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "lt"; MessagesFile: "C:\InnoSetupProjects\ExtraInnoLanguages\Lithuanian.isl"
Name: "nb"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "nl"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "pl"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "pt"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "ru"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "sk"; MessagesFile: "compiler:Languages\Slovak.isl"
Name: "sl"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "sv"; MessagesFile: "compiler:Languages\Swedish.isl"
Name: "th"; MessagesFile: "compiler:Languages\Thai.isl"
Name: "tr"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "uk"; MessagesFile: "compiler:Languages\Ukrainian.isl"
Name: "vi"; MessagesFile: "C:\InnoSetupProjects\ExtraInnoLanguages\Vietnamese.isl"
Name: "zh"; MessagesFile: "C:\InnoSetupProjects\ExtraInnoLanguages\ChineseSimplified.isl"

; ---------------------------------------------------
; CUSTOM MESSAGES
; ---------------------------------------------------
[CustomMessages]
; English
en.InstallOptions=Installation options:
en.ClassicOnly=Context menu via registry (classic only)
en.ModernClassic=Context menu handler (modern + classic)
en.EnableElevation=Enable privilege elevation task
en.ElevationNote=The privilege elevation task is required to eliminate UAC prompts within Right-Click Tools.%nUncheck for work computers to avoid any potential security risks.
en.InstallingContextHandler=Installing context menu handler... This may take a few minutes. Please wait.
en.RemovingContextHandler=Removing context menu handler... This may take a few minutes. Please wait.

; German
de.InstallOptions=Installationsoptionen:
de.ClassicOnly=Kontextmenü über Registry (nur klassisch)
de.ModernClassic=Kontextmenü-Handler (modern + klassisch)
de.EnableElevation=Privilegierungsaufgabe aktivieren
de.ElevationNote=Die Privilegierungsaufgabe ist erforderlich, um UAC-Eingabeaufforderungen in Right-Click Tools zu vermeiden.%nDeaktivieren Sie diese Option für Arbeitscomputer, um potenzielle Sicherheitsrisiken zu vermeiden.
de.InstallingContextHandler=Kontextmenü-Handler wird installiert... Dies kann einige Minuten dauern. Bitte warten.
de.RemovingContextHandler=Kontextmenü-Handler wird entfernt... Dies kann einige Minuten dauern. Bitte warten.

; Czech
cs.InstallOptions=Možnosti instalace:
cs.ClassicOnly=Kontextová nabídka přes registr (pouze klasická)
cs.ModernClassic=Obsluha kontextové nabídky (moderní + klasická)
cs.EnableElevation=Povolit úlohu zvýšení oprávnění
cs.ElevationNote=Úloha zvýšení oprávnění je vyžadována k odstranění výzev UAC v Right-Click Tools.%nZrušte zaškrtnutí pro pracovní počítače, abyste se vyhnuli potenciálním bezpečnostním rizikům.
cs.InstallingContextHandler=Instalace obsluhy kontextové nabídky... To může trvat několik minut. Prosím čekejte.
cs.RemovingContextHandler=Odstraňování obsluhy kontextové nabídky... To může trvat několik minut. Prosím čekejte.

; Danish
da.InstallOptions=Installationsindstillinger:
da.ClassicOnly=Kontekstmenu via register (kun klassisk)
da.ModernClassic=Kontekstmenu-handler (moderne + klassisk)
da.EnableElevation=Aktivér opgave til privilegieforøgelse
da.ElevationNote=Opgaven til privilegieforøgelse er påkrævet for at eliminere UAC-prompter i Right-Click Tools.%nFjern markeringen for arbejdscomputere for at undgå potentielle sikkerhedsrisici.
da.InstallingContextHandler=Installerer kontekstmenu-handler... Dette kan tage flere minutter. Vent venligst.
da.RemovingContextHandler=Fjerner kontekstmenu-handler... Dette kan tage flere minutter. Vent venligst.

; Greek
el.InstallOptions=Επιλογές εγκατάστασης:
el.ClassicOnly=Μενού περιβάλλοντος μέσω μητρώου (μόνο κλασικό)
el.ModernClassic=Χειριστής μενού περιβάλλοντος (σύγχρονο + κλασικό)
el.EnableElevation=Ενεργοποίηση εργασίας αύξησης προνομίων
el.ElevationNote=Η εργασία αύξησης προνομίων απαιτείται για την εξάλειψη των προτροπών UAC στο Right-Click Tools.%nΑποεπιλέξτε για υπολογιστές εργασίας για να αποφύγετε πιθανούς κινδύνους ασφαλείας.
el.InstallingContextHandler=Εγκατάσταση χειριστή μενού περιβάλλοντος... Αυτό μπορεί να διαρκέσει μερικά λεπτά. Παρακαλώ περιμένετε.
el.RemovingContextHandler=Κατάργηση χειριστή μενού περιβάλλοντος... Αυτό μπορεί να διαρκέσει μερικά λεπτά. Παρακαλώ περιμένετε.

; Spanish
es.InstallOptions=Opciones de instalación:
es.ClassicOnly=Menú contextual mediante registro (solo clásico)
es.ModernClassic=Controlador de menú contextual (moderno + clásico)
es.EnableElevation=Habilitar tarea de elevación de privilegios
es.ElevationNote=La tarea de elevación de privilegios es necesaria para eliminar las solicitudes de UAC en Right-Click Tools.%nDesmarque para equipos de trabajo para evitar posibles riesgos de seguridad.
es.InstallingContextHandler=Instalando controlador de menú contextual... Esto puede tardar varios minutos. Por favor espere.
es.RemovingContextHandler=Eliminando controlador de menú contextual... Esto puede tardar varios minutos. Por favor espere.

; Finnish
fi.InstallOptions=Asennusasetukset:
fi.ClassicOnly=Pikavalikko rekisterin kautta (vain klassinen)
fi.ModernClassic=Pikavalikon käsittelijä (moderni + klassinen)
fi.EnableElevation=Ota käyttöön oikeuksien korotustyö
fi.ElevationNote=Oikeuksien korotustyö vaaditaan UAC-kehoteiden poistamiseksi Right-Click Toolsissa.%nPoista valinta työkoneilta mahdollisten tietoturvariskien välttämiseksi.
fi.InstallingContextHandler=Asennetaan pikavalikon käsittelijää... Tämä voi kestää muutaman minuutin. Odota.
fi.RemovingContextHandler=Poistetaan pikavalikon käsittelijää... Tämä voi kestää muutaman minuutin. Odota.

; French
fr.InstallOptions=Options d'installation :
fr.ClassicOnly=Menu contextuel via le registre (classique uniquement)
fr.ModernClassic=Gestionnaire de menu contextuel (moderne + classique)
fr.EnableElevation=Activer la tâche d'élévation des privilèges
fr.ElevationNote=La tâche d'élévation des privilèges est nécessaire pour éliminer les invites UAC dans Right-Click Tools.%nDécochez pour les ordinateurs de travail afin d'éviter les risques de sécurité potentiels.
fr.InstallingContextHandler=Installation du gestionnaire de menu contextuel... Cela peut prendre quelques minutes. Veuillez patienter.
fr.RemovingContextHandler=Suppression du gestionnaire de menu contextuel... Cela peut prendre quelques minutes. Veuillez patienter.

; Croatian
hr.InstallOptions=Opcije instalacije:
hr.ClassicOnly=Kontekstni izbornik putem registra (samo klasični)
hr.ModernClassic=Rukovatelj kontekstnog izbornika (moderni + klasični)
hr.EnableElevation=Omogući zadatak povećanja privilegija
hr.ElevationNote=Zadatak povećanja privilegija potreban je za eliminaciju UAC upita u Right-Click Tools.%nPoništite oznaku za radna računala kako biste izbjegli moguće sigurnosne rizike.
hr.InstallingContextHandler=Instaliranje rukovatelja kontekstnog izbornika... Ovo može potrajati nekoliko minuta. Molimo pričekajte.
hr.RemovingContextHandler=Uklanjanje rukovatelja kontekstnog izbornika... Ovo može potrajati nekoliko minuta. Molimo pričekajte.

; Hungarian
hu.InstallOptions=Telepítési lehetőségek:
hu.ClassicOnly=Helyi menü a beállításjegyzéken keresztül (csak klasszikus)
hu.ModernClassic=Helyi menü kezelő (modern + klasszikus)
hu.EnableElevation=Jogosultság-emelési feladat engedélyezése
hu.ElevationNote=A jogosultság-emelési feladat szükséges az UAC-kérések kiküszöböléséhez a Right-Click Tools alkalmazásban.%nTörölje a jelölést a munkahelyi számítógépeken a potenciális biztonsági kockázatok elkerülése érdekében.
hu.InstallingContextHandler=Helyi menü kezelő telepítése... Ez több percig is tarthat. Kérjük, várjon.
hu.RemovingContextHandler=Helyi menü kezelő eltávolítása... Ez több percig is tarthat. Kérjük, várjon.

; Italian
it.InstallOptions=Opzioni di installazione:
it.ClassicOnly=Menu contestuale tramite registro (solo classico)
it.ModernClassic=Gestore menu contestuale (moderno + classico)
it.EnableElevation=Abilita attività di elevazione dei privilegi
it.ElevationNote=L'attività di elevazione dei privilegi è necessaria per eliminare i prompt UAC in Right-Click Tools.%nDeselezionare per i computer di lavoro per evitare potenziali rischi per la sicurezza.
it.InstallingContextHandler=Installazione gestore menu contestuale... Potrebbero essere necessari alcuni minuti. Attendere prego.
it.RemovingContextHandler=Rimozione gestore menu contestuale... Potrebbero essere necessari alcuni minuti. Attendere prego.

; Japanese
ja.InstallOptions=インストールオプション:
ja.ClassicOnly=レジストリ経由のコンテキストメニュー（クラシックのみ）
ja.ModernClassic=コンテキストメニューハンドラー（モダン + クラシック）
ja.EnableElevation=特権昇格タスクを有効にする
ja.ElevationNote=Right-Click Tools内のUACプロンプトを排除するには、特権昇格タスクが必要です。%n潜在的なセキュリティリスクを回避するため、業務用コンピューターではチェックを外してください。
ja.InstallingContextHandler=コンテキストメニューハンドラーをインストールしています... 数分かかる場合があります。お待ちください。
ja.RemovingContextHandler=コンテキストメニューハンドラーを削除しています... 数分かかる場合があります。お待ちください。

; Korean
ko.InstallOptions=설치 옵션:
ko.ClassicOnly=레지스트리를 통한 컨텍스트 메뉴(클래식만)
ko.ModernClassic=컨텍스트 메뉴 핸들러(모던 + 클래식)
ko.EnableElevation=권한 상승 작업 활성화
ko.ElevationNote=Right-Click Tools 내에서 UAC 프롬프트를 제거하려면 권한 상승 작업이 필요합니다.%n잠재적인 보안 위험을 방지하려면 업무용 컴퓨터에서는 선택을 취소하세요.
ko.InstallingContextHandler=컨텍스트 메뉴 핸들러 설치 중... 몇 분 정도 걸릴 수 있습니다. 기다려 주세요.
ko.RemovingContextHandler=컨텍스트 메뉴 핸들러 제거 중... 몇 분 정도 걸릴 수 있습니다. 기다려 주세요.

; Dutch
nl.InstallOptions=Installatieopties:
nl.ClassicOnly=Contextmenu via register (alleen klassiek)
nl.ModernClassic=Contextmenu-handler (modern + klassiek)
nl.EnableElevation=Taak voor rechtenuitbreiding inschakelen
nl.ElevationNote=De taak voor rechtenuitbreiding is vereist om UAC-prompts in Right-Click Tools te elimineren.%nSchakel uit voor werkcomputers om potentiële beveiligingsrisico's te vermijden.
nl.InstallingContextHandler=Contextmenu-handler installeren... Dit kan enkele minuten duren. Even geduld.
nl.RemovingContextHandler=Contextmenu-handler verwijderen... Dit kan enkele minuten duren. Even geduld.

; Norwegian
nb.InstallOptions=Installasjonsalternativer:
nb.ClassicOnly=Kontekstmeny via register (kun klassisk)
nb.ModernClassic=Kontekstmeny-behandler (moderne + klassisk)
nb.EnableElevation=Aktiver oppgave for rettighetsøkning
nb.ElevationNote=Oppgaven for rettighetsøkning er nødvendig for å eliminere UAC-forespørsler i Right-Click Tools.%nFjern avmerkingen for arbeidsdatamaskiner for å unngå potensielle sikkerhetsrisikoer.
nb.InstallingContextHandler=Installerer kontekstmeny-behandler... Dette kan ta noen minutter. Vennligst vent.
nb.RemovingContextHandler=Fjerner kontekstmeny-behandler... Dette kan ta noen minutter. Vennligst vent.

; Polish
pl.InstallOptions=Opcje instalacji:
pl.ClassicOnly=Menu kontekstowe przez rejestr (tylko klasyczne)
pl.ModernClassic=Obsługa menu kontekstowego (nowoczesne + klasyczne)
pl.EnableElevation=Włącz zadanie podnoszenia uprawnień
pl.ElevationNote=Zadanie podnoszenia uprawnień jest wymagane, aby wyeliminować monity UAC w Right-Click Tools.%nOdznacz dla komputerów służbowych, aby uniknąć potencjalnych zagrożeń bezpieczeństwa.
pl.InstallingContextHandler=Instalowanie obsługi menu kontekstowego... Może to potrwać kilka minut. Proszę czekać.
pl.RemovingContextHandler=Usuwanie obsługi menu kontekstowego... Może to potrwać kilka minut. Proszę czekać.

; Portuguese
pt.InstallOptions=Opções de instalação:
pt.ClassicOnly=Menu de contexto via registro (apenas clássico)
pt.ModernClassic=Manipulador de menu de contexto (moderno + clássico)
pt.EnableElevation=Ativar tarefa de elevação de privilégios
pt.ElevationNote=A tarefa de elevação de privilégios é necessária para eliminar os prompts UAC no Right-Click Tools.%nDesmarque para computadores de trabalho para evitar possíveis riscos de segurança.
pt.InstallingContextHandler=Instalando manipulador de menu de contexto... Isso pode levar alguns minutos. Por favor, aguarde.
pt.RemovingContextHandler=Removendo manipulador de menu de contexto... Isso pode levar alguns minutos. Por favor, aguarde.

; Russian
ru.InstallOptions=Параметры установки:
ru.ClassicOnly=Контекстное меню через реестр (только классическое)
ru.ModernClassic=Обработчик контекстного меню (современное + классическое)
ru.EnableElevation=Включить задачу повышения привилегий
ru.ElevationNote=Задача повышения привилегий необходима для устранения запросов UAC в Right-Click Tools.%nСнимите флажок для рабочих компьютеров, чтобы избежать потенциальных рисков безопасности.
ru.InstallingContextHandler=Установка обработчика контекстного меню... Это может занять несколько минут. Пожалуйста, подождите.
ru.RemovingContextHandler=Удаление обработчика контекстного меню... Это может занять несколько минут. Пожалуйста, подождите.

; Slovak
sk.InstallOptions=Možnosti inštalácie:
sk.ClassicOnly=Kontextová ponuka cez register (len klasická)
sk.ModernClassic=Obsluha kontextovej ponuky (moderná + klasická)
sk.EnableElevation=Povoliť úlohu zvýšenia oprávnení
sk.ElevationNote=Úloha zvýšenia oprávnení je potrebná na odstránenie výziev UAC v Right-Click Tools.%nZrušte začiarknutie pre pracovné počítače, aby ste sa vyhli potenciálnym bezpečnostným rizikám.
sk.InstallingContextHandler=Inštalácia obsluhy kontextovej ponuky... Môže to trvať niekoľko minút. Prosím čakajte.
sk.RemovingContextHandler=Odstraňovanie obsluhy kontextovej ponuky... Môže to trvať niekoľko minút. Prosím čakajte.

; Slovenian
sl.InstallOptions=Možnosti namestitve:
sl.ClassicOnly=Kontekstni meni prek registra (samo klasični)
sl.ModernClassic=Upravitelj kontekstnega menija (sodobni + klasični)
sl.EnableElevation=Omogoči nalogo povečanja privilegijev
sl.ElevationNote=Naloga povečanja privilegijev je potrebna za odpravo pozivov UAC v Right-Click Tools.%nPočistite za delovne računalnike, da se izognete morebitnim varnostnim tveganjem.
sl.InstallingContextHandler=Nameščanje upravitelja kontekstnega menija... To lahko traja nekaj minut. Prosimo počakajte.
sl.RemovingContextHandler=Odstranjevanje upravitelja kontekstnega menija... To lahko traja nekaj minut. Prosimo počakajte.

; Swedish
sv.InstallOptions=Installationsalternativ:
sv.ClassicOnly=Snabbmeny via registret (endast klassisk)
sv.ModernClassic=Snabbmenyhanterare (modern + klassisk)
sv.EnableElevation=Aktivera uppgift för behörighetshöjning
sv.ElevationNote=Uppgiften för behörighetshöjning krävs för att eliminera UAC-uppmaningar i Right-Click Tools.%nAvmarkera för arbetsdatorer för att undvika potentiella säkerhetsrisker.
sv.InstallingContextHandler=Installerar snabbmenyhanterare... Detta kan ta några minuter. Vänligen vänta.
sv.RemovingContextHandler=Tar bort snabbmenyhanterare... Detta kan ta några minuter. Vänligen vänta.

; Turkish
tr.InstallOptions=Kurulum seçenekleri:
tr.ClassicOnly=Kayıt defteri üzerinden bağlam menüsü (yalnızca klasik)
tr.ModernClassic=Bağlam menüsü işleyicisi (modern + klasik)
tr.EnableElevation=Ayrıcalık yükseltme görevini etkinleştir
tr.ElevationNote=Right-Click Tools'ta UAC istemlerini ortadan kaldırmak için ayrıcalık yükseltme görevi gereklidir.%nOlası güvenlik risklerini önlemek için iş bilgisayarlarında işareti kaldırın.
tr.InstallingContextHandler=Bağlam menüsü işleyicisi yükleniyor... Bu birkaç dakika sürebilir. Lütfen bekleyin.
tr.RemovingContextHandler=Bağlam menüsü işleyicisi kaldırılıyor... Bu birkaç dakika sürebilir. Lütfen bekleyin.

; Ukrainian
uk.InstallOptions=Параметри встановлення:
uk.ClassicOnly=Контекстне меню через реєстр (тільки класичне)
uk.ModernClassic=Обробник контекстного меню (сучасне + класичне)
uk.EnableElevation=Увімкнути завдання підвищення привілеїв
uk.ElevationNote=Завдання підвищення привілеїв необхідне для усунення запитів UAC у Right-Click Tools.%nЗніміть прапорець для робочих комп'ютерів, щоб уникнути потенційних ризиків безпеки.
uk.InstallingContextHandler=Встановлення обробника контекстного меню... Це може зайняти кілька хвилин. Будь ласка, зачекайте.
uk.RemovingContextHandler=Видалення обробника контекстного меню... Це може зайняти кілька хвилин. Будь ласка, зачекайте.

; Lithuanian
lt.InstallOptions=Diegimo parinktys:
lt.ClassicOnly=Kontekstinis meniu per registrą (tik klasikinis)
lt.ModernClassic=Kontekstinio meniu tvarkyklė (modernus + klasikinis)
lt.EnableElevation=Įgalinti privilegijų padidinimo užduotį
lt.ElevationNote=Privilegijų padidinimo užduotis reikalinga UAC raginimams pašalinti Right-Click Tools.%nPanaikinkite žymėjimą darbo kompiuteriams, kad išvengtumėte galimų saugumo rizikų.
lt.InstallingContextHandler=Diegiama kontekstinio meniu tvarkyklė... Tai gali užtrukti kelias minutes. Prašome palaukti.
lt.RemovingContextHandler=Šalinama kontekstinio meniu tvarkyklė... Tai gali užtrukti kelias minutes. Prašome palaukti.

; Thai
th.InstallOptions=ตัวเลือกการติดตั้ง:
th.ClassicOnly=เมนูบริบทผ่านรีจิสทรี (คลาสสิกเท่านั้น)
th.ModernClassic=ตัวจัดการเมนูบริบท (ทันสมัย + คลาสสิก)
th.EnableElevation=เปิดใช้งานการยกระดับสิทธิ์
th.ElevationNote=จำเป็นต้องมีงานยกระดับสิทธิ์เพื่อขจัดข้อความแจ้ง UAC ใน Right-Click Tools%nยกเลิกการเลือกสำหรับคอมพิวเตอร์ที่ทำงานเพื่อหลีกเลี่ยงความเสี่ยงด้านความปลอดภัยที่อาจเกิดขึ้น
th.InstallingContextHandler=กำลังติดตั้งตัวจัดการเมนูบริบท... อาจใช้เวลาสักครู่ กรุณารอ
th.RemovingContextHandler=กำลังลบตัวจัดการเมนูบริบท... อาจใช้เวลาสักครู่ กรุณารอ

; Vietnamese
vi.InstallOptions=Tùy chọn cài đặt:
vi.ClassicOnly=Menu ngữ cảnh qua registry (chỉ cổ điển)
vi.ModernClassic=Trình xử lý menu ngữ cảnh (hiện đại + cổ điển)
vi.EnableElevation=Bật tác vụ nâng cao đặc quyền
vi.ElevationNote=Tác vụ nâng cao đặc quyền là cần thiết để loại bỏ lời nhắc UAC trong Right-Click Tools.%nBỏ chọn cho máy tính công việc để tránh các rủi ro bảo mật tiềm ẩn.
vi.InstallingContextHandler=Đang cài đặt trình xử lý menu ngữ cảnh... Quá trình này có thể mất vài phút. Vui lòng đợi.
vi.RemovingContextHandler=Đang gỡ bỏ trình xử lý menu ngữ cảnh... Quá trình này có thể mất vài phút. Vui lòng đợi.

; Chinese (Simplified)
zh.InstallOptions=安装选项：
zh.ClassicOnly=通过注册表的上下文菜单（仅经典）
zh.ModernClassic=上下文菜单处理程序（现代 + 经典）
zh.EnableElevation=启用权限提升任务
zh.ElevationNote=需要权限提升任务来消除 Right-Click Tools 中的 UAC 提示。%n对于工作计算机，请取消选中以避免潜在的安全风险。
zh.InstallingContextHandler=正在安装上下文菜单处理程序... 这可能需要几分钟时间。请稍候。
zh.RemovingContextHandler=正在删除上下文菜单处理程序... 这可能需要几分钟时间。请稍候。

; ---------------------------------------------------
; FILES
; ---------------------------------------------------
[Files]

Source: "RightClickTools\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion; Excludes: "*.msix,InstallMsix.ps1"

Source: "RightClickTools\RightClickTools.msix"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "RightClickTools\InstallMsix.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "RightClickTools\UninstallMsix.ps1"; DestDir: "{app}"

; ---------------------------------------------------
; APP PATHS
; ---------------------------------------------------
[Registry]

Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\App Paths\{#MyAppExeName}"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName}"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\App Paths\{#MyAppExeName}"; ValueType: string; ValueName: "Path"; ValueData: "{app}"

; ---------------------------------------------------
; INSTALL EXECUTION
; ---------------------------------------------------
[Run]

Filename: "{app}\{#MyAppExeName}"; Parameters: "/HKUremove"; Check: IsModern
Filename: "{app}\{#MyAppExeName}"; Parameters: "/HKUinstall /Lang={language}"; Check: IsClassic and PrivTask
Filename: "{app}\{#MyAppExeName}"; Parameters: "/HKUinstallMin /Lang={language}"; Check: IsClassic and not PrivTask
Filename: "{app}\{#MyAppExeName}"; Parameters: "/HKUtaskonly /Lang={language}"; Check: IsModern and PrivTask

Filename: "powershell.exe"; Parameters: "-NoProfile -ExecutionPolicy Bypass -File ""{app}\UninstallMsix.ps1"""; StatusMsg: "{cm:RemovingContextHandler}"; Flags: runhidden waituntilterminated; Check: IsClassic and IsWindows11
Filename: "powershell.exe"; Parameters: "-NoProfile -ExecutionPolicy Bypass -File ""{tmp}\InstallMsix.ps1"""; StatusMsg: "{cm:InstallingContextHandler}"; Flags: runhidden waituntilterminated; Check: IsModern

; ---------------------------------------------------
; UNINSTALL
; ---------------------------------------------------
[UninstallRun]

Filename: "powershell.exe"; Parameters: "-NoProfile -ExecutionPolicy Bypass -File ""{app}\UninstallMsix.ps1"""; StatusMsg: "{cm:RemovingContextHandler}"; Flags: runhidden waituntilterminated; Check: IsWindows11
Filename: "{app}\{#MyAppExeName}"; Parameters: "/HKUremove"; RunOnceId: "Cleanup"

; ---------------------------------------------------
; CUSTOM UI
; ---------------------------------------------------
[Code]

var
  OptionsPage: TWizardPage;
  ClassicRadio: TRadioButton;
  ModernRadio: TRadioButton;
  ElevateCheck: TCheckBox;
  IsWin11: Boolean;
  NoteLabel: TLabel;

function IsWindows11OrHigher: Boolean;
var
  Version: TWindowsVersion;
begin
  GetWindowsVersionEx(Version);
  Result := (Version.Major = 10) and (Version.Build >= 22000);
end;

const
  HWND_TOPMOST = -1;
  HWND_NOTOPMOST = -2;
  SWP_NOSIZE = $0001;
  SWP_NOMOVE = $0002;
  SWP_SHOWWINDOW = $0040;

function SetWindowPos(hWnd: HWND; hWndInsertAfter: HWND; X: Integer; Y: Integer; cx: Integer; cy: Integer; uFlags: UINT): BOOL;
  external 'SetWindowPos@user32.dll stdcall';

procedure InitializeWizard;
var
  TopOffset: Integer;
begin

  // Bring the installer window to the front, then release the topmost lock
  SetWindowPos(WizardForm.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE or SWP_NOMOVE or SWP_SHOWWINDOW);
  SetWindowPos(WizardForm.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOSIZE or SWP_NOMOVE);

  IsWin11 := IsWindows11OrHigher;

  OptionsPage := CreateCustomPage(
    wpWelcome,
    ExpandConstant('{cm:InstallOptions}'),
    ''
  );

  TopOffset := 10;

  if IsWin11 then
  begin
    // Modern
    ModernRadio := TRadioButton.Create(OptionsPage);
    ModernRadio.Parent := OptionsPage.Surface;
    ModernRadio.Caption := ExpandConstant('{cm:ModernClassic}');
    ModernRadio.Top := TopOffset;
    ModernRadio.Left := 0;
    ModernRadio.Width := OptionsPage.SurfaceWidth;
    ModernRadio.Checked := True;

    // Classic
    ClassicRadio := TRadioButton.Create(OptionsPage);
    ClassicRadio.Parent := OptionsPage.Surface;
    ClassicRadio.Caption := ExpandConstant('{cm:ClassicOnly}');
    ClassicRadio.Top := ModernRadio.Top + 35;
    ClassicRadio.Left := 0;
    ClassicRadio.Width := OptionsPage.SurfaceWidth;

    TopOffset := ClassicRadio.Top + 45;
  end;

  // Elevation checkbox (always shown)
  ElevateCheck := TCheckBox.Create(OptionsPage);
  ElevateCheck.Parent := OptionsPage.Surface;
  ElevateCheck.Caption := ExpandConstant('{cm:EnableElevation}');
  ElevateCheck.Top := TopOffset;
  ElevateCheck.Left := 0;
  ElevateCheck.Width := OptionsPage.SurfaceWidth;
  ElevateCheck.Checked := True;

  // Informational note under checkbox
  NoteLabel := TLabel.Create(OptionsPage);
  NoteLabel.Parent := OptionsPage.Surface;
  NoteLabel.Caption := #13#10 + ExpandConstant('{cm:ElevationNote}');
  NoteLabel.Top := ElevateCheck.Top + 25;
  NoteLabel.Left := 0;
  NoteLabel.Width := OptionsPage.SurfaceWidth;
  NoteLabel.WordWrap := True;
end;

function IsClassic: Boolean;
begin
  if not IsWin11 then
    Result := True
  else
    Result := ClassicRadio.Checked;
end;

function IsModern: Boolean;
begin
  if not IsWin11 then
    Result := False
  else
    Result := ModernRadio.Checked;
end;

function PrivTask: Boolean;
begin
  Result := ElevateCheck.Checked;
end;

function IsWindows11: Boolean;
begin
  Result := IsWindows11OrHigher;
end;

; Thanks to Trevis/SunnujDecalPlugins for the installer template!
; Define your application name
!define APPNAME "Warehouse"
!define NFNAME "WarehouseFilter"
!define SOFTWARECOMPANY "FartwhifDecalPlugins"
!define VERSION	"0.3.2"
!define APPGUID "{DA4BFDDC-C110-45CE-8D08-4BD45C34CC2C}"

!define ASSEMBLY "Warehouse.dll"
!define CLASSNAME "Warehouse.PluginCore"
!define NFCLASSNAME "Warehouse.WarehouseFilter"


!define BUILDPATH ".\bin\Release"

; Main Install settings
; compressor goes first
SetCompressor LZMA

Name "${APPNAME} ${VERSION}"
InstallDir "C:\Games\Decal Plugins\${APPNAME}"
InstallDirRegKey HKLM "Software\${SOFTWARECOMPANY}\${APPNAME}" ""
;SetFont "Verdana" 8
;Icon "Installer\Res\Decal.ico"
OutFile ".\..\bin\${APPNAME}Installer-${VERSION}.exe"

; Use compression

; Modern interface settings
!include "MUI.nsh"

!define MUI_ABORTWARNING

!insertmacro MUI_PAGE_WELCOME
;!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Set languages (first is default language)
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_RESERVEFILE_LANGDLL


Section "" CoreSection
; Set Section properties
	SetOverwrite on

	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\"

	File "${BUILDPATH}\${ASSEMBLY}"
	File "${BUILDPATH}\Warehouse.dll.config"
	File "${BUILDPATH}\LinqBridge.dll"
	File "${BUILDPATH}\System.Data.SQLite.dll"
	File "${BUILDPATH}\System.Data.SQLite.dll.config"
	File "${BUILDPATH}\warehouse.default.db"

	SetOutPath "$INSTDIR\x86"
	File "${BUILDPATH}\x86\SQLite.Interop.dll"
	
SectionEnd

Section -FinishSection

	WriteRegStr HKLM "Software\${SOFTWARECOMPANY}\${APPNAME}" "" "$INSTDIR"
	WriteRegStr HKLM "Software\${SOFTWARECOMPANY}\${APPNAME}" "Version" "${VERSION}"

	;Register in decal
	WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "" "${APPNAME}"
	WriteRegDWORD HKLM "Software\Decal\Plugins\${APPGUID}" "Enabled" "1"
	WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "Object" "${CLASSNAME}"
	WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "Assembly" "${ASSEMBLY}"
	WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "Path" "$INSTDIR"
	WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "Surrogate" "{71A69713-6593-47EC-0002-0000000DECA1}"
	WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "Uninstaller" "${APPNAME}"
	
	;Decal Network Filter
	WriteRegStr HKLM "Software\Decal\NetworkFilters\${APPGUID}" "" "${NFNAME}"
	WriteRegDWORD HKLM "Software\Decal\NetworkFilters\${APPGUID}" "Enabled" "1"
	WriteRegStr HKLM "Software\Decal\NetworkFilters\${APPGUID}" "Object" "${NFCLASSNAME}"
	WriteRegStr HKLM "Software\Decal\NetworkFilters\${APPGUID}" "Assembly" "${ASSEMBLY}"
	WriteRegStr HKLM "Software\Decal\NetworkFilters\${APPGUID}" "Path" "$INSTDIR"
	WriteRegStr HKLM "Software\Decal\NetworkFilters\${APPGUID}" "Surrogate" "{71A69713-6593-47EC-0002-0000000DECA1}"
	WriteRegStr HKLM "Software\Decal\NetworkFilters\${APPGUID}" "Uninstaller" "${APPNAME}"

	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$INSTDIR\uninstall.exe"
	WriteUninstaller "$INSTDIR\uninstall.exe"
	;MessageBox MB_OK "Done"

SectionEnd

; Modern install component descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${CoreSection} ""
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;Uninstall section
Section Uninstall

	;Remove from registry...
	DeleteRegKey HKLM "Software\${SOFTWARECOMPANY}\${APPNAME}"
	DeleteRegKey HKLM "Software\Decal\Plugins\${APPGUID}"
	DeleteRegKey HKLM "Software\Decal\NetworkFilters\${APPGUID}"
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"

	; Delete self
	Delete "$INSTDIR\uninstall.exe"

	;Clean up
	Delete "$INSTDIR\${ASSEMBLY}"
	Delete "$INSTDIR\Warehouse.dll.config"
	Delete "$INSTDIR\LinqBridge.dll"
	Delete "$INSTDIR\System.Data.SQLite.dll"
	Delete "$INSTDIR\System.Data.SQLite.dll.config"
	Delete "$INSTDIR\warehouse.default.db"
	Delete "$INSTDIR\x86\SQLite.Interop.dll"
	RMDir "$INSTDIR\x86"
	RMDir "$INSTDIR\"

SectionEnd

; eof

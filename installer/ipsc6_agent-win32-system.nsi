;--------------------------------
;Include Modern UI
!include MUI2.nsh

;--------------------------------
;General
Unicode True
Name "IPSC6 座席工具条"
OutFile "out\ipsc6_agent_wpfapp-win32-system.exe"

;Default installation folder
InstallDir "$PROGRAMFILES32\ipsc6-agent-wpfapp"

;Get installation folder from registry if available
InstallDirRegKey HKCU "Software\ipsc6-agent-wpfapp" ""

;Request application privileges
RequestExecutionLevel admin

;--------------------------------
;Variables
Var StartMenuFolder

;--------------------------------
;Interface Settings

;--------------------------------
;Installer pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder

;Uninstaller pages
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

;--------------------------------
;Languages

!insertmacro MUI_LANGUAGE "SimpChinese"

;--------------------------------
Section "" ;No components page, name is not important
  SetOutPath $INSTDIR

  ;MY OWN FILES HERE...
  File /r /x "*.exp" /x "*.lib" /x "*.pdb" "..\ipsc6.agent.wpfapp\bin\x86\Release\*.*"

  ;Store installation folder
  WriteRegStr HKLM "Software\ipsc6-agent-wpfapp" "" $INSTDIR
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application    
    ;Create shortcuts
    CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
    CreateShortcut "$SMPROGRAMS\$StartMenuFolder\座席工具条.lnk" "$INSTDIR\ipsc6.agent.wpfapp.exe"  
    CreateShortcut "$SMPROGRAMS\$StartMenuFolder\卸载.lnk" "$INSTDIR\Uninstall.exe"  
  !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

;--------------------------------
;Uninstaller Section
Section "Uninstall"

  ;MY OWN FILES HERE...

  Delete "$INSTDIR\Uninstall.exe"

  RMDir /r "$INSTDIR"

  !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
  RMDir /r "$SMPROGRAMS\$StartMenuFolder"

  DeleteRegKey /ifempty HKLM "Software\ipsc6-agent-wpfapp"

SectionEnd

Function .onInit
  SetShellVarContext all
FunctionEnd
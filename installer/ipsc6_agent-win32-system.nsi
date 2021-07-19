!include LogicLib.nsh
!include Sections.nsh
!include x64.nsh

;--------------------------------
;Include Modern UI
!include MUI2.nsh

;--------------------------------
;General
Unicode True
Name "IPSC6 座席工具条 (x86)"
OutFile "out\ipsc6_agent_wpfapp-win32-system.exe"
Icon "${NSISDIR}\Contrib\Graphics\Icons\nsis3-install.ico"
UninstallIcon "${NSISDIR}\Contrib\Graphics\Icons\nsis3-uninstall.ico"

;Default installation folder
InstallDir "$PROGRAMFILES32\ipsc6-agent-wpfapp"

;Get installation folder from registry if available
InstallDirRegKey HKLM "Software\ipsc6_agent_wpfapp-win32-system" ""

;Request application privileges
RequestExecutionLevel admin

;--------------------------------
;Variables
Var DisplayName

;--------------------------------
;Interface Settings
ShowInstDetails show
ShowUnInstDetails show
!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_UNFINISHPAGE_NOAUTOCLOSE

;--------------------------------
;Installer pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_STARTMENU Application $DisplayName
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

;Uninstaller pages
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_COMPONENTS
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

;--------------------------------
;Languages
!insertmacro MUI_LANGUAGE "SimpChinese"

;--------------------------------
Section "!座席工具条" SEC_0
  SetShellVarContext all
  SetOutPath $INSTDIR

  ;MY OWN FILES HERE...
  File /r /x "*.exp" /x "*.lib" /x "*.pdb" "..\ipsc6.agent.wpfapp\bin\x86\Release\*.*"

  ;Store installation folder
  WriteRegStr HKLM "Software\ipsc6_agent_wpfapp-win32-system" "" $INSTDIR

  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application    
    ;Create shortcuts
    CreateDirectory "$SMPROGRAMS\$DisplayName"
    CreateShortcut "$SMPROGRAMS\$DisplayName\启动 座席工具条.lnk" "$INSTDIR\ipsc6.agent.wpfapp.exe"  
    CreateShortcut "$SMPROGRAMS\$DisplayName\卸载 座席工具条.lnk" "$INSTDIR\Uninstall.exe"  
  !insertmacro MUI_STARTMENU_WRITE_END

  ; Add/Remove list
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-win32-system" \
                  "DisplayName" "$DisplayName"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-win32-system" \
                  "UninstallString" "$\"$INSTDIR\Uninstall.exe$\" /S"
SectionEnd

Section "URI Scheme Handler" SEC_HANDLER
  SetShellVarContext all

  ${If} ${RunningX64}
    SetOutPath $PROGRAMFILES64\ipsc6-agent-launch
  ${Else}
    SetOutPath $PROGRAMFILES\ipsc6-agent-launch
  ${EndIf}
  CreateDirectory $OUTDIR

  ;MY OWN FILES HERE...
  File /r /x "*.pdb" "..\ipsc6.agent.launch\bin\Release\*.*"

  ;URI scheme handler in Registy
  WriteRegStr HKCR "ipsc6-agent-launch" "" "$\"URL:ipsc6-agent-launch Protocol$\""
  WriteRegStr HKCR "ipsc6-agent-launch\shell\open\command" "" "$\"$OUTDIR\ipsc6.agent.launch.exe$\" $\"%1$\""
SectionEnd

;--------------------------------
;Uninstaller Section
Section "!un.座席工具条" UNSEC_0
  SetShellVarContext all

  Delete "$INSTDIR\Uninstall.exe"

  ;Remove whole install dir
  RMDir /r "$INSTDIR"

  ;Remove from Startup menus
  !insertmacro MUI_STARTMENU_GETFOLDER Application $DisplayName
  RMDir /r "$SMPROGRAMS\$DisplayName"

  ;Remove from Add/Remove list
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-win32-system"

  ;Remove InstalledRegKey only when it is empty
  DeleteRegKey /ifempty HKLM "Software\ipsc6_agent_wpfapp-win32-system"
SectionEnd

Section /o "un.URI Scheme Handler" UNSEC_HANDLER
  SetShellVarContext all

  ${If} ${RunningX64}
    SetOutPath $PROGRAMFILES64\ipsc6-agent-launch
  ${Else}
    SetOutPath $PROGRAMFILES\ipsc6-agent-launch
  ${EndIf}
  RMDir /r $OUTDIR

  DeleteRegKey HKCR "ipsc6-agent-launch" 
SectionEnd

; components/sections
;--------------------------------------------
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_0} \
    "运行座席工具条所必须的文件。"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_HANDLER} \
    "这是一个系统级的 URI Scheme Handler 程序。当有程序通过访问 ipsc6-agent: 开头的 URL 时，操作系统通过调用该程序启动座席工具条。"
!insertmacro MUI_FUNCTION_DESCRIPTION_END

!insertmacro MUI_UNFUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${UNSEC_0} \
    "删除座席工具条。"
  !insertmacro MUI_DESCRIPTION_TEXT ${UNSEC_HANDLER} \
    "这是一个系统级的的 URI Scheme Handler 程序。当有程序通过访问 ipsc6-agent: 开头的 URL 时，操作系统通过调用该程序启动座席工具条。"
!insertmacro MUI_UNFUNCTION_DESCRIPTION_END

Function .onInit
  ; https://nsis.sourceforge.io/Graying_out_Section_(define_mandatory_sections)
  # set the section as selected and read-only
  IntOp $0 ${SF_SELECTED} | ${SF_RO}
  SectionSetFlags ${SEC_0} $0
FunctionEnd

Function un.onInit
  ; https://nsis.sourceforge.io/Graying_out_Section_(define_mandatory_sections)
  # set the section as selected and read-only
  IntOp $0 ${SF_SELECTED} | ${SF_RO}
  SectionSetFlags ${UNSEC_0} $0
FunctionEnd
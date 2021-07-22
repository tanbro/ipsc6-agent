;--------------------------------
!include MUI2.nsh

;--------------------------------
;General
Unicode True
Name "IPSC6 座席工具条 (system/x86)"
OutFile "out\ipsc6_agent_wpfapp-win32-system.exe"

;Default installation folder
InstallDir "$PROGRAMFILES32\ipsc6-agent-wpfapp"

;Get installation folder from registry if available
InstallDirRegKey HKLM "Software\ipsc6_agent_wpfapp-win32" ""

ShowInstDetails show
ShowUnInstDetails show

;--------------------------------
;Variables
Var DisplayName

;--------------------------------
;Interface Settings
!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_UNFINISHPAGE_NOAUTOCLOSE

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

;Language
!insertmacro MUI_LANGUAGE "SimpChinese"

;--------------------------------
!include vcredist_x86.nsh
!include netfx.nsh

;--------------------------------

Section "!座席工具条" SEC_0
  SetShellVarContext all
  SetOutPath $INSTDIR

  ;Program FILES HERE...
  File /r /x "*.exp" /x "*.lib" /x "*.pdb" "..\ipsc6.agent.wpfapp\bin\x86\Release\*.*"

  ;Store installation folder
  WriteRegStr HKLM "Software\ipsc6_agent_wpfapp-win32" "" $INSTDIR

  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application    
    ;Create shortcuts
    CreateDirectory "$SMPROGRAMS\$DisplayName"
    CreateShortcut "$SMPROGRAMS\$DisplayName\启动 座席工具条.lnk" "$INSTDIR\ipsc6.agent.wpfapp.exe"  
    CreateShortcut "$SMPROGRAMS\$DisplayName\卸载 座席工具条.lnk" "$INSTDIR\Uninstall.exe"  
  !insertmacro MUI_STARTMENU_WRITE_END

  ; Add/Remove list
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-win32" \
                  "DisplayName" "$DisplayName"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-win32" \
                  "UninstallString" "$\"$INSTDIR\Uninstall.exe$\" /S"
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
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-win32"

  ;Remove InstalledRegKey only when it is empty
  DeleteRegKey /ifempty HKLM "Software\ipsc6_agent_wpfapp-win32"
SectionEnd

;------------------------------------------
;launch Section
Section "URI 启动程序" SEC_LAUNCH
  SetOutPath "$PLUGINSDIR\launch"
  SetCompress off
  File "out\ipsc6_agent_launch.exe"
  SetCompress auto
  DetailPrint "安装 URI Scheme Handler 启动程序 ..."
  ExecWait '"$OUTDIR\ipsc6_agent_launch.exe" /S' $0
    BringToFront
    ${If} $0 == "0"
        DetailPrint "URI Scheme Handler 启动程序 安装完毕"
    ${Else}
        DetailPrint "URI Scheme Handler 启动程序 安装失败: $0"
        MessageBox MB_ICONEXCLAMATION "错误:$\r$\nURI Scheme Handler 启动程序 安装失败: $0"
    ${EndIf}
SectionEnd

; components/sections
;--------------------------------------------
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_0} \
    "运行座席工具条所必须的文件。"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_LAUNCH} \
    "这是一个系统级的 URI Scheme Handler 启动程序。当有程序通过访问 ipsc6-agent: 开头的 URL 时，操作系统通过调用该程序启动座席工具条。"
!insertmacro MUI_FUNCTION_DESCRIPTION_END

!insertmacro MUI_UNFUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${UNSEC_0} \
    "删除座席工具条。"
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

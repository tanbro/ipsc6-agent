;----------------------------------------------------------
; URI handler 程序
; 该程序是全局的，要安装到 %ProgramFils%，且写入注册表的 HKEY_CLASSES_ROOT
;----------------------------------------------------------

;--------------------------------
!include MUI2.nsh
!include x64.nsh

;--------------------------------
;General
Unicode True
Name "IPSC6 座席工具条 URI Scheme Handler 启动程序"
OutFile "out\ipsc6_agent_launch.exe"

;UI miscs
ShowInstDetails show
ShowUnInstDetails show

;--------------------------------
;UI Configurations

;Interface Settings
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\nsis3-install.ico"
!define MUI_UNICON  "${NSISDIR}\Contrib\Graphics\Icons\nsis3-uninstall.ico"

!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Header\nsis3-metro.bmp"

!define MUI_WELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\nsis3-metro.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP_STRETCH FitControl

!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_UNFINISHPAGE_NOAUTOCLOSE

;Installer pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

;Uninstaller pages
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

;lang
!insertmacro MUI_LANGUAGE "SimpChinese"


;--------------------------------
;Variables
Var DisplayName

;--------------------------------
;Sections

Section ""
  ${If} ${RunningX64}
    SetOutPath "$PROGRAMFILES64\ipsc6-agent-launch"
  ${Else}
    SetOutPath "$PROGRAMFILES\ipsc6-agent-launch"
  ${EndIf}
  CreateDirectory "$OUTDIR"

  ;Program FILES HERE...
  File /r /x "*.pdb" "..\ipsc6.agent.launch\bin\Release\*.*"

  ;Create uninstaller
  WriteUninstaller "$OUTDIR\Uninstall.exe"

  ;URI scheme handler in Registy
  WriteRegStr HKCR "ipsc6-agent-launch" "" "$\"URL:ipsc6-agent-launch Protocol$\""
  WriteRegStr HKCR "ipsc6-agent-launch" "URL Protocol" "$\"$\""
  WriteRegStr HKCR "ipsc6-agent-launch\shell\open\command" "" "$\"$OUTDIR\ipsc6.agent.launch.exe$\" $\"%1$\""

  ;Add/Remove list
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_launch" \
                  "DisplayName" "$DisplayName"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_launch" \
                  "UninstallString" "$\"$OUTDIR\Uninstall.exe$\" /S"
SectionEnd

Section "-Uninstall"
  ${If} ${RunningX64}
    SetOutPath "$PROGRAMFILES64\ipsc6-agent-launch"
  ${Else}
    SetOutPath "$PROGRAMFILES\ipsc6-agent-launch"
  ${EndIf}

  ;uninstaller
  Delete "$OUTDIR\Uninstall.exe"

  ;Program files
  RMDir /r "$OUTDIR"

  ;URI scheme handler in Registy
  DeleteRegKey HKCR "ipsc6-agent-launch"

  ;Add/Remove list
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_launch"
SectionEnd

;----------------------------------------------------------------
Function .onInit
  StrCpy $DisplayName "IPSC6 座席工具条 URI Scheme Handler 启动程序"
FunctionEnd

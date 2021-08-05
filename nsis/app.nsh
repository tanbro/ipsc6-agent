;Including MSIS Macros
!include LogicLib.nsh
!include x64.nsh

;产品名，版本……
!define PUBLISHER "广州市和声信息技术有限公司"
!define PRODUCT_NAME "IPSC6 座席工具条 (${PLATFORM} ${USER_TYPE})"

!ifdef INCLUDE_VERSION_STRING_IN_OUTFILE
  OutFile "out\ipsc6_agent_wpfapp-${PLATFORM}-${USER_TYPE}.${VERSION}.exe"
!else
  OutFile "out\ipsc6_agent_wpfapp-${PLATFORM}-${USER_TYPE}.exe"
!endif

Name "${PRODUCT_NAME}"
BrandingText "${PUBLISHER} ${PRODUCT_NAME} ${VERSION}"

!if ${USER_TYPE} == "system"
    ;Default installation folder
    !if ${PLATFORM} == "win64"
        InstallDir "$PROGRAMFILES64\ipsc6-agent-wpfapp"
    !else if ${PLATFORM} == "win32"
        InstallDir "$PROGRAMFILES32\ipsc6-agent-wpfapp"
    !else
        !error "invalide PLATFORM: ${PLATFORM}"
    !endif
!else if ${USER_TYPE} == "user"
    ;Request application privileges
    RequestExecutionLevel highest
    ;Default installation folder
    InstallDir "$LOCALAPPDATA\Programs\ipsc6-agent-wpfapp"
!else
    !error "invalide USER_TYPE: ${USER_TYPE}"
!endif

;Get installation folder from registry if available
!if ${USER_TYPE} == "system"
    InstallDirRegKey HKLM "Software\ipsc6_agent_wpfapp-${PLATFORM}" ""
!else if ${USER_TYPE} == "user"
    InstallDirRegKey HKCU "Software\ipsc6_agent_wpfapp-${PLATFORM}" ""
!else
    !error "invalide USER_TYPE: ${USER_TYPE}"
!endif

ShowInstDetails show
ShowUnInstDetails show

Section "!座席工具条" SEC_0
  SetOutPath $INSTDIR

  ;Program FILES HERE...
  File /r /x "*.exp" /x "*.lib" /x "*.pdb" "..\ipsc6.agent.wpfapp\bin\${ARCH}\Release\*.*"

  ;Store installation folder
  !if ${USER_TYPE} == "system"
    WriteRegStr HKLM "Software\ipsc6_agent_wpfapp-${PLATFORM}" "" $INSTDIR
  !else if ${USER_TYPE} == "user"
    WriteRegStr HKCU "Software\ipsc6_agent_wpfapp-${PLATFORM}" "" $INSTDIR
  !else
    !error "invalide USER_TYPE: ${USER_TYPE}"
  !endif

  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application    
    ;Create shortcuts
    CreateDirectory "$SMPROGRAMS\$DisplayName"
    CreateShortcut "$SMPROGRAMS\$DisplayName\启动 座席工具条.lnk" "$INSTDIR\ipsc6.agent.wpfapp.exe"  
    CreateShortcut "$SMPROGRAMS\$DisplayName\卸载 座席工具条.lnk" "$INSTDIR\Uninstall.exe"  
  !insertmacro MUI_STARTMENU_WRITE_END

  ; Add/Remove list
!if ${USER_TYPE} == "system"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "DisplayName" "$DisplayName"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "Publisher" "${PUBLISHER}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "DisplayVersion" "${VERSION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "UninstallString" "$\"$INSTDIR\Uninstall.exe$\" /S"
!else if ${USER_TYPE} == "user"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "DisplayName" "$DisplayName"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "Publisher" "${PUBLISHER}"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "DisplayVersion" "${VERSION}"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "UninstallString" "$\"$INSTDIR\Uninstall.exe$\" /S"
!else
    !error "invalide USER_TYPE: ${USER_TYPE}"
!endif

SectionEnd

;--------------------------------
;Uninstaller Section
Section "!un.座席工具条" UNSEC_0
  Delete "$INSTDIR\Uninstall.exe"

  ;Remove whole install dir
  RMDir /r "$INSTDIR"

  ;Remove from Startup menus
  !insertmacro MUI_STARTMENU_GETFOLDER Application $DisplayName
  RMDir /r "$SMPROGRAMS\$DisplayName"

!if ${USER_TYPE} == "system"
  ;Remove from Add/Remove list
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}"
  ;Remove InstalledRegKey only when it is empty
  DeleteRegKey /ifempty HKLM "Software\ipsc6_agent_wpfapp-${PLATFORM}"
!else if ${USER_TYPE} == "user"
  ;Remove from Add/Remove list
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}"
  ;Remove InstalledRegKey only when it is empty
  DeleteRegKey /ifempty HKCU "Software\ipsc6_agent_wpfapp-${PLATFORM}"
!else
  !error "invalide USER_TYPE: ${USER_TYPE}"
!endif

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
    "这是一个系统级的 URI Scheme Handler 启动程序。当访问 ipsc6-agent: 开头的 URL 时，操作系统调用该程序启动座席工具条。"
!insertmacro MUI_FUNCTION_DESCRIPTION_END

!insertmacro MUI_UNFUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${UNSEC_0} \
    "删除座席工具条。"
!insertmacro MUI_UNFUNCTION_DESCRIPTION_END

Function .onInit
!if ${PLATFORM} == "win64"
  ${IfNot} ${RunningX64}
    StrCpy $0 "无法安装：该程序只能在 Win64 环境下运行。"
    MessageBox MB_OK|MB_ICONSTOP "messagebox_text" 
    Abort $0
  ${EndIf}
!endif

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

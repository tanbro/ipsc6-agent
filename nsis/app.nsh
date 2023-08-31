;Including MSIS Macros
!include LogicLib.nsh
!include x64.nsh
!include WordFunc.nsh

;产品名，版本……
!define PUBLISHER "广州市和声信息技术有限公司"
!define PRODUCT_NAME "IPSC6 座席话务条"

Name "${PRODUCT_NAME}(${PLATFORM} ${USER_TYPE})"
BrandingText "${PUBLISHER} ${PRODUCT_NAME} ${PRODUCT_VERSION}(${PLATFORM} ${USER_TYPE})"
OutFile "out\ipsc6-agent-installers\ipsc6-agent-${PLATFORM}-${USER_TYPE}-${PRODUCT_VERSION}.exe"

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

;VARS
Var InstalledDisplayName
Var InstalledVersion
Var UninstallString

Section "!${PRODUCT_NAME}" SEC_0

;检查之前安装的版本
DetailPrint "检查之前安装的版本 (${PLATFORM} ${USER_TYPE}) ..."
!if ${USER_TYPE} == "system"
  ReadRegStr $InstalledDisplayName \
             HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
             "DisplayName"
  ReadRegStr $InstalledVersion \
             HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
             "DisplayVersion"
  ReadRegStr $UninstallString \
              HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
              "UninstallString"
!else if ${USER_TYPE} == "user"
  ReadRegStr $InstalledDisplayName \
             HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
             "DisplayName"
  ReadRegStr $InstalledVersion \
             HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
             "DisplayVersion"
  ReadRegStr $UninstallString \
             HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
             "UninstallString"
!else
    !error "invalide USER_TYPE: ${USER_TYPE}"
!endif

  ${If} $InstalledVersion != ""
    DetailPrint "发现已安装的版本: $InstalledDisplayName($InstalledVersion). Uninstall='$UninstallString'"

    Push $0
    ${VersionCompare} $InstalledVersion ${PRODUCT_VERSION} $0    

    ${If} $0 == "1"
      MessageBox MB_OK|MB_ICONINFORMATION \
          "安装程序检测到 $InstalledDisplayName($InstalledVersion) 已经安装。$\r$\n由于已经安装的版本高于将要安装的版本，此次安装将不会继续！$\r$\n$\r$\n按 “确定” 退出。"
      Abort "已经安装的版本高于将要安装的版本，此次安装将不会继续！"

    ${Else}
      MessageBox MB_YESNO|MB_ICONQUESTION \
          "安装程序检测到 $InstalledDisplayName($InstalledVersion) 已经安装。$\r$\n只有在删除之前已经安装的程序之后方可继续安装。$\r$\n$\r$\n是否要删除之前安装的程序？$\r$\n$\r$\n按 “是” 删除，按 “否” 退出。" \
          IDYES _INSTALLED_UN_TRUE IDNO _INSTALLED_UN_FALSE

      Push $1
      _INSTALLED_UN_TRUE:
        ExecWait "$UninstallString" $1
        BringToFront
        ${If} $1 != "0"
          DetailPrint "$InstalledDisplayName($InstalledVersion) 删除失败: $1"
          MessageBox MB_OK|MB_ICONSTOP "错误:$\r$\n$\r$\n无法删除 $InstalledDisplayName($InstalledVersion) ($1)。"
          Abort "无法删除 $InstalledDisplayName($InstalledVersion) ($1)。"
        ${Else}
          DetailPrint "$InstalledDisplayName($InstalledVersion) 删除成功，延时等待操作完成 ..."
          Sleep 10000
          Goto _INSTALLED_UN_NEXT
        ${EndIf}      
      _INSTALLED_UN_FALSE:
        Abort "取消安装"
      _INSTALLED_UN_NEXT:
      Pop $1
    ${EndIf}

    Pop $0

  ${Else}
    DetailPrint "未发现已安装的版本"
  ${EndIf}

  ;设置安装路径
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
    CreateShortcut "$SMPROGRAMS\$DisplayName\启动 座席话务条.lnk" "$INSTDIR\ipsc6.agent.wpfapp.exe"  
    CreateShortcut "$SMPROGRAMS\$DisplayName\卸载 座席话务条.lnk" "$INSTDIR\Uninstall.exe"  
  !insertmacro MUI_STARTMENU_WRITE_END

  ; Add/Remove list
!if ${USER_TYPE} == "system"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "DisplayName" "$DisplayName"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "Publisher" "${PUBLISHER}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "UninstallString" "$\"$INSTDIR\Uninstall.exe$\" /S"
!else if ${USER_TYPE} == "user"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "DisplayName" "$DisplayName"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "Publisher" "${PUBLISHER}"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\ipsc6_agent_wpfapp-${PLATFORM}" \
                  "UninstallString" "$\"$INSTDIR\Uninstall.exe$\" /S"
!else
    !error "invalide USER_TYPE: ${USER_TYPE}"
!endif

SectionEnd

;--------------------------------
;Uninstaller Section
Section "!un.座席话务条" UNSEC_0
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
  File "out\ipsc6-agent-launch.exe"
  SetCompress auto
  DetailPrint "安装 URI Scheme Handler 启动程序 ..."
  Push $0
  ExecWait '"$OUTDIR\ipsc6-agent-launch.exe" /S' $0
  BringToFront
  ${If} $0 == "0"
    DetailPrint "URI Scheme Handler 启动程序 安装完毕"
  ${Else}
    DetailPrint "URI Scheme Handler 启动程序 安装失败: $0"
    MessageBox MB_ICONEXCLAMATION "错误:$\r$\nURI Scheme Handler 启动程序 安装失败: $0"
  ${EndIf}
  Pop $0
SectionEnd

; components/sections
;--------------------------------------------
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_0} \
    "运行座席话务条所必须的文件。"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_LAUNCH} \
    "这是一个系统级的 URI Scheme Handler 启动程序。当访问 ipsc6-agent: 开头的 URL 时，操作系统调用该程序启动座席话务条。"
!insertmacro MUI_FUNCTION_DESCRIPTION_END

!insertmacro MUI_UNFUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${UNSEC_0} \
    "删除座席话务条。"
!insertmacro MUI_UNFUNCTION_DESCRIPTION_END

Function .onInit
!if ${PLATFORM} == "win64"
  ${IfNot} ${RunningX64}
	Push $0
    StrCpy $0 "无法安装：该程序只能在 Win64 环境下运行。"
    MessageBox MB_OK|MB_ICONSTOP "messagebox_text" 
    Abort $0
	Pop $0
  ${EndIf}
!endif

  ; https://nsis.sourceforge.io/Graying_out_Section_(define_mandatory_sections)
  # set the section as selected and read-only
  Push $0
  IntOp $0 ${SF_SELECTED} | ${SF_RO}
  SectionSetFlags ${SEC_0} $0
  Pop $0
FunctionEnd

Function un.onInit
  ; https://nsis.sourceforge.io/Graying_out_Section_(define_mandatory_sections)
  # set the section as selected and read-only
  Push $0
  IntOp $0 ${SF_SELECTED} | ${SF_RO}
  SectionSetFlags ${UNSEC_0} $0
  Pop $0
FunctionEnd

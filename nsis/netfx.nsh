;; 检查 .NET Framework 版本
;; 本应用要求 .NET Framework v4.6.1 以上版本
;; 如果不存在，应安装
;;
;; ref: https://docs.microsoft.com/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed

!include LogicLib.nsh

Var NETFX_RELEASE
Var NETFX_OK

Section "-SEC_NETFX"
    SetOutPath "$PLUGINSDIR\netfx"
    SetCompress off
    File "deps\NDP461-KB3102436-x86-x64-AllOS-ENU.exe"
    SetCompress auto

    StrCpy $NETFX_OK ""

    ReadRegDWORD $NETFX_RELEASE HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
    ${If} $NETFX_RELEASE == ""
        DetailPrint "未检测到 NET Framework Release"
    ${ElseIf} $NETFX_RELEASE < "394254"
        DetailPrint "检测到 NET Framework Release $NETFX_RELEASE, 版本低于要求"
    ${Else}
        DetailPrint "检测到 NET Framework Release $NETFX_RELEASE, 版本达到要求"
        StrCpy $NETFX_OK "1"
    ${EndIf}

    ${If} $NETFX_OK == ""
        MessageBox MB_YESNO|MB_ICONQUESTION \
            "在计算机上找不到运行本程序所需的 .NET Framework v4.6.1 或以上版本。$\r$\n$\r$\n现在是否要安装这个软件？$\r$\n$\r$\n按 “是” 立即执行，按 “否” 退出安装程序。" \
            IDYES _NETFX_TRUE IDNO _NETFX_FALSE
        _NETFX_TRUE:
            DetailPrint ".NET Framework 正在安装 ..."
            ExecWait '"$OUTDIR\NDP461-KB3102436-x86-x64-AllOS-ENU.exe" /norestart /passive /showrmui /showfinalerror' $0
            BringToFront
            ${If} $0 != "0"
                DetailPrint ".NET Framework 安装失败: $0"
                MessageBox MB_OK|MB_ICONSTOP "错误:$\r$\n$\r$\n.NET Framework v4.6.1 安装失败 ($0)。"
                Quit
            ${Else}
                DetailPrint ".NET Framework v4.6.1 安装完成"
            ${EndIf}
            Goto _NETFX_NEXT
        _NETFX_FALSE:
            Quit
        _NETFX_NEXT:
    ${EndIf}
SectionEnd


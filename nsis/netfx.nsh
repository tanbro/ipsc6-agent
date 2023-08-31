;; 检查 .NET Framework 版本
;; 本应用要求 .NET Framework v4.8 或以上版本
;; 如果不存在，应安装
;;
;; ref: https://docs.microsoft.com/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed

!include LogicLib.nsh

Var NETFX_RELEASE
Var NETFX_OK

Section "-SEC_NETFX"

    StrCpy $NETFX_OK ""

    ReadRegDWORD $NETFX_RELEASE HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
    ${If} $NETFX_RELEASE == ""
        DetailPrint "未检测到 NET Framework Release"
    ${ElseIf} $NETFX_RELEASE < "528049"
        DetailPrint "检测到 NET Framework Release $NETFX_RELEASE, 版本低于要求"
    ${Else}
        DetailPrint "检测到 NET Framework Release $NETFX_RELEASE, 版本达到要求"
        StrCpy $NETFX_OK "1"
    ${EndIf}

    Push $0

    ${If} $NETFX_OK == ""
        MessageBox MB_YESNO|MB_ICONQUESTION \
            "在计算机上找不到运行本程序所需的 .NET Framework v4.8 或以上版本。$\r$\n$\r$\n现在是否要安装这个软件？$\r$\n$\r$\n按 “是” 立即执行，按 “否” 退出安装程序。" \
            IDYES _NETFX_TRUE IDNO _NETFX_FALSE
    
        _NETFX_TRUE:

            SetOutPath "$PLUGINSDIR\netfx"
            SetCompress off
            File "deps\ndp48-x86-x64-allos-enu.exe"
            SetCompress auto

            DetailPrint ".NET Framework 正在安装 ..."
            ExecWait '"$OUTDIR\ndp48-x86-x64-allos-enu.exe" /norestart /passive /showrmui /showfinalerror' $0
            BringToFront
            ${If} $0 != "0"
                Push $1
                StrCpy $1 ".NET Framework v4.8 安装失败 ($0)"
                DetailPrint $1 
                MessageBox MB_OK|MB_ICONSTOP "错误:$\r$\n$\r$\n.$1"
                Abort "$1"
                Pop $1
            ${Else}
                DetailPrint ".NET Framework v4.8 安装成功"
            ${EndIf}
            Goto _NETFX_NEXT

        _NETFX_FALSE:
            Abort "取消安装"

        _NETFX_NEXT:
    ${EndIf}

    Pop $0
SectionEnd


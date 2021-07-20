;; 检查 VC_redist 版本
;; 本应用要求 Microsoft Visual C++ Redistributable for Visual Studio 2015, 2017 and 2019
;; 如果不存在，应安装
;;
;; ref: https://support.microsoft.com/en-us/topic/the-latest-supported-visual-c-downloads-2647da03-1eea-4433-9aff-95f26a218cc0

!include LogicLib.nsh

Var VCREDIST_OK
Section "-SEC_VCREDIST_X64"
    StrCpy $VCREDIST_OK ""
    SetOutPath "$PLUGINSDIR\vcredist"
    File "..\x64\Release\DummyCppConsoleApp.exe"

    DetailPrint "Visual C++ Runtime 检测 ..."
    ExecWait '"$OUTDIR\DummyCppConsoleApp.exe"' $0
    BringToFront
    ${If} $0 == "0"
        DetailPrint "Visual C++ Runtime 检测通过"
        StrCpy $VCREDIST_OK "1"
    ${Else}
        DetailPrint "Visual C++ Runtime 检测失败: $0"
    ${EndIf}

    ${If} $VCREDIST_OK == ""
        MessageBox MB_YESNO|MB_ICONQUESTION \
            "适用于 Visual Studio 2015、2017 和 2019 的 Microsoft Visual C++ 运行时检测失败。计算机上可能没有安装这个软件。$\r$\n$\r$\n现在是否要安装？$\r$\n$\r$\n选择“是”立即执行，选择“否”将退出安装程序。" \
            IDYES true IDNO false
        true:
            SetCompress off
            File "deps\VC_redist.x64.exe"
            SetCompress auto

            DetailPrint "Visual C++ Redistributable 正在安装 ..."
            ExecWait '"$OUTDIR\VC_redist.x64.exe" /quiet' $0
            BringToFront
            ${If} $0 != "0"
                DetailPrint "Visual C++ Redistributable 安装失败: $0"
                MessageBox MB_OK|MB_ICONSTOP "错误:$\r$\n$\r$\n适用于 Visual Studio 2015、2017 和 2019 的 Microsoft Visual C++ 可再发行软件包安装失败 ($0)。" 
                Quit
            ${Else}
                DetailPrint "Visual C++ Redistributable 安装完成"
            ${EndIf}
            Goto next
        false:
            Quit
        next:
    ${EndIf}
SectionEnd

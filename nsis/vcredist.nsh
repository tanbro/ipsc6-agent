;; 检查 VC_redist 版本
;; 本应用要求 Microsoft Visual C++ Redistributable for Visual Studio 2015, 2017 and 2019
;; 如果不存在，应安装
;;
;; ref: https://support.microsoft.com/en-us/topic/the-latest-supported-visual-c-downloads-2647da03-1eea-4433-9aff-95f26a218cc0

!include LogicLib.nsh

Var VCREDIST_OK

Section "-SEC_VCREDIST"
    StrCpy $VCREDIST_OK ""

    ; MessageBox MB_OK|MB_ICONINFORMATION \
    ;     "即将执行 “适用于 Visual Studio 2015、2017 和 2019 的 Microsoft Visual C++ (${ARCH})” 的 检测程序。期间可能会出现错误提示，但这并不表示安装失败。$\r$\n$\r$\n按 “确定” 开始检测。"

    SetOutPath "$PLUGINSDIR\vcredist"
    File "..\${ARCH}\Release\DummyCppConsoleApp.exe"
    
    DetailPrint "Visual C++ Runtime (${ARCH}) 检测 ..."
    Push $0
    ExecWait '"$OUTDIR\DummyCppConsoleApp.exe"' $0
    BringToFront
    ${If} $0 == "0"
        DetailPrint "Visual C++ Runtime 检测通过"
        StrCpy $VCREDIST_OK "1"
        ; MessageBox MB_OK|MB_ICONINFORMATION \
        ;     "“适用于 Visual Studio 2015、2017 和 2019 的 Microsoft Visual C++ (${ARCH})” 检测成功。$\r$\n$\r$\n按 “确定” 继续安装。"
    ${Else}
        DetailPrint "Visual C++ Runtime 检测失败: $0"
    ${EndIf}
    Pop $0

    ${If} $VCREDIST_OK == ""
        MessageBox MB_YESNO|MB_ICONQUESTION \
            "计算机上可能没有安装“适用于 Visual Studio 2015、2017 和 2019 的 Microsoft Visual C++ 可再发行软件包(${ARCH})”。$\r$\n$\r$\n现在是否要安装？$\r$\n$\r$\n按 “是” 立即执行，按 “否” 退出安装程序。" \
            IDYES _VCREDIST_TRUE IDNO _VCREDIST_FALSE

        _VCREDIST_TRUE:
    
            SetCompress off
            File "deps\VC_redist.${ARCH}.exe"
            SetCompress auto

            DetailPrint "安装“适用于 Visual Studio 2015、2017 和 2019 Microsoft Visual C++ 可再发行软件包(${ARCH})”..."
            Push $0
            ExecWait '"$OUTDIR\VC_redist.${ARCH}.exe" /norestart /passive' $0
            BringToFront
            ${If} $0 != "0"
                Push $1
                StrCpy $1 "“适用于 Visual Studio 2015、2017 和 2019 Microsoft Visual C++ 可再发行软件包(${ARCH})”安装失败 ($0)"
                DetailPrint $1
                MessageBox MB_OK|MB_ICONSTOP "错误:$\r$\n$\r$\n$1"
                Abort "$1"
                Pop $1
            ${Else}
                DetailPrint "“适用于 Visual Studio 2015、2017 和 2019 Microsoft Visual C++ 可再发行软件包(${ARCH})”安装完成"
            ${EndIf}
            Pop $0
            Goto _VCREDIST_NEXT

        _VCREDIST_FALSE:
            Abort "取消安装"

        _VCREDIST_NEXT:
    ${EndIf}
SectionEnd

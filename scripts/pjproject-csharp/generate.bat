@echo off

set PJPROJECT_ROOT=submodules\pjproject

swig -v ^
    -I%PJPROJECT_ROOT%\pjlib\include ^
    -I%PJPROJECT_ROOT%\pjlib-util\include ^
    -I%PJPROJECT_ROOT%\pjmedia\include ^
    -I%PJPROJECT_ROOT%\pjsip\include ^
    -I%PJPROJECT_ROOT%\pjnath\include ^
    -I%PJPROJECT_ROOT%\pjsip\include ^
    -c++ -w312 ^
    -csharp -namespace org.pjsip.pjsua2 ^
    -outdir pjsua2_csharp ^
    %PJPROJECT_ROOT%\pjsip-apps\src\swig\pjsua2.i

move %PJPROJECT_ROOT%\pjsip-apps\src\swig\pjsua2_wrap.* pjsua2_wrap\src

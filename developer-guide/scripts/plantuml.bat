@echo off

setlocal
set GRAPHVIZ_DOT=C:\Program Files\Graphviz\bin\dot.exe

java %PLANTUML_JAVAOPTS% -jar %~dp0\..\Lib\plantuml.jar -charset UTF8 %*

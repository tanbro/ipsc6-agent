# README

使用 mkdocs 书写的《IPSC 座席客户端开发人员指南》

## 环境搭建

1. 安装 Python 3.6 或以上

1. [_可选_] 在子目录 `venv` 建立该项目的 Python 虚拟环境，并激活:

   - Posix:

     ```sh
     python -m venv venv
     source venv/bin/activate
     ```

   - Windows 命令提示符:

     ```bat
     py -m venv venv
     venv\Scripts\Activate.bat
     ```

1. 安装依赖软件

   在上述 Python 虚拟环境激活的前提下执行命令:

   ```sh
   pip install -r requirements.txt
   ```

## PlantUML 相关设置

对于 Windows，可以使用以下 `plantuml.bat`:

```bat
@echo off

setlocal
set GRAPHVIZ_DOT=C:\Program Files\Graphviz\bin\dot.exe

java %PLANTUML_JAVAOPTS% -jar %~dp0\..\Lib\plantuml.jar -charset UTF8 %*
```

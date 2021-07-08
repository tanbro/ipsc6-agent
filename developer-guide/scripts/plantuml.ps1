$Env:GRAPHVIZ_DOT = "C:\Program Files\Graphviz\bin\dot.exe"
java $Env:PLANTUML_JAVAOPTS -jar $PSScriptRoot\..\Lib\plantuml.jar -charset UTF8 $args

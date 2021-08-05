;--------------------------------
!include MUI2.nsh

;--------------------------------
;General
Unicode True

;--------------------------------
;用于接收产品名称的全局变量
Var DisplayName

;--------------------------------
;用于配置不同平台和安装方式的宏定义
!define ARCH "x64"
!define PLATFORM "win64"
!define USER_TYPE "system"

;--------------------------------
;版本定义的宏
!include ver.nsh

;--------------------------------
;Interface Settings
!include ui.nsh

;--------------------------------
;依赖软件
!include vcredist_${ARCH}.nsh
!include netfx.nsh

;--------------------------------
;安装程序
!include app.nsh

;--------------------------------
; 安装程序文件属性
!include vi.nsh

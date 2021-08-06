!ifndef FILE_VERSION
  !define FILE_VERSION "1.0.0.0"
!endif

VIProductVersion "${FILE_VERSION}"
VIAddVersionKey /LANG=0 "ProductVersion" "${PRODUCT_VERSION}"
VIAddVersionKey /LANG=0 "ProductName" "${PRODUCT_NAME}"
VIAddVersionKey /LANG=0 "LegalCopyright" "© 2021 ${PUBLISHER}"
VIAddVersionKey /LANG=0 "FileDescription" "${PRODUCT_NAME} 安装程序"
VIAddVersionKey /LANG=0 "FileVersion" "${FILE_VERSION}"

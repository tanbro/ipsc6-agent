!ifndef PRODUCT_VERSION
    !system "py generate_version_file.py"
    !include version.tmp.nsh
!endif

# !/usr/env/bin/python3

from subprocess import check_output

COMMAND = "dotnet-gitversion  /showvariable SemVer"
VERSION_FILE = "version.tmp.nsh"

print("execute:", COMMAND)
sem_ver = check_output(COMMAND, shell=True).strip()
print("SemVer:", sem_ver)
data = b"!define PRODUCT_VERSION " + sem_ver

print("write:", VERSION_FILE)
with open(VERSION_FILE, "wb") as fp:
    fp.write(data)

#!/usr/bin/env bash

set -euxo pipefail

BIN_MAME="opengl-glib"
DOTNET_VER="net9.0"

function run_release () {
    (cd "$(dirname "$0")" && dotnet build --configuration Release && cd "./bin/Release/$DOTNET_VER" && "./$BIN_MAME")
}

function run_debug () {
    (cd "$(dirname "$0")" && dotnet build --configuration Debug && cd "./bin/Debug/$DOTNET_VER" && "./$BIN_MAME")
}

# Ручная сборка и запуск программы из директории с исполняемым файлом
# если аргумент не указан - собрать и запустить в конфигурации для отладки
if [ $# -ne 1 ]; then
    echo "Ожидается один аргумент. Получено: $#"
    exit 1
fi

if [[ "$1" == "--debug" ]]; then
    time run_debug
elif [[ "$1" == "--release" ]]; then
    time run_release
fi

exit 0

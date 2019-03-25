dotnet --version
dotnet build StbTrueTypeSharp.sln /p:Configuration=Release --no-incremental
call copy_zip_package_files.bat
rename "ZipPackage" "StbTrueTypeSharp.%APPVEYOR_BUILD_VERSION%"
7z a StbTrueTypeSharp.%APPVEYOR_BUILD_VERSION%.zip StbTrueTypeSharp.%APPVEYOR_BUILD_VERSION%

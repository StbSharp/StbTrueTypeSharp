rem delete existing
rmdir "ZipPackage" /Q /S

rem Create required folders
mkdir "ZipPackage"

set "CONFIGURATION=Release\netstandard1.1"

rem Copy output files
copy "src\StbTrueTypeSharp\bin\%CONFIGURATION%\StbTrueTypeSharp.dll" "ZipPackage" /Y
copy "src\StbTrueTypeSharp\bin\%CONFIGURATION%\StbTrueTypeSharp.pdb" "ZipPackage" /Y
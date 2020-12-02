@echo off
ilasm ShimSources.il /dll /out=ShimSources.dll
ilverify ShimSources.dll -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.0\*.dll" --ignore-error UnmanagedPointer
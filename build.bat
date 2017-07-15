@echo off
SET SOLUTIONPATH=./src/CompareAndCopy.sln
CALL ./msbuild %SOLUTIONPATH% /t:Restore /p:Configuration=Release
CALL ./msbuild %SOLUTIONPATH% /t:Build /p:Configuration=Release /p:BuildSetupOnBuild=true %*


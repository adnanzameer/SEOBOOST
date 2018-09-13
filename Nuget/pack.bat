@Echo Removing old files
del Package\lib /s /q
del Package\content /s /q

@Echo Setting up folder structure
md Package\lib\net45\
md Package\tools\

@Echo Copying new files
copy ..\SeoBoost\bin\Release\SeoBoost.dll Package\lib\net45\

@Echo Packing files
"..\.nuget\nuget.exe" pack Package\SeoBoost.nuspec

@Echo Moving package
move /Y *.nupkg c:\project\nuget.local\

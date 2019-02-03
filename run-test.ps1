dotnet tool install --global coverlet.console
dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools

ForEach ($folder in (Get-ChildItem -Path "tests" -Directory)) 
{
    dotnet test $folder.FullName /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput="../TestResults/$folder-opencover.xml";
}

tools\reportgenerator.exe "--reports:tests/TestResults/*-opencover.xml" "--targetdir:tests/TestResults/html"

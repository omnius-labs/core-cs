cd `dirname $0`

apt-get update
apt-get install -y --no-install-recommends libc6-dev

dotnet tool install --global coverlet.console
dotnet tool install --global dotnet-reportgenerator-globaltool

export PATH="$PATH:/root/.dotnet/tools"

if [ -z "$CI_PROJECT_DIR" ]; then
    CI_PROJECT_DIR=`dirname $0`
fi

for path in `find "$CI_PROJECT_DIR/tests" -maxdepth 2 -type f -name "*.csproj"`
do
    output="../TestResults/$(basename ${path%.*})-opencover.xml"
    dotnet test "$path" -p:CollectCoverage=true -p:CoverletOutputFormat=opencover -p:CoverletOutput=$output;
done

reportgenerator "--reports:tests/TestResults/*-opencover.xml" "--targetdir:tests/TestResults/html"

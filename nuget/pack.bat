dotnet pack ../src/libs/FastEnum/FastEnum.csproj -c Release -o ./packages
dotnet pack ../src/libs/FastEnum.Core/FastEnum.Core.csproj -c Release -o ./packages
dotnet pack ../src/libs/FastEnum.Generators/FastEnum.Generators.csproj -c Release -o ./packages
pause

param([string]$Day)

mkdir $Day
dotnet new console -o $Day\csharp -n $Day
"" > $Day\README
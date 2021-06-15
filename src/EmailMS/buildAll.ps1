rmdir .\SenderEmail\wwwroot\plugins\ -r -force
dotnet clean
dotnet restore
dotnet build -c Debug --no-restore
Get-ChildItem
copy-item -Path ".\SimpleSMTP\bin\Debug\net5.0\" -Destination ".\SenderEmail\wwwroot\plugins\smtpProviders\SimpleSMTP\" -Recurse -Force
copy-item -Path ".\EmailSmtpClientGmail\bin\Debug\net5.0\" -Destination ".\SenderEmail\wwwroot\plugins\smtpProviders\EmailSmtpClientGmail\" -Recurse -Force
dotnet run --project --project SenderEmail --no-build

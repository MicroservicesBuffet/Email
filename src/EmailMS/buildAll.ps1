dotnet restore
dotnet build
Get-ChildItem
copy-item -Path ".\SimpleSMTP\bin\Debug\net5.0\" -Destination ".\SenderEmail\wwwroot\plugins\smtpProviders\SimpleSMTP\" -Recurse -Force
copy-item -Path ".\EmailSmtpClientGmail\bin\Debug\net5.0\" -Destination ".\SenderEmail\wwwroot\plugins\smtpProviders\EmailSmtpClientGmail\" -Recurse -Force


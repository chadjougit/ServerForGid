# ServerForGid
Angular2/ASP.Net Core "money" transactions sample

This is a server-side part, for client app check https://github.com/chadjougit/clientFirGid

to restore packages, run "dotnet restore" in cmd

to start, use "dotnet run" in cmd

##Configuration
to manage database connection, change value of DefaultConnection in appsettings.json

to additional configuration, change Config.cs (for example, for changing cors for a client app)

change  authority in app.UseIdentityServerAuthentication in Startup.cs if needed 

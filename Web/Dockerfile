FROM microsoft/aspnetcore
COPY /deploy .
WORKDIR .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Server.dll"]
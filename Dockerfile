FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["VoicemeeterSliderControl.csproj", "./"]
RUN dotnet restore "VoicemeeterSliderControl.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "VoicemeeterSliderControl.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VoicemeeterSliderControl.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VoicemeeterSliderControl.dll"]

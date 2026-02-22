# syntax=docker/dockerfile:1.2

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
COPY ./docker/sources.list /etc/apt/sources.list
#RUN apt-get update  
#RUN apt-get install -y libgdiplus
  # && apt-get upgrade -y \
  # && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./Laiye.Customer.WebApi/Laiye.Customer.WebApi.csproj", "Laiye.Customer.WebApi/"]
ADD "https://www.random.org/cgi-bin/randbyte?nbytes=10&format=h" skipcache
RUN  --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
 dotnet restore "Laiye.Customer.WebApi/Laiye.Customer.WebApi.csproj"  -s "https://nexus.wul.ai/repository/nuget-group-uibot/index.json" -s "https://nexus.wul.ai/repository/nuget-group-internet-project/index.json"
COPY . .
WORKDIR "/src/Laiye.Customer.WebApi"
RUN --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
 dotnet build "Laiye.Customer.WebApi.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN  --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
 dotnet publish "Laiye.Customer.WebApi.csproj" -c Release -o /app/publish --no-restore
RUN mkdir /app/publish/logs

FROM base AS final
ADD https://laiye-ops-public.oss-cn-beijing.aliyuncs.com/docker/grpc-health-probe/grpc_health_probe-linux-amd64-v0.3.6 /bin/grpc_health_probe
RUN chmod 755 /bin/grpc_health_probe
RUN groupadd -g 5000 works && useradd -m -d /home/works -u 5000 -g works works
WORKDIR /app
COPY --from=publish --chown=works /app/publish /app
USER works
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "Laiye.Customer.WebApi.dll"]

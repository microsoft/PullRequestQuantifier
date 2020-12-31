### Base image that can be used to perform a standardized build
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder

RUN useradd -m dotnet
USER dotnet

### Copy the entire source
COPY --chown=dotnet . /app/
WORKDIR /app

# switch to root since we need to install node js.
USER root

# Install NodeJS
ENV NODE_VERSION 14.15.3
ENV NODE_DOWNLOAD_SHA 439490022fed1ef0945240210c3a84725adcd24ff238325199118323462cb43d
RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-linux-x64.tar.gz" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

# switch out to the dotnet user.
USER dotnet

WORKDIR /app/src/Clients/PullRequestQuantifier.GitHub.Client/

RUN pwd

RUN ls -l

### Install app dependencies
RUN dotnet restore

### Build
RUN dotnet build --configuration Release --no-restore 

### Test
#RUN dotnet test

# ### Copy to final container
# FROM mcr.microsoft.com/dotnet/aspnet:5.0

# RUN useradd -m dotnet
# USER dotnet

# ### copy from previous phase to this one/
# COPY --from=builder --chown=root /app/ ./app/

# ### switch to the work directory.
# WORKDIR /app

USER root

ENV AzureFunctionsJobHost__Logging__Console__IsEnabled=true

ENTRYPOINT exec npm run start
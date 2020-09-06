# Setup

## Run on docker

Docker image: ```ghcr.io/xxlo-devs/eru:latest```

I suggest running eru on docker-compose. 

Sample docker-compose.yml
```yaml
version: '3.8'
services:
  database:
    container_name: database
    image: postgres
    restart: unless-stopped
    environment:
      - POSTGRES_USER=administrator
      - POSTGRES_PASSWORD=sample_password
      - POSTGRES_DB=eru
    volumes:
     - /database_files:/var/lib/postgresql/data
  seq:
    container_name: seq
    image: datalust/seq
    restart: unless-stopped
    ports:
      - 1001:80
    environment:
      - ACCEPT_EULA=Y
    volumes:
     - /seq-data:/data
  eru:
    container_name: eru
    image: ghcr.io/xxlo-devs/eru:latest
    restart: unless-stopped
    ports:
    - 1000:80
    depends_on:
      - database
    environment:
      - DOTNET_ENVIRONMENT=docker # Important!
      - Database__Type=postgresql
      - Database__AutomaticallyMigrate=true
      - Database__ConnectionString=Host=database;Database=eru;Username=administrator;Password=sample_password
      - UploadKey=V3ryS3cureUpl0adK3y
      - Admins__0__Username=admin
      - Admins__0__Password=s@mpl3P@ssword
      - CultureSettings__DefaultCulture=pl
    volumes:
     - /logs:/Logs
```

1. You can add new admin by appending the environment with:
```
     - Admins__{incrementing number from 0}__Username={username}
     - Admins__{incrementing number from 0}__Password={password}
```
2. Other settings are same and can be viewed [here](/config)

## Compile from source code

### Download source code

Our source code is available for free (MIT License) in our [repo](https://github.com/xxlo-devs/eru). The source code you can download either via this [link (.zip archive)](https://github.com/xxlo-devs/eru/archive/master.zip)
or by running `git clone https://github.com/xxlo-dev/eru` in your system (installed git is required).

### Obtain Dotnet Core SDK

Download Dotnet Core SDK 3.1.302 from [here](https://dotnet.microsoft.com/download/dotnet-core/3.1) and install it.

### Configure your application

See [here](/config) to see all required and additional configuration 👍.

### Run application

Simply run `dotnet run -c Release -p src/eru.WebApp` in folder with source code. SDK will compile and run our application effortlessly.

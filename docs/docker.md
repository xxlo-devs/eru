# Docker - setup and configuration

All settings can be viewed in [config](/config) page.

## Docker images

Latest image is automatically built from [master](https://github.com/xxlo-devs/eru/tree/master) and can be run from image: `ghcr.io/xxlo-devs/eru`.

## Sample docker run

```sh
docker run ghcr.io/xxlo-devs/eru
```

## Recommended setup with docker-compose

We recommend setting up our app with docker-compose in order to quickly setup all required and useful services.

REMEMBER to setup correct volumes binding NOT to lose data!

sample docker-compose.yml

```yml
version: '3.8'
services: 
    database:
        image: postgres
        restart: unless-stopped
        environment: 
            POSTGRES_PASSWORD: s@mpl3P@ssword
            POSTGRES_USER: eru
            POSTGRES_DB: eru
        volumes: 
            - database:/var/lib/postgresql/data
    nginx:
        image: nginx
        restart: unless-stopped
        expose:
            - 80:80
            - 443:443
        volumes: 
            - nginx:/etc/nginx
            - letsencrypt:/etc/letsencrypt/
    seq:
        image: datalust/seq
        restart: unless-stopped
        environment: 
            ACCEPT_EULA: Y
        ports:
            - 80:80
        volumes: 
            - seq:/data
    eru:
        image: ghcr.io/xxlo-devs/eru:feature-docker
        restart: unless-stopped
        expose: 
            - 5001:80
        environment: 
            Serilog__WriteTo__1__Args__path: /logs/log.txt
            Serilog__WriteTo__2__Name: Seq
            Serilog__WriteTo__2__Args__serverUrl: http://seq:5341
            Database__Type: postgresql
            Database__ConnectionString: Host=database;Username=eru;Password=s@mpl3P@ssword;Database=eru
            UploadKey: V3ryS3cureUpl0adK3y
            CultureSettings__DefaultCulture: pl
            Admins__0__Username: admin
            Admins__0__Password: s@mpl3P@ssword
        volumes: 
            - logs:/logs
volumes: 
    database:
    seq:
    logs:
```
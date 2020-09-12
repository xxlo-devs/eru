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
        ports:
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
        expose:
          - 80
        volumes: 
            - seq:/data
    eru:
        image: ghcr.io/xxlo-devs/eru
        restart: unless-stopped
        expose: 
            - 80
        environment: 
            Serilog__WriteTo__1__Args__path: /logs/log.txt
            Serilog__WriteTo__2__Name: Seq
            Serilog__WriteTo__2__Args__serverUrl: http://seq
            Database__Type: postgresql
            Database__ConnectionString: Host=database;Username=eru;Password=s@mpl3P@ssword;Database=eru
            UploadKey: V3ryS3cureUpl0adK3y
            CultureSettings__DefaultCulture: pl
            Admins__0__Username: admin
            Admins__0__Password: s@mpl3P@ssword
            Paths__WebAppPathBase: /
            Paths__SeqUiPath: /seq
            PlatformClients__FacebookMessenger__VerifyToken: "sample-verify-token"
            PlatformClients__FacebookMessenger__AccessToken: "sample-access-token"
        volumes: 
            - logs:/logs
        depends_on:
            - database
            - nginx 
            - seq
volumes: 
    database: 
    seq: 
    logs: 
    nginx: 
    letsencrypt: 
```

## Sample nginx default.conf with docker-compose

```
server {
    listen       80;
    listen  [::]:80;
    server_name  localhost;

    error_page   500 502 503 504  /50x.html;
    location = /50x.html {
        root   /usr/share/nginx/html;
    }

    location / {
        proxy_pass      http://eru/;
        proxy_http_version      1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    location /seq/ {
        proxy_pass      http://seq/;
        proxy_http_version      1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```
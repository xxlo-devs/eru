# Setup

Currently you do not provide any way to easily deploy our application so,
if you want one you'll need to get your hands dirty and deploy from source code :/

## Download source code

Our source code is available for free (MIT License) in our [repo](https://github.com/xxlo-devs/eru). The source code you can download either via this [link (.zip archive)](https://github.com/xxlo-devs/eru/archive/master.zip)
or by running `git clone https://github.com/xxlo-dev/eru` in your system (installed git is required).

## Obtain Dotnet Core SDK

Download Dotnet Core SDK 3.1.302 from [here](https://dotnet.microsoft.com/download/dotnet-core/3.1) and install it.

## Configure your application

See [here](/config) to see all required and additional configuration 👍.

## Run application

Simply run `dotnet run -c Release -p src/eru.WebApp` in folder with source code. SDK will compile and run our application effortlessly.

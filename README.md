# ACI-Rental
This repository contains the entire ACI-Rental project. 

[![Build the ImageService](https://github.com/ACI-Rental/backend/actions/workflows/build-image-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/build-image-service.yml)

[![Release new ImageService to Docker hub](https://github.com/ACI-Rental/backend/actions/workflows/release-image-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/release-image-service.yml)

[![Build the OcelotAPIGateway](https://github.com/ACI-Rental/backend/actions/workflows/build-api-gateway.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/build-api-gateway.yml)

[![Release new OcelotAPIGateway to Docker hub](https://github.com/ACI-Rental/backend/actions/workflows/release-api-gateway.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/release-api-gateway.yml)

[![Build the ProductService](https://github.com/ACI-Rental/backend/actions/workflows/build-product-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/build-product-service.yml)

[![Release new ProductService to Docker hub](https://github.com/ACI-Rental/backend/actions/workflows/release-product-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/release-product-service.yml)

[![Build the ReservationService](https://github.com/ACI-Rental/backend/actions/workflows/build-reservation-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/build-reservation-service.yml)

[![Release new ReservationService to Docker hub](https://github.com/ACI-Rental/backend/actions/workflows/release-reservation-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/release-reservation-service.yml)

# Get started
This section will guide you into running the project in a local environment. The image below represents the architecture you will achieve while following this guide. 
![Software Architecture](https://i.imgur.com/4s6zQk8.png)
## Requirements
 - Docker desktop version 20.x and above
 - .NET SDK 6.0.102 and above
 - Visual Studio Version 17.2 (2022)
 - Surfconext client secret
 - .ASP.NET Core Runtime 6.0
 - NodeJS 16.x.x

## Setup
### Ensure the required dependencies are installed. 
1. Ensure .NET SDK 6.0.102 or above has been installed. 
Open a new terminal and run `dotnet --version`. If you don't see the `.NET SDK 6.0.102` please install it.

2. Ensure ASP.NET Core Runtime is installed 
Run `dotnet --list-runtimes` in the terminal, and ensure that `Microsoft.AspNetCore.App 6.0.x` has been installed.

3. Ensure Docker version 20 or above is installed .
In a terminal run `docker --version` and validate the version.
4. Obtain a client secret in the Surfconext environment.
5. Ensure NodeJS version 16 or above is installed.
In a terminal run `node --version` and validate the version

### Run the required dependencies.
Open a terminal in the root of this repository. In that terminal run `docker compose up -d`. This will launch the following services: 
- Azurite (Azure Blob Storage)
- KeyCloak
- RabbitMQ
- MSSQL Server

### Add your SurfConext secret to KeyCloak
1. Open your desired web browser and navigate to `localhost:8080/auth/`
2. On the left side click on `Identity-Providers`
3. Click on SurfConext
4. Scroll down to `Client Secret` and enter in your SurfConext client secret.
5. Press save at the bottom.

### Run the frontend
1. Create a new terminal from the root of the repository.
2. Navigate to `ACI.Frontend`
3. Run `npm install`.
4. Run `npm run start`.

### Run the microservices
1. Open the solution file in your desired IDE. For this example were using Visual Studio.

2. Restore the the nuget packages in the solution by running `dotnet restore` in the terminal.

---
#### Run the migration scripts
##### Method 1
1. Open the package manager.

2. In the package manager console set `Default Project` to `ACI.Images` and run `Update-Database`.

3. Change the `Default Project` to `ACI.Products` and run `Update-Database`.

4. Change the `Default Project` to `ACI.Reservations` and run `Update-Database`.

##### Method 2
1. In a new terminal run `dotnet ef --version` to ensure dotnet ef CLI tools are installed. 
If they're not installed run `dotnet tool install --global dotnet-ef` and update it after with `dotnet tool update --global dotnet-ef`. 
2. Inside a terminal navigate to `ACI.Images` and run `dotnet ef update-database`.
4. Inside a terminal navigate to `ACI.Products` and run `dotnet ef update-database`.
5. Inside a terminal navigate to `ACI.Reservations` and run `dotnet ef update-database`.

---
3. Once everything has been set up, run the following projects: `ACI.Images`, `ACI.Products`, `ACI.Reservations`, `ACI.Gateway` .
4. Once you navigate to `localhost:3000`. And click on `SurfConext`.
5. Navigate to `SurfConext test IDP` and login with username: `staff1` and password `staff1`.
6. You are now logged in.

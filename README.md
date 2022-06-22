# Introduction 
This repository contains all the microservices of the ACI Rental project. All microservices have their own project and Dockerfile. 

[![Build the ImageService](https://github.com/ACI-Rental/backend/actions/workflows/build-image-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/build-image-service.yml)

[![Release new ImageService to Docker hub](https://github.com/ACI-Rental/backend/actions/workflows/release-image-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/release-image-service.yml)

[![Build the OcelotAPIGateway](https://github.com/ACI-Rental/backend/actions/workflows/build-api-gateway.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/build-api-gateway.yml)

[![Release new OcelotAPIGateway to Docker hub](https://github.com/ACI-Rental/backend/actions/workflows/release-api-gateway.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/release-api-gateway.yml)

[![Build the ProductService](https://github.com/ACI-Rental/backend/actions/workflows/build-product-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/build-product-service.yml)

[![Release new ProductService to Docker hub](https://github.com/ACI-Rental/backend/actions/workflows/release-product-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/release-product-service.yml)

[![Build the ReservationService](https://github.com/ACI-Rental/backend/actions/workflows/build-reservation-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/build-reservation-service.yml)

[![Release new ReservationService to Docker hub](https://github.com/ACI-Rental/backend/actions/workflows/release-reservation-service.yml/badge.svg)](https://github.com/ACI-Rental/backend/actions/workflows/release-reservation-service.yml)

# Get started
This section will guide you into running the project in a local environment. 
![Software Architecture](https://i.imgur.com/4s6zQk8.png)
## Requirements
 - Docker desktop version 20.x and above
 - .NET SDK 6.0.102 and above
 - Visual Studio Version 17.2 (2022)
 - Surfconext client secret
 - .ASP.NET Core Runtime 6.0

## Setup
### Ensure the required dependencies are installed. 
1. Ensure .NET SDK 6.0.102 or above has been installed. 
Open a new terminal and run `dotnet --version`. If you don't see the `.NET SDK 6.0.102` please install it.

2. Ensure ASP.NET Core Runtime is installed 
Run `dotnet --list-runtimes` in the terminal, and ensure that `Microsoft.AspNetCore.App 6.0.` has been installed.

3. Ensure Docker version 20 or above is installed .
In a terminal run `docker --version` and validate the version.
4. Obtain a client secret in the Surfconext environment.

### Run the required dependencies.
Open a terminal in the root of this repository. In that terminal run `docker compose up -d`. This will launch the following services: 
- Azurite (Azure Blob Storage)
- KeyCloak
- RabbitMQ
- MSSQL Server

### Run the project
1. Open the solution file in your desired IDE. For this example were using Visual Studio.

2. Restore the the nuget packages in the solution by running `dotnet restore` in the terminal.

---
#### Run the migration scripts
1. Open the package manager.

2. Navigate to `ACI.Images` and run `Update-Database`

3. Navigate to `ACI.Products` and run `Update-Database`

4. Navigate to `ACI.Reservations` and run `Update-Database`

---
3. Once everything has been set up run all the following projects: `ACI.Images`, `ACI.Products`, `ACI.Reservations`, `OcelotAPIGateWay` 

The gateway will be available at `https://localhost:5001`

# Contributing
Please refer to the [CONTRIBUTING.md](https://github.com/ACI-Rental/docs/blob/main/CONTRIBUTING.md) in the [docs repository](https://github.com/ACI-Rental/docs) for more information

# PROJECT
* Project Name: <br/>
Parkla
<br>

* Project Team: <br/>
Fadıl Şahin
<br>

* Project Start Date: <br/>
February, 2022
<br>

* Project State And Duration: <br/>
Finished, 4 Months

* Project Description: <br/>
This repository is a completed graduation project in a project management aspect with a limited time and cost, defined scope and targeted quality about parking lot management and monitoring in real time. This software is designed to show all parking lots on a map with a real time status information, get data from any parking lot that support same protocols with the web application and collector service (easily extensible if any protocol is needed and status message format is customizable using a plugin code file named 'Handler'), show exact parking lot status on a 2D image model like blueprint of the building with multiple areas have a lot of parking space inside using that data, show status of all parking spaces and make them reservable for users (drivers, passengers, ...) using a reprensentetive payment page and user wallet, make parking lots managable by a parking lot manager like adding, deleting, updating parks, park areas, parking spaces, binding real parkig spaces of their parks to parking space models that user can see in the screen and reserve. Details, explanations, how to setup and execute the application, images of working application etc. are below.
<br/>
<br/>
Used Technologies, Protocols, Libraries, Concepts etc: <br/>
ASP.NET Core 6, Angular 13, PrimeNG, HTTP, GRPC, SERIALCOM(RS-232), SignalR, websocket, PostgreSQL 13, Entityframework, optimistic concurrency, asynchronous programming, configuration management, authentication, BCrypt password hashing salting peppering, JWT bearer tokens, sliding expiration refresh token, automapper, N-Tier architecture, Plugin architecture, MVC, Razor, SMTP, middleware, expression tree, fluent validation, tomtom web sdk, d3-zoom, sass, ngrx, interceptor, guard 

<br/>

- [PROJECT](#project)
- [SETUP AND RUN](#setup-and-run)
  - [Web Server and Client](#web-server-and-client)
  - [Collector Service](#collector-service)
  - [Park Simulator](#park-simulator)
- [FINAL PRODUCT AND FEATURES](#final-product-and-features)
  - [Client and Functionalities](#client-and-functionalities)
    - [Registeration and Authentication](#registeration-and-authentication)
    - [User and Manager Mode](#user-and-manager-mode)
    - [TomTom Maps and Car Parks](#tomtom-maps-and-car-parks)
    - [Parking Lots](#parking-lots)
    - [Parking Areas](#parking-areas)
    - [Parking Spaces and Reservation Management](#parking-spaces-and-reservation-management)
    - [Parking Lot Simulation](#parking-lot-simulation)
    - [Manager Dashboard](#manager-dashboard)
    - [QR Code Access](#qr-code-access)
  - [Web Server](#web-server)
  - [Collector Service](#collector-service-1)
    - [Receiver Config](#receiver-config)
    - [Exporter Config](#exporter-config)
    - [Custom Handler](#custom-handler)
- [LITERATURE RESEARCH AND ANALYSIS](#literature-research-and-analysis)
  - [Parking Problems](#parking-problems)
  - [Parking Lot Management](#parking-lot-management)
  - [Car Parking Systems](#car-parking-systems)
    - [Multilevel Car Parking System](#multilevel-car-parking-system)
    - [Full or Pratially Automated Car Parking System](#full-or-pratially-automated-car-parking-system)
    - [Smart Car Parking Systems](#smart-car-parking-systems)
      - [Variable Message Sign (VMS) System](#variable-message-sign-vms-system)
      - [Wireless Sensor Based System](#wireless-sensor-based-system)
      - [RFID Based System](#rfid-based-system)
      - [QR Code Based System](#qr-code-based-system)
      - [Image Processing Based System](#image-processing-based-system)
      - [E-Park System](#e-park-system)

# SETUP AND RUN
1. [Install](https://www.postgresql.org/download/) PostgreSQL database (v13 used)
2. [Install](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) .Net core 6 sdk
3. [Install](https://nodejs.org/en/download/) NODE and NPM (v16.13.2 used)
   
## Web Server and Client

Make a secret.json file inside [src/Parkla.web](src/Parkla.Web/) like [Example Secret Settings](src/Parkla.Web/secret-example.json). An outlook email and password is necessary to make the app working without error and not being crashed. Other ones are used for password hashing and peppering.

Set TomTom Map api key in [private.ts](src/Parkla.Web/ClientApp/src/app/core/constants/private.ts) which you will get after directions in https://developer.tomtom.com/how-to-get-tomtom-api-key. For your sake, the key already exists as freemium in the file.

Initiate a PostgreSQL v13 database named 'parkla' and a user with username 'postgres' and password '123' has all privileges like ddl and dcl below. Import [parkla-backup](parkla-backup) to the database (If you want an empty database, migrations at [src/Parkla.DataAccess/Migrations/](src/Parkla.DataAccess/Migrations/) can be used with [ef cli tool](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) using `'dotnet ef database update'` command after changing directory to [src/Parkla.DataAccess](src/Parkla.DataAccess/)).

```sql
CREATE USER parklauser WITH ENCRYPTED PASSWORD '123'
GRANT all privileges ON DATABASE parkla TO parklauser
GRANT all privileges ON ALL TABLES IN SCHEMA public to parklauser
```

Set "Parkla-admin" connection string in [appsettings.json](src/Parkla.Web/appsettings.json) according to postgresql server host, port, database name and new user's username and password. (By default it is compatible previous steps). Also set serialPortName config to receive park status data from that serial port.

Build web server using cli command `"dotnet build"` in [src/Parkla.Web/](src/Parkla.Web/) directory then run with `"dotnet run"`. When SPA proxy is ready, start a client with opening https://localhost:7070/ on a browser. It will open an angular development server and redirect to angular SPA client automatically.

For dotnet development certification problems https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs.

## Collector Service

Configure the service's [appsettings.json](src/Parkla.CollectorService/appsettings.json) file sections that are pluginLibrary to add custom handlers and pipelines has receivers, handlers and exporters (By default it is configured). Detailed informations are in [Collector Service Chapter](#collector-service).

Build collector service using cli command `"dotnet build"` in [src/Parkla.CollectorService](src/Parkla.CollectorService/) directory then run with `"dotnet run"`. Collector service has no ui.

## Park Simulator

Configure the simulator's [appsettings.json](src/Parkla.ParkSimulator/appsettings.json) file RealSpace section has realspace id and names according to web application parking space models.

Build simulator using cli command `"dotnet build"` in [src/Parkla.ParkSimulator](src/Parkla.ParkSimulator/) directory then run with "dotnet run". Open simulator ui on a browser with url https://localhost:7072.

# FINAL PRODUCT AND FEATURES

![Architecture](img/architecture.png)

Designed system consists of different components which are parking lots, collector service, web server, database, clients. Parking lots are not modelled as a software component and referred to the real ones. Parking lots produce real parking space status information (occupied, empty or unknown) and transfer these data to the web server directly or via collector service. These data are produced independent to the software system and involved by software system after received the data. So, the software system has a kind of ETL (Extract Transform Load) part to get data, convert it to acceptable vendor agnostic model and save to the database. At this point, parking lot executives provide the necessary infrastructure, sensors, and internal network. A lot of parking lots already has this kind of structure. For example, a parking lot can produce variable format of parking space data with Parksonar-Ez, Bosch PLS or other type of sensors. All this data can be transfered by internal network uses different kind of protocols like modbus and LoRaWAN to the network components. Some kind of network components like LoRa ethernet converter, RS485, embedded or high level softwares can produce http message can send the data to the designed software system in a supported or customized data format using supported protocols like HTTP, GRPC, SERIALCOM. When web server gets the parking space status data, It is possible to use this data and making real time car park, parking area, parking space modellings, reservations, generating analytical data etc. Software system is designed as easily extensible in terms of protocols. This means the system can be integrated with a lot of parking lots in time.

## Client and Functionalities

Clients (drivers, parking lot managers, passangers etc.) communicate with web server is built using Angular and runs as a SPA on browser. A client can fetch application data using REST API, realtime data by subscribing appropriate SignalR hubs until connection lost or unsubscription. With all these data, provides graphical user interface in web pages for car parks on TomTom Map which are shown as a pinned boxes has park name, location, total occupied, empty, reserved, occupied parking space count inside and shown in a modal dialog contains other data like min average and max pricing information opened after clicking the boxes, parking areas has same information of car parks in a parking area scope and other park area data, parking spaces in a real park area building structure image modelled with rectangles colored with green, red, orange, gray and transition colors close to these according to status and reservation information, dashboard has graphics and tables about analytical data.

### Registeration and Authentication

### User and Manager Mode

### TomTom Maps and Car Parks

### Parking Lots

### Parking Areas

### Parking Spaces and Reservation Management

### Parking Lot Simulation

### Manager Dashboard

### QR Code Access

## Web Server

Web server is a bridge between clients, realtime parking space status data and persisted storage data in the database. Server is a backend built on N-Layered architecture. It has a web API layer to communicate with collector service, clients and external system produces parking space status data.The API has seperate receivers for each protocol to receive supported protocols, SignalR hubs perform realtime data flow, REST API to access database storage. This web api layer has a dependency to the business layer contains internal logic, validations, data layer calls. Business layer has a dependency to the data layer contains ORM framework spesific implementations, database query and access functionalities, database table mapping configurations and migrations. Core layer communicates with all other layers and contains orm entity definitions, constants and other type of cross cutting concerns. Clients can authenticate, register, perform CRUD operations for car parks, parking areas, parking spaces, reservations.

## Collector Service

Collector service communicates with supported protocols. It is designed as a seperate executable application and capable to receive data come from car parks using receivers, reformat the data using handlers and transfer them using exporters. Collector service can be run on existing machines, servers, devices in parking lot building, buildings close that or other locations. Mulitple service can be configured to communicate together. In this way, very flexible(customizable format data, protocol independent), efficient(prevents transforming unnecessary data) and distributed communication structure can be set up. It is also possible not to use a collector service and transfer the correctly formatted data to the server. The difference using collector service is that the service can listen on multiple endpoints, groups, ports using multiple protocols with the capability of receiving custom formatted data but the server can listen on a single constant endpoint with single format. Custom format data support provides an advantage of increasing the integrated car park count to the software system by not forcing them to provide exact data and change their infrastructure. All data flow can be defined as pipelines. Each pipeline has a collection of receiver handler pair and exporters. Data comes to receivers, receivers give the data to its handlers, handlers deserialize the data in a format and load it to the memory as a park space status class instance and pass the data to the all exporters in the pipeline, exporters serialize the status data instance and send to other applications' protocol dependent endpoints in JSON format. Handlers also read data as JSON by default because custom format data can be received once after they are produced by a parking lot. After that the data will be vendor agnostic and simplified. Each pipeline flow is carried asynchronously not to block other flows. To make a custom handler, [src/Parkla.CollectorService.Library](src//Parkla.CollectorService.Library/) plugin project can be used. In this project, a new handler can be coded by using [HandlerBase](src/Parkla.CollectorService.Library/Bases/HandlerBase.cs) class, [ParamBase](src/Parkla.CollectorService.Library/Bases/ParamBase.cs) class, [ReceiverType](src/Parkla.CollectorService.Library/Enums/ReceiverType.cs) enum and [ExporterType](src/Parkla.CollectorService.Library/Enums/ExporterType.cs) enum declarations. After coding and building the project, generated dll file path has to be referred by pluginLibrary section in collector service configuration file. Example configuration of pipelines and plugin library is in [appsettings.json](src/Parkla.CollectorService/appsettings.json) file

### Receiver Config

### Exporter Config

### Custom Handler

# LITERATURE RESEARCH AND ANALYSIS

## Parking Problems

## Parking Lot Management

## Car Parking Systems

### Multilevel Car Parking System

### Full or Pratially Automated Car Parking System

### Smart Car Parking Systems

#### Variable Message Sign (VMS) System

#### Wireless Sensor Based System

#### RFID Based System

#### QR Code Based System

#### Image Processing Based System

#### E-Park System
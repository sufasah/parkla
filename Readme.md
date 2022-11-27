### Project Information
--- 
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
This repository is a completed graduation project in a project management aspect with a limited time and cost, defined scope and targeted quality about parking lot management and monitoring in real time. This software is designed to show all parking lots on a map with a real time status information, get data from any parking lot that support same protocols with the web application and collector service (easily extensible if any protocol is needed and status message format is customizable using a plugin code file named 'Handler'), show exact parking lot status on a 2D image model like blueprint of the building with multiple areas have a lot of park space inside using that data, show status of all park spaces and make them reservable for users (drivers, passengers, ...) using a reprensentetive payment page and user wallet, make parking lots managable by a parking lot manager like adding, deleting, updating parks, park areas, park spaces, binding real park spaces of their parks to park space models that user can see in the screen and reserve. Details, explanations, how to setup and execute the application, images of working application etc. are below.

* Used Technologies, Protocols, Libraries, Concepts etc: <br/>
ASP.NET Core 6, Angular 13, PrimeNG, HTTP, GRPC, SERIALCOM(RS-232), SignalR, websocket, PostgreSQL 13, Entityframework, optimistic concurrency, asynchronous programming, configuration management, authentication, BCrypt password hashing salting peppering, JWT bearer tokens, sliding expiration refresh token, automapper, N-Tier architecture, Plugin architecture, MVC, Razor, SMTP, middleware, expression tree, fluent validation, tomtom web sdk, d3-zoom, sass, ngrx, interceptor, guard 

<br/>

### 
Make a secret.json file inside [src/Parkla.web](src/Parkla.Web/) like [Example Secret Settings](src/Parkla.Web/secret-example.json). An outlook email and password is necessary to make the app working without error and not being crashed. Other ones are used for password hashing and peppering.

Set TomTom Map api key in [private.ts](src/Parkla.Web/ClientApp/src/app/core/constants/private.ts) which you will get after directions in https://developer.tomtom.com/how-to-get-tomtom-api-key. For your sake, the key already exists as freemium in the file.

Initiate a PostgreSQL v13 database named 'parkla' and a user with username 'postgres' and password '123' has all privileges like ddl and dcl below. Import [parkla-backup](parkla-backup) to the database (If you want an empty databse, migrations at [src/Parkla.DataAccess/Migrations/](src/Parkla.DataAccess/Migrations/) can be used with ef cli tool using `'dotnet ef database update'` command after changing directory to [src/Parkla.DataAccess](src/Parkla.DataAccess/)).

```sql
CREATE USER parklauser WITH ENCRYPTED PASSWORD '123'
GRANT all privileges ON DATABASE parkla TO parklauser
GRANT all privileges ON ALL TABLES IN SCHEMA public to parklauser
```
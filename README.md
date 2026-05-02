# tiny-url-app

# Angular
A modern, fast, and responsive URL shortening application built with Angular 18. This tool allows users to transform long, clunky URLs into clean, 6-character short links, featuring real-time click analytics and a sleek, scrollable management dashboard.🚀 Key FeaturesInstant Generation: Create short links with a single click using a custom Azure-hosted backend.Privacy Control: Toggle IsPrivate mode to manage link visibility.Success Feedback: Immediate visual feedback with a dedicated success box and "One-Click Copy" functionality.Live Analytics: Real-time tracking of totalClicks for every public link created.Smart Search: Instantly filter through your link history using the lightning-fast Signal-based search bar.Modern UI/UX: Features a custom-styled, pill-shaped scrollbar, linear gradient buttons, and a mobile-responsive layout.🛠️ Technical StackFrontend: Angular 18 (Standalone Components, Signals, @for/if control flow)Styling: CSS3 (Custom scrollbars, Flexbox layout, Responsive design)Backend API: Azure App Service (RESTful API)State Management: Angular Signals for reactive UI updates.🔄 Application Data FlowUnderstanding how data travels through the app:Creation: The UI collects a long URL and sends a POST request with a PascalCase payload to the /api/add endpoint.Storage: The Azure backend generates a unique code and stores the link in a database.Retrieval: On load, the app sends a GET request to /api/public.Mapping: The frontend maps the API's camelCase response (shortURL, originalURL, totalClicks) to the UI cards.Reactivity: Angular Signals detect the data change and automatically redraw the scrollable list without a page refresh.💻 Getting StartedPrerequisitesNode.js (v18 or higher)Angular CLI (npm install -g @angular/cli)InstallationClone the repository:Bashgit clonehttps://github.com/Narahari-Nallagatla/tiny-url-app.git

cd tiny-url-angular
Install dependencies:Bashnpm install
Run the Development Server:Bashng serve
Access the app:Open your browser to http://localhost:4200/.📂 Project StructurePlaintextsrc/app/
├── components/
│   └── dashboard/        # Analytics & management view
├── models/
│   └── tiny-url.model.ts # Interface definitions for API sync
├── services/
│   └── url.service.ts    # HTTP logic for Azure API communication
├── app.ts                # Main logic & Signal management
├── app.html              # Clean, modern template structure
└── app.css               # Global styles & custom scrollbar UI
📝 API ReferenceMethodEndpointDescriptionGET/api/publicFetches all public shortened URLs.POST/api/addCreates a new short URL.DELETE/api/delete/{code}Removes a URL from the database.

# Dotnet Core

## Tiny URL Backend - Part 2This project is a high-performance URL Shortener API built using .NET 8/9 Minimal APIs. It serves as the backend for the Tiny URL application, handling data persistence, unique code generation, and redirection logic.### 🛠 Features & FunctionalitiesUnique 6-Character Generation: Implements a custom algorithm to generate collision-resistant alphanumeric codes.SQLite Persistence: Uses Entity Framework Core to store URL mappings permanently in a local database.Dynamic Configuration: Base domains and Log file paths are managed through appsettings.json for easy environment switching.Structured Logging: Integrated with Serilog to log method calls, incoming parameters, and generated results to both the Console and a physical file.Global Error Handling: Middleware to catch unhandled exceptions, ensuring the API returns graceful JSON error messages instead of crashing.Swagger/OpenAPI: Automatic documentation for testing all API endpoints.### 🚀 Technologies UsedFramework: ASP.NET Core (Minimal APIs)Database: SQLiteORM: Entity Framework CoreLogging: Serilog (with Console and File sinks)Documentation: Swagger / Swashbuckle### 📂 Project StructureProgram.cs: The main entry point containing the API routes, middleware, and services.appsettings.json: Configuration file for the database connection, base domain, and logging paths.tinyurl.db: The SQLite database file (generated automatically on first run).Logs/: Directory containing daily rolling log files.### 🔌 API EndpointsMethodEndpointDescriptionPOST/api/addCreates a 6-character code and returns the full Short URL.GET/api/publicReturns a list of all URLs where isPrivate is false.GET/{code}Increments the click count and redirects to the original URL.DELETE/api/delete/{code}Removes a specific URL mapping from the database.### 🔧 Setup & InstallationNuget Packages: Ensure the following packages are installed:Microsoft.EntityFrameworkCore.SqliteSerilog.AspNetCoreSerilog.Sinks.FileSerilog.Sinks.ConsoleConfiguration: Update appsettings.json:JSON{
  "AppSettings": {
    "BaseDomain": "https://localhost:7206"
  },
  "Serilog": {
    "LogPath": "Logs/tinyurl_log.txt"
  }
}
Run the App: * Press F5 in Visual Studio.The database will be initialized automatically.Check the Console or the Logs folder to see the activity.### 📊 Logic Flow & RedirectionRequest: User sends a long URL from the Angular Frontend.Processing: Backend logs the parameters, generates a 6-character code, and saves the object to SQLite.Result: The Backend logs the generated shortURL and returns it to the user.Redirect: When the shortURL is accessed, the Backend looks up the code, increments the counter, and sends a 302 Redirect to the browser.### 🛡 Error HandlingThe application uses a Global Exception Middleware. If a database error or a Unique Constraint failure occurs, the log will capture a [CRITICAL ERROR] with a timestamp, and the client will receive a structured JSON response:JSON{
  "message": "An internal server error occurred.",
  "details": "Error message content..."
}

## Project Folder Structure
Plaintext
TinyUrlBackend/
├── .vs/                       # (Hidden) Visual Studio configuration files
├── bin/                       # Compiled output files
├── obj/                       # Temporary build files
├── Connected Services/        # (Optional) Generated by VS
│
├── Data/                      # Database Configuration
│   └── AppDbContext.cs        # (If you moved it to a separate file)
│
├── Logs/                      # Dynamic Logging Folder (Created automatically)
│   └── tinyurl_log_20260502.txt 
│
├── Models/                    # Data Transfer Objects & Entities
│   └── TinyUrl.cs             # (If you moved it to a separate file)
│
├── Properties/                # Project environment settings
│   └── launchSettings.json    # Defines ports like 7206
│
├── Services/                  # Business Logic
│   └── ShortenerService.cs    # (If you moved it to a separate file)
│
├── appsettings.json           # Your dynamic configuration (LogPath, BaseDomain)
├── appsettings.Development.json
├── Program.cs                 # THE CORE FILE: Routes, Middleware, & Setup
├── TinyUrlBackend.csproj      # The project file (contains NuGet references)
└── tinyurl.db                 # The SQLite Database file
## Important Notes on this Structure
The Logs/ Folder: You don't need to create this manually. Because we used Serilog, the application will create the Logs folder the first time it writes a log entry.

The tinyurl.db File: This file appears in your Project Root (the same folder as Program.cs). If you ever need to start over, just delete this file and the app will recreate it.

## Submission Checklist[x] tinyurl.db initialization logic verified.[x] Logging of method parameters and results verified.[x] CORS enabled for Angular Frontend communication.[x] Redirect logic tested and click counter functional.
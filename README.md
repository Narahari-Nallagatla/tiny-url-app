# tiny-url-app

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
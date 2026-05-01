import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app';
import { config } from './app/app.config.server'; 

// This function must pass 'config' to bootstrapApplication
const bootstrap = () => bootstrapApplication(AppComponent, config);

export default bootstrap;
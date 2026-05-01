import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app'; // This looks for 'AppComponent' inside 'app.ts'

bootstrapApplication(AppComponent, appConfig).catch(err => console.error(err));
import { mergeApplicationConfig, ApplicationConfig } from '@angular/core';
import { provideServerRendering } from '@angular/platform-server';
import { appConfig } from './app.config';

const serverConfig: ApplicationConfig = {
  providers: [
    provideServerRendering() // This is the "Platform" the error says is missing
  ]
};

export const config = mergeApplicationConfig(appConfig, serverConfig);
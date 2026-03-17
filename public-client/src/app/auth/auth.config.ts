import { KeycloakConfig } from 'keycloak-js';

export const keycloakConfig: KeycloakConfig = {
	url: 'http://localhost:8080', // Keycloak URL
	realm: 'angular-days',              // Your realm name
	clientId: 'demo' // Your client ID
};

import { KeycloakConfig } from 'keycloak-js';

export const keycloakConfig: KeycloakConfig = {
	url: 'http://localhost:8080', // Keycloak URL
	realm: 'master',              // Your realm name
	clientId: 'angular-days-demo' // Your client ID
};

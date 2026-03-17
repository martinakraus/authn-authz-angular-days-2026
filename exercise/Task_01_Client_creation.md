# Task 01 – Configure your Keycloak client and integrate it into your frontend application

## Goal

Register a new OpenID Connect client in Keycloak that represents your Frontend application. You will configure it to use only the **Authorization Code Flow** — the only flow that is appropriate for browser-based SPAs.

---

## Prerequisites

- Keycloak is running locally via Docker Compose on **http://localhost:8080**
- You have access to the **Admin UI** with the credentials `admin` / `admin`
- Your Frontend is running locally via Docker Compose on **http://localhost:4000**

---

## Configure your Keycloak client

### 1. Open the Keycloak Admin UI

Navigate to **http://localhost:8080** in your browser and log in with:

| Field    | Value   |
|----------|---------|
| Username | `admin` |
| Password | `admin` |

After login you will land in the **master** realm.

---

### 2. Create a new Realm

A **Realm** in Keycloak is an isolated space for managing users, roles, clients, and authentication settings. Each Realm is completely independent — users and configurations in one Realm are not visible to another. Think of it as a separate tenant or security domain. The **master** Realm is reserved for Keycloak administration, so you should create a dedicated Realm for your application.

1. In the top-left corner, click on the dropdown that says **master**.
2. Click **Create realm**.
3. Enter the following:

| Field      | Value           |
|------------|-----------------|
| Realm name | `angular-days`  |

4. Click **Create**.

Make sure the new **angular-days** Realm is selected (visible in the top-left dropdown) before continuing.

---

### 3. Navigate to Clients

In the left-hand sidebar, click on **Clients**.

You will see a list of built-in Keycloak clients. You will add a new one.

Click the **Create client** button (top right of the client list).

---

### 4. General Settings

Fill in the first step of the wizard:

| Field          | Value               |
|----------------|---------------------|
| Client type    | `OpenID Connect`    |
| Client ID      | `angular-days-demo` |
| Name           | *(optional)*        |
| Description    | *(optional)*        |

Click **Next**.

---

### 5. Capability Config — Enable ONLY Authorization Code Flow

This is the most important step. You need to ensure that **only the Authorization Code Flow** is active.

> **Why?** For SPAs (Single Page Applications), the Authorization Code Flow with PKCE is the only recommended flow according to the current OAuth 2.0 Security Best Current Practice (BCP). Implicit Flow is deprecated, and Direct Access Grants bypass the browser redirect entirely — both are unsuitable for Angular apps running in the browser.

Click **Next**.

---

### 6. Login Settings — Redirect URI & Web Origins

Now configure where Keycloak is allowed to redirect after a successful login, and which origins may communicate with Keycloak.

> **Think about it:** Your Angular app runs locally during development. What is the used port? What is the full URL your browser hits when you open your app?

Fill in:

| Field                  | Value                        |
|------------------------|------------------------------|
| Root URL               | *(leave empty)*              |
| Home URL               | *(leave empty)*              |
| Valid redirect URIs    | `http://localhost:4000/*`    |
| Valid post logout redirect URIs | `http://localhost:4000/*` |
| Web origins            | `http://localhost:4000`      |

**Explanation:**

- **Valid redirect URIs** — After a successful login, Keycloak will redirect the browser back to your app. The `/*` wildcard allows any path under your app's origin. Keycloak will reject any redirect to a URI not listed here, which protects against open redirect attacks.
- **Valid post logout redirect URIs** — Same principle, but for after logout.
- **Web origins** — Controls the CORS `Access-Control-Allow-Origin` header returned by Keycloak. If your app's origin is not listed here, the browser will block responses from Keycloak. Set this to exactly your app's origin (no trailing slash, no wildcard path).

> **Security note:** In production, replace `localhost:4000` with your actual deployed application URL. Never use `*` as a wildcard for Web Origins in production.

Click **Save**.

Your Keycloak client is now ready to be used in an Angular application. In the next task you will connect an Angular app to this client using the Authorization Code Flow with PKCE.


## Integrate Keycloak into your frontend application

To connect your Angular frontend app to Keycloak, you only need to provide the correct configuration. All other integration code is already present.

### 1. Open your Keycloak configuration file

Open the file:

		src/app/auth/auth.config.ts

### 2. Enter your Keycloak details

Fill in the following values based on your Keycloak setup:

- **url**: The base URL of your Keycloak server (e.g., `http://localhost:8080`)
- **realm**: The realm you are using (e.g., `angular-days`)
- **clientId**: The client ID you created (e.g., `angular-days-demo`)

Example:

```typescript
import { KeycloakConfig } from 'keycloak-js';

export const keycloakConfig: KeycloakConfig = {
	url: 'http://localhost:8080', // Keycloak URL
	realm: 'angular-days',              // Your realm name
	clientId: 'angular-days-demo' // Your client ID
};
```

**Replace the values** with those from your Keycloak instance if they differ.

### 3. Save and restart your frontend

- Save the file.
- Rebuild and restart your frontend app (e.g., with `docker compose up -d --build frontend` or your local dev command).

---

Your Angular app will now use Keycloak for authentication. When you access a protected route or click the login button, you will be redirected to Keycloak for login.

---

> **Note:**
>
> You can always log in to your app with the default Keycloak admin user (`admin` / `admin`) if you have not created any other users yet. However, for a real user experience, you may want to create a regular user account in Keycloak:
>
> **How to create a user in Keycloak:**
> 1. In the Keycloak Admin UI, go to **Users** in the left sidebar.
> 2. Click **Add user** (top right).
> 3. Fill in the required fields (e.g., username, email) and click **Create**.
> 4. Go to the **Credentials** tab for the new user, set a password, and disable **Temporary** if you want the password to be permanent.
> 5. Save the password.
> 6. You can now log in with this user in your frontend app.



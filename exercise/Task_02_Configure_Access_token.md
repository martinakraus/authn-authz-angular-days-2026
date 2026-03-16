# Task 02 – Inspect and Configure Tokens in Keycloak

## Goal

Learn what information is encoded inside a JWT Access Token, and configure a meaningful token lifetime and Refresh Token Rotation in Keycloak.

---

## Prerequisites

- Task 01 is completed — you have a working Keycloak client and your Angular app is connected to Keycloak.
- You are logged in to your Angular app at **http://localhost:4000**.
- You have access to the **Keycloak Admin UI** at **http://localhost:8080** (`admin` / `admin`).

---

## Part 1 – Inspect Your Access Token

### 1. Retrieve your current Access Token

Open your browser's **Developer Tools** (F12) and navigate to the **Network** tab.

Look for the POST request to Keycloak which returns the Access Token. 
Alternatively, you can print the token by adding a temporary log in your Angular app:

```typescript
import Keycloak from 'keycloak-js';
import { inject } from '@angular/core';

const keycloak = inject(Keycloak);
console.log(keycloak.token); // the raw JWT Access Token
```

Copy the full token string (it looks like `eyJ...`).

---

### 2. Decode the Token

Paste the token into one of these tools:

- **[https://jwt.ms](https://jwt.ms)** *(recommended — Microsoft, easy to read)*
- **[https://jwt.io](https://jwt.io)** *(classic, shows header/payload/signature separately)*

> **Note:** These tools decode the token client-side only — your token is never sent to a server. Still, avoid doing this with tokens from production systems.

---

### 3. Explore the Token Claims

Once decoded, examine the **payload**. You should find claims like:

| Claim | Description |
|-------|-------------|
| `sub` | The subject — the unique user ID in Keycloak |
| `iss` | The issuer — the URL of your Keycloak realm |
| `aud` | The audience this token is intended for |
| `exp` | Expiry timestamp (Unix epoch) |
| `iat` | Issued-at timestamp |
| `realm_access.roles` | Realm-level roles assigned to the user |
| `preferred_username` | The username |
| `email` | The user's email address |
| `email_verified` | Whether the email is verified |

> **Think about it:**
> - When does your token expire (`exp`)? Calculate how long that is from now.
> - What roles does your user have?
> - What is the value of `iss`? Does it match your Keycloak URL and realm?

---

## Part 2 – Configure Token Lifetimes in Keycloak

### 1. Open Realm Settings

In the **Keycloak Admin UI**, make sure you are in the **master** realm, then click **Realm settings** in the left sidebar.

---

### 2. Set the Access Token Lifespan

Go to the **Tokens** tab inside Realm Settings.

Find the **Access Token Lifespan** field and set it to:

| Field | Value |
|-------|-------|
| Access Token Lifespan | `15 Minutes` |

Click **Save**.

> **Why 15 minutes?**
> A short-lived access token limits the window of opportunity if a token is stolen. After 15 minutes it is useless without a valid refresh token. This is a common production-level setting.

---

### 3. Enable Refresh Token Rotation

On the same **Tokens** tab, find the **Refresh Token** section and enable:

| Setting | Value |
|---------|-------|
| Revoke Refresh Token | `On` ✅ |
| Refresh Token Max Reuse | `0` |

Click **Save**.

> **What does this do?**
> With **Refresh Token Rotation** enabled, every time your app uses a refresh token to get a new access token, Keycloak issues a **brand new refresh token** and immediately **invalidates the old one**.
> This means if an attacker steals a refresh token and tries to use it after your app has already rotated it, Keycloak will detect the reuse and reject the request — protecting the user's session.

---

### 4. Verify the new token lifetime

Log out of your Angular app and log back in. Decode the new access token at **jwt.ms** or **jwt.io** again.

Check the `exp` and `iat` claims — the difference should now be **900 seconds (= 15 minutes)**.

---

## Angular App — No Changes Needed

Your Angular app does **not** require any code changes for these token settings to take effect.

The `includeBearerTokenInterceptor` from `keycloak-angular` automatically calls `keycloak.updateToken()` before every outgoing HTTP request. This means:

- When your access token is about to expire, it is **silently refreshed** using the refresh token before the API call is made.
- When Refresh Token Rotation is active, `keycloak-js` receives the new refresh token automatically and stores it — the rotation is fully transparent.

> **Note:** You can control how early the token should be refreshed by setting `shouldUpdateToken` in your `INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG`. The default minimum validity is 30 seconds — meaning the token is refreshed if it expires within the next 30 seconds.

---

## Done!

You now understand what information a JWT Access Token contains, have set a secure token lifetime, and activated Refresh Token Rotation — all without touching your Angular code.

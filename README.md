# AuthN/AuthZ Angular Workshop – Project Setup

## Prerequisites

- **Docker** and **Docker Compose** must be installed on your machine.
  - Download: https://www.docker.com/products/docker-desktop

## Starting All Services

To build and start all services (Keycloak, API, Frontend) in the background:

```sh
docker compose up -d --build
```

- The Keycloak Admin UI will be available at: http://localhost:8080
- The frontend app will be available at: http://localhost:4000
- The API (if present) will be available at: http://localhost:3000

## Stopping All Services

To stop all running containers:

```sh
docker compose down
```

## Restarting or Rebuilding a Single Service

To restart a single service (e.g., after a config change):

```sh
docker compose restart <service-name>
```

To rebuild and restart only one service after code changes:

```sh
docker compose up -d --build <service-name>
```

Replace `<service-name>` with one of:
- `keycloak`
- `api`
- `frontend`

## Troubleshooting

- If you change code or dependencies, always use `--build` to ensure the container is rebuilt.
- If a service fails to start, check logs with:
  ```sh
  docker compose logs <service-name>
  ```
- Make sure no other process is using the required ports (8080, 4000, 3000).

---

For workshop details and exercises, see the `exercise/` folder.

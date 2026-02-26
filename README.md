# Auth Template API - V1
A production-ready **ASP.NET Core Web API** starter template built with layered architecture. Provides a clean, reusable foundation for building secure, scalable, and maintainable backend applications.

---

## Purpose

- Speed up backend development with a battle-tested starting point
- Enforce clean architecture principles across all layers
- Serve as a reusable base for new projects
- Provide a solid, production-ready security foundation out of the box

---

## Architecture

| Layer | Responsibility |
|-------|---------------|
| `DTOs` | Data transfer objects |
| `Entities` | Domain models |
| `Data` | EF Core DbContext, repository interfaces and implementations |
| `Business` | Services, token generation, business logic |
| `API` | Controllers, middleware, extensions, DI configuration |

---

## Security Layers

### Layer 1 — HTTPS + CORS Foundation
Encrypted communication via TLS ensures safe data transport. CORS policies control which origins can interact with the API. No other security layer is effective without a secure connection underneath.

### Layer 2 — JWT Authentication
Every request carries a signed JWT access token. The API validates identity on each call without maintaining server-side session state.

### Layer 3 — Role-Based Authorization
Roles (`admin`, `member`) restrict which operations a user can perform. Write and administrative operations are gated behind role checks.

### Layer 4 — Ownership Policies
Resource-level protection via a custom `UserOwnerOrAdminRequirement` policy. Users can only access or modify their own data, preventing horizontal privilege escalation. Admins can access any resource.

### Layer 5 — Refresh Tokens & Logout
Short-lived access tokens (default 30 min) paired with long-lived refresh tokens (default 7 days). Refresh tokens are hashed before storage — never stored in plaintext. Token revocation on logout invalidates the session immediately.

### Layer 6 — Rate Limiting
Fixed-window rate limiting per IP address on all auth endpoints (5 requests/minute by default). Protects against brute-force attacks and API abuse.

### Layer 7 — Logging & Auditing
Global exception handling with structured error responses. Extend this layer with Serilog or similar to track security events, audit admin actions, and detect suspicious behavior.

---

## Cryptography Reference

| Concept | Description | Reversible | Key Required | Use Cases |
|---------|-------------|------------|--------------|-----------|
| **Encryption** | Converts plaintext to ciphertext | ✅ Yes | ✅ Yes | HTTPS/TLS, file encryption, secure messages |
| **Encoding** | Converts data for transport/compatibility | ✅ Yes | ❌ No | Base64, URL encoding, UTF-8 |
| **Hashing** | Converts data to a fixed-length digest | ❌ No | ❌ No | Password storage, file integrity, digital signatures |

> Passwords and refresh tokens in this template are **hashed** using BCrypt — never encrypted or encoded.




## EF Core Commands

# Add a new migration
dotnet ef migrations add InitialCreate --project Data --startup-project API

# Apply migrations to the database
dotnet ef database update --project Data --startup-project API

# Remove the last migration
dotnet ef migrations remove --project Data --startup-project API

# Drop the entire database
dotnet ef database drop --project Data --startup-project API

---

## License
MIT — free to use as a base for any project.


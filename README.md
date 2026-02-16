# CoreApiFoundation
A production-ready ASP.NET Core Web API Starter Template built with layered architecture.
This template provides a clean and reusable foundation for building scalable and maintainable backend applications.

## Purpose
- This template is designed to:
- Speed up backend development
- Enforce clean architecture principles
- Serve as a reusable base for new projects
- Provide a solid production-ready foundation

# Built with secured Layers

## Layer 1 - HTTPS + CORS Foundation

- Encrypted communication (TLS)
- Controlled browser access
- Safe data transport

*No security works without a safe connection.*

## Layer 2 - JWT Authentication

- Identity for every request
- Stateless authentication
- No anonymous access

*Now the API knows who you are.*

## Layer 3 - Role-Based Authorization

- Admin vs Student
- Restricted write operations
- Clear authority boundaries

*Not everyone can do everything.*

## Layer 4 - Ownership Policies

- User can access only his data
- Prevents horizontal privilege escalation
- Resource-level protection

*You can access only what belongs to you.*

## Layer 5 - Refresh Tokens & Logout

- Short-lived access tokens
- Long-lived secure sessions
- Token revocation

*Security without killing user experience.*

## Layer 6 - Rate Limiting

- Brute-force protection
- Abuse prevention
- System stability

*Security under attack conditions.*

## Layer 7 - Logging & Auditing

- Track security events
- Audit admin actions
- Detect suspicious behavior

*If you can’t see it, you can’t secure it.*

---
## Encryption

- Converts readable data (plaintext) into unreadable form (ciphertext)
- Uses a key
- Reversible (you can get the original data back with the key)
- Used for confidentiality

**Example uses**

- HTTPS (TLS)
- Encrypting files at database 
- Secure messages

## Encoding

- Converts data into another format for transport or compatibility
- Uses no key
- Reversible by design
- Not security

**Example uses**

- Base64
- URL encoding
- UTF-8

## Hashing

- Converts data into a fixed-length digest
- Uses no key
- One-way (irreversible)
- Used for integrity and verification

**Example uses**

- Password storage
- File integrity checks
- Digital signatures (hash part)
  
---

## EF Core important comands
**To add a migration:** dotnet ef migrations add InitialCreate --project Data --startup-project API

**To update the database:** dotnet ef database update --project Data --startup-project API

**To Remove the Migration:** dotnet ef migrations remove --project Data --startup-project API

**To drop all database**: dotnet ef database drop --project Data --startup-project API

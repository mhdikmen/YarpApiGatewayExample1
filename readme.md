# YARP API Gateway with JWT Authentication

This project is an API Gateway built with [YARP (Yet Another Reverse Proxy)](https://github.com/microsoft/reverse-proxy), acting as a single entry point to multiple backend services, with JWT-based authentication and per-request audience forwarding.

---

## 🔧 Technologies

- [.NET 8](https://dotnet.microsoft.com/en-us/)
- [YARP](https://github.com/microsoft/reverse-proxy)
- JWT Authentication
- Reverse proxy to downstream microservices

---

## 📁 Project Structure

This gateway routes to 3 services:

| Route Path                 | Target Cluster     | Downstream Port |
|---------------------------|--------------------|-----------------|
| `/auth-service/{**}`      | `auth-cluster`     | `https://localhost:5001/` |
| `/service1/{**}`          | `service1-cluster` | `https://localhost:5002/` |
| `/service2/{**}`          | `service2-cluster` | `https://localhost:5003/` |

---

## 🔐 JWT Configuration

JWT settings are stored in `appsettings.json`:

```json
"Jwt": {
  "Issuer": "https://localhost:5001",
  "Key": "<very-long-secure-key>",
  "Audiences": [ "desktop-client" ]
}




curl --location 'https://localhost:5000/auth-service/auth/token' \
--header 'Content-Type: application/json' \
--data '{
    "Username": "user1",
    "Password": "pass"
}'


curl --location 'https://localhost:5000/service1/weatherforecast' \
--header 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyMSIsImp0aSI6Ijc1Y2VjNThmLTFjODctNDRiNy05ZjRkLTY0MjlkMTc5OWQ5ZiIsImV4cCI6MTc0ODYyMTQyMCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImF1ZCI6ImRlc2t0b3AtY2xpZW50In0.eDewGNmcJZ_mP7n0G_gQxwedyPUTTbU-AnbwplE5edQ'


curl --location 'https://localhost:5000/service2/weatherforecast'
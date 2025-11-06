# Simple Ledger API

This project implements a simple ledger system. The application provides APIs for recording money movements and retrieving account information. It follows a clean, layered architecture with clear separation of concerns.

## Features
- **Record money movements** (deposits and withdrawals)
- **View current balance** for a customer
- **View transaction history** for a customer

## Architecture
The project follows a **clean layered architecture**, consisting of:

- **Presentation Layer**  
  Contains API controllers.

- **Application Layer**  
  Contains  and request/response models and adapters logic.

- **Domain Layer**  
  Contains core domain models and entities and business logic implementation.

- **Data/Persistence Layer**  
  Uses an in-memory data store implemented as a `List<T>` based on the project requirements.

---

## API Endpoints
All endpoints are under: `api/{controller}`

### Get Current Balance
```
GET /api/{controller}/{customerId}/balance
```
Returns the current balance for the given customer.

### Get Transaction History
```
GET /api/{controller}/{customerId}/transactions
```
Returns a list of all recorded transactions for the given customer.

### Record a Transaction (Deposit or Withdrawal)
```
POST /api/{controller}/transaction
```
Body example:
```json
{
  "customerId": 1,
  "amount": 100.0,
  "type": "Deposit"
}
```
```json
{
  "customerId": 1,
  "amount": 50.0,
  "type": "Withdrawal"
}
```
---

## Data Storage
Since the requirement states no real database should be used, the persistence layer stores data via:
- An in-memory `List<T>`
- Services in the Domain layer interact with this list

---

## How to Run
1. Clone this repository
2. Open the project in Visual Studio
3. Run using IIS Express
4. Test endpoints using Swagger

---

## Notes

- Only added exception handling and logging to endpoints. Logging was not expected so I keep it simple.
- I made some decisions like Balance could not have a negative value, so I throw an exception with an appropriate message in that case.
- The project is structured so a real database could be added later with minimal changes
- Even though the project does not use an external database or communicate with external services, an asynchronous implementation was intentionally chosen. I believe that in a real-world scenario, these operations would typically involve I/O‑bound tasks such as database queries or API calls. Also, implementing the service methods as async/await ensures that transitioning to a real database later requires minimal refactoring.
- Implemented unit tests for the controller covering the happy-path and one exception scenarios. The goal is to showcase my testing approach rather than provide full test coverage for this exercise.
---



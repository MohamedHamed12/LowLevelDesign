# Rate Limiter

This project implements a rate limiter in C# with various strategies. It also includes an example e-commerce API that uses the rate limiter.

## Rate Limiting Strategies

- **Token Bucket:** Limits the rate based on a bucket of tokens that refills over time.
- **Fixed Window:** Limits the number of requests within a fixed time window.
- **Sliding Window:** Limits the number of requests within a sliding time window.

## E-Commerce API Example

The `ECommerceApp.API` project demonstrates how to use the rate limiter in a real-world scenario. It includes a `CartController` with rate-limited endpoints.

## How to Run the API

1.  Navigate to the `RateLimiter/ECommerceApp.API` directory.
2.  Build and run the project using `dotnet run`.
3.  The API will be available at `http://localhost:5000`.

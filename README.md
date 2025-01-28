# ECommerceBackend

**ECommerceBackend** is a .NET 8-based backend for an e-commerce application. It provides APIs for managing products, categories, orders, and user authentication. Additionally, it integrates with Stripe for seamless payment processing.

## Table of Contents

- [Features](#features)
- [Technologies](#technologies)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Configuration](#configuration)
  - [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
  - [Authentication](#authentication)
  - [Products](#products)
  - [Categories](#categories)
  - [Cart](#cart)
  - [Orders](#orders)
- [Contributing](#contributing)
- [License](#license)

## Features

- **User Authentication**: Secure user registration and login.
- **Role-Based Authorization**: Supports Admin and Customer roles.
- **Product Management**: Create, update, delete, and retrieve products.
- **Category Management**: Manage product categories with admin privileges.
- **Cart Management**: Add items to the cart and proceed to checkout.
- **Order Management**: Place and view orders.
- **Payment Integration**: Supports Stripe for handling payments.

## Technologies

- **.NET 8**
- **Entity Framework Core**
- **ASP.NET Core**
- **Stripe API**
- **SQL Server**

## Getting Started

### Prerequisites

Ensure the following are installed and configured:

- .NET 8 SDK
- SQL Server
- A Stripe account for payment processing

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Rahull-Adk/ECommerceBackend.git
   cd ECommerceBackend
   ```

2. Restore the required NuGet packages:
   ```bash
   dotnet restore
   ```

### Configuration

1. Update the `appsettings.json` or use `secrets.json` to configure the required settings:
   ```json
   {
     "ConnectionString": "Data Source=YOUR_SERVER;Initial Catalog=EcomerceBackend;User=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;",
     "JwtSettings": {
       "SecretKey": "YOUR_SECRET_KEY",
       "Issuer": "ECommerceBackend",
       "Audience": "ECommerceBackendUsers"
     },
     "StripeSettings": {
       "SecretKey": "YOUR_STRIPE_SECRET_KEY",
       "PublishableKey": "YOUR_STRIPE_PUBLISHABLE_KEY"
     },
     "SendGridSettings": {
       "ApiKey": "YOUR_SENDGRID_API_KEY"
     },
     "AdminCredentials": {
       "Email": "admin@example.com",
       "Password": "AdminPassword123"
     }
   }
   ```

2. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

### Running the Application

1. Start the application:
   ```bash
   dotnet run
   ```

2. Access the application at:
   - `https://localhost:5001`
   - `http://localhost:5000`

## API Endpoints

### Authentication

- `POST /api/auth/register` - Register a new user.
- `POST /api/auth/login` - Login a user.
- `POST /api/auth/register-admin` - Register a new admin user (requires admin credentials).

### Products

- `GET /api/products` - Retrieve all products.
- `GET /api/products/{id}` - Retrieve a product by ID.
- `POST /api/products` - Create a new product (Admin only).
- `PUT /api/products/{id}` - Update a product (Admin only).
- `DELETE /api/products/{id}` - Delete a product (Admin only).

### Categories

- `GET /api/categories` - Retrieve all categories.
- `GET /api/categories/{id}` - Retrieve a category by ID.
- `POST /api/categories` - Create a new category (Admin only).
- `PUT /api/categories/{id}` - Update a category (Admin only).
- `DELETE /api/categories/{id}` - Delete a category (Admin only).

### Cart

- `GET /api/cart` - Retrieve the current user's cart items.
- `POST /api/cart/add` - Add an item to the cart.
- `POST /api/cart/checkout` - Checkout the cart and create an order.

### Orders

- `POST /api/orders` - Create an order from the cart.
- `GET /api/orders` - Retrieve all orders for the current user.
- `GET /api/orders/{id}` - Retrieve an order by ID.

## Contributing

Contributions are welcome! If you have ideas for new features or find any issues, feel free to:

1. Open an issue.
2. Submit a pull request.

Make sure your contributions align with the coding standards and are well-documented.

## License

This project is licensed under the [MIT License](LICENSE).


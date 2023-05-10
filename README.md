## ASP.NET CRUD REST API 

### Description

 ASP.NET Core Web API application that allows users to perform CRUD (Create, Read, Update, Delete)
 The project has four models:
 1. Products : Used for storing product details (productId, product name, createdBy, time, description, price and availability)
 2. ProductVendors : Used for storing the relation between the vendors and their products for reference
 3. Purchase: Used for storing purchases made by customers (purchase Id, User Profile Id, product Id, and purchase cost)
 4. User Profile: Used to store the users details (User Id, password, name, email and phone)
 
The project is used for vendors to add, update and delete products and for customers to purchase these products and retrieve them whenever needed. 
The application uses JWT for password security and generates tokens that will expire every 30 days. 

The application has three types of roles: Admin, Vendor and customer
Users can do the following: 
1. Login
2. Register as either a vendor or customer
3. Change their password
4. Request for a new password using the Forgot Password Request
5. Reset Their password
6. View all available product details 
7. View a specific product details 

Vendors can do the following: 
1. Add a new product 
2. Retrieve the products they own 
3. Delete products they own 
4. Update products they own 

Customers can do the following: 
1. Purchase a product 
2. Retrieve their previously purchased products

 

### Guide to the repository 

### CI/CD & Deployment environment
The following project uses AWS Elastic beanstalk for the AWS Cloud and Github actions for deployment
### Instructions

#### Getting Started

These instructions will guide you through the process of setting up the ASP.NET Core Web API application and deploying it to AWS Elastic Beanstalk.

##### Prerequisites

- Visual Studio 2019 or later
- .NET Core SDK 3.1 or later
- Git
- AWS account

##### Installing

1. Clone the repository to your local machine using Git.
2. Open the solution file `WebApplication100.sln` in Visual Studio.
3. Build the solution to restore NuGet packages and ensure that everything is working as expected.

##### Configuration

1. Create an MS SQL Server database to store the data.
2. Update the `appsettings.json` file in the  project with the database connection string 
```
  "ConnectionStrings": {
    "Assignment": "server=<Server>;database=<database-Name>;Integrated Security=False; TrustServerCertificate=True; User ID=admin;Password=<password>"
  }
```
and the following JWT key, issuer and audience to create JWT Tokens for login and registration 

```
  "Jwt": {
    "Key": <JWT-Key>,
    "Issuer": "https://localhost:7169/",
    "Audience": "https://localhost:7169/"
  },
```
##### Running the Application 
1. When running the application, it will navigate you to the following page : "https://localhost:7169/swagger". The project uses Swagger UI to showcase the APIs and the documentation 

##### Deploying to AWS Cloud :  Elastic Beanstalk
1. You can create an Elastic Beanstalk environemnt for ASP.NET Core Web API in AWS 
2. In order to create a Github Actions workflow, we require a .yml file to set the commands.  In this project the .yml file is called "deploy.yml" and is located in  `.github/workflows/` 
3. Push the code to github to start a new GitHub Actions workflow 
4. Wait for the workflow to complete and verify the application is deployed  and running in Elastic Beanstalk 
5. The Elastic Beanstalk link for this project : http://aspassignment-env.eba-fm46p7dp.us-east-1.elasticbeanstalk.com/swagger/index.html


### Explanation of each API 
###### /AuthController
API 1: Login
HTTP method: POST
Endpoint: /api/Auth/Login
Description: This endpoint is used for users to login. 
After running the endpoint, copy the token, and paste it in the Swagger UI "Authorize" section

API 2: UserRegistration
HTTP method: POST
Endpoint: `/api/Auth/UserRegistration`
Description: This endpoint is used for users to create a new account, the roles of the users will be on default "customer"
Requires the users name, email , phone number and password.

API 3: VendorRegistration
HTTP method: POST
Endpoint: `/api/Auth/VendorRegistration`
Description: This endpoint is used for users to register as a vendor
Requires the vendor's name, email, phone number and password

API 4: User
HTTP method: Get
Endpoint: `/api/Auth/User/{id}`
Description: This endpoint is authorized only for the admin with the role "admin" to search by user ID.
The end point checks for the logged in user role from the Claims

API 5: ChangePassword
HTTP method: PUT
Endpoint: `/api/Auth/ChangePassword`
Description: This endpoint is used to change a user's password. 
It verifies that the current password provided in the is the same as the password from the database by using password and password salt.

API 6: ForgotPassword
HTTP method: POST
Endpoint: `/api/Auth/ForgotPassword`
Description: This endpoint is used to request for a token if the user has forgotten their password

API 7: ResetPassword
HTTP method: PUT
Endpoint: `/api/Auth/ResetPassword`
Description: This endpoint is used after requesting for token from the /api/Auth/ForgotPassword endpoint. 
Requires the requested token and a new password from the user.


###### /ProductsController
API 1: GetProductInformation
HTTP method: GET
Endpoint: `/api/Products/GetProductInformation/{productId}`
Description: This method is used to get product information by the product ID.

API 2: GetAllProducts
HTTP method: GET
Endpoint: `/api/Products/GetAllProducts`
Description: This endpoint is used to get all the available products in the database

API 3: AddProduct
HTTP method: POST
Endpoint: `/api/AddProduct`
Description: This endpoint is used to add a new product to the database by users with the roles of vendor and admin only.

API 4: GetVendorProducts
HTTP method: GET
Endpoint: `/api/GetVendorProducts`
Description: This endpoint is used to get all the vendors products. 
The users details is obtained using the User claims allowing for logged in vendors to retrieve all their products owned by them only.

API 5: UpdateProducts
HTTP method: PUT
Endpoint: `/api/UpdateProducts/{productId}`
Description: This endpoint is used to update a product. 
Admins are authorized to update any product. 
Vendors are authorized on update only their own products. 

API 5: DeleteProduct
HTTP method: Delete
Endpoint: `/api/DeleteProduct/{productId}`
Description: This endpoint is used to delete a product. 
Admins are authorized to delete any product. 
Vendors are authorized on delete only their own products. 

###### /PurchaseController
API 1: PurchaseProduct
HTTP method: Post
Endpoint: `/api/PurchaseProduct`
Description: This endpoint is used to purchase an available product from the database. 


API 2: GetCustomerPurchases
HTTP method: Post
Endpoint: `/api/PurchaseProduct/GetCustomerPurchases`
Description: This endpint retrieves all the customer purchases with the product details.


##### Built with the following: 
- ASP.NET CORE WEB API 
- ENTITY FRAMEWORK CORE 
- MS SQL SERVER 
- AWS ELASTIC BEANSTALK 
- GITHUB ACTIONS 

##### Author: 
Farah Shaheen



  

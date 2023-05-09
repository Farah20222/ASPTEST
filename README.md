## ASP.NET CRUD Application with REST API and CI/CD Deployment to AWS Cloud

### Description

Create a secure ASP.NET Core Web API application that allows users to perform CRUD (Create, Read, Update, Delete) operations on records in a database. Use Entity Framework for database access and ensure the application handles errors gracefully. Deploy the application using a CI/CD pipeline to AWS Cloud.

### Requirements

- Create an ASP.NET Core Web API project to handle CRUD operations for a simple entity (e.g., "Product" with properties like ID, Name, Description, and Price).
- Use Entity Framework Core for database access and create an MS SQL Server database to store the data.
- Implement user authentication and authorization to secure the application, using ASP.NET Core Identity or a token-based approach (e.g., JWT).
- Handle errors gracefully and provide meaningful error messages to the users.
- Set up a CI/CD pipeline using any popular CI/CD tool ()
- Deploy the application to AWS Cloud (e.g., using AWS Elastic Beanstalk, AWS App Runner, or Amazon ECS with Fargate).
- Commit the source code to a GitHub repository.
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
2. Open the solution file `MyApp.sln` in Visual Studio.
3. Build the solution to restore NuGet packages and ensure that everything is working as expected.

##### Configuration

1. Create an MS SQL Server database to store the data.
2. Update the `appsettings.json` file in the  project with the database connection string 
```
  "ConnectionStrings": {
    "Assignment": "server=<server-name>;database=<databaseName>;Integrated Security=False; TrustServerCertificate=True; User ID=admin;Password=<password>"
  }
```
##### Running the Application 
1. When running the application, it will navigate you to the following page : "https://localhost:7169/swagger". The project uses Swagger UI to showcase the APIs and the documentation 

##### Deploying to AWS Cloud :  Elastic Beanstalk
1. You can create an Elastic Beanstalk environemnt for ASP.NET Core Web API in AWS 
2. In order to create a Github Actions workflow, we require a .yml file to set the commands.  In this project the .yml file is called "deploy.yml" and is located in the .github/workflows/ folder
3. Push the code to github inorder to start a new GitHub Actions workflow 
4. Wait for the workflow to complete and verify the application is deployed  and running in Elastic Beanstalk 
5. The Elastic Beanstalk link for this project : http://aspassignment-env.eba-fm46p7dp.us-east-1.elasticbeanstalk.com/swagger/index.html

##### Built with the following: 
- ASP.NET CORE WEB API 
- ENTITY FRAMEWORK CORE 
- MS SQL SERVER 
- AWS ELASTIC BEANSTALK 
- GITHUB ACTIONS 

##### Author: 
Farah Shaheen


  

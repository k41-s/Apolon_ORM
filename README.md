# Project Apolon: Custom ORM Implementation

## Overview
Apolon is a bespoke Object Relational Mapper (ORM) developed in C# to demonstrate the fundamental principles of data persistence, 
relational mapping, and database management. The project bypasses high-level abstractions like Entity Framework, instead using Reflection and 
ADO.NET to bridge the gap between object-oriented code and a PostgreSQL relational database.

The system manages a medical environment consisting of patients, medical records, checkups, and prescriptions, satisfying specific academic 
requirements for data integrity and relational complexity.

## Technical Architecture

### Core ORM Engine
The system utilises a ModelParser to inspect entity classes at runtime. By evaluating custom attributes such as [Table] and [Column], 
the engine builds a metadata map of the schema. This allows the Generic Repository to generate dynamic SQL queries for all 
CRUD operations without hard-coded strings.



### Type Mapping and Integrity
A custom Type Mapping Engine ensures that .NET types are correctly translated to PostgreSQL types. This includes robust support for:
* VARCHAR, INT, DECIMAL, and FLOAT.
* DATETIME handling for medical records.
* Enumeration mapping: C# enums are stored as integers and reconstructed during retrieval.
* Constraints: The system enforces primary keys, auto-generation, nullability, and uniqueness.

### Relational Retrieval (LO4)
The ORM supports navigational properties through an aliased mapping strategy. By performing LEFT JOINs, 
the system can populate nested objects (such as a Patient inside a Prescription) in a single database round trip 
while maintaining the third normal form (3NF).



### Connection Management and Unit of Work (LO5)
Database connections are managed via a dedicated Database Service that handles the lifecycle of Npgsql connections. 
The project implements the Unit of Work pattern, allowing multiple repositories to share a single transaction. 
This ensures that complex operations are atomic: if one part of a process fails, the entire transaction is rolled back.



## Features

### CRUD Operations
The application supports full create, read, update, and delete functionality for:
* Patients: Basic information and record management.
* Medications: Cataloguing pharmaceutical data.
* Checkups: Recording various medical assessments (e.g., GP, MRI, EKG).
* Prescriptions: Managing the relationship between patients and medications, including dosage and duration.

### Data Filtering and Ordering
Retrieval methods support dynamic filtering and ordering. This allows the system to query specific subsets of data, 
such as finding all checkups for a specific patient sorted by date.

### Migration System
A separate migration utility handles the evolution of the database schema. It can:
1. Generate executable DDL queries based on the model classes.
2. Track applied migrations in a history table.
3. Execute new migrations or roll back to previous versions if required.



## Setup and Installation

### Prerequisites
* .NET 6.0 SDK or later.
* PostgreSQL instance (running via Docker is recommended).
* Npgsql driver.

### Initialisation
1. Clone the repository to your local machine.
2. Configure the connection string in the appsettings.json file within the Apolon.Web project.
3. Run the Migration Console application to provision the database schema in your PostgreSQL instance.
4. Launch the Apolon.Web application.

### Database Connection
The application connects to the PostgreSQL instance using the Npgsql library. 
It is designed to work with a cloud-provisioned or local Docker container. 
The connection management logic ensures that connections are opened as late as possible and closed immediately after the execution of commands to conserve resources.
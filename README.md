This project is a Student Management System built primarily using ASP.NET Core 5 Web API as the backend and Razor Pages for the frontend UI, with a separate ASP.NET Web Forms (.NET Framework) application integrated for SSRS report viewing. The system manages student records stored in a normalized SQL Server database across multiple related tables (such as students, cities, states, and colleges). Database operations—including adding, searching by name, deleting, and listing students—are performed using ADO.NET with raw SQL commands and SQL joins for data retrieval.

The Razor Pages frontend interacts asynchronously with the Web API, offering features like inline add modals, per-row delete buttons, and search functionality. Users can also export all student data to Excel files via EPPlus. For reporting, the solution incorporates SQL Server Reporting Services (SSRS) reports, which are generated via SOAP-based SSRS APIs and displayed securely through the Web Forms project hosting the ReportViewer control in remote processing mode. These reports can be accessed seamlessly from the Razor Pages UI, often opening in new tabs for comparison and user convenience. The overall architecture maintains a clean separation of concerns, enabling easy maintenance, extensibility, and integration between the Web API, frontend UI, and reporting components.

Backend:
The core backend is developed using ASP.NET Core 5 Web API, which serves as the central service layer. It performs all data access and manipulation operations on a normalized SQL Server database containing multiple related tables such as students, cities, states, and colleges. Data interactions are handled using ADO.NET with carefully crafted raw SQL queries, including joins, to ensure efficient and reliable CRUD (Create, Read, Update, Delete) operations. The Web API exposes RESTful endpoints for adding, searching by name, deleting, and listing student records, making it easy to integrate with different frontend clients or services.

Frontend:
The user interface is built with ASP.NET Core Razor Pages, which provides a modern and responsive UI. The frontend communicates asynchronously with the Web API via JavaScript, enabling smooth user experiences such as inline modal forms for adding students, instant search filtering, and per-row delete buttons directly within the data table. Additional UI enhancements include organized CSS and JavaScript files for styling and interactive functionality.

Export and Reporting Features:
To support business needs around data analysis and reporting, the system includes:

Excel Export: Users can export the full set of student data into Excel spreadsheets (.xlsx format) using the EPPlus library, facilitating offline review and sharing.

SSRS Integration: For advanced reporting, the system integrates SQL Server Reporting Services (SSRS). Since SSRS ReportViewer is primarily a .NET Framework control, a separate ASP.NET Web Forms application is included in the solution to host this control in remote processing mode. This Web Forms app securely renders SSRS reports and exposes them for consumption by the main Razor Pages UI. Users can trigger report generation and view reports either embedded via <iframe> or in new browser tabs, allowing easy comparison and navigation.

Architecture and Design:
The entire system follows a clear separation of concerns, with the Web API handling all business logic and database operations, Razor Pages delivering a responsive user experience, and the Web Forms app dedicated to report rendering. This layered approach improves maintainability, scalability, and ease of future enhancements.

Testing and Validation:
The API endpoints have been thoroughly tested using tools like Swagger UI and Postman to ensure correctness and reliability of all CRUD operations.

# FR_HKVision - Face Recognition Attendance System

## Project Overview
This is a Face Recognition-based Attendance System that integrates with HikVision cameras and Oracle Database. The system tracks student attendance using facial recognition technology.

## System Architecture

### 1. Core Components
- **Face Recognition**: Integration with HikVision cameras
- **Database**: Oracle Database for student and attendance records
- **Web Application**: ASP.NET Core MVC application

### 2. Key Features
- Student face registration
- Real-time attendance tracking
- Course management
- Attendance reporting
- Dashboard analytics

## Project Structure

### Controllers
- `HomeController.cs`: Main dashboard and course management
- `PersonController.cs`: Student face registration and management
- `AccountController.cs`: User authentication
- `DashboardController.cs`: Analytics and reporting

### Models
- `IndexModel.cs`: Main view model for home page
- `PersonViewModel.cs`: Student management view model
- `Student.cs`: Student data model
- `Programme.cs`: Course program model

### Data Access
- `OracleConnectionClass.cs`: Database operations
- `HttpClientImpl.cs`: HikVision API communication

### Views
- `Home/Index.cshtml`: Main dashboard
- `Person/Add.cshtml`: Student registration
- `Dashboard/Index.cshtml`: Analytics dashboard
- `Shared/_Layout.cshtml`: Main layout template

## Configuration

### Database Settings
```json
"OracleDBSettings": {
    "UserId": "your_user_id",
    "Password": "your_password",
    "DataSource": "your_data_source",
    "FacultyCode": "your_faculty_code"
}
```

### HikVision Settings
```json
"OpenApiSettings": {
    "ApiKey": "28109241",
    "ApiSecretKey": "BbWJe9x18ktAE5kqGozU",
    "ApiBaseUrl": "https://172.27.150.91:443/artemis/api"
}
```

## Key Workflows

### 1. Student Registration
1. Student photo is read from network path
2. Photo is converted to base64
3. Student is registered in HikVision system
4. Face data is stored for recognition

### 2. Attendance Tracking
1. Camera captures face
2. System matches face with registered students
3. Attendance is recorded in database
4. Transaction is created for the session

### 3. Reporting
1. Course attendance statistics
2. Student attendance history
3. Debtor tracking
4. Analytics dashboard

## Dependencies
- HikVision OpenAPI Client
- Oracle Database Client
- ASP.NET Core
- Entity Framework Core
- Chart.js (for analytics)

## Setup Instructions

1. **Database Setup**
   - Configure Oracle connection in `appsettings.json`
   - Ensure required tables exist in database

2. **HikVision Setup**
   - Configure API credentials in `appsettings.json`
   - Ensure camera is properly connected and configured

3. **Application Setup**
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

## Troubleshooting

### Common Issues
1. **Database Connection**
   - Verify connection string
   - Check database permissions
   - Ensure Oracle client is installed

2. **Camera Integration**
   - Verify API credentials
   - Check camera network connection
   - Ensure proper camera configuration

3. **Face Recognition**
   - Check photo quality
   - Verify face registration process
   - Monitor recognition accuracy

## Maintenance

### Regular Tasks
1. Database backup
2. Camera maintenance
3. System updates
4. Log monitoring

### Performance Optimization
1. Image processing optimization
2. Database query optimization
3. Caching implementation
4. Async operations

## Security Considerations
1. API key protection
2. Database access control
3. User authentication
4. Data encryption

## Changing Camera case(same API):
You should:
1. Change baseUrl to your new camera's IP address (keeping the same port and /artemis endpoint)
2. Update appKey with the new camera's API key
3. Update appSecret with the new camera's secret key

file directory: "/c:/Users/41800/Desktop/FR_HKVision/FR_HKVision/Class2.xxx"

## Changing Camera case(different API):
When switching to a different API system, the following components need to be modified:

### 1. API Client Implementation (`HttpClientImpl.cs`)
- Update authentication method (currently HMAC-SHA256)
- Modify header structure for new API requirements 
- Update request/response handling
- Adjust SSL/TLS settings if needed

### 2. Core API Configuration (`Class2.xxx`)
- Update base URL format and endpoints
- Modify authentication method implementation
- Adjust request/response data structures
- Update error handling

### 3. Controller Updates (`PersonController.cs`)
- Update API endpoints for person management
- Modify request/response models
- Adjust error handling for new API's error codes
- Update face recognition data format

### 4. Configuration Settings (`appsettings.json`)
```json
"NewApiSettings": {
    "ApiKey": "new_api_key_format",
    "AuthToken": "new_auth_token_if_required",
    "BaseUrl": "new_api_base_url",
    "AdditionalSettings": "as_required_by_new_api"
}
```

### 5. Model Updates
- Update `PersonViewModel.cs` for new API data structure
- Modify face recognition data models
- Adjust validation rules for new API requirements

### 6. Testing Steps
1. Verify API connectivity with new endpoints
2. Test face registration with new format
3. Validate attendance tracking
4. Check error handling
5. Perform load testing

### 7. Security Considerations
- Review new API's security requirements
- Update authentication mechanisms
- Implement required encryption methods
- Verify data protection compliance
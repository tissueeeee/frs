# Technical Documentation

## Code Architecture

### 1. Database Layer (`OracleConnectionClass.cs`)
```csharp
// Key Methods:
- studentListByCoursecode(): Retrieves students for a course
- programInfoByCoursecode(): Gets program information
- TransactionIsExist(): Checks for existing transactions
- UpdateAttendanceStatus(): Updates student attendance
```

### 2. HikVision Integration (`HttpClientImpl.cs`)
```csharp
// Key Features:
- API Authentication
- Request/Response Handling
- Error Management
- Base64 Image Processing
```

### 3. Controllers

#### HomeController
```csharp
// Main Responsibilities:
- Course Selection
- Attendance Management
- Student List Display
- Transaction Handling
```

#### PersonController
```csharp
// Key Functions:
- Student Registration
- Face Data Processing
- HikVision Person Management
- Error Handling
```

#### DashboardController
```csharp
// Features:
- Attendance Analytics
- Course Statistics
- Student Performance
- Report Generation
```

### 4. Models

#### IndexModel
```csharp
public class IndexModel
{
    public string CourseCode { get; set; }
    public List<Student> Students { get; set; }
    public List<Student> StudentListAttend { get; set; }
    public List<Student> StudentListAbsent { get; set; }
    public List<Student> StudentListDebt { get; set; }
    public List<Student> StudentListStranger { get; set; }
    public StudentTotal StudentTotal { get; set; }
    public Programme Programme { get; set; }
    public Message Message { get; set; }
}
```

#### PersonViewModel
```csharp
public class PersonViewModel
{
    public List<Person> Persons { get; set; }
}

public class Person
{
    public int StudentSeq { get; set; }
    public string StudentName { get; set; }
    public string StudentNumber { get; set; }
    public string StudentPhoto { get; set; }
    public string ProgrammeName { get; set; }
    public string ProgrammeCode { get; set; }
    public string AttendanceTime { get; set; }
    public string AttendancePhoto { get; set; }
    public string ProfileStatus { get; set; }
    public string ProfileRecord { get; set; }
    public string StudentMessage { get; set; }
}
```

## Key Workflows

### 1. Student Registration Process
```csharp
// Flow:
1. Read student photo from network path
2. Convert to base64
3. Check if student exists in HikVision
4. If not, register with face data
5. Update database records
```

### 2. Attendance Tracking
```csharp
// Steps:
1. Camera captures face
2. System matches with registered faces
3. Create/update transaction
4. Record attendance in database
5. Update statistics
```

### 3. Database Operations
```csharp
// Transaction Management:
1. Check for existing transaction
2. Create new if not exists
3. Update attendance status
4. Handle errors and rollbacks
```

## Error Handling

### 1. Database Errors
```csharp
try
{
    // Database operations
}
catch (OracleException ex)
{
    _logger.LogError($"Database error: {ex.Message}");
    // Handle specific Oracle errors
}
```

### 2. API Errors
```csharp
try
{
    // API calls
}
catch (HttpRequestException ex)
{
    _logger.LogError($"API error: {ex.Message}");
    // Handle API specific errors
}
```

### 3. General Exceptions
```csharp
try
{
    // General operations
}
catch (Exception ex)
{
    _logger.LogError($"General error: {ex.Message}");
    // Handle unexpected errors
}
```

## Configuration Management

### 1. Database Settings
```json
"OracleDBSettings": {
    "UserId": "string",
    "Password": "string",
    "DataSource": "string",
    "FacultyCode": "string"
}
```

### 2. HikVision Settings
```json
"OpenApiSettings": {
    "ApiKey": "string",
    "ApiSecretKey": "string",
    "ApiBaseUrl": "string"
}
```

## Performance Considerations

### 1. Database Optimization
- Use parameterized queries
- Implement proper indexing
- Batch operations where possible
- Connection pooling

### 2. Image Processing
- Optimize image size
- Implement caching
- Use async operations
- Handle memory efficiently

### 3. API Calls
- Implement retry logic
- Use connection pooling
- Cache responses where appropriate
- Handle timeouts properly

## Security Implementation

### 1. Authentication
```csharp
// Session Management
HttpContext.Session.SetString("Username", username);
HttpContext.Session.SetString("Coursecode", courseCode);
```

### 2. Authorization
```csharp
// Access Control
if (!User.Identity.IsAuthenticated)
{
    return RedirectToAction("Login", "Account");
}
```

### 3. Data Protection
```csharp
// Sensitive Data
var encryptedPassword = _dataProtector.Protect(password);
```

## Testing Guidelines

### 1. Unit Tests
- Test database operations
- Test API integrations
- Test business logic
- Test error handling

### 2. Integration Tests
- Test end-to-end workflows
- Test system interactions
- Test error scenarios
- Test performance

### 3. Manual Testing
- Test user interface
- Test camera integration
- Test reporting features
- Test security features

## Deployment Checklist

### 1. Pre-deployment
- [ ] Database backup
- [ ] Configuration review
- [ ] Security audit
- [ ] Performance testing

### 2. Deployment
- [ ] Database updates
- [ ] Application deployment
- [ ] Configuration updates
- [ ] Service restart

### 3. Post-deployment
- [ ] Functionality verification
- [ ] Performance monitoring
- [ ] Error logging review
- [ ] User feedback collection 
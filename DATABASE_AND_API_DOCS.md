# Database and API Integration Documentation

## Database Operations

### 1. Connection Management
```csharp
// Connection String Format
string connectionString = "User Id={userId};Password={password};Data Source={dataSource}";

// Connection Usage
using (OracleConnection conn = new OracleConnection(connectionString))
{
    conn.Open();
    // Database operations
}
```

### 2. Key Database Tables

#### FR_TRANSACTION
- Stores attendance session information
- Fields: TRANSACTION_ID, TIME, TRANSACTION_DATE, LECTURER_NAME, COURSE_CODE, ROOM, CREATE_DATE

#### FR_TRANSACTION_STUDENT
- Tracks individual student attendance
- Fields: STUDENT_NUMBER, STUDENT_NAME, TRANSACTION_ID, ATTENDANCE, FEE_DUE, CREATE_DATE

#### FR_STUDENT
- Student master data
- Fields: STUDENT_NUMBER, STUDENT_NAME, PROFILE_STATUS, STUDENTPHOTO, PROGRAMME_CODE, PROGRAMME_NAME

#### FR_COURSE_TIMETABLE
- Course scheduling information
- Fields: COURSECODE, COURSE_NAME, FACULTYCODE, ON_EVERY

### 3. Core Database Operations

#### Transaction Management
```csharp
// Check if transaction exists
public static int TransactionIsExist(string userId, string password, string dataSource, string CourseCode, string Room, string todayDate)
{
    // Returns transaction ID if exists, 0 if not
}

// Create new transaction
public static int insertTransaction(string userId, string password, string dataSource, Transactions transactions)
{
    // Returns new transaction ID
}

// Update student attendance
public static int updateStudentAttendanceStatus(string userId, string password, string dataSource, string StudentNumber, int TransactionId, int AttendanceStatus)
{
    // Returns:
    // 0: Success
    // -1: No rows affected
    // -2: Record not found
    // -3: Database error
    // -4: General error
}
```

#### Student Management
```csharp
// Get students by course
public static List<Student> studentListByCoursecode(string userId, string password, string dataSource, string course_code)
{
    // Returns list of students enrolled in course
}

// Verify student fee status
public static string studentFeeVerification(string userId, string password, string dataSource, string course_code, string student_number)
{
    // Returns student name and profile status
}
```

### 4. Error Handling
```csharp
try
{
    // Database operations
}
catch (OracleException ex)
{
    // Handle Oracle-specific errors
    Console.WriteLine($"Oracle Error: {ex.Message}");
    Console.WriteLine($"Error Code: {ex.Number}");
}
catch (Exception ex)
{
    // Handle general errors
    Console.WriteLine($"General Error: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
}
```

## HikVision API Integration

### 1. API Configuration
```csharp
// API Settings from appsettings.json
"OpenApiSettings": {
    "ApiKey": "your_api_key",
    "ApiSecretKey": "your_secret_key",
    "ApiBaseUrl": "your_api_base_url"
}
```

### 2. Key API Endpoints

#### Person Management
```csharp
// Search for person
POST /resource/v1/person/advance/personList
{
    "pageNo": 1,
    "pageSize": 10,
    "personName": "student_name"
}

// Add new person
POST /resource/v1/person/single/add
{
    "personCode": "student_number",
    "personName": "student_name",
    "personFamilyName": "",
    "personGivenName": "student_name",
    "gender": 0,
    "orgIndexCode": "2",
    "faces": [{"faceData": "base64_image"}]
}
```

### 3. Image Processing
```csharp
// Convert image to base64
public static string ConvertImageToBase64(string imagePath)
{
    byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
    return Convert.ToBase64String(imageBytes);
}
```

### 4. API Error Handling
```csharp
try
{
    // API call
    string response = HttpClientImpl.Post(url, headers, apiKey, apiSecretKey, jsonData);
    
    if (!string.IsNullOrEmpty(response))
    {
        JObject data = JObject.Parse(response);
        // Process response
    }
}
catch (HttpRequestException ex)
{
    // Handle API communication errors
    _logger.LogError($"API error: {ex.Message}");
}
```

## Integration Workflows

### 1. Student Registration
1. Read student photo from network path
2. Convert photo to base64
3. Check if student exists in HikVision
4. If not, register with face data
5. Update database records

### 2. Attendance Tracking
1. Camera captures face
2. System matches with registered faces
3. Create/update transaction
4. Record attendance in database
5. Update statistics

### 3. Reporting
1. Query attendance data
2. Calculate statistics
3. Generate reports
4. Update dashboard

## Performance Optimization

### 1. Database
- Use parameterized queries
- Implement proper indexing
- Batch operations where possible
- Connection pooling

### 2. API Calls
- Implement retry logic
- Use connection pooling
- Cache responses where appropriate
- Handle timeouts properly

### 3. Image Processing
- Optimize image size
- Implement caching
- Use async operations
- Handle memory efficiently

## Security Considerations

### 1. Database Security
- Use parameterized queries
- Implement proper access control
- Encrypt sensitive data
- Regular security audits

### 2. API Security
- Secure API credentials
- Implement proper authentication
- Use HTTPS
- Validate all inputs

### 3. Data Protection
- Encrypt sensitive data
- Implement proper access control
- Regular security audits
- Data backup procedures

## HikVision Camera API Implementation

### 1. API Configuration and Setup

#### Required NuGet Packages
```xml
<PackageReference Include="HikVision" Version="1.0.0" />
<PackageReference Include="hikvision-openapi-client" Version="1.2.0" />
<PackageReference Include="QuickNV.HikvisionNetSDK" Version="1.0.4" />
<PackageReference Include="QuickNV.HikvisionNetSDK.Native" Version="1.0.3" />
```

#### Configuration in appsettings.json
```json
"OpenApiSettings": {
    "ApiKey": "your_api_key",
    "ApiSecretKey": "your_secret_key",
    "ApiBaseUrl": "https://127.0.0.1:4433/artemis"
}
```

### 2. API Authentication

#### Authorization Header Generation
```csharp
private static string GenerateAuthorization(string urlPath, string method)
{
    string nonce = Guid.NewGuid().ToString("N");
    string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
    string stringToSign = method + "\n" + nonce + "\n" + timestamp + "\n" + urlPath;

    using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret)))
    {
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
        string signature = Convert.ToBase64String(hash);
        return $"appKey={appKey}, nonce={nonce}, timestamp={timestamp}, signature={signature}";
    }
}
```

### 3. Core API Operations

#### Face Recognition Operations

##### Search for Person
```csharp
public static string SearchPerson(string personName)
{
    string urlPath = "/api/resource/v1/person/advance/personList";
    string method = "POST";
    string authorization = GenerateAuthorization(urlPath, method);
    
    var requestBody = new
    {
        pageNo = 1,
        pageSize = 10,
        personName = personName
    };
    
    return HttpClientImpl.Post(
        baseUrl + urlPath,
        headers,
        appKey,
        appSecret,
        JsonConvert.SerializeObject(requestBody)
    );
}
```

##### Add New Person
```csharp
public static string AddPerson(string personCode, string personName, string faceData)
{
    string urlPath = "/api/resource/v1/person/single/add";
    string method = "POST";
    string authorization = GenerateAuthorization(urlPath, method);
    
    var requestBody = new
    {
        personCode = personCode,
        personName = personName,
        personFamilyName = "",
        personGivenName = personName,
        gender = 0,
        orgIndexCode = "2",
        faces = new[] { new { faceData = faceData } }
    };
    
    return HttpClientImpl.Post(
        baseUrl + urlPath,
        headers,
        appKey,
        appSecret,
        JsonConvert.SerializeObject(requestBody)
    );
}
```

### 4. Error Handling and Retry Logic

```csharp
public static async Task<string> MakeApiRequestWithRetry(string urlPath, string method, string authorization, string body = null, int maxRetries = 3)
{
    int retryCount = 0;
    while (retryCount < maxRetries)
    {
        try
        {
            string response = HttpClientImpl.Post(
                baseUrl + urlPath,
                headers,
                appKey,
                appSecret,
                body
            );
            
            JObject result = JObject.Parse(response);
            if (result["code"]?.ToString() == "0")
            {
                return response;
            }
            
            // Handle specific error codes
            switch (result["code"]?.ToString())
            {
                case "10002": // Invalid token
                    await RefreshToken();
                    break;
                case "10003": // Invalid signature
                    authorization = GenerateAuthorization(urlPath, method);
                    break;
                default:
                    throw new Exception($"API Error: {result["msg"]}");
            }
            
            retryCount++;
            await Task.Delay(1000 * retryCount); // Exponential backoff
        }
        catch (Exception ex)
        {
            if (retryCount == maxRetries - 1)
                throw;
            
            retryCount++;
            await Task.Delay(1000 * retryCount);
        }
    }
    
    throw new Exception("Max retries exceeded");
}
```

### 5. Camera Event Handling

#### Face Recognition Events
```csharp
public class FaceRecognitionEventArgs : EventArgs
{
    public string PersonCode { get; set; }
    public string PersonName { get; set; }
    public DateTime RecognitionTime { get; set; }
    public string CameraId { get; set; }
}

public event EventHandler<FaceRecognitionEventArgs> OnFaceRecognized;

private void HandleFaceRecognition(string personCode, string personName, string cameraId)
{
    OnFaceRecognized?.Invoke(this, new FaceRecognitionEventArgs
    {
        PersonCode = personCode,
        PersonName = personName,
        RecognitionTime = DateTime.Now,
        CameraId = cameraId
    });
}
```

### 6. Best Practices

1. **Connection Management**:
   - Use connection pooling
   - Implement proper timeout handling
   - Handle SSL/TLS certificate validation

2. **Error Handling**:
   - Implement retry logic with exponential backoff
   - Log all API errors
   - Handle specific error codes appropriately

3. **Performance Optimization**:
   - Cache frequently accessed data
   - Use async/await for API calls
   - Implement request batching where possible

4. **Security**:
   - Secure API credentials
   - Use HTTPS for all communications
   - Validate all input data
   - Implement proper access control

5. **Monitoring**:
   - Log all API calls
   - Track response times
   - Monitor error rates
   - Set up alerts for critical failures

### 7. Troubleshooting Guide

#### Common Issues and Solutions

1. **Authentication Failures**:
   - Verify API key and secret
   - Check timestamp synchronization
   - Validate signature generation

2. **Connection Issues**:
   - Verify network connectivity
   - Check firewall settings
   - Validate SSL/TLS configuration

3. **Face Recognition Failures**:
   - Check image quality
   - Verify face data format
   - Validate person registration

4. **Performance Issues**:
   - Monitor response times
   - Check for network latency
   - Verify server load

5. **Error Codes**:
   - 10002: Invalid token - Refresh token
   - 10003: Invalid signature - Regenerate signature
   - 10004: Invalid parameter - Check input data
   - 10005: System error - Contact support 
# FR_HKVision API Documentation

## Overview
This document provides comprehensive documentation for the FR_HKVision API, which integrates with HikVision cameras and manages student attendance through facial recognition.

## Base URL
```
https://[your-server-url]/api
```

## Authentication
The API uses session-based authentication. Users need to login through the Account controller before accessing protected endpoints.

### Login
```
POST /Account/Login
```

**Request Body:**
```json
{
  "Username": "string",
  "Password": "string"
}
```

**Response:**
- Success: Redirects to Home/Index
- Failure: Returns to login page with error message

## API Endpoints

### Person Management

#### 1. Add Person
Adds a student to the facial recognition system.

```
GET /Person/Add
```

**Description:** 
Retrieves students for the selected course code and registers them in the HikVision system by uploading their photos and details.

**Authorization:** 
Requires an authenticated session with a selected course code.

**Request Parameters:**
- None (Uses session data for course code)

**Response:**
- Success: View with list of registered students
- Failure: Error view with details

#### 2. Error (Person Controller)
```
GET /Person/Error
```

**Description:** 
Error handling endpoint for the Person controller.

**Response:**
- Returns error information

#### 3. Fail (Person Controller)
```
GET /Person/Fail
```

**Description:** 
Displays failure information for Person operations.

**Response:**
- Returns failure view

### Home Controller

#### 1. Index
```
GET /Home/Index
```

**Description:** 
Displays the main dashboard of the application.

**Authorization:** 
Requires authenticated session.

**Response:**
- Returns dashboard view with attendance information

#### 2. Privacy
```
GET /Home/Privacy
```

**Description:** 
Displays privacy information.

**Response:**
- Returns privacy policy view

#### 3. Error (Home Controller)
```
GET /Home/Error
```

**Description:** 
Error handling endpoint for Home controller.

**Response:**
- Returns error information

### Dashboard Controller

#### 1. Index
```
GET /Dashboard/Index
```

**Description:** 
Displays attendance statistics and other dashboard information.

**Authorization:** 
Requires authenticated session.

**Response:**
- Returns dashboard view with statistics

#### 2. Specific Dashboard Views
The Dashboard controller provides several specialized views for different aspects of attendance and statistics. These endpoints typically follow the pattern:

```
GET /Dashboard/[SpecificView]
```

### Report Controller

#### 1. Index
```
GET /Report/Index
```

**Description:** 
Displays report generation interface.

**Authorization:** 
Requires authenticated session.

**Response:**
- Returns report view

#### 2. Generate Reports
The Report controller provides endpoints for generating different types of reports:

```
GET /Report/GetReport
```

**Description:** 
Generates specific types of attendance reports.

**Authorization:** 
Requires authenticated session.

**Request Parameters:**
- selectedCourseCode: Course code to filter by
- selectedClassroom: Classroom to filter by
- selectedDate: Date to filter by
- selectedTime: Time to filter by

**Response:**
- Returns generated report view

#### 3. Export to CSV
```
GET /Report/ExportToCsv
```

**Description:** 
Exports report data to CSV format for download.

**Authorization:** 
Requires authenticated session.

**Request Parameters:**
- selectedCourseCode: Course code to filter by
- selectedClassroom: Classroom to filter by
- selectedDate: Date to filter by
- selectedTime: Time to filter by

**Response:**
- Returns CSV file download containing attendance data

## Data Models

### Person
```json
{
  "StudentSeq": "integer",
  "StudentNumber": "string",
  "StudentName": "string",
  "StudentPhoto": "string",
  "ProgrammeCode": "string",
  "ProgrammeName": "string",
  "ProfileStatus": "string",
  "ProfileRecord": "string",
  "AttendanceTime": "string",
  "AttendancePhoto": "string",
  "StudentMessage": "string"
}
```

### Student
```json
{
  "StudentNumber": "string",
  "StudentName": "string",
  "ProgrammeCode": "string",
  "ProgrammeName": "string",
  "ProfileStatus": "string",
  "StudentPhoto": "string"
}
```

### Transaction
```json
{
  "TransactionId": "integer",
  "TransactionTime": "string",
  "TransactionDate": "string",
  "LecturerName": "string",
  "CourseCode": "string",
  "Room": "string",
  "CreateDate": "datetime"
}
```

### ReportViewModel
```json
{
  "StudentNumber": "string",
  "StudentName": "string",
  "CourseCode": "string",
  "Attendance": "string",
  "FeeDue": "string"
}
```

## Integration with HikVision API

### Base Configuration
The application integrates with HikVision's API using configuration from appsettings.json:

```json
"OpenApiSettings": {
  "ApiKey": "your_api_key",
  "ApiSecretKey": "your_secret_key",
  "ApiBaseUrl": "https://127.0.0.1:4433/artemis"
}
```

### HikVision API Endpoints Used

#### 1. Get API Version
```
POST /common/v1/version
```

**Headers:**
- Accept: application/json
- Content-Type: application/json;charset=UTF-8

**Authentication:**
- API Key and Secret Key in headers

**Response:**
- Returns API version information

#### 2. Search Person
```
POST /resource/v1/person/advance/personList
```

**Request Body:**
```json
{
  "pageNo": 1,
  "pageSize": 10,
  "personName": "student_name"
}
```

**Authentication:**
- API Key and Secret Key in headers

**Response:**
- Returns list of matching persons

#### 3. Add Person
```
POST /resource/v1/person/single/add
```

**Request Body:**
```json
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

**Authentication:**
- API Key and Secret Key in headers

**Response:**
- Returns person index code on success

## Error Codes

### Application Error Codes
- **General Errors**: Displayed through Error and Fail actions
- **Specific Error Messages**: Provided in JSON responses or view models

### HikVision API Error Codes
The HikVision API returns specific error codes in the "code" field of the response:
- **0**: Success
- **Other values**: Error condition (specific error details in the response)

## Best Practices

1. **Authentication**: Always ensure proper authentication before accessing endpoints
2. **Session Management**: Set the course code in session before attempting to add persons
3. **Error Handling**: Check response codes and handle errors appropriately
4. **Image Processing**: Ensure images exist before attempting to convert to base64
5. **API Integration**: Properly handle HikVision API responses and error conditions
6. **Data Export**: Use appropriate export format (PDF/CSV) based on data volume and requirements

## Database Integration

The API integrates with an Oracle database using the OracleConnectionClass. Key database operations include:

1. **studentListByCoursecode**: Retrieves students enrolled in a specific course
2. **TransactionIsExist**: Checks if a transaction exists for a course on a specific date
3. **insertTransaction**: Creates a new attendance transaction
4. **updateStudentAttendanceStatus**: Updates student attendance status
5. **GetAttendanceReport**: Retrieves attendance data for reports

## Security Considerations

1. **Data Protection**: The application uses ASP.NET Core Data Protection for securing sensitive data
2. **Session Security**: Sessions are configured with HttpOnly cookies and essential flags
3. **HTTPS**: All production traffic should use HTTPS
4. **API Credentials**: HikVision API credentials are stored in appsettings.json and should be properly secured
5. **Database Credentials**: Oracle database credentials should be properly secured in configuration

## Troubleshooting

### Common Issues
1. **Missing Course Code**: Ensure course code is set in session before accessing Person/Add
2. **Image Access**: Verify network paths to student images are accessible
3. **API Connection**: Check HikVision API connection and credentials
4. **Database Connection**: Verify Oracle database connection parameters
5. **Export Issues**: For large datasets, use server-side export methods instead of client-side processing


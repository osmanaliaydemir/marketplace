-- =============================================
-- Exception Logs Migration Script
-- Version: 0003
-- Description: Creates ExceptionLog table for exception tracking and analytics
-- =============================================

-- Drop existing objects if they exist (for re-running the script)
IF OBJECT_ID('sp_CleanupOldExceptions', 'P') IS NOT NULL
    DROP PROCEDURE sp_CleanupOldExceptions;
GO

IF OBJECT_ID('vw_CriticalExceptions', 'V') IS NOT NULL
    DROP VIEW vw_CriticalExceptions;
GO

IF OBJECT_ID('vw_UnresolvedExceptions', 'V') IS NOT NULL
    DROP VIEW vw_UnresolvedExceptions;
GO

IF OBJECT_ID('fn_GetSimilarExceptions', 'FN') IS NOT NULL
    DROP FUNCTION fn_GetSimilarExceptions;
GO

IF OBJECT_ID('sp_GetExceptionTrends', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetExceptionTrends;
GO

IF OBJECT_ID('sp_GetExceptionAnalytics', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetExceptionAnalytics;
GO

IF OBJECT_ID('TR_exception_logs_updated_at', 'TR') IS NOT NULL
    DROP TRIGGER TR_exception_logs_updated_at;
GO

IF OBJECT_ID('exception_logs', 'U') IS NOT NULL
    DROP TABLE exception_logs;
GO

-- Create ExceptionLog table
CREATE TABLE exception_logs (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    exception_type NVARCHAR(255) NOT NULL,
    message NVARCHAR(MAX) NOT NULL,
    stack_trace NVARCHAR(MAX) NULL,
    source NVARCHAR(500) NULL,
    user_agent NVARCHAR(1000) NULL,
    user_id NVARCHAR(50) NULL,
    request_path NVARCHAR(500) NULL,
    request_method NVARCHAR(10) NULL,
    request_body NVARCHAR(MAX) NULL,
    query_string NVARCHAR(2000) NULL,
    ip_address NVARCHAR(45) NULL, -- IPv6 uyumlu
    correlation_id NVARCHAR(100) NULL,
    environment NVARCHAR(50) NULL,
    application_version NVARCHAR(50) NULL,
    severity TINYINT NOT NULL DEFAULT 2, -- 1=Low, 2=Medium, 3=High, 4=Critical
    status TINYINT NOT NULL DEFAULT 1, -- 1=New, 2=Investigating, 3=Resolved, 4=Ignored
    occurred_at DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    resolved_at DATETIME2(7) NULL,
    resolved_by NVARCHAR(100) NULL,
    resolution_notes NVARCHAR(MAX) NULL,
    occurrence_count INT NOT NULL DEFAULT 1,
    last_occurrence DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    tags NVARCHAR(MAX) NULL, -- JSON format
    created_at DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    updated_at DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    -- Indexes for performance
    INDEX IX_exception_logs_severity (severity),
    INDEX IX_exception_logs_status (status),
    INDEX IX_exception_logs_occurred_at (occurred_at),
    INDEX IX_exception_logs_last_occurrence (last_occurrence),
    INDEX IX_exception_logs_exception_type (exception_type),
    INDEX IX_exception_logs_correlation_id (correlation_id),
    INDEX IX_exception_logs_user_id (user_id),
    INDEX IX_exception_logs_request_path (request_path),
    INDEX IX_exception_logs_ip_address (ip_address),
    INDEX IX_exception_logs_environment (environment),
    
    -- Composite indexes for common queries
    INDEX IX_exception_logs_status_severity (status, severity),
    INDEX IX_exception_logs_occurred_status (occurred_at, status),
    INDEX IX_exception_logs_type_occurred (exception_type, occurred_at) INCLUDE (severity),
    INDEX IX_exception_logs_unresolved (status, severity) WHERE status IN (1, 2) -- New or Investigating
);

GO

-- Create trigger for updated_at
CREATE TRIGGER TR_exception_logs_updated_at
ON exception_logs
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE exception_logs 
    SET updated_at = GETUTCDATE()
    FROM exception_logs el
    INNER JOIN inserted i ON el.id = i.id;
END;
GO

-- Create stored procedure for exception analytics
CREATE PROCEDURE sp_GetExceptionAnalytics
    @StartDate DATETIME2(7),
    @EndDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Main analytics
    SELECT 
        COUNT(*) as TotalExceptions,
        COUNT(CASE WHEN status IN (1, 2) THEN 1 END) as UnresolvedExceptions,
        COUNT(CASE WHEN severity = 4 THEN 1 END) as CriticalExceptions,
        COUNT(CASE WHEN severity = 3 THEN 1 END) as HighSeverityExceptions,
        AVG(CASE 
            WHEN resolved_at IS NOT NULL AND status = 3 
            THEN DATEDIFF(HOUR, occurred_at, resolved_at) 
            ELSE NULL 
        END) as AverageResolutionTimeHours
    FROM exception_logs
    WHERE occurred_at BETWEEN @StartDate AND @EndDate;
    
    -- Exception type distribution
    SELECT 
        exception_type,
        COUNT(*) as Count
    FROM exception_logs
    WHERE occurred_at BETWEEN @StartDate AND @EndDate
    GROUP BY exception_type
    ORDER BY Count DESC;
    
    -- Severity distribution
    SELECT 
        severity,
        COUNT(*) as Count
    FROM exception_logs
    WHERE occurred_at BETWEEN @StartDate AND @EndDate
    GROUP BY severity
    ORDER BY severity;
    
    -- Status distribution
    SELECT 
        status,
        COUNT(*) as Count
    FROM exception_logs
    WHERE occurred_at BETWEEN @StartDate AND @EndDate
    GROUP BY status
    ORDER BY status;
    
    -- Top affected endpoints
    SELECT TOP 10
        request_path,
        COUNT(*) as Count
    FROM exception_logs
    WHERE occurred_at BETWEEN @StartDate AND @EndDate
        AND request_path IS NOT NULL
    GROUP BY request_path
    ORDER BY Count DESC;
END;
GO

-- Create stored procedure for exception trends
CREATE PROCEDURE sp_GetExceptionTrends
    @StartDate DATETIME2(7),
    @EndDate DATETIME2(7),
    @GroupBy NVARCHAR(10) = 'day' -- 'hour', 'day', 'week', 'month'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @DateFormat NVARCHAR(20);
    DECLARE @DateTrunc NVARCHAR(50);
    
    -- Determine date formatting based on groupBy parameter
    IF @GroupBy = 'hour'
    BEGIN
        SET @DateFormat = 'yyyy-MM-dd HH:00:00';
        SET @DateTrunc = 'DATEADD(HOUR, DATEDIFF(HOUR, 0, occurred_at), 0)';
    END
    ELSE IF @GroupBy = 'week'
    BEGIN
        SET @DateFormat = 'yyyy-MM-dd';
        SET @DateTrunc = 'DATEADD(WEEK, DATEDIFF(WEEK, 0, occurred_at), 0)';
    END
    ELSE IF @GroupBy = 'month'
    BEGIN
        SET @DateFormat = 'yyyy-MM-01';
        SET @DateTrunc = 'DATEADD(MONTH, DATEDIFF(MONTH, 0, occurred_at), 0)';
    END
    ELSE -- default to day
    BEGIN
        SET @DateFormat = 'yyyy-MM-dd';
        SET @DateTrunc = 'CAST(occurred_at as DATE)';
    END
    
    DECLARE @SQL NVARCHAR(MAX) = '
    SELECT 
        ' + @DateTrunc + ' as Date,
        ''' + @GroupBy + ''' as GroupBy,
        COUNT(*) as Count,
        MAX(severity) as MaxSeverity,
        (SELECT TOP 1 exception_type 
         FROM exception_logs el2 
         WHERE ' + @DateTrunc + ' = ' + REPLACE(@DateTrunc, 'occurred_at', 'el2.occurred_at') + '
         GROUP BY exception_type 
         ORDER BY COUNT(*) DESC) as TopExceptionType
    FROM exception_logs
    WHERE occurred_at BETWEEN @StartDate AND @EndDate
    GROUP BY ' + @DateTrunc + '
    ORDER BY Date';
    
    EXEC sp_executesql @SQL, N'@StartDate DATETIME2(7), @EndDate DATETIME2(7)', @StartDate, @EndDate;
END;
GO

-- Create function for similar exception detection
CREATE FUNCTION fn_GetSimilarExceptions(
    @ExceptionType NVARCHAR(255),
    @Message NVARCHAR(MAX),
    @WithinHours INT = 24
)
RETURNS TABLE
AS
RETURN
(
    SELECT *
    FROM exception_logs
    WHERE exception_type = @ExceptionType
        AND message = @Message
        AND occurred_at >= DATEADD(HOUR, -@WithinHours, GETUTCDATE())
);
GO

-- Create view for unresolved exceptions
CREATE VIEW vw_UnresolvedExceptions
AS
SELECT 
    id,
    exception_type,
    message,
    severity,
    status,
    request_path,
    request_method,
    user_id,
    ip_address,
    occurred_at,
    last_occurrence,
    occurrence_count,
    DATEDIFF(HOUR, occurred_at, GETUTCDATE()) as HoursSinceOccurred
FROM exception_logs
WHERE status IN (1, 2) -- New or Investigating
    AND occurred_at >= DATEADD(DAY, -30, GETUTCDATE()); -- Last 30 days
GO

-- Create view for critical exceptions
CREATE VIEW vw_CriticalExceptions
AS
SELECT 
    id,
    exception_type,
    message,
    severity,
    status,
    request_path,
    request_method,
    user_id,
    ip_address,
    occurred_at,
    last_occurrence,
    occurrence_count,
    stack_trace,
    correlation_id
FROM exception_logs
WHERE severity = 4 -- Critical
    AND status IN (1, 2) -- New or Investigating
    AND occurred_at >= DATEADD(DAY, -7, GETUTCDATE()); -- Last 7 days
GO

-- Create cleanup procedure for old resolved exceptions
CREATE PROCEDURE sp_CleanupOldExceptions
    @DaysToKeep INT = 90
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CutoffDate DATETIME2(7) = DATEADD(DAY, -@DaysToKeep, GETUTCDATE());
    DECLARE @DeletedCount INT;
    
    -- Delete old resolved/ignored exceptions
    DELETE FROM exception_logs
    WHERE status IN (3, 4) -- Resolved or Ignored
        AND resolved_at < @CutoffDate;
        
    SET @DeletedCount = @@ROWCOUNT;
    
    -- Log cleanup activity
    PRINT 'Cleaned up ' + CAST(@DeletedCount as NVARCHAR(10)) + ' old exception records.';
END;
GO

-- Create initial data - severity and status lookup values (for reference)
/*
Severity Values:
1 = Low
2 = Medium (Default)
3 = High
4 = Critical

Status Values:
1 = New (Default)
2 = Investigating
3 = Resolved
4 = Ignored
*/

PRINT 'Exception logs table and related objects created successfully!';
PRINT 'Remember to:';
PRINT '1. Schedule sp_CleanupOldExceptions to run monthly';
PRINT '2. Monitor index fragmentation on large exception_logs table';
PRINT '3. Consider partitioning if exception volume is very high';
GO

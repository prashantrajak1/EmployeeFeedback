USE FeedbackTrackDB;
GO

-- Insert Users if they don't exist
IF NOT EXISTS (SELECT 1 FROM TUsers WHERE Email = 'admin@feedback.com')
BEGIN
    INSERT INTO TUsers (Name, Email, Password, IsActive, RoleId, DepartmentId)
    VALUES ('System Admin', 'admin@feedback.com', 'admin123', 1, 1, 1);
END

IF NOT EXISTS (SELECT 1 FROM TUsers WHERE Email = 'manager@feedback.com')
BEGIN
    INSERT INTO TUsers (Name, Email, Password, IsActive, RoleId, DepartmentId)
    VALUES ('John Manager', 'manager@feedback.com', 'manager123', 1, 2, 1);
END

IF NOT EXISTS (SELECT 1 FROM TUsers WHERE Email = 'employee@feedback.com')
BEGIN
    INSERT INTO TUsers (Name, Email, Password, IsActive, RoleId, DepartmentId)
    VALUES ('Jane Employee', 'employee@feedback.com', 'employee123', 1, 3, 1);
END
GO

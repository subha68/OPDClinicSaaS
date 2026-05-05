INSERT INTO Roles VALUES
(1,'SuperAdmin'),
(2,'ClinicAdmin'),
(3,'ClinicUser'),
(4,'Doctor'),
(5,'BillingUser');

INSERT INTO Users (ClinicId, RoleId, Username, PasswordHash, FirstName, LastName, Email, ContactNumber, IsFirstLogin, IsActive) VALUES
(1, 1, 'superadmin', 'admin', 'Test', 'Name2', 'superadmin@example.com', '0987654321', 1, 1),
(1, 1, 'admin', 'admin', 'Test', 'Name1', 'admin@example.com', '1234567890', 1, 1);



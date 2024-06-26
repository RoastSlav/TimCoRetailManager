﻿CREATE PROCEDURE [dbo].[spUser_Create]
	@Id nvarchar(128),
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@EmailAddress nvarchar(256)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.[User](Id, FirstName, LastName, EmailAddress)
	values (@Id, @FirstName, @LastName, @EmailAddress);
END

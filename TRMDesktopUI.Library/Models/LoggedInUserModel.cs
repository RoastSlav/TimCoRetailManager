﻿namespace TRMDesktopUI.Library.Models;

public class LoggedInUserModel : ILoggedInUserModel
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string EmailAddress { get; set; }

    public DateTime CreatedDate { get; set; }

    public string Token { get; set; }

    public void ResetUserModel()
    {
        Token = "";
        FirstName = "";
        LastName = "";
        EmailAddress = "";
        CreatedDate = DateTime.MinValue;
        Id = "";
    }
}
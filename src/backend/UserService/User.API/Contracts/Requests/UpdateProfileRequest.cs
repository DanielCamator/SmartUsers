namespace User.API.Contracts.Requests;

public record UpdateProfileRequest(string FirstName, string LastName, string PhoneNumber, string Address);
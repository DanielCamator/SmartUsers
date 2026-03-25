namespace User.Api.Contracts.Requests;

public record RegisterUserRequest(
    string Email,
    string Password,
    Guid RoleId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Address);
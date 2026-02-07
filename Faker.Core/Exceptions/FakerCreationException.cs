namespace Faker.Core.Exceptions;

public class FakerCreationException(string? message, Exception innerException)
    : Exception(message, innerException);
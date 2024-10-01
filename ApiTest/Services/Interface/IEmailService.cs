public interface IEmailService
{
    /// <summary>
    /// Sends a reminder email to the specified recipient.
    /// </summary>
    /// <param name="email">The recipient's email address.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendReminder(string email);
}
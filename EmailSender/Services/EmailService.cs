using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _sendGridApiKey;

    public EmailService(IConfiguration configuration)
    {
        _sendGridApiKey = configuration["SendGrid:ApiKey"];

        if (string.IsNullOrEmpty(_sendGridApiKey))
        {
            throw new Exception("SendGrid API Key is missing in configuration.");
        }
    }

    public async Task<string> SendEmailAsync(string senderEmail, List<string> recipients, string subject, string body, string filePath)
    {
        var client = new SendGridClient(_sendGridApiKey);
        var from = new EmailAddress(senderEmail, "Sanketh Ganji");

        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(
            from,
            recipients.ConvertAll(email => new EmailAddress(email)),
            subject,
            body,
            body
        );

        // 🔹 Attach File
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            string fileBase64 = Convert.ToBase64String(fileBytes);
            string fileName = Path.GetFileName(filePath);

            msg.AddAttachment(fileName, fileBase64);
        }

        var response = await client.SendEmailAsync(msg);
        var responseBody = await response.Body.ReadAsStringAsync();

        Console.WriteLine($"SendGrid Response: {response.StatusCode} - {responseBody}");
        return response.StatusCode.ToString();
    }
}

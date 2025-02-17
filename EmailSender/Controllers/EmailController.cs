using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EmailSender.Models;

[Route("api/email")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly EmailService _emailService;

    public EmailController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromForm] EmailRequest request, IFormFile attachment)
    {
        string filePath = null;

        // 🔹 Save the uploaded file temporarily
        if (attachment != null && attachment.Length > 0)
        {
            filePath = Path.Combine(Path.GetTempPath(), attachment.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await attachment.CopyToAsync(stream);
            }
        }

        var response = await _emailService.SendEmailAsync(request.SenderEmail, request.Recipients, request.Subject, request.Body, filePath);

        // 🔹 Cleanup the file after sending the email
        if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        return Ok(new { message = "Email sent", status = response });
    }
}

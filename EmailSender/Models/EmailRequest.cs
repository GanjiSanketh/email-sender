namespace EmailSender.Models
{
    public class EmailRequest
    {
        public string SenderEmail { get; set; }
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}

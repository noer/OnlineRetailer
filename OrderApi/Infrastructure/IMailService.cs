namespace OrderApi.Infrastructure
{
    public interface IMailService
    {
        void sendMessage(string to, string subject, string message);
    }
}
namespace TfsMonitor.Service.Interfaces
{
    public interface INotificationService
    {
        bool SendAlert(IAlertBody alertBody);
    }
}
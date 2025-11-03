namespace IDM.Core.Exceptions;

public class DownloadException : Exception
{
    public Guid TaskId { get; }

    public DownloadException(Guid taskId, string message) : base(message)
    {
        TaskId = taskId;
    }

    public DownloadException(Guid taskId, string message, Exception innerException) : base(message, innerException)
    {
        TaskId = taskId;
    }
}

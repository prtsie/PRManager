namespace PRManager.Notifying.Services.Infrastructure;

public class ChatIdsProvider(long[] ids)
{
    public long[] ChatsToNotify { get; private set; } = ids;
}
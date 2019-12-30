public class ClientInfo
{
    public readonly string roomId;
    public readonly string clientId;

    public ClientInfo(string roomId, string clientId)
    {
        this.roomId = roomId;
        this.clientId = clientId;
    }
}

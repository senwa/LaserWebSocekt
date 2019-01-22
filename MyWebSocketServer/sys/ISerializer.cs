namespace MyWebSocketServer.Sys
{
    public interface ISerializer
    {
        string Serialize(object o);

        T Deserialize<T>(string s);
    }
}

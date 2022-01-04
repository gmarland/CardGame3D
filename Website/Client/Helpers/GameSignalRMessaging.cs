namespace Website.Client.Helpers
{
    public class GameSignalRMessaging
    {
        public static string GetEndpoint()
        {
            return "https://localhost:5001/messaging/gamehub";
        }
    }
}

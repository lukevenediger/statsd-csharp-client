namespace StatsdClient
{
    public static class OutputChannelExtensions
    {
        public static void Send(this IOutputChannel outputChannel, string line)
        {
            outputChannel.SendAsync(line).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
namespace StatsdClient
{
    public static class OutputChannelExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="outputChannel"></param>
        /// <param name="line"></param>
        public static void Send(this IOutputChannel outputChannel, string line)
        {
            outputChannel.SendAsync(line).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
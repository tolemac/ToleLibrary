using System.IO;

namespace ToleLibraries.ApiBus.Requests
{
    public class WebRequest
    {
        public string Url { get; }
        public string Body { get; }

        public WebRequest(string url, string body)
        {
            Url = url;
            Body = body;
        }

        public static WebRequest FromByteArray(string url, Stream body)
        {
            string bodyText = null;
            using (var reader = new StreamReader(body))
            {
                if (!reader.EndOfStream)
                    bodyText = reader.ReadToEnd();
            }
            return new WebRequest(url, bodyText);
        }
    }
}

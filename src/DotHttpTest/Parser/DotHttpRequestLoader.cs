using DotHttpTest.Models;
using DotHttpTest.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Parser
{
    internal class DotHttpRequestLoader
    {
        private readonly ClientOptions mOptions;
        private readonly VariableLoaderState mVariableLoaderState;
        private StringBuilder mContent = new StringBuilder();

        public DotHttpRequestLoader()
        {
            mOptions = ClientOptions.DefaultOptions();
            mVariableLoaderState = new VariableLoaderState(mOptions);
        }
        public DotHttpRequestLoader(ClientOptions options)
        {
            mOptions = options;
            mVariableLoaderState = new VariableLoaderState(mOptions);
        }

        /// <summary>
        /// Loads a single request from the specified lines
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static DotHttpRequest ParseRequest(string[] lines, ClientOptions? options = null)
        {
            options ??= ClientOptions.DefaultOptions();
            var loader = new DotHttpRequestLoader(options);
            return loader.ParseRequestLines(lines);
        }

        public static List<DotHttpRequest> ParseRequests(string[] lines, ClientOptions? options = null)
        {
            options ??= ClientOptions.DefaultOptions();
            var loader = new DotHttpRequestLoader(options);
            return loader.ParseRequestsLines(lines);
        }


        private DotHttpRequest ParseRequestLines(string[] lines)
        {
            return ParseRequestsLines(lines).First();
        }

        private List<DotHttpRequest> ParseRequestsLines(string[] lines)
        {
            var messages = new List<DotHttpRequest>();

            var currentMessage = new DotHttpRequest();
            currentMessage.RequestName = $"request_{messages.Count}";
            messages.Add(currentMessage);

            var tokenizer = new DotHttpTokenizer(lines);
            foreach (var token in tokenizer.TokenizeRequest())
            {
                switch (token.Type)
                {
                    case Models.HttpTokenType.RequestSeparator:
                        // Assign body to the previous message
                        AssignRequestBody(currentMessage);

                        currentMessage = new DotHttpRequest();
                        currentMessage.RequestName = $"request_{messages.Count}";
                        messages.Add(currentMessage);
                        break;

                    default:
                        UpdateRequestMessageWithToken(currentMessage, token);
                        break;
                }
            }

            // Body is read
            AssignRequestBody(currentMessage);

            // If there is a trailing request separation line, we are adding a new message.
            // But if there is no last message we must clean it up here..
            if(messages.Count > 0)
            {
                var last = messages[^1];
                if(!last.HasUrl)
                {
                    // Invalid or trailing separator line
                    messages.RemoveAt(messages.Count-1);
                }
            }

            return messages;
        }

        private DotHttpRequest AssignRequestBody(DotHttpRequest currentMessage)
        {
            currentMessage.Body = Encoding.UTF8.GetBytes(mContent.ToString());
            mContent.Clear();

            return currentMessage;
        }

        private void UpdateRequestMessageWithToken(DotHttpRequest request, HttpToken token)
        {
            var tokenData = ProcessTokenData(token.Data);

            switch(token.Type)
            {
                case HttpTokenType.Comment:
                    CommentDataParser.ParseCommentMetadata(request, tokenData);
                    break;
                case HttpTokenType.VariableAssignmentLine:
                    mVariableLoaderState.ParseVariableAssignmentLine(tokenData);
                    break;
                case HttpTokenType.Method:
                    request.Request.Method = new HttpMethod(tokenData);
                    break;
                case HttpTokenType.HttpVersion:
                    request.Request.Version = ParseHttpVersion(tokenData);
                    break;
                case HttpTokenType.HeaderLine:
                    HttpHeaderParser.ParseHeader(request, tokenData);
                    break;
                case HttpTokenType.BodyLine:
                    mContent.Append(tokenData);
                    mContent.Append('\n');
                    break;
                case HttpTokenType.Url:
                    request.HasUrl = true;
                    ParseRequestUrl(request, tokenData);
                    break;
            }
        }

        private Version ParseHttpVersion(string tokenData)
        {
            if(tokenData.StartsWith("HTTP/"))
            {
                return Version.Parse(tokenData.Substring(5));
            }
            return Version.Parse("1.1");
        }

        private string ProcessTokenData(string data)
        {
            return mVariableLoaderState.ReplaceDataVariables(data);
        }

        private void ParseRequestUrl(DotHttpRequest request, string data)
        {
            if (Uri.TryCreate(data, UriKind.Absolute, out Uri? uri))
            {
                request.Request.RequestUri = uri;
            }
        }
    }
}

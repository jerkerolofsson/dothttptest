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
        private ExpressionList mContent = new ExpressionList();

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
            currentMessage.Body = mContent;
            mContent = new();

            return currentMessage;
        }

        private void UpdateRequestMessageWithToken(DotHttpRequest request, HttpToken token)
        {
            var tokenData = ProcessTokenData(token.Data);

            switch(token.Type)
            {
                case HttpTokenType.Comment:
                    CommentDataParser.ParseCommentMetadata(request, tokenData.ToString());
                    break;
                case HttpTokenType.VariableAssignmentLine:
                    mVariableLoaderState.ParseVariableAssignmentLine(tokenData.ToString());
                    break;
                case HttpTokenType.Method:
                    request.Method = tokenData;
                    break;
                case HttpTokenType.HttpVersion:
                    request.Version = tokenData;
                    break;
                case HttpTokenType.HeaderLine:
                    HttpHeaderParser.ParseHeader(request, tokenData);
                    break;
                case HttpTokenType.BodyLine:
                    if (tokenData != null)
                    {
                        mContent.Add(tokenData);
                        mContent.Add("\n");
                    }
                    break;
                case HttpTokenType.Url:
                    request.HasUrl = true;
                    request.Url = tokenData;
                    break;
            }
        }

        private ExpressionList ProcessTokenData(string data)
        {
            return mVariableLoaderState.ReplaceDataVariables(data);
        }
    }
}

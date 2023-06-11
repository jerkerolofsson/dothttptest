using DotHttpTest.Exceptions;
using DotHttpTest.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Parser
{
    internal class DotHttpTokenizer
    {
        private readonly string[] mLines;
        private int mCurrentLine = 1;
        private TokenizerState mState = TokenizerState.ExpectedRequestHeader;

        public DotHttpTokenizer(string[] lines) 
        {
            mLines = lines;
        }

        public IEnumerable<HttpToken> TokenizeRequest()
        {
            foreach(var line in mLines)
            {
                foreach(var token in ParseRequestLine(line))
                {
                    yield return token;
                }
                mCurrentLine++;
            }
        }

        private IEnumerable<HttpToken> ParseRequestLine(string line)
        {
            if(IsVariableAssignment(line))
            {
                yield return new HttpToken(HttpTokenType.VariableAssignmentLine, line);
                yield break;
            }

            if (IsRequestSeparator(line))
            {
                yield return new HttpToken(HttpTokenType.RequestSeparator, line);
                mState = TokenizerState.ExpectedRequestHeader;
                yield break;
            };

            // If we are reading the body, we ignore all text except request separators
            if(mState == TokenizerState.ExpectBody)
            {
                yield return new HttpToken(HttpTokenType.BodyLine, line);
                yield break;
            }

            // Check for comment. This is done anywhere except when we are reading the body
            if (IsLineComment(line))
            {
                yield return new HttpToken(HttpTokenType.Comment, line);
                yield break;
            };

            if (string.IsNullOrWhiteSpace(line))
            {
                // If we are reading the headers and there is an empty line, then we will continue with the body
                switch (mState)
                {
                    case TokenizerState.ExpectHeaders:
                        mState = TokenizerState.ExpectBody;
                        break;
                }
                yield break;
            }

            switch(mState)
            {
                case TokenizerState.ExpectedRequestHeader:
                    foreach (var token in ParseRequestHeader(mCurrentLine, line))
                    {
                        yield return token;
                    }
                    mState = TokenizerState.ExpectHeaders;
                    break;
                case TokenizerState.ExpectHeaders:
                    foreach (var token in ParseHeader(mCurrentLine, line))
                    {
                        yield return token;
                    }
                    break;
            }
        }

        internal static IEnumerable<HttpToken> ParseRequestHeader(int currentLine, string line)
        {
            var p1 = line.IndexOf(' ');
            var p2 = line.LastIndexOf(' ');
            if(p1 == -1 || p2 == -1) 
            {
                throw new HttpParsingException(currentLine, $"Malformed HTTP request: '{line}'");
            }

            var left = line.Substring(0, p1);

            var middleLength = p2 - p1 - 1;
            var middle = line.Substring(p1+1, middleLength);
            var right = line.Substring(p2+1);

            yield return new HttpToken(HttpTokenType.Method, left);
            yield return new HttpToken(HttpTokenType.Url, middle);
            yield return new HttpToken(HttpTokenType.HttpVersion, right);
        }


        internal static IEnumerable<HttpToken> ParseHeader(int currentLine, string line)
        {
            var p1 = line.IndexOf(':');
            if (p1 == -1)
            {
                throw new HttpParsingException(currentLine, $"Malformed HTTP header: '{line}'");
            }

            yield return new HttpToken(HttpTokenType.HeaderLine, line);
        }

        internal static bool IsRequestSeparator(string line)
        {
            var trimmedLine = line.Trim();
            return trimmedLine.StartsWith("###");
        }
        internal static bool IsLineComment(string line)
        {
            var trimmedLine = line.Trim();

            if(trimmedLine.StartsWith("###"))
            {
                // This is a request separator line
                return false;
            }

            return trimmedLine.StartsWith("#") || trimmedLine.StartsWith("//");
        }

        internal static bool IsVariableAssignment(string line)
        {
            var trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("@") && trimmedLine.Contains('='))
            {
                return true;
            }

            return false;
        }
    }
}

using DotHttpTest.Verification.Models;
using DotHttpTest.Verification.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class DotHttpRequest
    {
        /// <summary>
        /// Collection of various metadata
        /// </summary>
        private Dictionary<string, string?> mMetadata = new();

        /// <summary>
        /// Stage data for soak/stress testing
        /// </summary>
        private List<StageAttributes> mStages = new();

        /// <summary>
        /// Verification checks to be performed when the response is received
        /// </summary>
        private List<VerificationCheck> mVerificationChecks = new();

        /// <summary>
        /// The name of the request. This can be loaded from a comment that looks like this
        /// # @name MyRequestName
        /// </summary>
        public string? RequestName { get; set; }

        /// <summary>
        /// The URL for the request
        /// </summary>
        public ExpressionList? Url { get; internal set; }

        /// <summary>
        /// HTTP headers
        /// </summary>
        public ExpressionListHeaderCollection Headers { get; internal set; } = new();

        /// <summary>
        /// The requested HTTP method
        /// </summary>
        public ExpressionList? Method { get; internal set; }

        /// <summary>
        /// The HTTP version in the request
        /// </summary>
        public ExpressionList? Version { get; internal set; } = ExpressionList.Create("HTTP/1.1");

        public bool HasStages => mStages.Any();

        /// <summary>
        /// Returns all stages
        /// </summary>
        public IReadOnlyList<StageAttributes> Stages => mStages.AsReadOnly();
        internal IReadOnlyList<VerificationCheck> VerificationChecks => mVerificationChecks.AsReadOnly();

        /// <summary>
        /// If set to a positive value, there will be a delay after this request is sent
        /// </summary>
        public TimeSpan DelayAfterRequest { get; set; } = TimeSpan.Zero;
        //public byte[] Body { get; internal set; } = Array.Empty<byte>();

        public ExpressionList? Body { get; internal set; } = null;

        public bool HasUrl { get; internal set; }

        public string GetBodyAsText()
        {
            return GetBodyAsText(Encoding.UTF8);
        }
        public string GetBodyAsText(Encoding encoding)
        {
            if(Body is null)
            {
                return string.Empty;
            }
            return Body.ToString();
        }

        /// <summary>
        /// Sets a metadata field from a comment that looks like this
        /// # @metaValue
        /// </summary>
        /// <param name="metaValue"></param>
        internal void SetMeta(string metaValue)
        {
            mMetadata[metaValue] = null;
        }

        /// <summary>
        /// Sets a metadata field from a comment that looks like this
        /// # @name val
        /// </summary>
        /// <param name="metaValue"></param>
        internal void SetMeta(string name, string val)
        {
            var nameLower = name.ToLower();

            if (nameLower.StartsWith("stage"))
            {
                AddStage(val);
            }
            else if (nameLower.StartsWith("verify"))
            {
                AddVerificationCheck(val);
            }
            else
            {
                switch (nameLower)
                {
                    case "name":
                        RequestName = val;
                        break;
                    case "delay":
                        if(int.TryParse(val, out int millis))
                        {
                            DelayAfterRequest = TimeSpan.FromMilliseconds(millis);
                        }
                        break;
                    default:
                        mMetadata[name] = val;
                        break;
                }
            }
        }

        private void AddStage(string val)
        {
            mStages.Add(StageParser.Parse(val));
        }
        private void AddVerificationCheck(string val)
        {
            mVerificationChecks.Add(VerificationCheckParser.Parse(val));
        }

        public static List<DotHttpRequest> FromFile(string httpFilePath, ClientOptions? options = null)
        {
            var request = DotHttpRequestLoader.ParseRequests(File.ReadAllLines(httpFilePath), options);
            return request;
        }
    }
}

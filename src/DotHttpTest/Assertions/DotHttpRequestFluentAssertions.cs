using FluentAssertions.Execution;
using FluentAssertions;
using FluentAssertions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotHttpTest.Converters;
using System.Net.Http.Headers;

namespace DotHttpTest.Assertions
{
    public class DotHttpRequestFluentAssertions : ReferenceTypeAssertions<DotHttpRequest, DotHttpRequestFluentAssertions>
    {
        protected override string Identifier => ".http";

        public DotHttpRequestFluentAssertions(DotHttpRequest subject) : base(subject)
        {
        }

        public AndConstraint<DotHttpRequestFluentAssertions> ContainHeader(
            string name, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(!string.IsNullOrEmpty(name))
                .FailWith("You can't assert that a header exists with a null value")
                .Then
                .Given(() => Subject.ToHttpRequestMessage(null).Headers)
                .ForCondition(headers => headers.Any(header => header.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                .FailWith($"Did not find the header {name}");

            return new AndConstraint<DotHttpRequestFluentAssertions>(this);
        }

        public AndConstraint<DotHttpRequestFluentAssertions> HaveContentMediaType(
            string mediaType, string because = "", params object[] becauseArgs)
        {

            var requestMessage = Subject.ToHttpRequestMessage(null);

            var content = requestMessage.Content;
            MediaTypeHeaderValue? contentType = null;
            string? actualMediaType = null;

            if(content?.Headers.ContentType != null)
            {
                contentType = content.Headers.ContentType;
                actualMediaType = contentType.MediaType;
            }

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(content != null)
                .FailWith("HttpRequestMessage.Content is null")
                .Then
                .ForCondition(contentType != null)
                .FailWith("HttpRequestMessage.Content.ContentType is null")
                .Then
                .ForCondition(actualMediaType != null)
                .FailWith("HttpRequestMessage.Content.MediaType is null")
                .Then
                .ForCondition(mediaType.Equals(actualMediaType))
                .FailWith($"Expected '{mediaType}' but got '{actualMediaType}'");

            return new AndConstraint<DotHttpRequestFluentAssertions>(this);
        }


        public AndConstraint<DotHttpRequestFluentAssertions> HaveMethod(
            string method, string because = "", params object[] becauseArgs)
        {
            return HaveMethod(new HttpMethod(method), because, becauseArgs);
        }
        public AndConstraint<DotHttpRequestFluentAssertions> HaveMethod(
            HttpMethod method, string because = "", params object[] becauseArgs)
        {
            var request = Subject.ToHttpRequestMessage(null);
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Method is not null)
                .FailWith("Request Method is null")
                .Then
                .ForCondition(request.Method!.Equals(method))
                .FailWith($"Request Method was {request.Method} but expected {method}");

            return new AndConstraint<DotHttpRequestFluentAssertions>(this);
        }

        public AndConstraint<DotHttpRequestFluentAssertions> HaveRequestUri(
           string url, string because = "", params object[] becauseArgs)
        {
            return HaveRequestUri(new Uri(url), because, becauseArgs);
        }
        public AndConstraint<DotHttpRequestFluentAssertions> HaveRequestUri(
           Uri url, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Url is not null)
                .FailWith("Request URI is null")
                .Then
                .ForCondition(Subject.Url!.ToString().Equals(url.ToString()))
                .FailWith($"Request URI was {Subject.Url.ToString()} but expected {url}");

            return new AndConstraint<DotHttpRequestFluentAssertions>(this);
        }


        public AndConstraint<DotHttpRequestFluentAssertions> ContainHeaderWithValue(
            string name, string value, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(!string.IsNullOrEmpty(name))
                .FailWith("You can't assert that a header exists with a null value")
                .Then
                .Given(() => Subject.ToHttpRequestMessage(null).Headers)
                .ForCondition(headers => 
                        headers.Any(
                            header => 
                                header.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase) && 
                                header.Value.Contains(value)))
                .FailWith($"Did not find the header {name} with value {value}");

            return new AndConstraint<DotHttpRequestFluentAssertions>(this);
        }
    }
}

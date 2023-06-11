using FluentAssertions.Execution;
using FluentAssertions;
using FluentAssertions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                .Given(() => Subject.Request.Headers)
                .ForCondition(headers => headers.Any(header => header.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                .FailWith($"Did not find the header {name}");

            return new AndConstraint<DotHttpRequestFluentAssertions>(this);
        }

        public AndConstraint<DotHttpRequestFluentAssertions> HaveContentMediaType(
            string mediaType, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Request.Content == null)
                .FailWith("Request Content is not set")
                .Then
                .ForCondition(Subject.Request.Content?.Headers == null)
                .FailWith("Content Headers is not set")
                .Then
                .ForCondition(Subject.Request.Content?.Headers?.ContentType == null)
                .FailWith("Content Type is not set")
                .Then
                .ForCondition(Subject.Request.Content?.Headers?.ContentType?.MediaType == null)
                .FailWith("Content MediaType is not set")
                .Then
                .ForCondition(Subject.Request.Content?.Headers?.ContentType?.MediaType != mediaType)
                .FailWith("Content MediaType is not set");

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
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Request.Method is not null)
                .FailWith("Request Method is null")
                .Then
                .ForCondition(Subject.Request.Method!.Equals(method))
                .FailWith($"Request Method was {Subject.Request.Method} but expected {method}");

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
                .ForCondition(Subject.Request.RequestUri is not null)
                .FailWith("Request URI is null")
                .Then
                .ForCondition(Subject.Request.RequestUri!.Equals(url))
                .FailWith($"Request URI was {Subject.Request.RequestUri} but expected {url}");

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
                .Given(() => Subject.Request.Headers)
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

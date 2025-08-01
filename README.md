# dothttptest
.NET Test Framework for API tests with .http files

Supporting HTTP and MCP

## Why another HTTP test framework?

I like .http files. They make development and debugging easy. 
With integration in VS Code, Visual Studio and Rider I like that I don't have to leave my editor to run them. 

They are simple and efficient. Can be shared within the development team. They can be shared with a QA team.

As a developer, I also don't like duplicating work. I was searching for a test framework where I can re-use my .http files for API testing, but I didn't find anything that allowed me to do it using C#.

## MCP

To test MCP, set the VERSION in the request header to MCP

### List tools

```http
LIST  http://localhost/mcp MCP
```

This will verify that tools can be listed

#### Verify that a tool exists

```http
# @verify tool browser_close exists
LIST http://localhost/mcp MCP
```
The "tool" verifier will evaluate the MCP ListToolsResult. This example will check if there is a tool with the name "browser_close" in the result.

#### Verify that a tool parameter is declared

```http
# @verify tool browser_navigate.inputSchema.properties.url exists
LIST http://localhost/mcp MCP
```

#### Using json verifier for MCP tool listing

You can also use the json verifier for advanced evaluation of the list tools result.
The content of the tool listing is an array of tools according to MCP schema.

### Invoke a MCP tool

To call a tool use the CALL method with the tool name in the request URL after a #

```http
CALL http://localhost/mcp#browser_close MCP
```

#### Invoke a tool with parameters 

To invoke a tool with parameters, use the CALL method with the tool name in the request URL after a # and pass the parameters in the body of the request:
```http
CALL http://localhost/mcp#browser_navigate MCP

{
	"url": "https://www.github.com"
}
```

## Extensions in .http file

dothttptest supports extensions in the form of .http file comments, allowing the same file to be used within existing tools without breaking compliance, while also allowing automating tests with the same file.
All extensions including those that add verification checks are added as instructions within a comment similarily to how some .http formats allow setting a name for a request:
```http
# @name MyRequestName
GET http://localhost/get HTTP/1.1
```

### Verification checks within .http files

A verification check can be added in a similar way to @name by using the @verify command:
```http
# @verify http status-code 200
GET http://localhost/get HTTP/1.1
```

The @verify command is followed by the module performing the verification. Additional modules can be added to support additional checks.

Verifiers can be created in code:
```csharp
[ResponseVerifier("myVerifier")]
public class MyVerifier : IVerifier
{
	public void Verify(DotHttpResponse response, VerificationCheckResult result)
	{
		// ...
	}
}
```
.. and used within the .http file by speciying the same name as was specified in code on the ResponseVerifier attribute on the class.

#### Verify that an HTTP header exists in the response
```http
# @verify header content-type exists
GET http://localhost/get HTTP/1.1
```

#### Verify that an HTTP header value exists in a response
```http
# @verify header accept-ranges == bytes
GET http://localhost/get HTTP/1.1
```

#### Verify a JSON property 
```http
# @verify json PropertyName == Value
GET http://localhost/get HTTP/1.1
```

#### Use a JSON property in the next request
```http
GET http://localhost/get HTTP/1.1

PATCH http://localhost/post/{{$json.PropertyName}}

{
	"UpdatedField": "123"
}
```


## Usage

### CLI Example Usage

```
$ dothttp run <httpfile>
```
CLI will run the requests specified in the .http and generate a junit-xml as output.

### Adding .NET package

#### Add package using .NET CLI
```
dotnet add package DotHttpTest
```

### C# Example Usage

#### Running all requests in a .http file and getting the result

```csharp
using DotHttpTest;

var runner = new TestPlanRunnerOptionsBuilder()
		.LoadHttpFile(pathToHttpFile)
        .Build();

var testStatus = await runner.RunAsync();
Console.WriteLine($"Requests failed: {testStatus.HttpRequestFails.Value} / {testStatus.HttpRequests.Value}");
```

#### Running a single request

```csharp
using DotHttpTest;
using var client = new DotHttpClient();
var requests = client.LoadFile("testfile.http");
var request = requests.First();

var response = await client.SendAsync(request);

System.Net.Http.HttpResponseMessage httpResponse = response.AsHttpResponseMessage();
Console.WriteLine($"{httpResponse.IsSuccessStatusCode}");

```

## Running stress, soak and iterative tests

dothttptest implements stress and soak inspired by K6. 
Stages can be defined on a request including concurrency (virtual users) and a duration.
```http
# @stage ramp-up   duration:20s target:10
# @stage main      duration:10m target:10
# @stage ramp-down duration:20s target:0
GET http://localhost/get HTTP/1.1
```

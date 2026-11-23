namespace RestClient.Net.FsTest

open System
open System.Net
open System.Net.Http
open System.Threading.Tasks
open Microsoft.VisualStudio.TestTools.UnitTesting
open RestClient.Net
open RestClient.Net.Utilities
open RestClient.Net.CsTest.Fakes
open RestClient.Net.CsTest.Utilities
open System.Text.Json
open Urls
open Outcome

#nowarn "3265"

type MyErrorModel = { message: string }

module Patterns =
    let (|Success|Failure|) (result: Result<'T, HttpError<MyErrorModel>>) =
        match result with
        | :? Result<'T, HttpError<MyErrorModel>>.Ok<'T, HttpError<MyErrorModel>> as ok -> Success ok.Value
        | :? Result<'T, HttpError<MyErrorModel>>.Error<'T, HttpError<MyErrorModel>> as err -> Failure err.Value
        | _ -> failwith "Unexpected result type"

    let (|ErrorResponse|ExceptionError|) (error: HttpError<MyErrorModel>) =
        match error with
        | :? HttpError<MyErrorModel>.ErrorResponseError as err -> ErrorResponse err
        | :? HttpError<MyErrorModel>.ExceptionError as err -> ExceptionError err
        | _ -> failwith "Unexpected error type"

open Patterns

module Helpers =
    let normalizeJson json =
        JsonSerializer.Deserialize<JsonElement>(json: string)
        |> fun e -> JsonSerializer.Serialize(e, JsonSerializerOptions(WriteIndented = false))

    let deserializeSuccess response ct =
        TestDeserializer.Deserialize<string>(response, ct)

    let deserializeError response ct =
        TestDeserializer.Deserialize<MyErrorModel>(response, ct)

    let createFactory useRealApi response =
        if useRealApi then
            FakeHttpClientFactory.CreateHttpClientFactory true
        else
            FakeHttpClientFactory.CreateMockHttpClientFactory(response = response)

    let assertJson expected actual =
        Assert.AreEqual(normalizeJson expected, normalizeJson actual)

    let assertSuccessResult expected (result: Result<string, HttpError<MyErrorModel>>) =
        Assert.IsTrue(result.IsOk)
        Assert.IsFalse(result.IsError)
        let mutable matchCalls = 0

        let actual =
            result.Match(
                (fun value ->
                    matchCalls <- matchCalls + 1
                    assertJson expected value
                    value),
                (fun _ ->
                    Assert.Fail "Success invoked the failure callback"
                    "")
            )

        Assert.AreEqual(1, matchCalls)
        assertJson expected actual

        let mutable tapCalls = 0

        let tapped =
            result.Tap(
                onSuccess =
                    (fun value ->
                        tapCalls <- tapCalls + 1
                        assertJson expected value),
                onError = (fun _ -> Assert.Fail "Success invoked the error tap")
            )

        Assert.AreSame(result, tapped)
        Assert.AreEqual(1, tapCalls)

        let mutable mapCalls = 0

        let mapped =
            result.Map(fun value ->
                mapCalls <- mapCalls + 1
                normalizeJson value)

        Assert.IsTrue(mapped.IsOk)
        Assert.IsFalse(mapped.IsError)
        Assert.AreEqual(normalizeJson expected, mapped.GetValueOrDefault("unexpected fallback"))
        Assert.AreEqual(1, mapCalls)

        let unchanged =
            result.MapError(fun _ -> failwith "Success invoked the error mapper")

        Assert.IsTrue(unchanged.IsOk)
        assertJson expected (unchanged.GetValueOrDefault("unexpected fallback"))
        assertJson expected (result.GetValueOrDefault(fun () -> failwith "Success invoked fallback"))

    let assertFailureResult (result: Result<string, HttpError<MyErrorModel>>) =
        Assert.IsFalse(result.IsOk)
        Assert.IsTrue(result.IsError)
        let mutable matchCalls = 0

        let original =
            result.Match(
                (fun _ -> failwith "Failure invoked the success callback"),
                (fun error ->
                    matchCalls <- matchCalls + 1
                    error)
            )

        Assert.AreEqual(1, matchCalls)

        let mutable tapCalls = 0

        let tapped =
            result.Tap(
                onSuccess = (fun _ -> Assert.Fail "Failure invoked the success tap"),
                onError =
                    (fun error ->
                        tapCalls <- tapCalls + 1
                        Assert.AreSame(original, error))
            )

        Assert.AreSame(result, tapped)
        Assert.AreEqual(1, tapCalls)
        let mapped = result.Map(fun _ -> failwith "Failure invoked success mapper")
        Assert.IsTrue(mapped.IsError)
        Assert.IsFalse(mapped.IsOk)
        let mutable errorMaps = 0

        let mappedError =
            result.MapError(fun error ->
                errorMaps <- errorMaps + 1
                Assert.AreSame(original, error)
                "mapped error")

        Assert.IsTrue(mappedError.IsError)
        Assert.AreEqual(1, errorMaps)
        Assert.AreEqual("mapped error", mappedError.Match((fun _ -> "unexpected"), id))
        let mutable fallbackCalls = 0

        let fallback =
            result.GetValueOrDefault(fun () ->
                fallbackCalls <- fallbackCalls + 1
                "recovered")

        Assert.AreEqual("recovered", fallback)
        Assert.AreEqual(1, fallbackCalls)
        Assert.AreSame(original, result.Match((fun _ -> failwith "Expected error"), id))

    let failUnexpected =
        function
        | ErrorResponse _ -> Assert.Fail "Expected success, got ErrorResponse"
        | ExceptionError _ -> Assert.Fail "Expected success, got ExceptionError"

open Helpers

[<TestClass>]
type HttpClientFactoryExtensionsTests() =
    let post1Json =
        """{"userId": 1,"id": 1,"title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit","body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"}"""

    [<TestMethod>]
    [<DataTestMethod>]
    [<DataRow(true)>]
    [<DataRow(false)>]
    member _.``GetAsync returns success result``(useRealApi: bool) : Task =
        task {
            let response =
                new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(post1Json))

            let factory = createFactory useRealApi response

            let! result =
                factory.GetAsync(
                    "JsonPlaceholderClient",
                    "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
                    deserializeSuccess,
                    deserializeError
                )

            match result with
            | Success value ->
                assertJson post1Json value
                assertSuccessResult post1Json result
            | Failure error -> failUnexpected error
        }

    [<TestMethod>]
    [<DataTestMethod>]
    [<DataRow(true)>]
    [<DataRow(false)>]
    member _.``PutAsync returns success result``(useRealApi: bool) : Task =
        task {
            let expected = """{"id": 1}"""

            let response =
                new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(expected))

            let factory = createFactory useRealApi response

            let body =
                new ProgressReportingHttpContent(
                    """{"id": 1,"title": "Updated Title","body": "Updated body","userId": 1}"""
                )

            let! result =
                factory.PutAsync(
                    "JsonPlaceholderClient",
                    "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
                    body,
                    deserializeSuccess,
                    deserializeError
                )

            match result with
            | Success value ->
                assertJson expected value
                assertSuccessResult expected result
            | Failure error -> failUnexpected error
        }

    [<TestMethod>]
    [<DataTestMethod>]
    [<DataRow(true)>]
    [<DataRow(false)>]
    member _.``PostAsync returns success result``(useRealApi: bool) : Task =
        task {
            let expected = """{"id": 101}"""

            let response =
                new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(expected))

            let factory = createFactory useRealApi response

            let body =
                new ProgressReportingHttpContent(
                    """{"title": "New Post","body": "This is the body of the new post","userId": 1}"""
                )

            let! result =
                factory.PostAsync(
                    "JsonPlaceholderClient",
                    "https://jsonplaceholder.typicode.com/posts".ToAbsoluteUrl(),
                    body,
                    deserializeSuccess,
                    deserializeError
                )

            match result with
            | Success value ->
                assertJson expected value
                assertSuccessResult expected result
            | Failure error -> failUnexpected error
        }

    [<TestMethod>]
    [<DataTestMethod>]
    [<DataRow(true)>]
    [<DataRow(false)>]
    member _.``DeleteAsync returns success result``(useRealApi: bool) : Task =
        task {
            let response =
                new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent("{}"))

            let factory = createFactory useRealApi response

            let! result =
                factory.DeleteAsync(
                    "JsonPlaceholderClient",
                    "https://jsonplaceholder.typicode.com/posts/1".ToAbsoluteUrl(),
                    deserializeSuccess,
                    deserializeError
                )

            match result with
            | Success value ->
                Assert.AreEqual("{}", value)
                assertSuccessResult "{}" result
            | Failure error -> failUnexpected error
        }

    [<TestMethod>]
    member _.``GetAsync returns failure result on error response``() : Task =
        task {
            let response =
                new HttpResponseMessage(
                    HttpStatusCode.BadRequest,
                    Content = new StringContent("""{"message":"Error occurred"}""")
                )

            response.Headers.Add("X-Custom-Header", "CustomValue")
            let factory = FakeHttpClientFactory.CreateMockHttpClientFactory(response = response)

            let! result =
                factory.SendAsync<string, MyErrorModel>(
                    "TestClient",
                    "http://test.com".ToAbsoluteUrl(),
                    HttpMethod.Get,
                    deserializeSuccess,
                    deserializeError
                )

            match result with
            | Success _ -> Assert.Fail "Expected failure, got success"
            | Failure(ErrorResponse err) ->
                assertFailureResult result
                Assert.IsTrue(err.IsErrorResponse)
                Assert.IsFalse(err.IsExceptionError)
                Assert.AreSame(response.Headers, err.Headers)
                Assert.AreEqual(1, err.Headers |> Seq.length)
                Assert.AreEqual(HttpStatusCode.BadRequest, err.StatusCode)
                Assert.AreEqual("Error occurred", err.Body.message)
                Assert.IsTrue(err.Headers.Contains "X-Custom-Header")
                Assert.AreEqual("CustomValue", err.Headers.GetValues("X-Custom-Header") |> Seq.head)
            | Failure(ExceptionError _) -> Assert.Fail "Expected ErrorResponse, got ExceptionError"
        }

    [<TestMethod>]
    member _.``GetAsync returns failure result on exception``() : Task =
        task {
            let expectedException = new Exception("Network failure")

            let factory =
                FakeHttpClientFactory.CreateMockHttpClientFactory(exceptionToThrow = expectedException)

            let! result =
                factory.SendAsync<string, MyErrorModel>(
                    "TestClient",
                    "http://test.com".ToAbsoluteUrl(),
                    HttpMethod.Get,
                    deserializeSuccess,
                    deserializeError
                )

            match result with
            | Success _ -> Assert.Fail "Expected failure, got success"
            | Failure(ExceptionError err) ->
                assertFailureResult result
                Assert.AreEqual("Network failure", err.Exception.Message)
                Assert.AreSame(expectedException, err.Exception)
                Assert.IsTrue(err.IsExceptionError)
                Assert.IsFalse(err.IsErrorResponse)
                Assert.IsNull(err.Exception.InnerException)
            | Failure(ErrorResponse _) -> Assert.Fail "Expected ExceptionError, got ErrorResponse"
        }

module internal Marker =
    let private dummy = ()

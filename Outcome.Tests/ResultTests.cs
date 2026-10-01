namespace Outcome.Tests;

[TestClass]
public class ResultTests
{
    [TestMethod]
    public void Ok_IsOk_ReturnsTrue()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        Assert.IsTrue(result.IsOk);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void Ok_IsError_ReturnsFalse()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        Assert.IsFalse(result.IsError);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void Error_IsOk_ReturnsFalse()
    {
        var result = Result<int, string>.Failure("error");
        Assert.IsFalse(result.IsOk);

        OutcomeAssertions.Error(result, "error");
    }

    [TestMethod]
    public void Error_IsError_ReturnsTrue()
    {
        var result = Result<int, string>.Failure("error");
        Assert.IsTrue(result.IsError);

        OutcomeAssertions.Error(result, "error");
    }

    [TestMethod]
    public void Failure_CreatesErrorResult()
    {
        var result = Result<int, string>.Failure("test error");
        Assert.IsTrue(result is Result<int, string>.Error<int, string>);
        var error = (Result<int, string>.Error<int, string>)result;
        Assert.AreEqual("test error", error.Value);

        OutcomeAssertions.Error(result, "test error");
    }

    [TestMethod]
    public void Map_OnOk_TransformsValue()
    {
        var result = new Result<int, string>.Ok<int, string>(5);
        var mapped = result.Map(static x => x * 2);
        Assert.IsTrue(mapped is Result<int, string>.Ok<int, string>);
        var ok = (Result<int, string>.Ok<int, string>)mapped;
        Assert.AreEqual(10, ok.Value);

        OutcomeAssertions.Success(result, 5);
        OutcomeAssertions.Success(mapped, 10);
    }

    [TestMethod]
    public void Map_OnError_PropagatesError()
    {
        var result = Result<int, string>.Failure("error");
        var mapped = result.Map(static x => x * 2);
        Assert.IsTrue(mapped is Result<int, string>.Error<int, string>);
        var error = (Result<int, string>.Error<int, string>)mapped;
        Assert.AreEqual("error", error.Value);

        OutcomeAssertions.Error(result, "error");
        OutcomeAssertions.Error(mapped, "error");
    }

    [TestMethod]
    public void Map_ChangesSuccessType()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var mapped = result.Map(x => x.ToString(System.Globalization.CultureInfo.InvariantCulture));
        var value = mapped.Match(s => s, _ => string.Empty);
        Assert.AreEqual("42", value);

        OutcomeAssertions.Success(result, 42);
        OutcomeAssertions.Success(mapped, "42");
    }

    [TestMethod]
    public void MapError_OnOk_PropagatesSuccess()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var mapped = result.MapError(e =>
            int.Parse(e, System.Globalization.CultureInfo.InvariantCulture)
        );
        var value = mapped.Match(s => s, _ => 0);
        Assert.AreEqual(42, value);

        OutcomeAssertions.Success(result, 42);
        OutcomeAssertions.Success(mapped, 42);
    }

    [TestMethod]
    public void MapError_OnError_TransformsError()
    {
        var result = Result<int, string>.Failure("123");
        var mapped = result.MapError(e =>
            int.Parse(e, System.Globalization.CultureInfo.InvariantCulture)
        );
        Assert.IsTrue(mapped is Result<int, int>.Error<int, int>);
        var error = (Result<int, int>.Error<int, int>)mapped;
        Assert.AreEqual(123, error.Value);

        OutcomeAssertions.Error(result, "123");
        OutcomeAssertions.Error(mapped, 123);
    }

    [TestMethod]
    public void Bind_OnOk_AppliesBinder()
    {
        var result = new Result<int, string>.Ok<int, string>(5);
        var bound = result.Bind(static x => new Result<int, string>.Ok<int, string>(x * 2));
        Assert.IsTrue(bound is Result<int, string>.Ok<int, string>);
        var ok = (Result<int, string>.Ok<int, string>)bound;
        Assert.AreEqual(10, ok.Value);

        OutcomeAssertions.Success(result, 5);
        OutcomeAssertions.Success(bound, 10);
    }

    [TestMethod]
    public void Bind_OnOk_CanReturnError()
    {
        var result = new Result<int, string>.Ok<int, string>(5);
        var bound = result.Bind(static x => Result<int, string>.Failure("error from binder"));
        Assert.IsTrue(bound is Result<int, string>.Error<int, string>);
        var error = (Result<int, string>.Error<int, string>)bound;
        Assert.AreEqual("error from binder", error.Value);

        OutcomeAssertions.Success(result, 5);
        OutcomeAssertions.Error(bound, "error from binder");
    }

    [TestMethod]
    public void Bind_OnError_PropagatesError()
    {
        var result = Result<int, string>.Failure("original error");
        var bound = result.Bind(static x => new Result<int, string>.Ok<int, string>(x * 2));
        Assert.IsTrue(bound is Result<int, string>.Error<int, string>);
        var error = (Result<int, string>.Error<int, string>)bound;
        Assert.AreEqual("original error", error.Value);

        OutcomeAssertions.Error(result, "original error");
        OutcomeAssertions.Error(bound, "original error");
    }

    [TestMethod]
    public void Bind_ChangesSuccessType()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var bound = result.Bind(static x => new Result<string, string>.Ok<string, string>(
            x.ToString(System.Globalization.CultureInfo.InvariantCulture)
        ));
        Assert.IsTrue(bound is Result<string, string>.Ok<string, string>);
        var ok = (Result<string, string>.Ok<string, string>)bound;
        Assert.AreEqual("42", ok.Value);

        OutcomeAssertions.Success(result, 42);
        OutcomeAssertions.Success(bound, "42");
    }

    [TestMethod]
    public void Match_OnOk_CallsOnSuccess()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var matched = result.Match(
            onSuccess: static x => $"Success: {x}",
            onError: static e => $"Error: {e}"
        );
        Assert.AreEqual("Success: 42", matched);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void Match_OnError_CallsOnError()
    {
        var result = Result<int, string>.Failure("test error");
        var matched = result.Match(
            onSuccess: static x => $"Success: {x}",
            onError: static e => $"Error: {e}"
        );
        Assert.AreEqual("Error: test error", matched);

        OutcomeAssertions.Error(result, "test error");
    }

    [TestMethod]
    public void Tap_OnOk_InvokesOnSuccess()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var tapped = false;
        var returnedResult = result.Tap(onSuccess: _ => tapped = true);
        Assert.IsTrue(tapped);
        Assert.AreSame(result, returnedResult);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void Tap_OnError_InvokesOnError()
    {
        var result = Result<int, string>.Failure("error");
        var tapped = false;
        var returnedResult = result.Tap(onError: _ => tapped = true);
        Assert.IsTrue(tapped);
        Assert.AreSame(result, returnedResult);

        OutcomeAssertions.Error(result, "error");
    }

    [TestMethod]
    public void Tap_WithNullActions_DoesNotThrow()
    {
        var okResult = new Result<int, string>.Ok<int, string>(42);
        var errorResult = Result<int, string>.Failure("error");

        var ok = okResult.Tap();
        var err = errorResult.Tap();

        Assert.AreSame(okResult, ok);
        Assert.AreSame(errorResult, err);

        OutcomeAssertions.Success(ok, 42);
        OutcomeAssertions.Error(err, "error");
    }

    [TestMethod]
    public void Tap_OnOk_DoesNotInvokeOnError()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var errorInvoked = false;
        _ = result.Tap(onError: _ => errorInvoked = true);
        Assert.IsFalse(errorInvoked);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void Tap_OnError_DoesNotInvokeOnSuccess()
    {
        var result = Result<int, string>.Failure("error");
        var successInvoked = false;
        _ = result.Tap(onSuccess: _ => successInvoked = true);
        Assert.IsFalse(successInvoked);

        OutcomeAssertions.Error(result, "error");
    }

    [TestMethod]
    public void GetValueOrDefault_OnOk_ReturnsValue()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var value = result.GetValueOrDefault(0);
        Assert.AreEqual(42, value);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void GetValueOrDefault_OnError_ReturnsDefault()
    {
        var result = Result<int, string>.Failure("error");
        var value = result.GetValueOrDefault(99);
        Assert.AreEqual(99, value);

        OutcomeAssertions.Error(result, "error");
    }

    [TestMethod]
    public void GetValueOrDefault_WithProvider_OnOk_ReturnsValue()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var providerCalled = false;
        var value = result.GetValueOrDefault(() =>
        {
            providerCalled = true;
            return 99;
        });
        Assert.AreEqual(42, value);
        Assert.IsFalse(providerCalled);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void GetValueOrDefault_WithProvider_OnError_CallsProvider()
    {
        var result = Result<int, string>.Failure("error");
        var providerCalled = false;
        var value = result.GetValueOrDefault(() =>
        {
            providerCalled = true;
            return 99;
        });
        Assert.AreEqual(99, value);
        Assert.IsTrue(providerCalled);

        OutcomeAssertions.Error(result, "error");
    }

    [TestMethod]
    public void UnaryNegation_OnError_ReturnsError()
    {
        var result = Result<int, string>.Failure("test error");
        var error = !result;
        Assert.AreEqual("test error", error);

        OutcomeAssertions.Error(result, "test error");
    }

    [TestMethod]
    public void UnaryNegation_OnOk_Throws()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var exception = Assert.ThrowsException<InvalidOperationException>(() => !result);
        Assert.AreEqual("Expected error result", exception.Message);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void UnaryPlus_OnOk_ReturnsValue()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var value = +result;
        Assert.AreEqual(42, value);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void UnaryPlus_OnError_Throws()
    {
        var result = Result<int, string>.Failure("error");
        var exception = Assert.ThrowsException<InvalidOperationException>(() => +result);
        Assert.AreEqual("Expected success result", exception.Message);

        OutcomeAssertions.Error(result, "error");
    }

    [TestMethod]
    public void Ok_ToString_ReturnsFormattedString()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        Assert.AreEqual("Ok(42)", result.ToString());

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void Error_ToString_ReturnsFormattedString()
    {
        var result = new Result<int, string>.Error<int, string>("test error");
        Assert.AreEqual("Error(test error)", result.ToString());

        OutcomeAssertions.Error(result, "test error");
    }

    [TestMethod]
    public void Error_FromTFailure_CreatesError()
    {
        var error = Result<int, string>.Error<int, string>.FromTFailure("test");
        Assert.AreEqual("test", error.Value);
        Assert.IsInstanceOfType<Result<int, string>.Error<int, string>>(error);
        Assert.IsTrue(error.IsError);
        Assert.IsFalse(error.IsOk);

        OutcomeAssertions.Error(error, "test");
    }

    [TestMethod]
    public void Error_FromTFailure_WithComplexType_CreatesError()
    {
        var exception = new InvalidOperationException("test exception");
        var error = Result<int, Exception>.Error<int, Exception>.FromTFailure(exception);
        Assert.AreEqual(exception, error.Value);
        Assert.AreEqual("test exception", error.Value.Message);
        Assert.IsInstanceOfType<Result<int, Exception>.Error<int, Exception>>(error);

        OutcomeAssertions.Error(error, exception);
        Assert.AreSame(exception, !error);
    }

    [TestMethod]
    public void Error_Constructor_CreatesError()
    {
        var error = new Result<int, string>.Error<int, string>("direct construction");
        Assert.AreEqual("direct construction", error.Value);
        Assert.IsTrue(error.IsError);
        Assert.IsFalse(error.IsOk);
        Assert.AreEqual("Error(direct construction)", error.ToString());

        OutcomeAssertions.Error(error, "direct construction");
    }

    [TestMethod]
    public void Error_WithNullValue_AllowsNull()
    {
        var error = new Result<int, string?>.Error<int, string?>(null);
        Assert.IsNull(error.Value);
        Assert.AreEqual("Error()", error.ToString());

        OutcomeAssertions.Error(error, null);
    }

    [TestMethod]
    public void Error_Equality_ComparesValues()
    {
        var error1 = new Result<int, string>.Error<int, string>("same");
        var error2 = new Result<int, string>.Error<int, string>("same");
        var error3 = new Result<int, string>.Error<int, string>("different");

        Assert.AreEqual(error1, error2);
        Assert.AreNotEqual(error1, error3);
        Assert.AreEqual(error1.GetHashCode(), error2.GetHashCode());

        OutcomeAssertions.Error(error1, "same");
        OutcomeAssertions.Error(error2, "same");
        OutcomeAssertions.Error(error3, "different");
        Assert.AreNotSame(error1, error2);
    }

    [TestMethod]
    public void Error_WithRecord_UsesRecordEquality()
    {
        var error1 = new Result<int, string>.Error<int, string>("value");
        var error2 = error1 with { };

        Assert.AreEqual(error1, error2);
        Assert.AreNotSame(error1, error2);

        OutcomeAssertions.Error(error1, "value");
        OutcomeAssertions.Error(error2, "value");
        Assert.AreEqual(error1.GetHashCode(), error2.GetHashCode());
    }

    [TestMethod]
    public void Error_Value_PropertyWorks()
    {
        var error = new Result<int, string>.Error<int, string>("test value");
        Assert.AreEqual("test value", error.Value);

        var error2 = new Result<string, int>.Error<string, int>(42);
        Assert.AreEqual(42, error2.Value);

        OutcomeAssertions.Error(error, "test value");
        OutcomeAssertions.Error(error2, 42);
    }

    [TestMethod]
    public void Result_WithReferenceTypes_Works()
    {
        var okResult = new Result<string, Exception>.Ok<string, Exception>("success");
        var expectedException = new InvalidOperationException("error");
        var errorResult = Result<string, Exception>.Failure(expectedException);

        Assert.IsTrue(okResult.IsOk);
        Assert.IsTrue(errorResult.IsError);

        OutcomeAssertions.Success(okResult, "success");
        Assert.IsInstanceOfType<InvalidOperationException>(!errorResult);
        Assert.AreEqual("error", (!errorResult).Message);
        OutcomeAssertions.Error(errorResult, expectedException);
        Assert.AreSame(expectedException, !errorResult);
    }

    [TestMethod]
    public void Result_WithNullableTypes_Works()
    {
        var okResult = new Result<int?, string>.Ok<int?, string>(null);
        var value = okResult.Match(static x => x, static _ => -1);
        Assert.IsNull(value);

        OutcomeAssertions.Success(okResult, null);
    }

    [TestMethod]
    public void Result_Equality_WorksCorrectly()
    {
        var ok1 = new Result<int, string>.Ok<int, string>(42);
        var ok2 = new Result<int, string>.Ok<int, string>(42);
        var ok3 = new Result<int, string>.Ok<int, string>(99);

        Assert.AreEqual(ok1, ok2);
        Assert.AreNotEqual(ok1, ok3);

        var err1 = Result<int, string>.Failure("error");
        var err2 = Result<int, string>.Failure("error");
        var err3 = Result<int, string>.Failure("different");

        Assert.AreEqual(err1, err2);
        Assert.AreNotEqual(err1, err3);

        OutcomeAssertions.Success(ok1, 42);
        OutcomeAssertions.Success(ok2, 42);
        OutcomeAssertions.Success(ok3, 99);
        OutcomeAssertions.Error(err1, "error");
        OutcomeAssertions.Error(err2, "error");
        OutcomeAssertions.Error(err3, "different");
        Assert.AreNotEqual<Result<int, string>>(ok1, err1);
    }

    [TestMethod]
    public void Result_GetHashCode_WorksCorrectly()
    {
        var ok1 = new Result<int, string>.Ok<int, string>(42);
        var ok2 = new Result<int, string>.Ok<int, string>(42);

        Assert.AreEqual(ok1.GetHashCode(), ok2.GetHashCode());

        var err1 = Result<int, string>.Failure("error");
        var err2 = Result<int, string>.Failure("error");

        Assert.AreEqual(err1.GetHashCode(), err2.GetHashCode());

        OutcomeAssertions.Success(ok1, 42);
        OutcomeAssertions.Success(ok2, 42);
        OutcomeAssertions.Error(err1, "error");
        OutcomeAssertions.Error(err2, "error");
        Assert.AreEqual(ok1, ok2);
        Assert.AreEqual(err1, err2);
    }

    [TestMethod]
    public void Result_ChainedOperations_WorkCorrectly()
    {
        var result = new Result<int, string>.Ok<int, string>(5)
            .Map(static x => x * 2)
            .Bind(static x =>
                x > 5
                    ? new Result<int, string>.Ok<int, string>(x)
                    : Result<int, string>.Failure("too small")
            )
            .Map(static x => x + 1);

        Assert.IsTrue(result is Result<int, string>.Ok<int, string>);
        var ok = (Result<int, string>.Ok<int, string>)result;
        Assert.AreEqual(11, ok.Value);

        OutcomeAssertions.Success(result, 11);
    }

    [TestMethod]
    public void Tap_OnOk_WithOnlyOnErrorAction_DoesNotInvokeIt()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var errorActionInvoked = false;
        var returnedResult = result.Tap(onError: _ => errorActionInvoked = true);
        Assert.IsFalse(errorActionInvoked);
        Assert.AreSame(result, returnedResult);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void Tap_OnError_WithOnlyOnSuccessAction_DoesNotInvokeIt()
    {
        var result = Result<int, string>.Failure("error");
        var successActionInvoked = false;
        var returnedResult = result.Tap(onSuccess: _ => successActionInvoked = true);
        Assert.IsFalse(successActionInvoked);
        Assert.AreSame(result, returnedResult);

        OutcomeAssertions.Error(result, "error");
    }

    [TestMethod]
    public void Tap_OnOk_WithBothActions_InvokesOnlyOnSuccess()
    {
        var result = new Result<int, string>.Ok<int, string>(42);
        var successInvoked = false;
        var errorInvoked = false;
        var returnedResult = result.Tap(
            onSuccess: _ => successInvoked = true,
            onError: _ => errorInvoked = true
        );
        Assert.IsTrue(successInvoked);
        Assert.IsFalse(errorInvoked);
        Assert.AreSame(result, returnedResult);

        OutcomeAssertions.Success(result, 42);
    }

    [TestMethod]
    public void Tap_OnError_WithBothActions_InvokesOnlyOnError()
    {
        var result = Result<int, string>.Failure("error");
        var successInvoked = false;
        var errorInvoked = false;
        var returnedResult = result.Tap(
            onSuccess: _ => successInvoked = true,
            onError: _ => errorInvoked = true
        );
        Assert.IsFalse(successInvoked);
        Assert.IsTrue(errorInvoked);
        Assert.AreSame(result, returnedResult);

        OutcomeAssertions.Error(result, "error");
    }
}

#pragma warning disable IDE0005 // Using directive is unnecessary.
global using ErrorStringAndHttpError = Outcome.Result<
    string,
    Outcome.HttpError<RestClient.Net.CsTest.Models.MyErrorModel>
>.Error<string, Outcome.HttpError<RestClient.Net.CsTest.Models.MyErrorModel>>;
global using ErrorUnit = Outcome.Result<Outcome.Unit, Outcome.HttpError<string>>.Error<
    Outcome.Unit,
    Outcome.HttpError<string>
>;
global using ExceptionError = Outcome.HttpError<RestClient.Net.CsTest.Models.MyErrorModel>.ExceptionError;
global using ExceptionErrorString = Outcome.HttpError<string>.ExceptionError;
global using OkStringAndHttpError = Outcome.Result<
    string,
    Outcome.HttpError<RestClient.Net.CsTest.Models.MyErrorModel>
>.Ok<string, Outcome.HttpError<RestClient.Net.CsTest.Models.MyErrorModel>>;
global using OkUnit = Outcome.Result<Outcome.Unit, Outcome.HttpError<string>>.Ok<
    Outcome.Unit,
    Outcome.HttpError<string>
>;
global using ResponseError = Outcome.HttpError<RestClient.Net.CsTest.Models.MyErrorModel>.ErrorResponseError;
global using ResponseErrorString = Outcome.HttpError<string>.ErrorResponseError;

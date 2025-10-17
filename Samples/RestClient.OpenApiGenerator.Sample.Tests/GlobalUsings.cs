#pragma warning disable IDE0005 // Using directive is unnecessary.

global using JSONPlaceholder.Generated;
global using ErrorPost = Outcome.Result<
    JSONPlaceholder.Generated.Post,
    Outcome.HttpError<string>
>.Error<JSONPlaceholder.Generated.Post, Outcome.HttpError<string>>;
global using ErrorPosts = Outcome.Result<
    System.Collections.Generic.List<JSONPlaceholder.Generated.Post>,
    Outcome.HttpError<string>
>.Error<System.Collections.Generic.List<JSONPlaceholder.Generated.Post>, Outcome.HttpError<string>>;
global using ErrorTodo = Outcome.Result<
    JSONPlaceholder.Generated.Todo,
    Outcome.HttpError<string>
>.Error<JSONPlaceholder.Generated.Todo, Outcome.HttpError<string>>;
global using ErrorTodos = Outcome.Result<
    System.Collections.Generic.List<JSONPlaceholder.Generated.Todo>,
    Outcome.HttpError<string>
>.Error<System.Collections.Generic.List<JSONPlaceholder.Generated.Todo>, Outcome.HttpError<string>>;
global using ErrorUnit = Outcome.Result<Outcome.Unit, Outcome.HttpError<string>>.Error<
    Outcome.Unit,
    Outcome.HttpError<string>
>;
global using ErrorUser = Outcome.Result<
    JSONPlaceholder.Generated.User,
    Outcome.HttpError<string>
>.Error<JSONPlaceholder.Generated.User, Outcome.HttpError<string>>;
global using ExceptionErrorString = Outcome.HttpError<string>.ExceptionError;
global using OkPost = Outcome.Result<JSONPlaceholder.Generated.Post, Outcome.HttpError<string>>.Ok<
    JSONPlaceholder.Generated.Post,
    Outcome.HttpError<string>
>;
global using OkPosts = Outcome.Result<
    System.Collections.Generic.List<JSONPlaceholder.Generated.Post>,
    Outcome.HttpError<string>
>.Ok<System.Collections.Generic.List<JSONPlaceholder.Generated.Post>, Outcome.HttpError<string>>;
global using OkTodo = Outcome.Result<JSONPlaceholder.Generated.Todo, Outcome.HttpError<string>>.Ok<
    JSONPlaceholder.Generated.Todo,
    Outcome.HttpError<string>
>;
global using OkTodos = Outcome.Result<
    System.Collections.Generic.List<JSONPlaceholder.Generated.Todo>,
    Outcome.HttpError<string>
>.Ok<System.Collections.Generic.List<JSONPlaceholder.Generated.Todo>, Outcome.HttpError<string>>;
global using OkUnit = Outcome.Result<Outcome.Unit, Outcome.HttpError<string>>.Ok<
    Outcome.Unit,
    Outcome.HttpError<string>
>;
global using OkUser = Outcome.Result<JSONPlaceholder.Generated.User, Outcome.HttpError<string>>.Ok<
    JSONPlaceholder.Generated.User,
    Outcome.HttpError<string>
>;
global using ResponseErrorString = Outcome.HttpError<string>.ErrorResponseError;

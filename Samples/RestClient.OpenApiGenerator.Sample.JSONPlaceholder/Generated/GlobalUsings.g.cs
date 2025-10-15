#pragma warning disable IDE0005 // Using directive is unnecessary.
using OkPosts = Outcome.Result<System.Collections.Generic.List<JSONPlaceholder.Generated.Post>, Outcome.HttpError<string>>.Ok<System.Collections.Generic.List<JSONPlaceholder.Generated.Post>, Outcome.HttpError<string>>;
using ErrorPosts = Outcome.Result<System.Collections.Generic.List<JSONPlaceholder.Generated.Post>, Outcome.HttpError<string>>.Error<System.Collections.Generic.List<JSONPlaceholder.Generated.Post>, Outcome.HttpError<string>>;
using OkTodos = Outcome.Result<System.Collections.Generic.List<JSONPlaceholder.Generated.Todo>, Outcome.HttpError<string>>.Ok<System.Collections.Generic.List<JSONPlaceholder.Generated.Todo>, Outcome.HttpError<string>>;
using ErrorTodos = Outcome.Result<System.Collections.Generic.List<JSONPlaceholder.Generated.Todo>, Outcome.HttpError<string>>.Error<System.Collections.Generic.List<JSONPlaceholder.Generated.Todo>, Outcome.HttpError<string>>;
using OkPost = Outcome.Result<JSONPlaceholder.Generated.Post, Outcome.HttpError<string>>.Ok<JSONPlaceholder.Generated.Post, Outcome.HttpError<string>>;
using ErrorPost = Outcome.Result<JSONPlaceholder.Generated.Post, Outcome.HttpError<string>>.Error<JSONPlaceholder.Generated.Post, Outcome.HttpError<string>>;
using OkTodo = Outcome.Result<JSONPlaceholder.Generated.Todo, Outcome.HttpError<string>>.Ok<JSONPlaceholder.Generated.Todo, Outcome.HttpError<string>>;
using ErrorTodo = Outcome.Result<JSONPlaceholder.Generated.Todo, Outcome.HttpError<string>>.Error<JSONPlaceholder.Generated.Todo, Outcome.HttpError<string>>;
using OkUnit = Outcome.Result<Outcome.Unit, Outcome.HttpError<string>>.Ok<Outcome.Unit, Outcome.HttpError<string>>;
using ErrorUnit = Outcome.Result<Outcome.Unit, Outcome.HttpError<string>>.Error<Outcome.Unit, Outcome.HttpError<string>>;
using OkUser = Outcome.Result<JSONPlaceholder.Generated.User, Outcome.HttpError<string>>.Ok<JSONPlaceholder.Generated.User, Outcome.HttpError<string>>;
using ErrorUser = Outcome.Result<JSONPlaceholder.Generated.User, Outcome.HttpError<string>>.Error<JSONPlaceholder.Generated.User, Outcome.HttpError<string>>;
namespace JSONPlaceholder.Generated;

/// <summary>Todo</summary>
/// <param name="UserId">UserId</param>
/// <param name="Id">Id</param>
/// <param name="Title">Title</param>
/// <param name="Completed">Completed</param>
public record Todo(long UserId, long Id, string Title, bool Completed);

/// <summary>TodoInput</summary>
/// <param name="UserId">UserId</param>
/// <param name="Title">Title</param>
/// <param name="Completed">Completed</param>
public record TodoInput(long UserId, string Title, bool Completed);

/// <summary>Post</summary>
/// <param name="UserId">UserId</param>
/// <param name="Id">Id</param>
/// <param name="Title">Title</param>
/// <param name="Body">Body</param>
public record Post(long UserId, long Id, string Title, string Body);

/// <summary>PostInput</summary>
/// <param name="UserId">UserId</param>
/// <param name="Title">Title</param>
/// <param name="Body">Body</param>
public record PostInput(long UserId, string Title, string Body);

/// <summary>User</summary>
/// <param name="Id">Id</param>
/// <param name="Name">Name</param>
/// <param name="Username">Username</param>
/// <param name="Email">Email</param>
/// <param name="Address">Address</param>
/// <param name="Phone">Phone</param>
/// <param name="Website">Website</param>
/// <param name="Company">Company</param>
public record User(long Id, string Name, string Username, string Email, Address Address, string Phone, string Website, Company Company);

/// <summary>Address</summary>
/// <param name="Street">Street</param>
/// <param name="Suite">Suite</param>
/// <param name="City">City</param>
/// <param name="Zipcode">Zipcode</param>
/// <param name="Geo">Geo</param>
public record Address(string Street, string Suite, string City, string Zipcode, Geo Geo);

/// <summary>Geo</summary>
/// <param name="Lat">Lat</param>
/// <param name="Lng">Lng</param>
public record Geo(string Lat, string Lng);

/// <summary>Company</summary>
/// <param name="Name">Name</param>
/// <param name="CatchPhrase">CatchPhrase</param>
/// <param name="Bs">Bs</param>
public record Company(string Name, string CatchPhrase, string Bs);
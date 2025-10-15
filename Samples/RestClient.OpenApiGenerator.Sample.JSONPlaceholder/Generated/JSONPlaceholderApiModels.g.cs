namespace JSONPlaceholder.Generated;

/// <summary>Todo</summary>
public class Todo
{
    /// <summary>UserId</summary>
    public long UserId { get; set; }

    /// <summary>Id</summary>
    public long Id { get; set; }

    /// <summary>Title</summary>
    public string Title { get; set; }

    /// <summary>Completed</summary>
    public bool Completed { get; set; }
}

/// <summary>TodoInput</summary>
public class TodoInput
{
    /// <summary>UserId</summary>
    public long UserId { get; set; }

    /// <summary>Title</summary>
    public string Title { get; set; }

    /// <summary>Completed</summary>
    public bool Completed { get; set; }
}

/// <summary>Post</summary>
public class Post
{
    /// <summary>UserId</summary>
    public long UserId { get; set; }

    /// <summary>Id</summary>
    public long Id { get; set; }

    /// <summary>Title</summary>
    public string Title { get; set; }

    /// <summary>Body</summary>
    public string Body { get; set; }
}

/// <summary>PostInput</summary>
public class PostInput
{
    /// <summary>UserId</summary>
    public long UserId { get; set; }

    /// <summary>Title</summary>
    public string Title { get; set; }

    /// <summary>Body</summary>
    public string Body { get; set; }
}

/// <summary>User</summary>
public class User
{
    /// <summary>Id</summary>
    public long Id { get; set; }

    /// <summary>Name</summary>
    public string Name { get; set; }

    /// <summary>Username</summary>
    public string Username { get; set; }

    /// <summary>Email</summary>
    public string Email { get; set; }

    /// <summary>Address</summary>
    public Address Address { get; set; }

    /// <summary>Phone</summary>
    public string Phone { get; set; }

    /// <summary>Website</summary>
    public string Website { get; set; }

    /// <summary>Company</summary>
    public Company Company { get; set; }
}

/// <summary>Address</summary>
public class Address
{
    /// <summary>Street</summary>
    public string Street { get; set; }

    /// <summary>Suite</summary>
    public string Suite { get; set; }

    /// <summary>City</summary>
    public string City { get; set; }

    /// <summary>Zipcode</summary>
    public string Zipcode { get; set; }

    /// <summary>Geo</summary>
    public Geo Geo { get; set; }
}

/// <summary>Geo</summary>
public class Geo
{
    /// <summary>Lat</summary>
    public string Lat { get; set; }

    /// <summary>Lng</summary>
    public string Lng { get; set; }
}

/// <summary>Company</summary>
public class Company
{
    /// <summary>Name</summary>
    public string Name { get; set; }

    /// <summary>CatchPhrase</summary>
    public string CatchPhrase { get; set; }

    /// <summary>Bs</summary>
    public string Bs { get; set; }
}
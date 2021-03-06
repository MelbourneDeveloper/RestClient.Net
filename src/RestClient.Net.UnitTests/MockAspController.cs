﻿using System;

namespace RestClient.Net.UnitTests
{
    public class MockAspController
    {
        public IClient Client { get; }

        public MockAspController(CreateClient clientFactory)
            => Client = clientFactory != null ? clientFactory("test",
                //TODO: The test is actually calling this which is bad
                (o) => o.BaseUrl = new("https://restcountries.eu/rest/v2/")
                ) : throw new ArgumentNullException(nameof(clientFactory));
    }
}
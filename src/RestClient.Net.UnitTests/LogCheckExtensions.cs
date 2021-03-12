#if !NET45

using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RestClient.Net.UnitTests
{
    public static class LogCheckExtensions
    {

        public static void VerifyLog<TException>(
                   Mock<ILogger<Client>> loggerMock,
                   Expression<Func<object, Type, bool>> match,
                   LogLevel logLevel,
                   int times) where TException : Exception
        {
            loggerMock.Verify
            (
                l => l.Log
                (
                    //Check the severity level
                    logLevel,
                    //This may or may not be relevant to your scenario
                    It.IsAny<EventId>(),
                    //This is the magical Moq code that exposes internal log processing from the extension methods
                    It.Is<It.IsAnyType>(match),
                    //Confirm the exception type
                    It.IsAny<TException>(),
                    //Accept any valid Func here. The Func is specified by the extension methods
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                //Make sure the message was logged the correct number of times
                Times.Exactly(times)
            );
        }

        public static void VerifyLog(
           this Mock<ILogger<Client>> loggerMock,
           Expression<Func<object, Type, bool>> match,
           LogLevel logLevel,
           int times)
        {
            loggerMock.Verify
            (
                l => l.Log
                (
                    //Check the severity level
                    logLevel,
                    //This may or may not be relevant to your scenario
                    It.IsAny<EventId>(),
                    //This is the magical Moq code that exposes internal log processing from the extension methods
                    It.Is<It.IsAnyType>(match),
                    //Confirm the exception type
                    null,
                    //Accept any valid Func here. The Func is specified by the extension methods
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                //Make sure the message was logged the correct number of times
                Times.Exactly(times)
            );
        }

        public static bool CheckValue<T>(this object state, T expectedValue, string key)
        => CheckValue(state, expectedValue, key, (a, b) =>
        (a == null && b == null) || (a != null && a.Equals(b)));

        public static bool CheckValue<T>(this object state, T expectedValue, string key, Func<T, T, bool> compare)
        {
            var keyValuePairList = (IReadOnlyList<KeyValuePair<string, object>>)state;

            var actualValue = (T)keyValuePairList.First(kvp => string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0).Value;

            return compare(actualValue, expectedValue);
        }
    }
}

#endif
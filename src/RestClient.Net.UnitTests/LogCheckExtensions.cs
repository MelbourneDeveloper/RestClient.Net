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

        public static void VerifyLog<T, TException>(
                   Mock<ILogger<T>> loggerMock,
                   Expression<Func<object, Type, bool>> match,
                   LogLevel logLevel,
                   int times,
                   bool checkException = false) where TException : Exception
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
                    checkException ? It.IsAny<TException>() : null,
                    //Accept any valid Func here. The Func is specified by the extension methods
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                //Make sure the message was logged the correct number of times
                Times.Exactly(times)
            );
        }

        public static void VerifyLog<T>(
           this Mock<ILogger<T>> loggerMock,
           Expression<Func<object, Type, bool>> match,
           LogLevel logLevel,
           int times)
        => VerifyLog<T, Exception>(loggerMock, match, logLevel, times);

        public static bool CheckValue<T>(this object state, T expectedValue, string key)
        => CheckValue<T>(state, key, (actualValue)
               => (actualValue == null && expectedValue == null) || (actualValue != null && actualValue.Equals(expectedValue)));

        public static bool CheckValue<T>(this object state, string key, Func<T, bool> compare)
        {
            var keyValuePairList = (IReadOnlyList<KeyValuePair<string, object>>)state;

            var keyValuePair = keyValuePairList.FirstOrDefault(kvp => string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0);

            //Check to make sure we were able to get the key from the dictionary or return false
            if (keyValuePair.Key == null) return false;

            var actualValue = (T)keyValuePair.Value;

            return compare(actualValue);
        }
    }
}
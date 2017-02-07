using System;

namespace AdaW10.Models.EventsLoaderServices
{
    public class EventsLoaderException : Exception
    {
        public EventsLoaderException()
        {
        }

        public EventsLoaderException(string message)
        : base(message)
        {
        }

        public EventsLoaderException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}

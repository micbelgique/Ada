using System;

namespace MartineobotIOTMvvm.Models.EventsLoaderServices
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

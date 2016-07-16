using System;
using System.Collections.Generic;

namespace ThreeOldFloor.Core
{
    public class ThreeOldFloorException : Exception
    {
        private Dictionary<string, object> Parameters { get; set; }

        public ThreeOldFloorException()
        {
            Parameters = new Dictionary<string, object>();
        }

        public ThreeOldFloorException(int messageFormat, string message) : base(message)
        {
            Parameters = new Dictionary<string, object>();
        }

        public ThreeOldFloorException(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        public ThreeOldFloorException(string message, Dictionary<string, object> parameters)
            : base(message)
        {
            Parameters = parameters;
        }

        public void AddParameter(string key, object value)
        {
            Parameters.Add(key, value);
        }
    }
}
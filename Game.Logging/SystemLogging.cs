using System;
using Microsoft.Extensions.Logging;

namespace Game.Logging
{
    public class SystemLogging
    {
        private ILogger _logger;

        public SystemLogging()
        {
            _logger = null;
        }

        public SystemLogging(ILogger logger)
        {
            _logger = logger;
        }

        public void Fatal(String message, Exception ex = null)
        {

        }
    }
}

using System;

namespace eru.Application.Common.Exceptions
{
    public class DatabaseSettingsException : Exception
    {
        public DatabaseSettingsException()
            : base("Database was not or was wrongly configured! Head to https://xxlo-devs.github.io/eru/config/ for samples how to configure database correctly!")
        {
            
        }
    }
}
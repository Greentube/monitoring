using System;
using System.Collections.Generic;
using System.Text;
using Greentube.Monitoring.InternalResource;

namespace Greentube.Monitoring.IntegrationTests
{
    class SimpleInternalResource: IInternalResourceMonitored
    {
        public bool IsUp { get; set;  } = false;
    }
}

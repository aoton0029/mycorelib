using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.DataProcessors
{
    public interface IDataProcessor
    {
        Task ScheduleDataProcessing(DataWithKey dataWithKey);
    }
}

using System.Collections.Generic;

namespace Dose.Core.Library.Common
{
    public interface IActionPerformer
    {        
        public void Perform(Dictionary<string, object?> args);
    }
}
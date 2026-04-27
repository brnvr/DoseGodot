using System.Collections.Generic;

namespace Dose.Core.Library.Common
{
    public interface IActionPerformer
    {        
        public bool Perform(object sender, Dictionary<string, object?> args);
    }
}
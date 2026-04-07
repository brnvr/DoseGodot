using System.Collections.Generic;

namespace Dose.Core.Library.Common
{
    public interface IActionPerformer
    {        
        public void Perform(object sender, Dictionary<string, object?> args);
    }
}
using System.Collections.Generic;

namespace Dose.Core.Library.Common
{
    public interface IActionExecuter
    {
        public bool ExecuteAction(IActionPerformer actionPerformer, Dictionary<string, object?> args);
    }
}
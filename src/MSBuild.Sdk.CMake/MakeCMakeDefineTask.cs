using System;
using System.Collections.Generic;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Linq;

namespace MSBuild.Sdk.CMake
{
    public class MakeCMakeDefineTask : Task
    {
        public ITaskItem[] Defines { get; set; }
        [Output]
        public ITaskItem GeneratedDefineArg { get; set; }
        public override bool Execute()
        {
            if (Defines == null || !Defines.Any())
            {
                Log.LogMessage("define empty");
                return true;
            }
            var retarg = string.Join(" ", Defines.Select(x => string.Format($"-D\"{x.ItemSpec}={x.GetMetadata("Value")}\"")));
            GeneratedDefineArg = new TaskItem(retarg);
            Log.LogMessage("define args = `{0}`", retarg);
            return true;
        }
    }
}
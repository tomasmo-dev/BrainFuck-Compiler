using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF_Runner
{
    class Runner
    {
        private string[] code;
        public Runner(string[] commandList)
        {
            code = commandList;
        }

        private void run()
        {
            for (int row = 0; row < code.Length; row++)
            {
                var codeStream = code[row];

                
            }
        }
    }
}

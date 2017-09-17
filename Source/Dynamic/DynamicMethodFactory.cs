using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

namespace Torian.Common.Dynamic
{

    public class DynamicMethodFactory
    {

        private static string _template = @"using System;

namespace Torian.Common.Dynamic.CodeGen {{

    public static class OnlyClass
    {{

       public static {0} OnlyMethod({1} {2}, {3} {4}, {5} {6}, {7} {8})
       {{
           {9}
       }}

    }}

}}";

        public static Func<T1, T2, T3, T4, TResult> CreateMethod<T1, T2, T3, T4, TResult>(string innerCode, string arg1Name, string arg2Name, string arg3Name, string arg4Name)
        {
            CompilerParameters param = new CompilerParameters();
            param.GenerateExecutable = false;
            param.GenerateInMemory = true;
            param.IncludeDebugInformation = false;
            param.ReferencedAssemblies.Add("mscorlib.dll");
            param.ReferencedAssemblies.Add("System.dll");
            param.TreatWarningsAsErrors = false;

            string code = string.Format(_template, 
                                        typeof(TResult).FullName,
                                        typeof(T1).FullName,
                                        arg1Name,
                                        typeof(T2).FullName,
                                        arg2Name,
                                        typeof(T3).FullName,
                                        arg3Name,
                                        typeof(T4).FullName,
                                        arg4Name,
                                        innerCode);


            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerResults cr = codeProvider.CompileAssemblyFromSource(param, code);
            if (cr.Errors != null && cr.Errors.Count > 0)
            {
                throw new Exception("Compile Error: " + cr.Errors[0].ErrorText);
            }
            Assembly ass = cr.CompiledAssembly;
            Type t = ass.GetType("Torian.Common.Dynamic.CodeGen.OnlyClass");
            MethodInfo mi = t.GetMethod("OnlyMethod");
            Func<T1, T2, T3, T4, TResult> retFunc = (a,b,c,d) => {
                return (TResult)mi.Invoke(null, new object[] { a, b, c, d });
            };
            return retFunc;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Torian.Common.Patterns
{

    public abstract class Singleton<T>
        where T : new()
    {

        private static T _instance;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }

    }

}

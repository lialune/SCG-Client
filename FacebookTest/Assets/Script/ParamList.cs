using System.Collections.Generic;

namespace Homura
{
    public class PARAM_LIST
    {
        public class Enumerator
        {
            public Dictionary<string, string>.Enumerator mEnumerator;

            public Enumerator()
            {
            }

            public Enumerator(Dictionary<string, string>.Enumerator _Enumerator)
            {
                mEnumerator = _Enumerator;
            }
        }

        public Dictionary<string, string> mParamList;

        public PARAM_LIST()
        {
            mParamList = new Dictionary<string, string>();
        }

       public Dictionary<string, string>.Enumerator GetEnumerator()
        {
            return mParamList.GetEnumerator();
        }
    }
}

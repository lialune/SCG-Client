using System;

namespace Homura
{
    public class Singletone<ClassType> where ClassType : new()
    {
        static ClassType mInstance;

        protected Singletone(){}
        protected Singletone(Singletone<ClassType> _Single) { }

        public ClassType Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = new ClassType();
                }
                return mInstance;
            }
        }
    }
}
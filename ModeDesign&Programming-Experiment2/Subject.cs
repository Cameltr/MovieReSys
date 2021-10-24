using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReSys
{
    abstract class Subject
    {
        protected ArrayList observers = new ArrayList();
        public abstract void Attach(Observer observer);
        public abstract void Detach(Observer observer);
        public abstract void Notify(string messageType);

    }
}

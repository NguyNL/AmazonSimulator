using Controllers;
using System;
using System.Collections.Generic;

namespace Models
{
    public class ServerCommunication : IObservable<Command>
    {
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        public void SendCommandToObservers(Command c)
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].OnNext(c);
            }
        }

        private void SendCreationCommandsToObserver(IObserver<Command> obs)
        {
            //foreach (Object m3d in worldObjects)
            //{
            //    obs.OnNext(new UpdateModel3DCommand(m3d));
            //}
        }

        internal class Unsubscriber<Command> : IDisposable
        {
            private List<IObserver<Command>> _observers;
            private IObserver<Command> _observer;

            internal Unsubscriber(List<IObserver<Command>> observers, IObserver<Command> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}

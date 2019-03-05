using DataInterface;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UI.WPF.Models
{
    public class UIObservableConcurrentDictionary<T1, T2> : ObservableConcurrentDictionary<T1, T2>
    {
        public override event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs Args)
        {
            if (base._Notify)
            {
                Dispatcher.CurrentDispatcher.Invoke(((Action)(() =>
                {                   
                    CollectionChanged?.Invoke(this, Args);                    
                })));
            }
        }

        private void NotifyObserversOfChange()
        {
            var collectionHandler = CollectionChanged;
            var propertyHandler = PropertyChanged;
            if (collectionHandler != null || propertyHandler != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(((Action)(() =>
                {
                    if (collectionHandler != null)
                    {
                        collectionHandler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    }
                    if (propertyHandler != null)
                    {
                        propertyHandler(this, new PropertyChangedEventArgs("Count"));
                        propertyHandler(this, new PropertyChangedEventArgs("Keys"));
                        propertyHandler(this, new PropertyChangedEventArgs("Values"));
                    }
                })), new object[] { });
            }
        }
    }
   
}

using DataInterface;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UI.WPF.Models
{
    public class UIObservableConcurrentDictionary<T1, T2> : ObservableConcurrentDictionary<T1, T2>
    {
        public override event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;
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
    }
   
}

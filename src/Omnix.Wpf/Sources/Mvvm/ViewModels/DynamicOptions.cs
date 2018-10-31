using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Omnius.Wpf
{
    // http://blog.okazuki.jp/entry/20100702/1278056325
    public class DynamicOptions : DynamicObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private Dictionary<string, object> _pairs = new Dictionary<string, object>();

        public DynamicOptions()
        {

        }

        public void SetProperties(IEnumerable<DynamicPropertyInfo> properties)
        {
            _pairs.Clear();

            foreach (var p in properties)
            {
                _pairs[p.Name] = p.Value;
            }
        }

        public IEnumerable<DynamicPropertyInfo> GetProperties()
        {
            return _pairs.Select(n => new DynamicPropertyInfo(n.Key, n.Value)).ToArray();
        }

        public T GetValue<T>(string propertyName)
        {
            return (T)_pairs[propertyName];
        }

        public T GetValueOrDefault<T>(string propertyName, Func<T> defaultValueFactory)
        {
            if (_pairs.TryGetValue(propertyName, out object value)) return (T)value;

            return defaultValueFactory();
        }

        public T GetValueOrDefault<T>(string propertyName)
        {
            if (_pairs.TryGetValue(propertyName, out object value)) return (T)value;

            return default;
        }

        public void SetValue<T>(string propertyName, T value)
        {
            _pairs[propertyName] = value;
            this.OnPropertyChanged(propertyName);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_pairs.TryGetValue(binder.Name, out var value))
            {
                result = value;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _pairs[binder.Name] = value;
            this.OnPropertyChanged(binder.Name);

            return true;
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class DynamicPropertyInfo
        {
            [JsonConstructor]
            public DynamicPropertyInfo(string name, object value)
            {
                this.Name = name;
                this.Value = value;
            }

            [JsonProperty]
            public string Name { get; }

            [JsonProperty]
            public object Value { get; }
        }
    }
}

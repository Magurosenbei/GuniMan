using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace Engine
{
    public class IEServiceContainer : IServiceProvider
    {
        // Holds information of all services
        Dictionary<Type, object> services = new Dictionary<Type, object>();
            
        public void AddService(Type Service, object Provider)
        {
            if(services.ContainsKey(Service))
                throw new Exception("Service already added");
            services.Add(Service, Provider);
        }

        public object GetService(Type Service)
        {
            foreach (Type type in services.Keys)
                if (type == Service)
                    return services[type];
            throw new Exception("Service not found " + Service.Name);
        }
        public T GetService<T>()
        {
            object result = GetService(typeof(T));
            if (result != null)
                return (T)result;
            return default(T);
        }
        public void RemoveService(Type Service)
        {
            if(services.ContainsKey(Service))
                services.Remove(Service);
        }

        public bool ContainsService(Type Service)
        {
            return services.ContainsKey(Service);
        }
    }
}

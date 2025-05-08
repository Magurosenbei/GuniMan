using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Engine
{
    public class IEContentManager : ContentManager
    {
        public IEContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        public bool PreserveAsset = true;
        public bool UseDefaultLoad = false;
        List<IDisposable> dustbin = new List<IDisposable>();
        Dictionary<string, object> Loaded = new Dictionary<string, object>();

        public override T Load<T>(string assetName)
        {
            T r = this.ReadAsset<T>(assetName, RecordIDisposable);
            if (PreserveAsset && !Loaded.ContainsKey(assetName))
                Loaded.Add(assetName, r);

            if (!UseDefaultLoad)
                return (T)Loaded[assetName];
            else
                return base.Load<T>(assetName);
        }
        void RecordIDisposable(IDisposable asset)
        {
            //if (PreserveAsset)
             //   dustbin.Add(asset);
        }
        public override void Unload()
        {
            base.Unload();
            foreach (IDisposable trash in dustbin)
                trash.Dispose();
            foreach (object item in Loaded)
            {
                if (item is IDisposable)
                {
                    IDisposable Crap = (IDisposable)item;
                    Crap.Dispose();
                }
            }
            Loaded.Clear();
            dustbin.Clear();        
        }
        public void Unload(string asset)
        {
            if (!Loaded.ContainsKey(asset)) return;
            Loaded.Remove(asset);
            if (Loaded[asset] is IDisposable && dustbin.Contains((IDisposable)Loaded[asset]))
            {
                IDisposable obj = dustbin[dustbin.IndexOf((IDisposable)Loaded[asset])];
                obj.Dispose();
                dustbin.Remove(obj);
            }
        }
        public void ClearDustbin()
        {
            foreach (IDisposable trash in dustbin)
                trash.Dispose();
            dustbin.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manning.MyPhotoAlbum
{
   public class PhotoAlbum : Collection<Photograph>,  IDisposable 
    {
        private bool _hasChanged = false;
        public bool HasChaged
        {
            get
            {
                if (_hasChanged) return true;

                foreach (Photograph p in this)
                    if (p.HasChanged) return true;

                return false;
            }
            set
            {
                _hasChanged = value;
                if (value == false)
                    foreach (Photograph p in this)
                        p.HasChanged = false;
            }
        }

        public Photograph Add(string filename)
        {
            Photograph p = new Photograph(filename);
            base.Add(p);
            return p;
        }

        protected override void ClearItems()
        {
            if(Count > 0)
            {
                Dispose();
                base.ClearItems();
                HasChaged = true;
            }   
        }

        protected override void InsertItem(int index, Photograph item)
        {
            base.InsertItem(index, item);
            HasChaged = true;
        }

        protected override void RemoveItem(int index)
        {
            Items[index].Dispose();
            base.RemoveItem(index);
            HasChaged = true;
        }

        protected override void SetItem(int index, Photograph item)
        {
            base.SetItem(index, item);
            HasChaged = true;
        }

        public void Dispose()
        {
            foreach (Photograph p in this)
                p.Dispose();
        }
    }
}

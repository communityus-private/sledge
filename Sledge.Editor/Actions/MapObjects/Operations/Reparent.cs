using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    public class Reparent : IAction
    {
        private class ReparentReference
        {
            public long ID { get; set; }
            public long OriginalParentID { get; set; }
            public MapObject MapObject { get; set; }
        }

        private readonly long _parentId;
        private List<ReparentReference> _objects; 

        public Reparent(long parentId, IEnumerable<MapObject> objects)
        {
            _parentId = parentId;
            _objects = objects.Select(x => new ReparentReference
                                               {
                                                   ID = x.ID,
                                                   OriginalParentID = x.Parent.ID,
                                                   MapObject = x
                                               }).ToList();
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            var parents = _objects.Select(x => x.OriginalParentID)
                .Distinct()
                .ToDictionary(x => x, x => document.Map.WorldSpawn.FindByID(x));
            foreach (var o in _objects)
            {
                o.MapObject.SetParent(parents[o.OriginalParentID]);
            }
        }

        public void Perform(Document document)
        {
            var parent = document.Map.WorldSpawn.FindByID(_parentId);
            _objects.ForEach(x => x.MapObject.SetParent(parent));
        }
    }
}
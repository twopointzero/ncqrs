﻿using System;
using System.IO;
using System.Reflection;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.NoDB
{
    public class NoDBSnapshotStore : ISnapshotStore
    {
        private readonly string _path;

        public NoDBSnapshotStore(string path)
        {
            _path = path;
        }

        public void SaveShapshot(ISnapshot source)
        {
            FileInfo file = source.EventSourceId.GetSnapshotFileInfo(_path);
            if (!file.Exists && !file.Directory.Exists)
                file.Directory.Create();
            var jo = JObject.FromObject(source);
            File.WriteAllText(file.FullName, source.GetType().AssemblyQualifiedName + "\n\r" + jo.ToString());
        }

        public ISnapshot GetSnapshot(Guid eventSourceId)
        {
            FileInfo file = eventSourceId.GetSnapshotFileInfo(_path);
            var snapshottext = File.ReadAllText(file.FullName);
            var reader = new StringReader(snapshottext);
            var type = Type.GetType(reader.ReadLine().Trim());
            return (ISnapshot) new JsonSerializer().Deserialize(reader, type);
        }
    }
}
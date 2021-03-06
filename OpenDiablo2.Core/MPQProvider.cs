﻿using OpenDiablo2.Common.Interfaces;
using OpenDiablo2.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDiablo2.Core
{
    public sealed class MPQProvider : IMPQProvider
    {
        private readonly GlobalConfiguration globalConfiguration;
        private readonly MPQ[] mpqs;
        private Dictionary<string, int> mpqLookup = new Dictionary<string, int>();

        public MPQProvider(GlobalConfiguration globalConfiguration)
        {
            this.globalConfiguration = globalConfiguration;
            // TODO: Make this less dumb. We need to an external file to configure mpq load order.
            var mpqsToLoad = new string[]
            {

            };
            this.mpqs = Directory
                .EnumerateFiles(globalConfiguration.BaseDataPath, "*.mpq")
                .Where(x => !Path.GetFileName(x).StartsWith("patch"))
                .Select(file => new MPQ(file))
                .ToArray();

            

            // Load the base game files
            for(var i = 0; i < mpqs.Count(); i++)
            {
                if (Path.GetFileName(mpqs[i].Path).StartsWith("d2exp") || Path.GetFileName(mpqs[i].Path).StartsWith("d2x"))
                    continue;

                foreach(var file in mpqs[i].Files)
                {
                    mpqLookup[file.ToLower()] = i;
                }
            }

            // Load the expansion game files
            for (var i = 0; i < mpqs.Count(); i++)
            {
                if (!Path.GetFileName(mpqs[i].Path).StartsWith("d2exp") && !Path.GetFileName(mpqs[i].Path).StartsWith("d2x"))
                    continue;

                foreach (var file in mpqs[i].Files)
                {
                    mpqLookup[file.ToLower()] = i;
                }
            }
        }

        public IEnumerable<MPQ> GetMPQs() => mpqs;

        public Stream GetStream(string fileName)
            => mpqs[mpqLookup[fileName.ToLower()]].OpenFile(fileName);
        

        public IEnumerable<string> GetTextFile(string fileName)
            => new StreamReader(mpqs[mpqLookup[fileName.ToLower()]].OpenFile(fileName)).ReadToEnd().Split('\n');


    }
}

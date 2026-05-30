using System;
using System.Collections.Generic;
using System.IO;
using Godot;

namespace Dose.Core.Library.Loaders
{
    public class JascPalLoader(string path)
    {
        public string Path { get; } = path;
    
        public List<Color> Load()
        {
            var lines = File.ReadAllLines(Path);

            if (lines[0] != "JASC-PAL")
                throw new Exception("Invalid palette format.");

            int colorCount = int.Parse(lines[2]);

            var colors = new List<Color>();

            for (int i = 0; i < colorCount; i++)
            {
                var parts = lines[i + 3]
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                float r = int.Parse(parts[0]) / 255f;
                float g = int.Parse(parts[1]) / 255f;
                float b = int.Parse(parts[2]) / 255f;

                colors.Add(new Color(r, g, b));
            }

            return colors;
        }
    }
}
using System;
using Godot;

namespace Dose.Core.Library.Locations
{
    public abstract class LocationBuilder(IBuildableLocation location)
    {
        public IBuildableLocation Location { get; } = location;

        public void Build()
        {
            if (Location.Map == null)
            {
                throw new Exception("Location doesn't have a defined Map.");
            }

            OnStart();

            foreach (var position in Location.Map.GetFilledPositions())
            {
                OnBuildCell(position);
            }

            OnFinish();
        }

        protected virtual void OnStart() { }
        
        protected virtual void OnFinish() { }

        protected abstract void OnBuildCell(Vector2I position);
    }
}
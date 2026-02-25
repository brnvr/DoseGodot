using Dose.Core.Library.Actors;
using Godot;

namespace Dose.Core.Actors.Items
{
    public record CurrantSyrup : ItemContext
    {
        public CurrantSyrup()
        {
            DisplayName = "currant syrup";
            Texture = ResourceLoader.Load<Texture2D>("res://Media/Textures/Npcs/Agnes/Idle_000.png");
            MaxQuantity = 1;
        }
    }
}
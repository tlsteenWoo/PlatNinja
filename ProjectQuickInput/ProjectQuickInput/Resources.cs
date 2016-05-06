using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectQuickInput
{
    public static class Resources
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();

        public static void AddTexture(Texture2D Texture, string Name)
        {
            Textures.Add(Name, Texture);
        }
        public static Texture2D GetTexture(string Name)
        {
            if(Textures.ContainsKey(Name))
                return Textures[Name];
            throw new IndexOutOfRangeException();
        }
        public static void AddFont(SpriteFont Font, string Name)
        {
            Fonts.Add(Name, Font);
        }
        public static SpriteFont GetFont(string Name)
        {
            if (Fonts.ContainsKey(Name))
                return Fonts[Name];
            throw new IndexOutOfRangeException();
        }
    }
}

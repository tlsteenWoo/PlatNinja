using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProjectQuickInput
{
    //a class that produces animation by
    //rendering a 'frame' from a texture.
    //Then moving onto the next frame.
    //Repeating until it reaches the last
    //frame, where it can loop and repeat.
    public class SpriteSheet
    {
        public int x, y;
        public int height, width;
        public int frameHeight, frameWidth;
        public int textureHeight, textureWidth;
        public Texture2D spritesheet;

        public float fps = 60;
        public float spf;
        public float elapsedSeconds;
        public int currentFrameIndex;
        public Rectangle frame;

        public SpriteSheet(Texture2D Texture, int FrameWidth, int FrameHeight, float FPS)
        {
            spritesheet = Texture;
            textureWidth = spritesheet.Width;
            textureHeight = spritesheet.Height;
            frameHeight = FrameHeight;
            frameWidth = FrameWidth;
            width = (int)Math.Ceiling((double)textureWidth / (double)frameWidth);
            height = (int)Math.Ceiling((double)textureHeight / (double)frameHeight);
            fps = FPS;
        }

        public void Update(float dt)
        {
            elapsedSeconds += dt;
            spf = (1 / fps);
            if (elapsedSeconds > spf * width * height) //LOOP
                elapsedSeconds = 0;
            currentFrameIndex = (int)(elapsedSeconds / spf);
            x = currentFrameIndex / height;
            y = currentFrameIndex / width;
            frame.X = x * frameWidth;
            frame.Y = y * frameHeight;
            frame.Width = frameWidth;
            frame.Height = frameHeight;
        }

        public void Draw(Vector2 Position, Color Color, SpriteEffects Effect, float Scale)
        {
            World.spritebatch.Draw(spritesheet, Position, frame, Color, 0, Vector2.Zero, Scale, Effect, 0);
        }
    }
}

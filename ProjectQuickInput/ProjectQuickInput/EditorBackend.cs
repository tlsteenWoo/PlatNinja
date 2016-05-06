using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectQuickInput
{
    /// <summary>
    /// Provides a palette of entities to place in the world.
    /// Also allows selection and transformation of entities in the world.
    /// </summary>
    public class EditorBackend
    {
        //default tile size in pixels
        const float DefaultWidthInPixels = 50;
        const float DefaultHeightInPixels = 50;
        //default editor snapping size in pixels
        const float snapSizeInPixels = 5;
        //?
        public static readonly Vector2 DefaultPositionInPixels = Vector2.Zero;

        //array of all placeable entities
        Entity[] entityPalette;
        //index of current entity in palette
        int current = 0;
        //reference to an entity selected in the world
        Entity selection;

        /// <summary>
        /// Creates the entity palette
        /// </summary>
        public EditorBackend()
        {
            float width = World.Scale(DefaultWidthInPixels);
            float height = World.Scale(DefaultHeightInPixels);
            Vector2 position = World.Scale(DefaultPositionInPixels);
            //Create default entities to populate our palette
            Entity[] es = {
                new Entity(position, width, height, Color.PaleGoldenrod, Resources.GetTexture("Pixel")),
                new Player(position),
                new Enemy(position),
                new Spikes(position, width, height),
                new FallingPlatform(position, width, height),
                new Ice(position, height, height)
            };
            entityPalette = es;
        }

        public void Update()
        {
            //Abort updating if the game isn't being played
            if (!PlatNinja.isActive) return;

            //deselect current entity then try to select a new entity
            if (Input.i.IsRightMouseClick())
            {
                selection = null;
                foreach (Entity entity in World.curLevel.entities)
                {
                    if (entity.GetAABB().Contains(World.ToWorld(Input.MouseLoc)))
                    {
                        selection = entity;
                        break;
                    }
                }
            }
            //cycle through the entity palette
            if (Input.IsKeyPress(Keys.E))
            {
                //cycle right
                current = (current + 1) % entityPalette.Length;
            }
            else if (Input.IsKeyPress(Keys.Q))
            {
                //cycle left
                current--;
                if (current < 0)
                {
                    current = entityPalette.Length - 1;
                }
            }
            //delete the selection (cannot delete player)
            if (Input.IsKeyPress(Keys.Back) || Input.IsKeyPress(Keys.Delete))
            {
                if (selection != null &&
                    selection != World.curLevel.p)
                {
                    World.curLevel.RemoveEntity(selection);
                    //remove our (hopefully) last reference to the entity
                    selection = null;
                }
            }
            //add the current entity, from the palette, to the world
            if (Input.IsKeyPress(Keys.Enter))
            {
                Entity e = World.curLevel.AddEntity(entityPalette[current].Clone() as Entity);
                //move the entity at the mouse position
                e.state.position.X = World.Scale(Input.MouseX - (Input.MouseX % snapSizeInPixels));
                e.state.position.Y = World.Scale(Input.MouseY - (Input.MouseY % snapSizeInPixels));
                //todo: reset the entities phsics perhaps, its not gameplay so its ok to block some potential fun stuff (right?)
            }
            TransformSelection();
        }
        /// <summary>
        /// Based on user input. Move the selection. Push out or Pull in its sides.
        /// </summary>
        public void TransformSelection()
        {
            //abort the transform if we dont have anything selected
            if (selection == null) return;

            //scale defaults to the original size
            float scale = 1;
            //increase the scale if we want larger snapping
            if (Input.IsKeyDown(Keys.RightShift) || Input.IsKeyDown(Keys.LeftShift))
                scale = snapSizeInPixels; //(snapSize ^ 2)
            float scaledSnap = World.Scale(snapSizeInPixels * scale);

            //place the selection at the mouse
            if (Input.i.IsLeftMouseClick())
            {
                Vector2 newPosition = new Vector2( Input.MouseX - (Input.MouseX % snapSizeInPixels),
                    Input.MouseY - (Input.MouseY % snapSizeInPixels));
                selection.state.position = newPosition;
            }
            //move the selection right or left
            selection.state.position.X += Input.IsKeyPress(Keys.D) ? scaledSnap : Input.IsKeyPress(Keys.A) ? -scaledSnap : 0;
            //move the selection down or up
            selection.state.position.Y += Input.IsKeyPress(Keys.S) ? scaledSnap : Input.IsKeyPress(Keys.W) ? -scaledSnap : 0;
            //push out the RIGHT side
            if (Input.IsKeyPress(Keys.K))
            {
                selection.state.size.X += scaledSnap;
            }
            //pull in the RIGHT side
            if (Input.IsKeyPress(Keys.J))
            {
                selection.state.size.X -= scaledSnap;
            }
            //push out the LEFT side
            if (Input.IsKeyPress(Keys.G))
            {
                selection.state.size.X += scaledSnap;
                selection.state.position.X -= scaledSnap;
            }
            //pull in the LEFT side
            if (Input.IsKeyPress(Keys.H))
            {
                selection.state.size.X -= scaledSnap;
                selection.state.position.X += scaledSnap;
            }
            //push out the BOTTOM side
            if (Input.IsKeyPress(Keys.M))
            {
                selection.state.size.Y += scaledSnap;
            }
            //pull in the BOTTOM side
            if (Input.IsKeyPress(Keys.N))
            {
                selection.state.size.Y -= scaledSnap;
            }
            //push out the TOP side
            if (Input.IsKeyPress(Keys.Y))
            {
                selection.state.size.Y += scaledSnap;
                selection.state.position.Y -= scaledSnap;
            }
            //pull in the TOP side
            if (Input.IsKeyPress(Keys.U))
            {
                selection.state.size.Y -= scaledSnap;
                selection.state.position.Y += scaledSnap;
            }
        }

        public void Draw()
        {
            //use non premultiplied so we can adjust transparency through alpha alone
            World.spritebatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied,null,null,null,null,World.GetRenderMatrix());
            //draw the current entity in the palette
            if (entityPalette[current] != null)
            {
                entityPalette[current].Draw();
            }
            //draw a selection box around our selection
            if(selection != null)
                World.spritebatch.Draw(Resources.GetTexture("Pixel"), new Rectangle(
                    (int)selection.state.position.X, (int)selection.state.position.Y,
                    (int)selection.state.size.X, (int)selection.state.size.Y),
                    new Color(0, 1, 0, 0.4f));
            World.spritebatch.End();
        }
    }
}

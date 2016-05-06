using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ProjectQuickInput
{
    public class Level
    {
        public Player p;
        public List<Entity> entities;
        public Action updateLogic;

        public Level()
        {
            entities = new List<Entity>();
        }
        public Entity AddEntity(Entity E)
        {
            entities.Add(E);
            return E;
        }
        public void RemoveEntity(Entity E)
        {
            entities.Remove(E);
        }
        //enter an entity into a writer tream
        public void SerializeEntity(Entity E, StreamWriter Writer)
        {

        }
        void Clear()
        {
            updateLogic = null;
            entities.Clear();
        }
        public void CreateSquishBoxLevel()
        {
            Clear();
            p = new Player(new Vector2(200, 400));
            entities.Add(p);
            entities.Add(new Entity(new Vector2(0, 400), 800, 40, Color.Red, PlatNinja.pixelRUNTIME));
            entities.Add(new Ice(new Vector2(300, 350), 100, 40));
            entities.Add(new Ice(new Vector2(400, 300), 100, 40));
            //pillar
            entities.Add(new Ice(new Vector2(460, 240), 40, 100));
            entities.Add(new Spikes(new Vector2(500, 400), 100, 40));
            entities.Add(new Ice(new Vector2(600, 300), 100, 40));
            entities.Add(new Ice(new Vector2(700, 350), 100, 40));

            //backtrack
            entities.Add(new Ice(new Vector2(180, 300), 60, 30));
            //70 + 10 for wiggle room
            entities.Add(new Ice(new Vector2(208, 180), 185, 40));
            entities.Add(new Ice(new Vector2(0, 275), 208, 40));
            entities.Add(new Ice(new Vector2(World.graphics.Viewport.Width - 200, 275), 200, 40));
            entities.Add(new Ice(new Vector2(World.graphics.Viewport.Width - 240, 300), 40, 25));
            entities.Add(new Ice(new Vector2(World.graphics.Viewport.Width - 200, 205), 200, 40));
            entities.Add(new Ice(new Vector2(0, 175), 200, 40));
        }
        public void CreateDipDiveLevel()
        {
            Clear();
            p = new Player(new Vector2(100, 400));
            entities.Add(p);
            entities.Add(new Entity(new Vector2(200, 350), 100, 40, Color.Gray, Resources.GetTexture("Pixel")));
            entities.Add(new Ice(new Vector2(300, 300), 100, 40));
            entities.Add(new Ice(new Vector2(425, 300), 100, 40));
            entities.Add(new Entity(new Vector2(525, 350), 100, 40, Color.Gray, Resources.GetTexture("Pixel")));
        }
        public void AWholeNewWorld()
        {
            //clear the level in case we are re-loading
            Clear();
            //create the player and add him
            p = new Player(new Vector2(World.Scale(100), World.Scale(400)));
            entities.Add(p);
            //make a surface to stand on and ADD it to the world
            Vector2 position = new Vector2(0, World.GetWorldHeight() * .8f);
            Vector2 size = new Vector2(World.GetWorldWidth(), World.GetWorldHeight() * .2f);
            Texture2D block = Resources.GetTexture("Block");
            var ground = new Entity(position, size.X,size.Y, Color.White, block);
            entities.Add(ground);
            //create an enemy so we can test precision failure
            var enemy = new Enemy(World.Scale(new Vector2(600, 200)));
            entities.Add(enemy);
        }
        public void CreateEnemyLevel()
        {
            Clear();
            p = new Player(new Vector2(1, 4));
            entities.Add(p);
            Texture2D house = Resources.GetTexture("House");
            Texture2D houseTall = Resources.GetTexture("House_Tall");
            entities.Add(new Prop(World.Scale(new Vector2(160, 340)), house));
            entities.Add(new Prop(World.Scale(new Vector2(220f, 280f)), houseTall));
            entities.Add(new Prop(World.Scale(new Vector2(280f, 340f)), house));
            entities.Add(new Prop(World.Scale(new Vector2(100, 200f)), Resources.GetTexture("Car"), 5.0f));
            entities.Add(new Entity(World.Scale(new Vector2(0, 0)), 10, World.GetWorldHeight(), Color.White, Resources.GetTexture("Block")));
            entities.Add(new Entity(World.Scale(new Vector2(World.graphics.Viewport.Width - 10f, 0)), 1.0f, World.GetWorldHeight(), Color.White, Resources.GetTexture("Block")));
            entities.Add(new Entity(World.Scale(new Vector2(0, 400f)), World.graphics.Viewport.Width, World.Scale(400f), Color.White, Resources.GetTexture("Block")));
            //entities.Add(new Entity(new Vector2(400, 350), 50, 40, Color.Gray, Resources.GetTexture("Pixel")));
            AddEntity(new Ice(World.Scale(new Vector2(400f, 350f)), World.Scale(50), World.Scale(40f)));
            updateLogic = () => { 
                if(EnemyCount() < 5)
                    entities.Insert(0,new Enemy(new Vector2(Helper.Random.Next(0, (int)World.GetWorldWidth()), 0)));
            };
        }
        public void CreateEmptyLevel()
        {
            Clear();
            p = new Player(new Vector2(100, 400));
            entities.Add(p);
        }
        public void CreateCollisionResponseLvl()
        {
            Clear();
            p = new Player(new Vector2(400, 400));
            World.i.Gravity = new Vector2(0, 100);
            entities.Add(p);
            Enemy a = new Enemy(new Vector2(200, 300)), b = new Enemy(new Vector2(World.graphics.Viewport.Width-200, 300));
            entities.Add(a);
            entities.Add(b);
            a.state.velocity = new Vector2(200, -100);
            b.state.velocity = new Vector2(-200, -100);
        }

        public int EnemyCount()
        {
            int count = 0;
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] is Enemy)
                    count++;
            }
            return count;
        }
    }
}

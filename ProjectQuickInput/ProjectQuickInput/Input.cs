using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ProjectQuickInput
{
    public struct InputState
    {
        public KeyboardState key, keyOld;
        public MouseState mouse, mouseOld;

        public void Update()
        {
            keyOld = key;
            key = Keyboard.GetState();
            mouseOld = mouse;
            mouse = Mouse.GetState();
        }
        public int MouseX
        {
            get { return mouse.X; }
        }
        public int MouseY
        {
            get { return mouse.Y; }
        }
        #region Left Mouse
        public bool IsLeftMouseDown()
        {
            return mouse.LeftButton == ButtonState.Pressed;
        }
        public bool IsLeftMouseUp()
        {
            return mouse.LeftButton == ButtonState.Released;
        }
        /// <summary>
        /// Returns true the instant the left mouse button is released
        /// </summary>
        /// <returns>Left Button Clicked</returns>
        public bool IsLeftMouseClick()
        {
            return mouse.LeftButton == ButtonState.Pressed && mouseOld.LeftButton == ButtonState.Released;
        }
        public bool IsLeftMouseReleased()
        {
            return mouse.LeftButton == ButtonState.Released && mouseOld.LeftButton == ButtonState.Pressed;
        }
        #endregion

        #region Right Mouse
        public bool IsRightMouseDown()
        {
            return mouse.RightButton == ButtonState.Pressed;
        }
        public bool IsRightMouseUp()
        {
            return mouse.RightButton == ButtonState.Released;
        }
        /// <summary>
        /// Returns true the instant that the right mouse button is released
        /// <returns></returns>
        public bool IsRightMouseClick()
        {
            return mouse.RightButton == ButtonState.Pressed && mouseOld.RightButton == ButtonState.Released;
        }
        /// <summary>
        /// Returns true while the right mouse button is released
        /// </summary>
        /// <returns></returns>
        public bool IsRightMouseReleased()
        {
            return mouse.RightButton == ButtonState.Released && mouseOld.RightButton == ButtonState.Pressed;
        }
        #endregion

        #region Middle Mouse
        public bool IsMiddleMouseDown()
        {
            return mouse.MiddleButton == ButtonState.Pressed;
        }
        public bool IsMiddleMouseUp()
        {
            return mouse.MiddleButton == ButtonState.Released;
        }
        public bool IsMiddleMousePress()
        {
            return mouse.MiddleButton == ButtonState.Pressed && mouseOld.MiddleButton == ButtonState.Released;
        }
        public bool IsMiddleMouseReleased()
        {
            return mouse.MiddleButton == ButtonState.Released && mouseOld.MiddleButton == ButtonState.Pressed;
        }
        #endregion

        public bool IsKeyDown(Keys K)
        {
            return key.IsKeyDown(K);
        }
        public bool IsKeyUp(Keys K)
        {
            return key.IsKeyUp(K);
        }
        public bool IsKeyPress(Keys K)
        {
            return key.IsKeyDown(K) && keyOld.IsKeyUp(K);
        }
        public bool IsKeyRelease(Keys K)
        {
            return keyOld.IsKeyDown(K) && key.IsKeyUp(K);
        }
    }

    public struct KC
    {
        public Keys[] keys;

        public KC(params Keys[] Input)
        {
            keys = Input;
        }
    }

    public static class Input
    {
        public static float sequenceResetTarget= 0.2f, sequenceResetTimer = 0;
        public static List<Keys> sequence = new List<Keys>();
        public static InputState i = new InputState();

        public static int MouseX
        {
            get { return i.MouseX; }
        }
        public static int MouseY
        {
            get { return i.MouseY; }
        }
        public static Vector2 MouseLoc
        {
            get { return new Vector2(MouseX, MouseY); }
        }

        public static void Update(float dt)
        {
            i.Update();
            sequenceResetTimer += dt;
            //reset the sequence if its been to long since last input
            Keys[] pressed = i.key.GetPressedKeys();
            if (pressed.Length > 0)
            {
                for (int k = 0; k < pressed.Length; k++)
                {
                    if (i.keyOld.IsKeyUp(pressed[k]))
                    {
                        sequence.Add(pressed[k]);
                        sequenceResetTimer = 0;
                        break;
                    }
                }
            }
            if (sequenceResetTimer > sequenceResetTarget)
            {
                Reset();
            }
            //if there is input, appen it to the sequence
            //there should probably be some sort of criteria for input being combined
        }

        public static bool IsKeyDown(Keys K)
        {
            return i.IsKeyDown(K);
        }
        public static bool IsKeyUp(Keys K)
        {
            return i.IsKeyUp(K);
        }
        public static bool IsKeyPress(Keys K)
        {
            return i.IsKeyPress(K);
        }
        public static bool IsKeyRelease(Keys K)
        {
            return i.IsKeyRelease(K);
        }
        public static void Reset()
        {
            sequence.Clear();
            sequenceResetTimer = 0;
        }
        //Currently only does a sequence of single keys
        public static bool RecentKeySequence(bool clearIfTrue, params Keys[] InputSequence)
        {
            if (InputSequence.Length > sequence.Count)
                return false;
            //[12345] [123]
            for (int i = 0; i < InputSequence.Length; i++)
                if (sequence[(sequence.Count - InputSequence.Length)+i] != InputSequence[i])
                    return false;
            if(clearIfTrue)
                Reset();
            return true;
        }
    }
}
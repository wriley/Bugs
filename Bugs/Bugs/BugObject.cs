using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bugs
{
    public class BugObject
    {
        Texture2D Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public float Speed = 2f;
        private Vector2 Origin;
        private bool condBlocked = false;

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    Texture.Width,
                    Texture.Height);
            }
        }

        public BugObject(Texture2D texture, Vector2 position)
        {
            this.Texture = texture;
            this.Position = position;
            this.Rotation = 0f;
            this.Velocity = new Vector2(0f, 0f);
            this.Origin.X = this.Texture.Width / 2;
            this.Origin.Y = this.Texture.Height / 2;
        }

        public BugObject(Texture2D texture, Vector2 position, float rotation)
        {
            this.Texture = texture;
            this.Position = position;
            this.Rotation = rotation;
            this.Velocity = new Vector2(0f, 0f);
            this.Origin.X = this.Texture.Width / 2;
            this.Origin.Y = this.Texture.Height / 2;
        }

        public BugObject(Texture2D texture, Vector2 position, Vector2 velocity)
        {
            this.Texture = texture;
            this.Position = position;
            this.Velocity = velocity;
            this.Rotation = (float)Math.Atan2(this.Velocity.X, this.Velocity.Y);
            this.Origin.X = this.Texture.Width / 2;
            this.Origin.Y = this.Texture.Height / 2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Origin, 1f, SpriteEffects.None, 0f);
        }

        public void Move()
        {
            if (this.condBlocked)
            {
                this.Rotation += 2f;
                condBlocked = false;
            }
            else
            {
                float x = this.Position.X;
                float y = this.Position.Y;
                Matrix m = Matrix.CreateRotationZ(this.Rotation);
                x += m.M12 * Speed;
                y -= m.M11 * Speed;
                this.Position = new Vector2(x, y);
            }
        }

        public void CheckCollision(Viewport viewport)
        {
            int MaxX = viewport.Width - (this.Texture.Width / 2);
            int MinX = this.Texture.Width / 2;
            int MaxY = viewport.Height - (this.Texture.Height / 2);
            int MinY = this.Texture.Height / 2;

            if (this.Position.X > MaxX)
            {
                this.Position.X = MaxX;
                this.condBlocked = true;
            }
            else if (this.Position.X < MinX)
            {
                this.Position.X = MinX;
                this.condBlocked = true;
            }

            if (this.Position.Y > MaxY)
            {
                this.Position.Y = MaxY;
                this.condBlocked = true;
            }
            else if (this.Position.Y < MinY)
            {
                this.Position.Y = MinY;
                this.condBlocked = true;
            }
        }
    }
}
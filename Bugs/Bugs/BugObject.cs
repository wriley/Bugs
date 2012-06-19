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
        private Texture2D _texture;
        private float _rotation;
        private float _speed = 20f;
        private float _rotationStep = 10f;
        private Vector2 _origin;
        private Random _random = new Random();
        private Vector2 _position;
        private Vector2 _velocity;

        public Rectangle BoundingBox
        {
            get
            {
                Matrix toWorldSpace =
                    Matrix.CreateTranslation(new Vector3(-this._origin, 0.0f)) *
                    Matrix.CreateRotationZ(this._rotation) *
                    Matrix.CreateTranslation(new Vector3(this._position, 0.0f));

                return CalculateTransformedBoundingBox(
                    new Rectangle(0, 0, _texture.Width, _texture.Height),
                    toWorldSpace);
            }
        }

        private Rectangle CalculateTransformedBoundingBox(Rectangle local, Matrix toWorldSpace)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(local.Left, local.Top);
            Vector2 rightTop = new Vector2(local.Right, local.Top);
            Vector2 leftBottom = new Vector2(local.Left, local.Bottom);
            Vector2 rightBottom = new Vector2(local.Right, local.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref toWorldSpace,
                             out leftTop);
            Vector2.Transform(ref rightTop, ref toWorldSpace,
                             out rightTop);
            Vector2.Transform(ref leftBottom, ref toWorldSpace,
                             out leftBottom);
            Vector2.Transform(ref rightBottom, ref toWorldSpace,
                             out rightBottom);

            // Find the minimum and maximum extents of the
            // rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                     Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                     Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public BugObject(Texture2D texture, Vector2 position)
        {
            this._texture = texture;
            this._position = position;
            this._rotation = 0f;
            this._velocity = new Vector2(0f, 0f);
            this._origin.X = this._texture.Width / 2;
            this._origin.Y = this._texture.Height / 2;
        }

        public BugObject(Texture2D texture, Vector2 position, float rotation)
        {
            this._texture = texture;
            this._position = position;
            this._rotation = rotation;
            this._velocity = new Vector2(0f, 0f);
            this._origin.X = this._texture.Width / 2;
            this._origin.Y = this._texture.Height / 2;
        }

        public BugObject(Texture2D texture, Vector2 position, Vector2 velocity)
        {
            this._texture = texture;
            this._position = position;
            this._velocity = velocity;
            this._rotation = (float)Math.Atan2(this._velocity.X, this._velocity.Y);
            this._origin.X = this._texture.Width / 2;
            this._origin.Y = this._texture.Height / 2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, null, Color.White, _rotation, _origin, 1f, SpriteEffects.None, 0f);
        }

        public void Move(GameTime gameTime)
        {
            float x = this._position.X;
            float y = this._position.Y;
            Matrix m = Matrix.CreateRotationZ(this._rotation);
            x += m.M12 * (float)gameTime.ElapsedGameTime.TotalSeconds * _speed;
            y -= m.M11 * (float)gameTime.ElapsedGameTime.TotalSeconds * _speed;
            this._position = new Vector2(x, y);
        }

        public void CheckBoundaryCollision(GameTime gameTime, Rectangle limits)
        {
            int MaxX = limits.Width - 1;
            int MinX = 1;
            int MaxY = limits.Height - 1;
            int MinY = 1;
            int angle = (int)RadianToDegree(this._rotation);

            if (this._position.X > MaxX)
            {
                this._position.X = MaxX;
                if (angle >= 0 && angle < 90)
                {
                    this._rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
                }
                else
                {
                    this._rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
                }
            }
            else if (this._position.X < MinX)
            {
                this._position.X = MinX;
                if (angle >= 270 && angle < 360)
                {
                    this._rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
                }
                else
                {
                    this._rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
                }
            }

            if (this._position.Y > MaxY)
            {
                this._position.Y = MaxY;
                if (angle >= 180 && angle < 270)
                {
                    this._rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
                }
                else
                {
                    this._rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
                }
            }
            else if (this._position.Y < MinY)
            {
                this._position.Y = MinY;
                if (angle >= 0 && angle < 90)
                {
                    this._rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
                }
                else
                {
                    this._rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
                }
            }
        }

        public void Avoid(GameTime gameTime)
        {
            double r = _random.NextDouble();
            if (r >= 0.5)
            {
                this._rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
            }
            else
            {
                this._rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
            }
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}
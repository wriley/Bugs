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
        private float _speed = 25f;
        private float _rotationStep = 10f;
        private Vector2 _origin;
        private Random _random = new Random();
        private Vector2 _position;
        private int _wanderTimer;
        private int _wanderTimerMax = 50;
        private int _backupTimer;
        private int _backupTimerMax = 15;
        private int _turnTimer;
        private int _turnTimerMax = 5;
        private int _restTimer;
        private int _restTimerMax = 150;
        private int _stamina = 10000;
        private int _longestSide;
        private _states _state;
        private int _turnLeft = 1;
        private int _life = 100000;
        private int _stuckCounter = 0;
        private int _stuckCounterMax = 10;

        private enum _states
        {
            wander,
            backup,
            turn,
            stuck,
            rest,
            dead
        };

        public BugObject(Texture2D texture, Vector2 position, float rotation = 0f)
        {
            _texture = texture;
            _position = position;
            _rotation = rotation;
            _origin.X = _texture.Width / 2;
            _origin.Y = _texture.Height / 2;
            _longestSide = _texture.Width > _texture.Height ? _texture.Width : _texture.Height;
            _state = _states.wander;
            _wanderTimer = 0;
            _backupTimer = 0;
            _turnTimer = 0;
            _restTimer = 0;
            _stamina += _random.Next(0, 2501);
            _life += _random.Next(0, 25001);
        }

        public bool isDead()
        {
            return _life == 0;
        }

        public bool isStuck()
        {
            return _state == _states.stuck;
        }

        public Rectangle BoundingBox
        {
            get
            {
                Matrix toWorldSpace =
                    Matrix.CreateTranslation(new Vector3(-_origin, 0.0f)) *
                    Matrix.CreateRotationZ(_rotation) *
                    Matrix.CreateTranslation(new Vector3(_position, 0.0f));

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

        public void Update(GameTime gameTime, Rectangle limits)
        {
                this.Move(gameTime);
                this.CheckBoundaryCollision(gameTime, limits);
                if (_rotation > DegreeToRadian(360))
                {
                    _rotation = 0f;
                }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, null, Color.White, _rotation, _origin, 1f, SpriteEffects.None, 0f);
        }

        private void Move(GameTime gameTime)
        {
            float x, y;
            Matrix m;

            if (--_life == 0)
            {
                _state = _states.dead;
            }
            else if (_stamina < 1)
            {
                _state = _states.rest;
            }

            switch (_state)
            {
                case _states.wander:
                    if (_stuckCounter > 0) { _stuckCounter--; }
                    if (_wanderTimer++ >= _wanderTimerMax)
                    {
                        double d = _random.NextDouble();
                        if (d > 0.5)
                        {
                            _rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
                        }
                        else
                        {
                            _rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
                        }
                        _wanderTimer = (int)(d * 10) + 10;
                    }
                    x = _position.X;
                    y = _position.Y;
                    m = Matrix.CreateRotationZ(_rotation);
                    x += m.M12 * (float)gameTime.ElapsedGameTime.TotalSeconds * _speed;
                    y -= m.M11 * (float)gameTime.ElapsedGameTime.TotalSeconds * _speed;
                    _position = new Vector2(x, y);
                    _stamina--;
                    break;
                case _states.backup:
                    if (_backupTimer++ < _backupTimerMax)
                    {
                        x = _position.X;
                        y = _position.Y;
                        m = Matrix.CreateRotationZ(_rotation);
                        x -= m.M12 * (float)gameTime.ElapsedGameTime.TotalSeconds * _speed;
                        y += m.M11 * (float)gameTime.ElapsedGameTime.TotalSeconds * _speed;
                        _position = new Vector2(x, y);
                    }
                    else
                    {
                        _backupTimer = 0;
                        _state = _states.turn;
                    }
                    _stamina--;
                    if (_stuckCounter++ > _stuckCounterMax)
                    {
                        _state = _states.stuck;
                    }
                    break;
                case _states.turn:
                    if (_turnTimer++ < _turnTimerMax)
                    {
                        _rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep * _turnLeft;
                    }
                    else
                    {
                        _turnTimer = 0;
                        _state = _states.wander;
                    }
                    break;
                case _states.stuck:
                    if (_turnTimer++ < _turnTimerMax*2)
                    {
                        _rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * _rotationStep;
                    }
                    else
                    {
                        _turnTimer = 0;
                        _state = _states.wander;
                    }
                    break;
                case _states.rest:
                    if (_restTimer++ < _restTimerMax)
                    {
                        _stamina += 10;
                    }
                    else
                    {
                        _restTimer = 0;
                        _state = _states.wander;
                    }
                    break;
                case _states.dead:
                    break;
            }
        }

        private void CheckBoundaryCollision(GameTime gameTime, Rectangle limits)
        {
            int MaxX = limits.Width - (_longestSide / 2);
            int MinX = _longestSide / 2;
            int MaxY = limits.Height - (_longestSide / 2);
            int MinY = _longestSide / 2;
            double angle = RadianToDegree(_rotation);

            if (_position.X > MaxX)
            {
                _position.X = MaxX;
                _state = _states.backup;
                if (angle > 0 && angle <= 90)
                {
                    _turnLeft = 1;
                }
                else
                {
                    _turnLeft = -1;
                }
            }
            else if (_position.X < MinX)
            {
                _position.X = MinX;
                _state = _states.backup;
                if (angle > 180 && angle <= 270)
                {
                    _turnLeft = 1;
                }
                else
                {
                    _turnLeft = -1;
                }
            }

            if (_position.Y > MaxY)
            {
                _position.Y = MaxY;
                _state = _states.backup;
                if (angle > 90 && angle <= 180)
                {
                    _turnLeft = 1;
                }
                else
                {
                    _turnLeft = -1;
                }
            }
            else if (_position.Y < MinY)
            {
                _position.Y = MinY;
                _state = _states.backup;
                if (angle > 270 && angle <= 360)
                {
                    _turnLeft = 1;
                }
                else
                {
                    _turnLeft = -1;
                }
            }
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private double DegreeToRadian(double radian)
        {
            return radian * (Math.PI / 180.0);
        }
    }
}
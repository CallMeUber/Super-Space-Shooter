using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Super_Space_Shooter
{
    class PlayerClass
    {
        public Texture2D _sprite;
        private Rectangle _rectangle;
        private Rectangle _transformedrec;
        private Vector2 _position;
        private Vector2 _speed;
        private float _rotation;
        private Vector2 _accelerate;
        private Vector2 _center;
        private float _health;
        private GamePadState input;
        private GamePadState oldinput;
        private Projectile bullet;
        private List<Projectile> _bullets;
        private List<Projectile> _deleteBul;
        private int _selfnum;
        private Color _shipcolor;
        public PlayerClass(Texture2D sprite,Vector2 position, float health, int playernumber)
        {
            _sprite = sprite;
            _position = position;
            _speed = new Vector2(0);
            _rotation = (float)Math.PI / 2;
            _health = health;
            _bullets = new List<Projectile>();
            _deleteBul = new List<Projectile>();
            _transformedrec = new Rectangle();
            _selfnum = playernumber;
            switch (_selfnum)
            {
                case 0: _shipcolor = Color.White; break;
                case 1: _shipcolor = Color.Yellow; break;
                case 2: _shipcolor = Color.Green; break;
                case 3: _shipcolor = Color.Blue; break;
            }
        }

        public void Update(int boundwidth, int boundheight)
        {
            input = GamePad.GetState(_selfnum);
            _center = new Vector2(_position.X + (float)Math.Cos(_rectangle.X), _position.Y + 10 + (float)Math.Sin(_rectangle.Y));
            _rectangle = new Rectangle((int)_position.X - (int)_center.X, (int)_position.Y - (int)_center.Y, _sprite.Width, _sprite.Height);
            if (input.DPad.Right == ButtonState.Pressed)
            {
                _rotation += 0.1f;
            }
            else if(input.DPad.Left == ButtonState.Pressed)
            {
                _rotation -= 0.1f;
            }
            if(input.Buttons.A == ButtonState.Pressed)
            {
                _accelerate.X = (float)Math.Cos(_rotation);
                _accelerate.Y = (float)Math.Sin(_rotation);
            }
            else if (input.Buttons.B == ButtonState.Pressed)
            {
                _accelerate.X = -(float)Math.Cos(_rotation);
                _accelerate.Y = -(float)Math.Sin(_rotation);
            }
            else
            {
                _accelerate = new Vector2(0);
            }
            _speed += _accelerate;
            if((_position.X > boundwidth && _speed.X > 0) || (_position.X < 0 && _speed.X < 0))
            {
                _speed.X = 0;
            }
            if ((_position.Y > boundheight && _speed.Y > 0) || (_position.Y < 0 && _speed.Y < 0))
            {
                _speed.Y = 0;
            }
            _position += _speed / 10;
            if (input.IsButtonDown(Buttons.Back))
                _health = 0;
            if (_bullets != null)
            {
                foreach (Projectile item in _bullets)
                {
                    item.UpdateBullet();
                }
                foreach (Projectile item2 in _bullets)
                {
                    if (item2.RemoveItem())
                        _deleteBul.Add(item2);
                }
                foreach (Projectile item3 in _deleteBul)
                {
                    _bullets.Remove(item3);
                }
            }
        }

        public void Shoot(SoundEffect shootsound, Texture2D bulletsprite)
        {
            if(_health > 0)
            {
                input = GamePad.GetState(_selfnum);
                if (input.Buttons.X == ButtonState.Pressed && oldinput.Buttons.X == ButtonState.Released)
                {
                    bullet = new Projectile(bulletsprite, _center, 3f, _rotation); /////3f bullet speed
                    _bullets.Add(bullet);
                    shootsound.Play();
                }
                oldinput = input;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D healthsprite)
        {
            if(_health > 0)
            {
                spriteBatch.Draw(_sprite, _position, new Rectangle(0, 0, _sprite.Width, _sprite.Height), _shipcolor, _rotation, new Vector2(70, 150), 0.3f, SpriteEffects.None, 1f);
                if (_bullets != null)
                {
                    foreach (Projectile item in _bullets)
                        item.Draw(spriteBatch);
                }
                spriteBatch.Draw(healthsprite, new Rectangle(_transformedrec.Center.X - 20, _transformedrec.Top - 10, (int)_health, 10), Color.White);
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return _rectangle;
            }
            set
            {
                _rectangle = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        public List<Projectile> BulletList
        {
            get
            {
                return _bullets;
            }
        }

        public List<Projectile> DeleteBulletList
        {
            get
            {
                return _deleteBul;
            }
        }

        public Vector2 Center
        {
            get
            {
                return _center;
            }
        }

        public float Rotation
        {
            get
            {
                return _rotation;
            }
        }

        public Vector2 Speed
        {
            set
            {
                _speed = value;
            }
        }
        public float Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
            }
        }

        public Rectangle TransformedRectangle
        {
            get
            {
                return _transformedrec;
            }
            set
            {
                _transformedrec = value;
            }
        }

        public int PlayerNumber
        {
            get
            {
                return _selfnum;
            }
        }
    }
}

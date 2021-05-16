using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Space_Shooter
{
    class EnemyClass
    {
        public Texture2D _enemysprite;
        private Vector2 _enemypos;
        private Vector2 _enemyspeed;
        private Rectangle _enemyrec;
        public int _size;
        public Color[] _textureData;
        public Vector2 _enemyorigin;

        public EnemyClass(Texture2D image, Vector2 position)///EDITTHIS FOR INHERITANCE
        {
            _enemysprite = image;
            _enemypos = position;
            _enemyorigin = new Vector2(_enemysprite.Width / 2, _enemysprite.Height / 2);
            _textureData = new Color[image.Width * image.Height];
            image.GetData(_textureData);
        }

        public void Update(Vector2 unitfollowpoint, float speed, int size)
        {
            _size = size;
            _enemyspeed = new Vector2(speed);
            _enemypos.X += _enemyspeed.X * unitfollowpoint.X;
            _enemypos.Y += _enemyspeed.Y * unitfollowpoint.Y;
            _enemyrec = new Rectangle((int)_enemypos.X - (int)_enemyorigin.X, (int)_enemypos.Y - (int)_enemyorigin.Y, _enemysprite.Width, _enemysprite.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_enemysprite, _enemypos, new Rectangle(0,0, _enemysprite.Width, _enemysprite.Height), Color.White, 0f, _enemyorigin, 1f, SpriteEffects.None, 0);
        }

        public Rectangle EnemyRec
        {
            get
            {
                return _enemyrec;
            }
            set
            {
                _enemyrec = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _enemypos;
            }           
        }
    }
}

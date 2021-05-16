using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Space_Shooter
{
    class Projectile
    {
        public Texture2D _projectileSprite;
        private Vector2 _projectilevector;
        private Rectangle _projectilerec;
        private float _projectiledirection;
        private float _projectilespeed;

        public Projectile(Texture2D newTexture, Vector2 spawn, float speed, float direction)
        {
            _projectileSprite = newTexture;
            _projectilevector = spawn;
            _projectilerec = new Rectangle((int)_projectilevector.X, (int)_projectilevector.Y, 50, 50);
            _projectiledirection = direction;
            _projectilespeed = speed;
        }

        public void UpdateBullet()
        {
            _projectilevector.Y += (float)Math.Sin(_projectiledirection) * _projectilespeed;
            _projectilevector.X += (float)Math.Cos(_projectiledirection) * _projectilespeed;
            _projectilerec = new Rectangle((int)_projectilevector.X, (int)_projectilevector.Y, 20, 20);
        }

        public Rectangle CollisionRec
        {
            get
            {
                return _projectilerec;
            }              
        }

        public void Draw(SpriteBatch spriteBatch)
        {
                spriteBatch.Draw(_projectileSprite, _projectilerec, null, Color.White, _projectiledirection, new Vector2(13,42), SpriteEffects.None, 1f);
        }
        public Vector2 Position
        {
            get
            {
                return _projectilevector;
            }            
        }
        public bool RemoveItem()
        {
            if (_projectilevector.X > 800 || _projectilevector.X < 0 || _projectilevector.Y > 600 || _projectilevector.X < 0)
                return true;
            else
                return false;
        }
    }
}

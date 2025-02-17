using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace TileEngine
{
	/// <summary>
	/// Describes the view of the map visible through the game's viewport
	/// </summary>
    public class Camera
    {

		private float _speed = 5f;   
  		private bool _isFocusedCursor = false, _isFocusedUnit = false;
		private Cursor cursorFocus;
		private BaseUnit unitFocus;

		/// <summary>
		/// The position (in pixels) of the camera
		/// </summary>
		public Vector2 position = Vector2.Zero;

		/// <summary>
		/// Get or set the speed (pixels per update) of the camera motion
		/// </summary>
        public float speed {
            get { return _speed; }
            set { _speed = (float)Math.Max(value, 1f); }
        }

		/// <summary>
		/// Get the Matrix used to shift the position of the camera relative to the map
		/// </summary>
		/// <remarks>More accurately, the Matrix shifts the map to the camera location in the viewport</remarks>
		public Matrix transformationMatrix
		{
			get { return Matrix.CreateTranslation(new Vector3(-position, 0f)); }
		}

		/// <summary>
		/// Returns whether or not the camera is focused on a particular Animated Sprite
		/// </summary>
		public bool isFocused
		{
			get { return _isFocusedCursor || _isFocusedUnit; }
		}

		/// <summary>
		/// Sets the camera to follow around a particular AnimatedSprite
		/// </summary>
		/// <param name="focusCursor">The AnimatedSprite the camera should follow</param>
		/// <remarks>When focused, the Camera cannot be controlled by the keyboard.</remarks>
		public void setFocus(Cursor focusCursor)
		{
			cursorFocus = focusCursor;
			_isFocusedCursor = true;
			_isFocusedUnit = false;
		}

		public void setFocus(BaseUnit focusUnit)
		{
			unitFocus = focusUnit;
			_isFocusedUnit = true;
			_isFocusedCursor = false;
		}

		public void toggleFocus()
		{
			if (cursorFocus != null)
				_isFocusedCursor = !_isFocusedCursor;
			if (unitFocus != null)
				_isFocusedUnit = !_isFocusedUnit;
		}

		/// <summary>
		/// Unsets the focus of the camera
		/// </summary>
		/// <remarks>The camera will reobtain keyboard functionality</remarks>
		public void unsetFocus()
		{
			unitFocus = null;
			cursorFocus = null;
			_isFocusedUnit = false;
			_isFocusedCursor = false;
		}

		/// <summary>
		/// Updates the camera location with respect to its focus, the viewport, and the current TileMap
		/// </summary>
		/// <param name="screenWidth">Integer width in pixels of the viewport</param>
		/// <param name="screenHeight">Integer height in pixels of the viewport</param>
		/// <param name="map">The TileMap currently being rendered in the viewport</param>
		public void update(int screenWidth, int screenHeight, TileMap map)
		{
			if (!isFocused)
			{
				KeyboardState keyState = Keyboard.GetState();
				Vector2 motion = Vector2.Zero;

				if (keyState.IsKeyDown(Keys.Up))
					--motion.Y;
				if (keyState.IsKeyDown(Keys.Down))
					++motion.Y;
				if (keyState.IsKeyDown(Keys.Left))
					--motion.X;
				if (keyState.IsKeyDown(Keys.Right))
					++motion.X;

				if (motion != Vector2.Zero)
				{
					motion.Normalize();
					this.position += motion * this.speed;
				}
			}
			else
			{
				if (_isFocusedCursor)
				{
					position.X =
						cursorFocus.position.X +
						(cursorFocus.currentAnimation.currentFrame.Width / 2) -
						(screenWidth / 2);
					position.Y =
						cursorFocus.position.Y +
						(cursorFocus.currentAnimation.currentFrame.Height / 2) -
						(screenHeight / 2);
				}
				else if (_isFocusedUnit)
				{
					position.X =
						unitFocus.unitSprite.position.X +
						(Engine.TILE_WIDTH / 2) -
						(screenWidth / 2);
					position.Y =
						unitFocus.unitSprite.position.Y +
						(Engine.TILE_HEIGHT / 2) -
						(screenHeight / 2);
				}
			}

			clampToArea(map.getWidthInPixels() - screenWidth, map.getHeightInPixels() - screenHeight);

		}

		/// <summary>
		/// Clamps the camera to only move within a specific area
		/// </summary>
		/// <param name="width">Width in pixels of the area the camera can move</param>
		/// <param name="height">Height in pixels of the area the camera can move</param>
		private void clampToArea(int width, int height)
		{
			if (position.X > width)
				position.X = width;
			if (position.Y > height)
				position.Y = height;

			if (position.X < 0)
				position.X = 0;
			if (position.Y < 0)
				position.Y = 0;
		}

	}
}

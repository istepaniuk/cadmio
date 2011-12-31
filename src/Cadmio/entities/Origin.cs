using GtkGL;
using gl = Tao.OpenGl.Gl;

namespace Cadmio
{
	
	public class Origin : BaseEntity, IGLObject
	{
		public Origin (int ID) : base(ID)
		{
			Name = "Origin";
			Lighting = false;
			Init ();
		}
		private Vector3 position = new Vector3 (0, 0, 0);
		public Vector3 Position {
			get { return position; }
		}

		protected override void DrawObject ()
		{
			float size = 1;
			
			gl.glBegin (gl.GL_LINES);
			gl.glLineWidth (1.5f);
			
			// x-axis
			gl.glColor3f (0.5f, 0, 0);
			gl.glVertex3f (-size, 0, 0);
			gl.glVertex3f (size, 0, 0);
			
			// y-axis
			gl.glColor3f (0, 0, 0.5f);
			gl.glVertex3f (0, -size, 0);
			gl.glVertex3f (0, size, 0);
			
			// z-axis
			gl.glColor3f (0, 0.5f, 0);
			gl.glVertex3f (0, 0, -size);
			gl.glVertex3f (0, 0, size);
			
			gl.glEnd ();
		}
	}
}